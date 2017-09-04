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
            var exporter = new XmlExporter(_log);
            exporter.Export(foundRecords.Entities, options.File);

            _log.Debug("Completed");
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
