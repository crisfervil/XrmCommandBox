using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using XrmCommandBox.Tools;

namespace XrmCommandBox.Tests.Tools
{
    [TestClass]
    public class ExportToolTests
    {
        [TestMethod]
        public void Exports_Accounts_Xml()
        {
            const string fileName = "account.xml";
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // set the xrm context
            var account1 = new Entity("account");
            account1.Id = Guid.NewGuid();
            account1["name"] = "Account1";

            var account2 = new Entity("account");
            account2.Id = Guid.NewGuid();
            account2["name"] = "Account2";

            var accounts = new List<Entity> {account1, account2};
            context.Initialize(accounts);

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            // The file name is not provided, so the default path should be used
            var options = new ExportToolOptions {EntityName = "account", RowNumber = true};

            // TODO: Delete this if the FaxeXrmEaxy pull request is accepted
            options.File = fileName;

            // run the tool
            var exportTool = new ExportTool(service);
            exportTool.Run(options);

            // Checks the file exists
            Assert.IsTrue(File.Exists(fileName));

            // Make sure the file is not empty and is a valid xml file
            var xml = new XmlDocument();
            xml.Load(fileName);

            // check the contents of the exported file
            Assert.AreEqual(account1["name"].ToString(), xml.SelectSingleNode("Data/row[1]/name")?.InnerText);
            Assert.AreEqual(account2["name"].ToString(), xml.SelectSingleNode("Data/row[2]/name")?.InnerText);

            // Check the record numbers are there
            Assert.AreEqual("1", xml.SelectSingleNode("Data/row[1]/rownumber")?.InnerText);
            Assert.AreEqual("2", xml.SelectSingleNode("Data/row[2]/rownumber")?.InnerText);
        }

        [TestMethod]
        public void Exports_Fails_If_Not_Exporter_Available()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // The file name is not provided, so the default path should be used
            var options = new ExportToolOptions
            {
                EntityName = "account",
                File = "account.xyz" /* There's no exporter for extension xyz */
            };

            // run the tool
            var exportTool = new ExportTool(service);
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
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            // The file name is not provided, so the default path should be used
            var options = new ExportToolOptions();

            // run the tool
            var exportTool = new ExportTool(service);
            try
            {
                exportTool.Run(options);
                Assert.Fail("Exeption not thrown");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Either the entity or the fetch-query options are required", ex.Message);
            }
        }

        [TestMethod]
        public void Exports_From_Fetch_Query()
        {
            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();
            var exportedFile = "exported.xml";
            var fetchXml = @"<fetch top='50' >
                               <entity name='account' />
                              </fetch>";

            // The file name is not provided, so the default path should be used
            var options = new ExportToolOptions {FetchQuery = fetchXml, File = exportedFile};

            // run the tool
            var exportTool = new ExportTool(service);
            exportTool.Run(options);

            // Checks the file exists
            Assert.IsTrue(File.Exists(exportedFile));

            // Make sure the file is not empty and is a valid xml file
            var xml = new XmlDocument();
            xml.Load(exportedFile);

            // delete files to avoid unexpected effects in other tests
            File.Delete(exportedFile);
        }
    }
}