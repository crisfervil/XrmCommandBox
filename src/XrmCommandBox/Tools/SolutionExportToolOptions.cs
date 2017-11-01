using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace XrmCommandBox.Tools
{
    [Verb("solution-export", HelpText = "Exports the specified solution to a .zip file")]
    [Handler(typeof(SolutionExportTool))]
    public class SolutionExportToolOptions : CrmCommonOptions
    {
        [Option('s', "solution-name", Required = true, HelpText = "Unique name of the solution to export")]
        public string SolutionName { get; set; }

        [Usage(ApplicationAlias = "xrm")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Export mysolution to the mysolution.zip file in the current directory",
                    new SolutionExportToolOptions { ConnectionName = "TestEnvironment", SolutionName = "mysolution" });
            }
        }
    }
}