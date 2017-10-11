using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox.Tools
{
    public class PublishAllTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ExportTool));

        public PublishAllTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(PublishAllToolOptions options)
        {
            _log.Info("Running Publish All Tool...");

            _crmService.Execute(new PublishAllXmlRequest());

            _log.Info("Done!");
        }
    }
}