using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using DynamicsDataTools.ImportTool;
using FakeXrmEasy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DynamicsDataTools.Tests
{
    [TestClass]
    public class ImportToolTests
    {
        [TestMethod]
        public void Import_Simple_Xml_File()
        {

            string xmlContent = @"<Data attr='value'>
                                <account attr1='value1'>
                                    <attr1>Value1</attr1>
                                    <attr2>Value2</attr2>
                                </account>
                                <account i='2'>
                                    <attr1>Value3</attr1>
                                    <attr2>Value4</attr2>
                                    <attr3>Value5</attr3>
                                </account>
                               </Data>";

            // Save the content to a file
            var fileName = Path.GetTempFileName();
            // add the xml extension
            var xmlFile = Path.ChangeExtension(fileName,"xml");
            // rename the temp file
            File.Move(fileName, xmlFile);
            // save the contents
            File.WriteAllBytes(xmlFile, Encoding.Default.GetBytes(xmlContent));

            var log = new FakeLog();
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var options = new ImportOptions() { File = xmlFile };

            var importTool = new ImportTool.ImportTool(log, service);
            importTool.Run(options);


            var accountsCreated = context.Data["account"].Values.ToList();

            Assert.AreEqual(2, accountsCreated.Count);
            Assert.AreEqual("Value1", accountsCreated[0]["attr1"]);
            Assert.AreEqual("Value2", accountsCreated[0]["attr2"]);
            Assert.AreEqual("Value3", accountsCreated[1]["attr1"]);
            Assert.AreEqual("Value4", accountsCreated[1]["attr2"]);
            Assert.AreEqual("Value5", accountsCreated[1]["attr3"]);

        }
    }
}
