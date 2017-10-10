using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("solution-import", HelpText = "Imports the specified solution .zip file into CRM")]
    [Handler(typeof(SolutionImportTool))]
    public class SolutionImportToolOptions : CommonOptions
    {
        [Option('s', "solution-file", Required = true, HelpText = ".zip file containing the solution to import")]
        public string SolutionFile { get; set; }

        [Option('a', "async", HelpText = "Indicates wether the import should be performed asynchronously")]
        public bool Async { get; set; }
    }
}