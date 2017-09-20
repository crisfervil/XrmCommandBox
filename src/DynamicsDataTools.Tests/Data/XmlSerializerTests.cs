using DynamicsDataTools.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataTools.Tests.Data
{
    [TestClass]
    public class XmlSerializerTests
    {
        [TestMethod]
        public void Serialize_Simple_Data()
        {

        }

        [TestMethod]
        public void Deserialize_Simple_Data()
        {
            var xmlData = @"<DataTable>
                                <row>
                                    <attr1>Value1</attr1>
                                    <attr2>Value2</attr2>
                                    <attr3>Value3</attr3>
                                </row>
                                <row>
                                    <attr2>Value4</attr2>
                                    <attr3>Value5</attr3>
                                    <attr4>Value6</attr4>
                                </row>
                            </DataTable>";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlData));
            var serializer = new XmlSerializer();
            var dataTable = serializer.Deserialize(ms);

            Assert.AreEqual(2, dataTable.Count);
            Assert.AreEqual("Value1", dataTable[0]["attr1"]);
            Assert.AreEqual("Value2", dataTable[0]["attr2"]);
            Assert.AreEqual("Value3", dataTable[0]["attr3"]);
            Assert.AreEqual(false, dataTable[0].ContainsKey("attr4"));
            Assert.AreEqual(false, dataTable[1].ContainsKey("attr1"));
            Assert.AreEqual("Value4", dataTable[1]["attr2"]);
            Assert.AreEqual("Value5", dataTable[1]["attr3"]);
            Assert.AreEqual("Value6", dataTable[1]["attr4"]);
        }
    }
}
