using System;
using CommandLine;
using log4net;
using log4net.Config;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using XrmCommandBox.Tools;

namespace XrmCommandBox
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

                Parser.Default.ParseArguments<ExportToolOptions, ImportToolOptions>(args)
                    .MapResult((ImportToolOptions opts) => RunImportAndReturnExitCode(opts),
                                (ExportToolOptions opts) => RunExportAndReturnExitCode(opts),
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

        private static int RunImportAndReturnExitCode(ImportToolOptions opts)
        {
            Init(opts);
            new ImportTool(Log, _crmService).Run(opts);
            return 0;
        }

        private static int RunExportAndReturnExitCode(ExportToolOptions opts)
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
