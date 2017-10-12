using log4net;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using XrmCommandBox.Data;

namespace XrmCommandBox.Tools
{
    public class ImportTool
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ImportTool));
        private readonly IOrganizationService _crmService;

        public ImportTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(ImportToolOptions options)
        {
            _log.Info("Running Import Tool...");

            var serializer = new DataTableSerializer();

            _log.Info("Reading file...");
            var dataTable = serializer.Deserialize(options.File);

            _log.Info("Processing records...");
            var records = dataTable.AsEntityCollection();

            var recordCount = 0;
            foreach (var entityRecord in records.Entities)
            {
                _log.Debug($"{entityRecord.LogicalName} {++recordCount} of {records.Entities.Count} : {entityRecord.Id}");
                _crmService.Create(entityRecord);
            }

            _log.Info("Done!");
        }
    }
}
