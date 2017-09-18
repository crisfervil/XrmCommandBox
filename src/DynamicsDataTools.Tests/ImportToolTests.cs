using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using DynamicsDataTools.ImportTool;
using FakeXrmEasy;

namespace DynamicsDataTools.Tests
{
    [TestClass]
    public class ImportToolTests
    {
        [TestMethod]
        public void Import_Simple_Xml_File()
        {

            string xmlContent = @"<Data>
                                <account>
                                </account>
                                <account>
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

        }
    }
}
