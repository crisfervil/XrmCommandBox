using System;
using System.IO;
using System.Linq;
using System.Text;
using FakeXrmEasy;
using FakeXrmEasy.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using XrmCommandBox.Tools;

namespace XrmCommandBox.Tests.Tools
{
    [TestClass]
    public class ImportToolTests
    {
        [TestMethod]
        public void Import_Simple_Xml_File()
        {
            var randomGuid = Guid.NewGuid();
            int optionValue1 = 1, optionValue2 = 2;
            var moneyValue1 = 123.456m;
            var xmlContent = $@"<DataTable name='account'>
                                    <row n='1'>
                                        <accountid>E08BAD83-CA69-4961-B5FC-3A0A704F85FE</accountid>
                                        <attr1>Value1</attr1>
                                        <attr2>{optionValue1}</attr2>
                                        <attr4>{moneyValue1}</attr4>
                                    </row>
                                    <row n='2'>
                                        <accountid>4296FCA9-EE5A-4AA8-85D6-CBDD05E21F30</accountid>
                                        <attr1>Value3</attr1>
                                        <attr2>{optionValue2}</attr2>
                                        <attr3>{randomGuid}</attr3>
                                        <attr3.type>myentitytype</attr3.type>
                                        <attr3.name>Name1</attr3.name>
                                    </row>
                               </DataTable>";

            // Save the content to a file
            var fileName = Path.GetTempFileName();
            // add the xml extension
            var xmlFile = Path.ChangeExtension(fileName, "xml");
            // rename the temp file
            File.Move(fileName, xmlFile);
            // save the contents
            File.WriteAllBytes(xmlFile, Encoding.Default.GetBytes(xmlContent));

            var context = new XrmFakedContext();
            var service = context.GetOrganizationService();

            var accountMetadata = new EntityMetadata
            {
                LogicalName = "account"
            };
            accountMetadata.SetSealedPropertyValue("PrimaryIdAttribute", "accountid");

            var accountidMetadata = new UniqueIdentifierAttributeMetadata
            {
                LogicalName = "accountid"
            };
            var attr1Metadata = new StringAttributeMetadata
            {
                LogicalName = "attr1"
            };
            var attr2Metadata = new PicklistAttributeMetadata
            {
                LogicalName = "attr2"
            };
            var attr3Metadata = new LookupAttributeMetadata
            {
                LogicalName = "attr3"
            };
            var attr4Metadata = new MoneyAttributeMetadata
            {
                LogicalName = "attr4"
            };

            accountMetadata.SetAttributeCollection(new AttributeMetadata[]
                {accountidMetadata, attr1Metadata, attr2Metadata, attr3Metadata, attr4Metadata});

            context.InitializeMetadata(accountMetadata);

            var options = new ImportToolOptions {File = xmlFile};

            var importTool = new ImportTool(service);
            importTool.Run(options);


            var accountsCreated = context.Data["account"].Values.ToList();

            Assert.AreEqual(2, accountsCreated.Count);
            Assert.AreEqual("Value1", accountsCreated[0]["attr1"]);
            Assert.AreEqual(optionValue1, ((OptionSetValue) accountsCreated[0]["attr2"]).Value);
            Assert.AreEqual(moneyValue1, ((Money) accountsCreated[0]["attr4"]).Value);

            Assert.AreEqual("Value3", accountsCreated[1]["attr1"]);
            Assert.AreEqual(optionValue2, ((OptionSetValue) accountsCreated[1]["attr2"]).Value);
            var refValue = (EntityReference) accountsCreated[1]["attr3"];
            Assert.AreEqual("myentitytype", refValue.LogicalName);
            Assert.AreEqual(randomGuid, refValue.Id);
        }
    }
}