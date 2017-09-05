using System;
using CommandLine;
using log4net;
using log4net.Config;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace DynamicsDataTools
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static IOrganizationService CrmService = null;

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
            CrmService = new ConnectionBuilder().GetConnection(options.ConnectionName);
        }

        private static int RunNoVerb(DefaultVerb arg)
        {
            return 0;
        }

        private static int RunExportAndReturnExitCode(ExportOptions opts)
        {
            InitConnection(opts);
            new ExportTool(Log,CrmService).Run(opts);
            return 0;
        }

        private static int HandleErrors(IEnumerable<Error> errors)
        {
            return -1;
        }
    }
}
