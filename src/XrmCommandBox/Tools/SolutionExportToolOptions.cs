using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("solution-export", HelpText = "Export the specified solution to a .zip file")]
    [Handler(typeof(SolutionExportTool))]
    public class SolutionExportToolOptions : CrmCommonOptions
    {
        [Option('s', "solution-name", Required = true, HelpText = "Unique name of the solution to export")]
        public string SolutionName { get; set; }
    }
}