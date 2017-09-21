﻿using DynamicsDataTools.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DynamicsDataTools.Data
{
    public class XmlSerializer : ISerializer
    {
        public string Extension { get; } = ".xml";

        public void Serialize(DataTable data, TextWriter writer, bool addRecordNumber = false)
        {
            int recordNumber = 1;
            using (var docWriter = new XmlTextWriter(writer))
            {
                docWriter.WriteStartDocument();
                docWriter.Formatting = Formatting.Indented;

                docWriter.WriteStartElement("Data");

                foreach (var entityRecord in data)
                {
                    docWriter.WriteStartElement(data.Name);
                    if (addRecordNumber) docWriter.WriteAttributeString("i", "", recordNumber.ToString()); ;
                    WriteAttributeValues(entityRecord, docWriter);
                    docWriter.WriteEndElement();
                    recordNumber++;
                }

                docWriter.Flush();
            }
        }

        public void Serialize(DataTable data, string fileName)
        {
            using (var ts = File.CreateText(fileName))
            {
                Serialize(data, ts);
            }
        }

        public DataTable Deserialize(string fileName)
        {
            var dataTable = new DataTable();

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
                // read the first element
                reader.Read();

                // read table attributes
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name == "name")
                    {
                        dataTable.Name = reader.Value;
                    }
                }

                // read all the child elements
                while (reader.Read())
                {
                    // Ignore anything that is not an element
                    if (!reader.IsStartElement()) continue;

                    // read attributes
                    var content = reader.ReadSubtree();
                    var record = ReadAttributes(content);

                    dataTable.Add(record);

                    // Move the reader to the next sibling
                    reader.Skip();
                }
            }

            return dataTable;
        }

        private Dictionary<string, object> ReadAttributes(XmlReader reader)
        {
            var row = new Dictionary<string, object>();

            reader.MoveToContent();
            while (reader.Read())
            {
                // Ignore anything that is not an element
                if (!reader.IsStartElement()) continue;

                // the element name at this level should match the attribute name
                var attrName = reader.Name;
                var attrValue = reader.ReadElementContentAsString();

                // add the attribute value
                row[attrName] = attrValue;
            }

            return row;
        }

        private void WriteAttributeValues(Dictionary<string,object> entityRecord, XmlTextWriter docWriter)
        {
            foreach (var attributeKey in entityRecord.Keys)
            {
                docWriter.WriteStartElement(attributeKey);
                if(entityRecord[attributeKey] != null)
                {
                    if (entityRecord[attributeKey] is EntityReferenceValue) WriteXmlAttributes(docWriter, (EntityReferenceValue) entityRecord[attributeKey]);
                    var attrValue = GetAttributeValue(entityRecord[attributeKey]);
                    var strAttrValue = attrValue != null ? attrValue.ToString() : null;
                    docWriter.WriteValue(strAttrValue);
                }
                docWriter.WriteEndElement();
            }
        }

        private void WriteXmlAttributes(XmlTextWriter docWriter, EntityReferenceValue attribute)
        {
                docWriter.WriteAttributeString("Name", attribute.Name);
                docWriter.WriteAttributeString("LogicalName", attribute.LogicalName);
        }

        private object GetAttributeValue(object attributeValue)
        {
            object value = null;

            if (attributeValue is EntityReferenceValue)
            {
                value = ((EntityReferenceValue)attributeValue).Value;
            }
            else
            {
                value = attributeValue;
            }

            return value;
        }

    }
}
