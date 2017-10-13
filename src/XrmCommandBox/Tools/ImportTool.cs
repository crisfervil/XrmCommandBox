using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using XrmCommandBox.Data;

namespace XrmCommandBox.Tools
{
    public class ImportTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ImportTool));

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

            _log.Debug("Querying metadata...");
            var metadata = GetMetadata(dataTable.Name);

            _log.Info("Processing records...");
            var records = dataTable.AsEntityCollection(metadata);

            var recordCount = 0;
            foreach (var entityRecord in records.Entities)
            {
                _log.Info(
                    $"{entityRecord.LogicalName} {++recordCount} of {records.Entities.Count} : {entityRecord.Id}");
                _crmService.Create(entityRecord);
            }

            _log.Info("Done!");
        }

        private EntityMetadata GetMetadata(string entityName)
        {
            var request = new RetrieveEntityRequest {EntityFilters = EntityFilters.Entity, LogicalName = entityName};
            var response = (RetrieveEntityResponse) _crmService.Execute(request);
            return response.EntityMetadata;
        }
    }
}