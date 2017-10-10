using System.IO;
using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox.Tools
{
    public class SolutionExportTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(SolutionExportTool));

        public SolutionExportTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(SolutionExportToolOptions options)
        {
            _log.Info("Running Solution Export Tool...");

            _log.Debug($"Solution Name: {options.SolutionName}");

            var fileName = $"{options.SolutionName}.zip";
            _log.Debug($"File Name: {fileName}");

            var request = new ExportSolutionRequest
            {
                // TODO: Add more options
                SolutionName = options.SolutionName
            };

            var response = (ExportSolutionResponse) _crmService.Execute(request);

            _log.Info($"Completed. {response.ExportSolutionFile.Length} bytes retrieved");
            _log.Info("Writing File...");
            File.WriteAllBytes(fileName, response.ExportSolutionFile);

            _log.Info("Done!");
        }
    }
}