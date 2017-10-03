using XrmCommandBox.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace XrmCommandBox.Tests.Data
{
    [TestClass]
    public class XmlSerializerTests
    {
        [TestMethod]
        public void Serialize_Simple_Data()
        {
            // Prepare the data to serialize
            var dataTable = new DataTable() { Name="MyTable" };

            // add row
            var numberValue = 2.3;
            var dateValue = DateTime.Now;
            var referenceValue = new EntityReferenceValue() { LogicalName="MyEntity", Name="EntityText", Value=Guid.NewGuid() };
            var row1 = new Dictionary<string, object>() { { "Attr1", "Value1" }, { "Attr2", numberValue } };
            var row2 = new Dictionary<string, object>() { { "Attr2", dateValue }, { "Attr4", referenceValue } };
            dataTable.Add(row1);
            dataTable.Add(row2);

            var serializer = new XmlSerializer();
            var sb = new StringBuilder();
            serializer.Serialize(dataTable,new StringWriter(sb));
            var serializedXml = sb.ToString();

            // Check the results in sb
            var xml = new System.Xml.XmlDocument();
            xml.LoadXml(serializedXml);
            Assert.AreEqual("MyTable", xml.SelectSingleNode("/Data/@name")?.Value);
            Assert.AreEqual("Value1", xml.SelectSingleNode("/Data/row[1]/Attr1")?.InnerText);
            Assert.AreEqual("2.3", xml.SelectSingleNode("/Data/row[1]/Attr2")?.InnerText);
            Assert.AreEqual(dateValue.ToString(CultureInfo.CurrentCulture), xml.SelectSingleNode("/Data/row[2]/Attr2")?.InnerText);
            Assert.AreEqual(referenceValue.Value.ToString(), xml.SelectSingleNode("/Data/row[2]/Attr4")?.InnerText);
            Assert.AreEqual(referenceValue.Name, xml.SelectSingleNode("/Data/row[2]/Attr4/@Name")?.Value);
            Assert.AreEqual(referenceValue.LogicalName, xml.SelectSingleNode("/Data/row[2]/Attr4/@LogicalName")?.Value);
        }

        [TestMethod]
        public void Deserialize_Simple_Data()
        {
            var xmlData = @"<DataTable name='MyDataTable'>
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
            Assert.AreEqual("MyDataTable", dataTable.Name);
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
