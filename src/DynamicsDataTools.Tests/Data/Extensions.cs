using DynamicsDataTools.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;

namespace DynamicsDataTools.Tests.Data
{
    [TestClass]
    public class Extensions
    {
        [TestMethod]
        public void Converts_To_Data_Table()
        {
            var data = new EntityCollection();

            var referenceGuid1 = Guid.NewGuid();
            var entity1 = new Entity("myEntity1");
            entity1["attr1"] = "Value1";
            entity1["attr2"] = new EntityReference("myEntity2", referenceGuid1);
            data.Entities.Add(entity1);

            var entity2 = new Entity("myEntity1");
            entity2["attr1"] = new OptionSetValue(12);
            entity2["attr2"] = new Money(16.55m);
            data.Entities.Add(entity2);

            var dt = data.AsDataTable();

            // check output values
            Assert.AreEqual("Value1", dt[0]["attr1"]);
            Assert.AreEqual("myEntity2", ((EntityReferenceValue)dt[0]["attr2"]).LogicalName);
            Assert.AreEqual(referenceGuid1, ((EntityReferenceValue)dt[0]["attr2"]).Value);

            Assert.AreEqual(12, dt[1]["attr1"]);
            Assert.AreEqual(16.55m, dt[1]["attr2"]);
        }
    }
}
