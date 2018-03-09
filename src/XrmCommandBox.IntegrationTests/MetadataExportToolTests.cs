using System;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XrmCommandBox.Data;
using Newtonsoft.Json;
using Microsoft.Xrm.Sdk.Metadata;

namespace XrmCommandBox.IntegrationTests
{
    [TestClass]
    public class MetadataExportToolTests
    {
        private string _connectionString = "integrationTests";

        [TestInitialize]
        public void TestInitialize()
        {
            var environmentConnectionString = Environment.GetEnvironmentVariable("XrmToolBoxIntegrationTestsConnectionString");
            if (environmentConnectionString != null)
                _connectionString = environmentConnectionString;
        }

        [TestMethod]
        [TestCategory("IntegrationTests")]
        public void Export_Account_Metadata()
        {
            var commandParameters = new[]
            {
                "metadata-export",
                "--connection", _connectionString,
                "--entity", "account",
                "--file", "account.json"
            };

            // Run the command
            var exitCode = Program.Run(commandParameters);

            // Check the exit code
            Assert.AreEqual(0, exitCode);

            // Expected serialized file name
            var exportedFileName = "account.json";


            var strJson = File.ReadAllText(exportedFileName);

            var metadata = JsonConvert.DeserializeObject<EntityMetadata>(strJson);
            
            Assert.AreEqual("account", metadata.LogicalName);

            // delete files to avoid unexpected effects in other tests
            File.Delete(exportedFileName);
        }
    }
}