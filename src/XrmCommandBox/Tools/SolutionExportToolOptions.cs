using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("solution-export", HelpText = "Export the specified solution to a file")]
    [Handler(typeof(SolutionExportTool))]
    public class SolutionExportToolOptions
    {
        [Option("solution-name", HelpText = "Unique name of the solution to export")]
        public string SolutionName { get; set; }
    }
}