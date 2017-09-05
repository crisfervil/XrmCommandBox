using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace DynamicsDataTools
{
    [Verb("export", HelpText="Exports an entity or query from CRM to a file")]
    class ExportOptions : CommonOptions
    {
        [Option("file")]
        public string File { get; set; }

        [Option("connection", Required = true)]
        public string ConnectionName { get; set; }

        [Option("entity")]
        public string EntityName { get; set; }

        [Option("fetchfile")]
        public string FetchFile { get; set; }

        [Usage(ApplicationAlias = "dynamicsdatatools")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Simple Export", new ExportOptions() { File="Accounts.xml", ConnectionName = "DEV", EntityName = "account"});
            }
        }
    }
}
