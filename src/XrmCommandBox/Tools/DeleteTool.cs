using System;
using System.Activities.Statements;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmCommandBox.Tools
{
    public class DeleteTool
    {
        private readonly ILog _log;
        private readonly IOrganizationService _crmService;

        public DeleteTool(ILog log, IOrganizationService service)
        {
            _log = log;
            _crmService = service;
        }

        public void Run(DeleteToolOptions options)
        {
            _log.Info("Running Export tool...");

            ValidateOptions(options);

            _log.Debug("Executing query...");
            var foundRecords = GetRecords(options);
            _log.Info($"{foundRecords.Entities.Count} records found");

            // Convert to a data table
            DeleteRecords(foundRecords, options.ContinueOnError);

            _log.Info("Completed");
        }

        private void DeleteRecords(EntityCollection foundRecords, bool continueOnError)
        {
            var count = 0;
            _log.Debug("Starting to delete records...");
            foreach (var recordToDelete in foundRecords.Entities)
            {
                try
                {
                    _log.Info($"Deleting {recordToDelete.LogicalName} {++count} of {foundRecords.Entities.Count}: {recordToDelete.Id}");
                    _crmService.Delete(recordToDelete.LogicalName, recordToDelete.Id);                    
                }
                catch(Exception ex)
                {
                    _log.Error(ex);
                    _log.Info("Unexpected error while deleting records.");
                    if (!continueOnError) throw;
                }
            }
        }

        private EntityCollection GetRecords(DeleteToolOptions options)
        {
            var qry = new FetchExpression(options.FetchQuery);
            var foundRecords = _crmService.RetrieveMultiple(qry);
            return foundRecords;
        }


        private void ValidateOptions(DeleteToolOptions options)
        {
            
        }
    }
}
