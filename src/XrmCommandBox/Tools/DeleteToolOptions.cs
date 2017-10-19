using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("delete", HelpText = "Delete all the records returned by the specified query")]
    [Handler(typeof(DeleteTool))]
    public class DeleteToolOptions : CrmCommonOptions
    {
        [Option('q', "fetch-query", HelpText = "Fetch query to retrieve the records to delete")]
        public string FetchQuery { get; set; }

        [Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }
    }
}