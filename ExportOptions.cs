using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace DynamicsDataTools
{
    [Verb("export", HelpText="Exports an entity or query from CRM to a file")]
    class ExportOptions
    {
        [Option("file", Required = true)]
        public string File { get; set; }

        [Option("connection", Required = true)]
        public string ConnectionName { get; set; }


        [Usage(ApplicationAlias = "dynamicsdatatools")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Simple Export", new ExportOptions() { File="Accounts.xml", ConnectionName = "DEV"});
            }
        }
    }
}
