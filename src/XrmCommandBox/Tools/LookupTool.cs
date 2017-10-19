using System;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace XrmCommandBox.Tools
{
    public class LookupTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(DeleteTool));

        public LookupTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(DeleteToolOptions options)
        {
            _log.Info("Running Lookup Tool...");



            _log.Info("Done!");
        }

    }
}