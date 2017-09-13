using System;
using CommandLine;
using log4net;
using log4net.Config;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace DynamicsDataTools
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static IOrganizationService _crmService = null;

        static void Main(string[] args)
        {
            // Log configuration
            BasicConfigurator.Configure();

            try
            {

                Parser.Default.ParseArguments<DefaultVerb, ExportOptions>(args)
                    .MapResult((DefaultVerb opts) => RunNoVerb(opts),
                                (ExportOptions opts) => RunExportAndReturnExitCode(opts),
                                HandleErrors);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                Log.Info($"Unexpected error: {ex.Message}");
                Environment.Exit(-1);
            }
        }

        private static void InitConnection(CommonOptions options)
        {
            Log.Debug("Connecting to CRM...");
            _crmService = new ConnectionBuilder().GetConnection(options.ConnectionName);
            var client = _crmService as CrmServiceClient;
            if (client != null)
            {
                if (!string.IsNullOrEmpty(client.LastCrmError))
                {
                    var url = client.CrmConnectOrgUriActual != null ? client.CrmConnectOrgUriActual.ToString() : "CRM";
                    throw new Exception($"Error when connecting to {url} - {client.LastCrmError}");
                }
                Log.Info($"Connected to: {client.CrmConnectOrgUriActual}");
            }
        }

        private static void Init(CommonOptions options)
        {
            if (options.DebugBreak)
            {
                System.Diagnostics.Debugger.Launch();
            }
            InitConnection(options);
        }

        private static int RunNoVerb(DefaultVerb arg)
        {
            return 0;
        }

        private static int RunExportAndReturnExitCode(ExportOptions opts)
        {
            Init(opts);
            new ExportTool(Log,_crmService).Run(opts);
            return 0;
        }

        private static int HandleErrors(IEnumerable<Error> errors)
        {
            return -1;
        }
    }
}
