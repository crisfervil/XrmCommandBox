using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System;

namespace XrmCommandBox.Data
{
    public class XmlSerializer : ISerializer
    {
        public string Extension { get; } = ".xml";

        public void Serialize(DataTable data, TextWriter writer, bool addRecordNumber = false)
        {
            var recordNumber = 1;
            using (var docWriter = new XmlTextWriter(writer))
            {
                docWriter.WriteStartDocument();
                docWriter.Formatting = Formatting.Indented;

                docWriter.WriteStartElement("Data");

                if (!string.IsNullOrEmpty(data.Name))
                {
                    docWriter.WriteAttributeString("name",data.Name);
                }

                foreach (var entityRecord in data)
                {
                    docWriter.WriteStartElement("row");
                    if (addRecordNumber) docWriter.WriteAttributeString("i", "", recordNumber.ToString());
                    WriteAttributeValues(entityRecord, docWriter);
                    docWriter.WriteEndElement();
                    recordNumber++;
                }

                docWriter.Flush();
            }
        }

        public void Serialize(DataTable data, string fileName, bool addRecordNumber = false)
        {
            using (var ts = File.CreateText(fileName))
            {
                Serialize(data, ts, addRecordNumber);
                ts.Close();
            }
        }

        public DataTable Deserialize(string fileName)
        {
            DataTable dataTable;

            // read the xml file
            using(var fs = File.OpenRead(fileName))
            {
                dataTable = Deserialize(fs);
            }

            return dataTable;
        }

        public DataTable Deserialize(Stream data)
        {
            var dataTable = new DataTable();

            using (var reader = XmlReader.Create(data))
            {
                // read all the child elements
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        // ignore this element and move to next node
                    }
                    else if (reader.NodeType == XmlNodeType.Attribute)
                    {
                        if (reader.Name=="name")
                        {
                            dataTable.Name = reader.Name;
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.Element)
                    {
                        // Read the name attribute (if the attribute is not set or is null, the table name will be set to null)
                        dataTable.Name = reader.GetAttribute("name");

                        // This should be the main Data node
                        var content = reader.ReadSubtree();
                        ReadRows(content, dataTable);
                    }
                }
            }

            return dataTable;
        }

        private void ReadRows(XmlReader reader, DataTable dataTable)
        {
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {

                    // read attributes
                    var content = reader.ReadSubtree();
                    var record = ReadAttributes(content);

                    dataTable.Add(record);                    
                }
            }
        }

        private Dictionary<string, object> ReadAttributes(XmlReader reader)
        {
            var row = new Dictionary<string, object>();

            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    // the element name at this level should match the attribute name
                    var attrName = reader.Name;

                    // move to the element value
                    var content = reader.ReadSubtree();

                    var attrValue = ReadAttrValue(content);

                    // add the attribute value
                    row[attrName] = attrValue;
                }
            }

            return row;
        }

        private object ReadAttrValue(XmlReader reader)
        {
            object attrValue = null;
            reader.MoveToContent();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    attrValue = reader.Value;
                }
                else
                {
                    // TODO: Add support for this
                }
            }
            return attrValue;
        }

        private void WriteAttributeValues(Dictionary<string,object> entityRecord, XmlTextWriter docWriter)
        {
            foreach (var attributeKey in entityRecord.Keys)
            {
                docWriter.WriteStartElement(attributeKey);
                if(entityRecord[attributeKey] != null)
                {
                    WriteXmlAttributes(docWriter, entityRecord[attributeKey]);
                    var attrValue = GetAttributeValue(entityRecord[attributeKey]);
                    var strAttrValue = attrValue?.ToString();
                    if (strAttrValue != null)
                    {
                        docWriter.WriteValue(strAttrValue);
                    }
                }
                docWriter.WriteEndElement();
            }
        }

        private void WriteXmlAttributes(XmlTextWriter docWriter, object attributeValue)
        {
            var referenceValue = attributeValue as EntityReferenceValue;
            if (referenceValue != null)
            {
                docWriter.WriteAttributeString("Name", referenceValue.Name);
                docWriter.WriteAttributeString("LogicalName", referenceValue.LogicalName);
            }
        }

        private object GetAttributeValue(object attributeValue)
        {
            object value = null;

            var referenceValue = attributeValue as EntityReferenceValue;
            if (referenceValue != null)
            {
                value = referenceValue.Value;
            }
            else
            {
                value = attributeValue;
            }

            return value;
        }

    }
}
