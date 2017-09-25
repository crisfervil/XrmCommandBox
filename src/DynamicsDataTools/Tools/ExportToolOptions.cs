using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace DynamicsDataTools.Tools
{
    [Verb("export", HelpText="Exports an entity or query from CRM to a file")]
    public class ExportToolOptions : CommonOptions
    {
        [Option("recordNumber", HelpText = "Adds the record number to each exported record")]
        public bool RecordNumber { get; set; }

        [Option("file", HelpText="Path of the file where to save the exported data")]
        public string File { get; set; }

        [Option("entity", HelpText = "Name of the entity you want to export")]
        public string EntityName { get; set; }

        [Option("fetchfile", HelpText = "Name of the file containing que FetchXml query to retrieve the data")]
        public string FetchFile { get; set; }

        [Usage(ApplicationAlias = "dynamicsdatatools")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Simple Export", new ExportToolOptions() { File="Accounts.xml", ConnectionName = "DEV", EntityName = "account"});
            }
        }
    }
}
