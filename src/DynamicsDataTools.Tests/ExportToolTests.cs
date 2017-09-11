using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicsDataTools;
using Microsoft.Xrm.Sdk;

namespace DynamicsDataTools.Tests
{
    [TestClass]
    public class ExportToolTests
    {
        [TestMethod]
        public void Exports_Accounts()
        {
            const string fileName = "account.xml";
            var log = new FakeLog();
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // set the xrm context
            var account1 = new Entity("account");
            account1.Id = Guid.NewGuid();
            account1["name"] = "Account1";

            var account2 = new Entity("account");
            account2.Id = Guid.NewGuid();
            account2["name"] = "Account2";

            var accounts = new List<Entity>() {account1,account2};
            context.Initialize(accounts);

            if(System.IO.File.Exists(fileName)) System.IO.File.Delete(fileName);

            // The file name is not provided, so the default path should be used
            var options = new ExportOptions() { EntityName = "account"};

            // run the tool
            var exportTool = new ExportTool(log,service);
            exportTool.Run(options);

            // Checks the file exists
            Assert.IsTrue(System.IO.File.Exists(fileName));

            // Make sure the file is not empty and is a valid xml file
            var xml = new XmlDocument();
            xml.Load(fileName);

            // check the contents of the exported file
            Assert.AreEqual(accounts.Count, xml.SelectNodes("Data/account")?.Count);
        }

        [TestMethod]
        public void Exports_Fails_If_Not_Exporter_Available()
        {
            var log = new FakeLog();
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // The file name is not provided, so the default path should be used
            var options = new ExportOptions() { EntityName = "account", File = "account.xyz" /* There's no exporter for extension xyz */};

            // run the tool
            var exportTool = new ExportTool(log, service);
            try
            {
                exportTool.Run(options);
                Assert.Fail("Exeption not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("No exporter found for extension .xyz", ex.Message);
            }
        }

        [TestMethod]
        public void Export_Fails_With_Wrong_Options()
        {
            var log = new FakeLog();
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // The file name is not provided, so the default path should be used
            var options = new ExportOptions();

            // run the tool
            var exportTool = new ExportTool(log, service);
            try
            {
                exportTool.Run(options);
                Assert.Fail("Exeption not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Either the entityname or the fetchfile options are required", ex.Message);
            }
        }

        [TestMethod]
        public void Exports_From_Fetch_Query()
        {
            var log = new FakeLog();
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();
            var fileName = "exported.xml";
            var fetchFile = "fetch.xml";
            var fetchQuery = @"<fetch top='50' >
                                <entity name='account' />
                               </fetch>";

            // save query to a file
            System.IO.File.WriteAllBytes(fetchFile,Encoding.Default.GetBytes(fetchQuery));

            // The file name is not provided, so the default path should be used
            var options = new ExportOptions() { FetchFile = fetchFile, File = fileName};

            // run the tool
            var exportTool = new ExportTool(log, service);
            exportTool.Run(options);

            // Checks the file exists
            Assert.IsTrue(System.IO.File.Exists(fileName));

            // Make sure the file is not empty and is a valid xml file
            var xml = new XmlDocument();
            xml.Load(fileName);
        }


        [TestMethod]
        public void Exports_From_Fetch_With_Wrong_Fetch()
        {
            var log = new FakeLog();
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();
            var fetchFile = "fetch.xml";
            var fetchQuery = @"<data><fetch top='50' >
                                <entity name='account' />
                               </fetch></data>";

            // save query to a file
            System.IO.File.WriteAllBytes(fetchFile, Encoding.Default.GetBytes(fetchQuery));

            // The file name is not provided, so the default path should be used
            var options = new ExportOptions() { FetchFile = fetchFile };

            // run the tool
            var exportTool = new ExportTool(log, service);
            try
            {
                exportTool.Run(options);
                Assert.Fail("Exeption not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid xml document. The first node in the document must be a fetch", ex.Message);
            }
        }


    }
}
