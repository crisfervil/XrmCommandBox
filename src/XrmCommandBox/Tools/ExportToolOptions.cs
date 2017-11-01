using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace XrmCommandBox.Tools
{
    [Verb("export", HelpText = "Exports an entity or query from CRM to a file")]
    [Handler(typeof(ExportTool))]
    public class ExportToolOptions : CrmCommonOptions
    {
        [Option('r', "row-number", HelpText = "Adds the row number to each exported record", Default = false)]
        public bool RowNumber { get; set; }

        [Option('f', "file", HelpText = "Path of the file where to save the exported data")]
        public string File { get; set; }

        [Option('e', "entity", HelpText = "Name of the entity you want to export", SetName = "entity")]
        public string EntityName { get; set; }

        [Option('q', "fetch-query", HelpText = "Fetch query to retrieve the records to export", SetName = "fetch")]
        public string FetchQuery { get; set; }

        [Option('s', "page-size", HelpText = "Number of records to retrieve from a page", SetName = "entity", Default = 5000)]
        public int PageSize { get; set; } = 5000;

        [Option('p', "page", HelpText = "Page of records to retrieve", SetName = "entity", Default = 1)]
        public int Page { get; set; } = 1;

        [Usage(ApplicationAlias = "xrm")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Export all the existing accounts to an Accounts.xml file",
                    new ExportToolOptions { File = "Accounts.xml", ConnectionName = "DEV", EntityName = "account" });
                yield return new Example("Export all the existing contacts returned by a fetch query to the contacts.xml file",
                    new ExportToolOptions { File = "Contacts.xml", ConnectionName = "DEV", ConfigFile="ContactsQuery.xml" });
            }
        }
    }
}