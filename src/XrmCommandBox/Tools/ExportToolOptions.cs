using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace XrmCommandBox.Tools
{
    [Verb("export", HelpText = "Exports an entity or query from CRM to a file")]
    [Handler(typeof(ExportTool))]
    public class ExportToolOptions : CommonOptions
    {
        [Option("recordNumber", HelpText = "Adds the record number to each exported record")]
        public bool RecordNumber { get; set; }

        [Option("file", HelpText = "Path of the file where to save the exported data")]
        public string File { get; set; }

        [Option("entity", HelpText = "Name of the entity you want to export")]
        public string EntityName { get; set; }

        [Option("fetch-query", HelpText = "Fetch query to retrieve the records to export")]
        public string FetchQuery { get; set; }

        [Usage(ApplicationAlias = "dynamicsdatatools")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Simple Export",
                    new ExportToolOptions {File = "Accounts.xml", ConnectionName = "DEV", EntityName = "account"});
            }
        }
    }
}