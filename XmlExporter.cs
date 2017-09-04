using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using System.Xml;
using log4net;

namespace DynamicsDataTools
{
    public class XmlExporter : IExporter
    {
        private readonly ILog _log;
        private string _extension = ".xml";

        public XmlExporter(ILog log)
        {
            _log = log;
        }

        public string Extension => _extension;

        public void Export(DataCollection<Entity> data, string fileName)
        {
            using (var docWriter = new XmlTextWriter(fileName, null))
            {
                docWriter.Formatting = Formatting.Indented;

                docWriter.WriteStartElement("Data");

                foreach (var entityRecord in data)
                {
                    docWriter.WriteStartElement(entityRecord.LogicalName);
                    WriteAttributeValues(entityRecord, docWriter);
                    docWriter.WriteEndElement();
                }

                docWriter.Flush();
            }
        }

        private void WriteAttributeValues(Entity entityRecord, XmlTextWriter docWriter)
        {
            foreach (var attribute in entityRecord.Attributes)
            {
                docWriter.WriteStartElement(attribute.Key);
                WriteXmlAttributes(docWriter,attribute);
                docWriter.WriteValue(GetAttributeValue(attribute.Value));
                docWriter.WriteEndElement();
            }
        }

        private void WriteXmlAttributes(XmlTextWriter docWriter, KeyValuePair<string, object> attribute)
        {
            if (attribute.Value is EntityReference)
            {
                EntityReference entityRefValue = (EntityReference) attribute.Value;
                docWriter.WriteAttributeString("Name", entityRefValue.Name);
                docWriter.WriteAttributeString("LogicalName", entityRefValue.LogicalName);
            }
        }

        private object GetAttributeValue(object attributeValue)
        {
            object value = null;

            if (attributeValue is OptionSetValue)
            {
                value = ((OptionSetValue)attributeValue).Value;
            }
            else if (attributeValue is EntityReference)
            {
                value = ((EntityReference)attributeValue).Id.ToString();
            }
            else
            {
                value = attributeValue.ToString();
            }

            return value;
        }
    }
}
