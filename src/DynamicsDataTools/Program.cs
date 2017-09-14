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
                if(ex.InnerException!= null) Log.Error(ex.InnerException);
                Log.Error(ex.ToString());
                Log.Info($"Unexpected error: {ex.Message}");
                Environment.Exit(-1);
            }
        }

        private static void InitConnection(CommonOptions options)
        {
            _crmService = new ConnectionBuilder(Log).GetConnection(options.ConnectionName);
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
