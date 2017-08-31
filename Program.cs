using System;
using System.Collections.Generic;
using CommandLine;

namespace DynamicsDataTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ExportOptions>(args)
                .MapResult(
                    RunExportAndReturnExitCode,
                    HandleErrors);

        }

        private static int HandleErrors(IEnumerable<Error> errors)
        {
            return -1;
        }

        private static int RunExportAndReturnExitCode(ExportOptions opts)
        {
            Console.WriteLine("Running Export...");
            return 0;
        }
    }
}
