using DynamicsDataTools.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DynamicsDataTools.Data
{
    public class XmlSerializer : ISerializer
    {
        public string Extension { get; } = ".xml";

        public void Serialize(DataTable data, string fileName)
        {

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

        private Dictionary<string, string> ReadAttributes(XmlReader reader)
        {
            var row = new Dictionary<string, string>();

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
    }
}
