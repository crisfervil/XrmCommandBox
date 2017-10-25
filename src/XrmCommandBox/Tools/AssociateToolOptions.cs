using System.Collections.Generic;
using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("associate", HelpText = "Imports N to N relationshipts to CRM")]
    [Handler(typeof(AssociateTool))]
    public class AssociateToolOptions : CrmCommonOptions
    {
        [Option('f', "file", HelpText = "File containing the data to import")]
        public string File { get; set; }

        [Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }
    }
}