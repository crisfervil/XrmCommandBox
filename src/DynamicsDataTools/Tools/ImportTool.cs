using DynamicsDataTools.Data;
using log4net;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicsDataTools.Tools
{
    public class ImportTool
    {
        private readonly ILog _log;
        private readonly IOrganizationService _crmService;

        public ImportTool(ILog log, IOrganizationService service)
        {
            _log = log;
            _crmService = service;
        }

        public void Run(ImportOptions options)
        {
            _log.Info("Running Import tool...");

            var serializer = new DataTableSerializer(_log);

            _log.Info("Reading file...");
            var dataTable = serializer.Deserialize(options.File);

            _log.Info("Processing records...");
            foreach (var row in dataTable)
            {
                var entity = GetEntity(row, dataTable.Name);
                var recordId = _crmService.Create(entity);
            }
        }

        private Entity GetEntity(Dictionary<string,object> record, string entityName)
        {
            var entity = new Entity(entityName);

            foreach (var attrName in record.Keys)
            {
                // Convert this to the type specified in the metadata for the attribute in the entity
                entity[attrName] = record[attrName];
            }

            return entity;
        }
    }
}
