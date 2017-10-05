using System;
using System.IO;
using System.Text;
using System.Xml;
using XrmCommandBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmCommandBox.Data;

namespace XrmCommandBox.IntegrationTests
{
    [TestClass]
    public class ExportTests
    {
        [TestMethod]
        public void Export_Accounts_Using_Entity_Name()
        {
            var commandParameters = new[] { "export",
                                                "--connection", "integrationTests",
                                                "--entity", "account",
                                                "--recordNumber" };

            // Run the command
            Program.Main(commandParameters);

            // Expected serialized file name
            var serializedFileName = "account.xml";

            // Check that a file named account.xml was created and its readable
            var ser = new DataTableSerializer();
            var dt = ser.Deserialize(serializedFileName);

            Assert.AreEqual("account", dt.Name);

            // delete files to avoid unexpected effects in other tests
            File.Delete(serializedFileName);
        }

        [TestMethod]
        public void Export_Contacts_Using_FetchXml()
        {
            var fetchFile = "fetch.xml";
            var fetchQuery = @"<fetch top='50' >
                                <entity name='contact' />
                               </fetch>";

            // save query to a file
            File.WriteAllBytes(fetchFile, Encoding.Default.GetBytes(fetchQuery));

            var commandParameters = new[] { "export",
                                            "--connection", "integrationTests",
                                            "--fetchfile", fetchFile,
                                            "--recordNumber" };
            // Run the command
            Program.Main(commandParameters);

            // Expected serialized file name
            var serializedFileName = "contact.xml";

            // Check that a file named account.xml was created and its readable
            var ser = new DataTableSerializer();
            var dt = ser.Deserialize(serializedFileName);

            var xml = new XmlDocument();
            xml.Load(serializedFileName);

            Assert.AreEqual("contact", dt.Name);

            // Make sure the record numbers are there
            Assert.AreEqual("1", xml.SelectSingleNode("Data/row[1]/@i")?.Value);
            Assert.AreEqual("systemuser", ((EntityReferenceValue)dt[0]["ownerid"]).LogicalName);
            Assert.IsNotNull(((EntityReferenceValue)dt[0]["ownerid"]).Value);

            // delete files to avoid unexpected effects in other tests
            File.Delete(serializedFileName);
            File.Delete(fetchFile);
        }

    }
}
