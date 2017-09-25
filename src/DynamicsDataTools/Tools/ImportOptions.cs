using CommandLine;

namespace DynamicsDataTools.Tools
{
    public class ImportOptions : CommonOptions
    {
        [Option("file", HelpText = "File containing the data to import")]
        public string File { get; set; }

    }
}
