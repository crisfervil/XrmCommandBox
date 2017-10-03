using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("import", HelpText = "Imports information from a file to Dynamics")]
    public class ImportToolOptions : CommonOptions
    {
        [Option("file", HelpText = "File containing the data to import")]
        public string File { get; set; }
    }
}
