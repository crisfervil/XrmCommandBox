using System.Collections.Generic;
using System.Xml.Serialization;
using CommandLine;

namespace XrmCommandBox.Tools.DataLoader
{
    [Verb("import", HelpText = "Imports data to Dynamics")]
    [Handler(typeof(DataLoaderTool))]
    public class DataLoaderToolOptions : CrmCommonOptions
    {
        [Option('f', "file", HelpText = "File containing the configuration of the data to import")]
        public string File { get; set; }

        [Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }

        public IEnumerable<TableMappingOptions> MappingOptions;
    }
}