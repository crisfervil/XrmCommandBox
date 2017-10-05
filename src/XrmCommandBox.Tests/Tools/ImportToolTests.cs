using System;
using XrmCommandBox.Tests;
using XrmCommandBox.Tools;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using XrmCommandBox.Data;

namespace XrmCommandBox.Tests.Tools
{
    [TestClass]
    public class ImportToolTests
    {
        [TestMethod]
        public void Import_Simple_Xml_File()
        {
            var randomGuid = Guid.NewGuid();
            string xmlContent = $@"<DataTable name='account'>
                                    <row n='1'>
                                        <attr1>Value1</attr1>
                                        <attr2>Value2</attr2>
                                    </row>
                                    <row n='2'>
                                        <attr1>Value3</attr1>
                                        <attr2>Value4</attr2>
                                        <attr3 LogicalName='Name1' Name='Some Random Name'>{randomGuid}</attr3>
                                    </row>
                               </DataTable>";

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

            var options = new ImportToolOptions { File = xmlFile };

            var importTool = new ImportTool(log, service);
            importTool.Run(options);


            var accountsCreated = context.Data["account"].Values.ToList();

            Assert.AreEqual(2, accountsCreated.Count);
            Assert.AreEqual("Value1", accountsCreated[0]["attr1"]);
            Assert.AreEqual("Value2", accountsCreated[0]["attr2"]);
            Assert.AreEqual("Value3", accountsCreated[1]["attr1"]);
            Assert.AreEqual("Value4", accountsCreated[1]["attr2"]);
            var refValue = (EntityReference)accountsCreated[1]["attr3"];
            Assert.AreEqual("Name1", refValue.LogicalName);
            Assert.AreEqual(randomGuid, refValue.Id);
        }
    }
}
