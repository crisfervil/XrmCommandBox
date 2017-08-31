using System;
using System.Collections.Generic;
using CommandLine;
using log4net;
using log4net.Config;

namespace DynamicsDataTools
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();

            BasicConfigurator.Configure();

            Parser.Default.ParseArguments<DefaultVerb, ExportOptions>(args)
                .MapResult(
                    (DefaultVerb opts) => RunNoVerb(opts),
                    (ExportOptions opts) => RunExportAndReturnExitCode(opts),
                    HandleErrors);
        }

        private static int RunNoVerb(DefaultVerb arg)
        {
            return 0;
        }

        private static int RunExportAndReturnExitCode(ExportOptions opts)
        {
            new ExportTool(log).Run(opts);
            return 0;
        }

        private static int HandleErrors(IEnumerable<Error> errors)
        {
            return -1;
        }
    }
}
