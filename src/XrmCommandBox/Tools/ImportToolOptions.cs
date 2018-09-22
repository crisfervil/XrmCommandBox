using System.Collections.Generic;
using System.Xml.Serialization;
using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("import", HelpText = "Imports information from a file to Dynamics")]
    [Handler(typeof(ImportTool))]
    public class ImportToolOptions : CrmCommonOptions
    {
        [Option('f', "file", HelpText = "File containing the data to import")]
        public string File { get; set; }

		[Option('o', "file-options", HelpText = "Options regarding the data file")]
		public string FileOptions { get; set; }

		[Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }

        [Option('m', "match-attributes", HelpText = "Attributes used to know if the record exists. Default is id attribute")]
        public IEnumerable<string> MatchAttributes { get; set; }

		[Option('n', "entity", HelpText = "Name of the entity where to load the data")]
		public string EntityName { get; set; }

		public IEnumerable<LookupToolOptions> Lookups { get; set; }
	}
}