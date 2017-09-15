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
                    // the element name at this level should match the entity name
                    var entityName = reader.Name;

                    var entity = new Entity(entityName);

                    // read attributes

                    records.Add(entity);
                }
            }

            return records;
        }
    }
}
