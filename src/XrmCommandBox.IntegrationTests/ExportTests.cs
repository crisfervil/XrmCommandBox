using System;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmCommandBox.Data;

namespace XrmCommandBox.IntegrationTests
{
    [TestClass]
    public class ExportTests
    {
        private string _connectionString = "integrationTests";

        [TestInitialize]
        public void TestInitialize()
        {
            var environmentConnectionString = Environment.GetEnvironmentVariable("IntegrationTestsConnectionString");
            if (environmentConnectionString != null)
            {
                _connectionString = environmentConnectionString;
            }            
        }

        [TestMethod]
        [TestCategory("IntegrationTests")]
        public void Export_Accounts_Using_Entity_Name()
        {
            var commandParameters = new[]
            {
                "export",
                "--connection", _connectionString,
                "--entity", "account",
                "--recordNumber",
                "--page-size", "10",
                "--page", "1"
            };

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
        [TestCategory("IntegrationTests")]
        public void Export_Contacts_Using_FetchXml()
        {
            var fetchQuery = @"<fetch top='10' >
                                <entity name='contact' />
                               </fetch>";

            var commandParameters = new[]
            {
                "export",
                "--connection", _connectionString,
                "--fetch-query", fetchQuery,
                "--recordNumber"
            };
            // Run the command
            Program.Main(commandParameters);

            // Check the exit code
            Assert.AreEqual(0, Environment.ExitCode);

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
            Assert.AreEqual("systemuser", dt[0]["ownerid.type"]);
            Assert.IsNotNull(dt[0]["ownerid"]);

            // delete files to avoid unexpected effects in other tests
            File.Delete(serializedFileName);
        }
    }
}