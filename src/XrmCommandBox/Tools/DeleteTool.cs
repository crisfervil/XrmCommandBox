using System;
using System.Activities.Statements;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmCommandBox.Tools
{
    public class DeleteTool
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(DeleteTool));
        private readonly IOrganizationService _crmService;

        public DeleteTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(DeleteToolOptions options)
        {
            _log.Info("Running Delete tool...");

            ValidateOptions(options);

            _log.Debug("Executing query...");
            var foundRecords = GetRecords(options);
            _log.Info($"{foundRecords.Entities.Count} records found");

            // Convert to a data table
            var statistics = DeleteRecords(foundRecords, options.ContinueOnError);
            var withErrors = statistics.Item2 > 0 ? "with errors" : "successfully";
            _log.Info($"Completed {withErrors}. Processed: {statistics.Item1} Errors: {statistics.Item2}");
        }

        private Tuple<int,int> DeleteRecords(EntityCollection foundRecords, bool continueOnError)
        {
            var processedCount = 0;
            var errorsCount = 0;
            _log.Debug("Starting to delete records...");
            foreach (var recordToDelete in foundRecords.Entities)
            {
                try
                {
                    _log.Info($"Deleting {recordToDelete.LogicalName} {++processedCount} of {foundRecords.Entities.Count}: {recordToDelete.Id}");
                    _crmService.Delete(recordToDelete.LogicalName, recordToDelete.Id);                    
                }
                catch(Exception ex)
                {
                    errorsCount++;
                    _log.Error("Unexpected error while deleting records.");
                    _log.Error(ex);
                    if (!continueOnError) throw;
                }
            }
            return Tuple.Create(processedCount, errorsCount);
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
