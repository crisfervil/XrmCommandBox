using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox.Tools
{
    public class SolutionExportTool
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(SolutionExportTool));
        private readonly IOrganizationService _crmService;

        public SolutionExportTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(SolutionExportToolOptions options)
        {
            _log.Info("Running Solution Export tool...");

            _log.Debug($"Solution Name: {options.SolutionName}");

            var fileName = $"{options.SolutionName}.zip";
            _log.Debug($"File Name: {fileName}");

            var request = new ExportSolutionRequest
            {
                // TODO: Add more options
                SolutionName = options.SolutionName
            };

            var response = (ExportSolutionResponse)_crmService.Execute(request);

            // TODO: Create log file

            File.WriteAllBytes(fileName, response.ExportSolutionFile);

            _log.Info("done");
        }
    }
}
