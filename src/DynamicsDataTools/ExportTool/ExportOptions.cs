using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace DynamicsDataTools.ExportTools
{
    [Verb("export", HelpText="Exports an entity or query from CRM to a file")]
    public class ExportOptions : CommonOptions
    {
        [Option("recordNumber", HelpText = "Adds the record number next to each record")]
        public bool RecordNumber { get; set; }

        [Option("file")]
        public string File { get; set; }

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
