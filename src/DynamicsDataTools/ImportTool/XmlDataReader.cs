using DynamicsDataTools.Data;
using System;
using System.Collections.Generic;
using System.Xml;

namespace DynamicsDataTools.ImportTool
{
    public class XmlDataReader : IDataReader
    {
        public string Extension { get; } = ".xml";

        public DataTable Read(string fileName)
        {
            var dataTable = new DataTable();

            // read the xml file
            using (var reader = XmlReader.Create(fileName))
            {
                // read the first element
                reader.Read();

                // read table attributes
                while (reader.MoveToNextAttribute())
                {
                    if (reader.Name=="name")
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
