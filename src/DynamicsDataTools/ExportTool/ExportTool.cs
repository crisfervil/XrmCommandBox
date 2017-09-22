using log4net;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Xml;
using Microsoft.Xrm.Sdk;
using DynamicsDataTools.Data;

namespace DynamicsDataTools.ExportTools
{
    public class ExportTool
    {
        private readonly ILog _log;
        private readonly IOrganizationService _crmService;

        public ExportTool(ILog log, IOrganizationService service)
        {
            _log = log;
            _crmService = service;
        }

        public void Run(ExportOptions options)
        {
            _log.Info("Running Export tool...");

            ValidateOptions(options);

            _log.Debug("Executing query...");
            var foundRecords = GetRecords(options);
            _log.Info($"{foundRecords.Entities.Count} records found");

            // Convert to a data table
            var recordsTable = foundRecords.AsDataTable();

            // Export 

            // set a default file name
            if (string.IsNullOrEmpty(options.File))
            {
                options.File = $"{foundRecords.EntityName}.xml";
            }

            var serializer = new DataTableSerializer(_log);
            serializer.Serialize(recordsTable, options.File);

            _log.Info("Completed");
        }

        private EntityCollection GetRecords(ExportOptions options)
        {
            EntityCollection foundRecords = null;
            if (!string.IsNullOrEmpty(options.EntityName))
            {
                foundRecords = _crmService.RetrieveMultiple(GetAllRecordsQuery(options.EntityName));
            }
            else if (!string.IsNullOrEmpty(options.FetchFile))
            {
                foundRecords = _crmService.RetrieveMultiple(GetFetchQuery(options.FetchFile));
            }
            return foundRecords;
        }

        private void ValidateOptions(ExportOptions options)
        {
            if (string.IsNullOrEmpty(options.FetchFile) && string.IsNullOrEmpty(options.EntityName))
            {
                throw new Exception("Either the entityname or the fetchfile options are required");
            }
        }

        private QueryBase GetFetchQuery(string fileName)
        {
            // read xml file
            var xml = new XmlDocument();
            xml.Load(fileName);

            if (xml.DocumentElement == null || xml.DocumentElement.Name != "fetch")
            {
                throw new Exception("Invalid xml document. The first node in the document must be a fetch");
            }

            return new FetchExpression(xml.DocumentElement.OuterXml);
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
