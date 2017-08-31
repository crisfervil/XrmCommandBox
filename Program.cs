using System;
using System.Collections.Generic;
using CommandLine;

namespace DynamicsDataTools
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

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
            new ExportTool().Run(opts);
            return 0;
        }

        private static int HandleErrors(IEnumerable<Error> errors)
        {
            return -1;
        }
    }
}
