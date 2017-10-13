using System;
using System.IO;
using System.Threading;
using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace XrmCommandBox.Tools
{
    public class SolutionImportTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(SolutionExportTool));

        public SolutionImportTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(SolutionImportToolOptions options)
        {
            _log.Info("Running Solution Import Tool...");

            _log.Debug($"Solution File: {options.SolutionFile}");

            _log.Info($"Reading file {options.SolutionFile}...");

            var fileContents = File.ReadAllBytes(options.SolutionFile);

            _log.Info($"{fileContents.Length} bytes read");

            var importJobId = Guid.NewGuid();
            var request = new ImportSolutionRequest
            {
                // TODO: Add more options
                CustomizationFile = fileContents,
                ImportJobId = importJobId
            };


            if (options.Async)
            {
                _log.Info("Start importing asynchronously...");
                var asyncRequest = new ExecuteAsyncRequest
                {
                    Request = request
                };

                var response = (ExecuteAsyncResponse) _crmService.Execute(asyncRequest);
                AwaitCompletion(response.AsyncJobId);
            }
            else
            {
                _log.Info("Importing synchrounously...");
                var response = (ImportSolutionResponse) _crmService.Execute(request);
                // TODO: Check the response
            }

            _log.Info("Done!");
        }

        private void AwaitCompletion(Guid asyncJobId)
        {
            // TODO: Set the timeout as a parameter option
            var asyncTimeout = 3600; // seconds
            var sleepInterval = 1; // secconds
            var end = DateTime.Now.AddSeconds(asyncTimeout);

            var completed = false;
            while (!completed)
            {
                if (end < DateTime.Now)
                    throw new Exception($"Import Timeout Exceeded: {asyncTimeout}");

                Thread.Sleep(sleepInterval * 1000);
                _log.Debug($"Sleeping for {sleepInterval} seconds");

                Entity asyncOperation;

                try
                {
                    asyncOperation = _crmService.Retrieve("asyncoperation", asyncJobId,
                        new ColumnSet("asyncoperationid", "statuscode", "message"));
                }
                catch (Exception ex)
                {
                    _log.Debug(ex);
                    _log.Warn(ex.Message);
                    continue;
                }

                var statusCode = (OptionSetValue) asyncOperation["statuscode"];
                _log.Debug($"Status Code: {statusCode.Value}");

                switch (statusCode.Value)
                {
                    //Succeeded
                    case 30:
                        completed = true;
                        break;
                    //Pausing //Canceling //Failed //Canceled
                    case 21:
                    case 22:
                    case 31:
                    case 32:
                        throw new Exception($"Solution Import Failed: {statusCode.Value} {asyncOperation["message"]}");
                }
            }
        }
    }
}