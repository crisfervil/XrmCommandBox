using CommandLine;
using System.Collections.Generic;

namespace XrmCommandBox.Tools
{
    [Verb("lookup", HelpText = "Updates a specific column from a data table querying data in CRM")]
    [Handler(typeof(LookupTool))]
    public class LookupToolOptions : CrmCommonOptions
    {
        [Option('f', "file", HelpText = "File containing the data table with the data")]
        public string File { get; set; }

        [Option('l', "column", Required=true, HelpText = "Name of the column to lookup")]
        public string Column { get; set; }

        [Option('n', "entity", HelpText = "Name of the entity where search for the data")]
        public string EntityName { get; set; }

        [Option('m', "match-attributes", HelpText = "Attributes used to know if the record exists. Default is display attribute")]
        public IEnumerable<string> MatchAttributes { get; set; }

        [Option('y', "match-columns", HelpText = "Columns containing the with the data to lookup. The number of columns must match the number of matching attributes")]
        public IEnumerable<string> MatchColumns { get; set; }

        [Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }

    }
}