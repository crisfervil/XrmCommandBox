using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("lookup", HelpText = "Updated a specific column from a data table querying data in CRM")]
    [Handler(typeof(LookupTool))]
    public class LookupToolOptions : CrmCommonOptions
    {

    }
}