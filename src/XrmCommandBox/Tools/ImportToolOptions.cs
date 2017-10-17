using System;
using System.Collections;
using System.Collections.Generic;
using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("import", HelpText = "Imports information from a file to Dynamics")]
    [Handler(typeof(ImportTool))]
    public class ImportToolOptions : CommonOptions
    {
        [Option('f', "file", HelpText = "File containing the data to import")]
        public string File { get; set; }

        [Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }

        [Option('m', "match-attributes", HelpText = "Attributes used to know if the record exists. Default is id attribute")]
        public IEnumerable<string> MatchAttributes { get; set; }

    }
}