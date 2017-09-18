using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Xml;

namespace DynamicsDataTools.ImportTool
{
    public class XmlDataReader : IDataReader
    {
        public string Extension { get; } = ".xml";

        public IList<Entity> Read(string fileName)
        {

            var records = new List<Entity>();

            // read the xml file
            using (var reader = XmlReader.Create(fileName))
            {
                reader.ReadStartElement("Data");

                // read all the child elements
                while (reader.Read())
                {
                    // Ignore anything that is not an element
                    if (!reader.IsStartElement()) continue;

                    // the element name at this level should match the entity name
                    var entityName = reader.Name;

                    var entity = new Entity(entityName);

                    // read attributes
                    var content = reader.ReadSubtree();
                    ReadAttributes(content, entity);

                    records.Add(entity);

                    // Move the reader to the next sibling
                    reader.Skip();
                }
            }

            return records;
        }

        private void ReadAttributes(XmlReader reader, Entity entity)
        {
            reader.MoveToContent();
            while (reader.Read())
            {
                // Ignore anything that is not an element
                if (!reader.IsStartElement()) continue;

                // the element name at this level should match the attribute name
                var attrName = reader.Name;
                var attrValue = reader.ReadElementContentAsString();

                // add the attribute value
                entity.Attributes.Add(attrName, attrValue);
            }
        }
    }
}
