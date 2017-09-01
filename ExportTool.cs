using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsDataTools
{
    class ExportTool
    {
        private readonly ILog _log;

        public ExportTool(ILog log)
        {
            _log = log;
        }

        public void Run(ExportOptions options)
        {
            _log.Debug("Running Export tool...");

            _log.Debug("Creating connection...");
            var crmService = new ConnectionBuilder().GetConnection(options.ConnectionName);

            _log.Debug("Executing query...");
            var foundRecords = crmService.RetrieveMultiple(GetAllRecordsQuery(options.EntityName));
            _log.Debug($"{foundRecords.Entities.Count} records found");

            // Save records to an Xml file
            _log.Debug("Peparing file...");
            Save(foundRecords.Entities, options.File);

            _log.Debug("Completed");
        }

        private void Save(DataCollection<Entity> data, string fileName)
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
                docWriter.WriteValue(GetAttributeValue(attribute.Value));
                docWriter.WriteEndElement();
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

        private QueryBase GetAllRecordsQuery(string entityName)
        {
            return new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(true), // retrieve all columns
            };
        }
    }
}
