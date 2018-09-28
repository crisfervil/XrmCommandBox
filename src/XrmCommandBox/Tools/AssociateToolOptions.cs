using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace XrmCommandBox.Tools
{
    [Verb("associate", HelpText = "Imports N to N relationshipts to CRM")]
    [Handler(typeof(AssociateTool))]
    public class AssociateToolOptions : CrmCommonOptions
    {
        /// <remarks>The input file should contain a data table named as the intersect entity name, with at least one column for each of the related records. Any additional attributes will be ignored. 
        /// You can use the lookup tool to resolve the GUIDs of the key attributes</remarks>
        /// <example>The intersect entity name for the relationship between Accounts and Leads is accountsleads, so the input xml should be:
        /// 
        /// ``` xml
        /// <datatable name="accountsleads">
        ///     <row>
        ///         <accountid>A1E29A4E-78CE-4164-BC26-A93D8B449F87</accountid>
        ///         <leadid>1B3C099D-BBDC-4C9E-8668-BF53C5871A2B</leadid>
        ///         <accountid1.name>contoso</accountid1.name>
        ///         <leadid1.name>contoso lead</leadid1.name>
        ///     </row>
        /// </datatable>
        /// ```
        /// </example>
        [Option('f', "file", HelpText = "File containing the data to import")]
        public string File { get; set; }

        [Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }

		[Option('o', "file-options", HelpText = "Options regarding the data file")]
		public string FileOptions { get; set; }

		[Option('n', "entity", HelpText = "Name of the entity where to load the data")]
		public string EntityName { get; set; }

		public IEnumerable<LookupToolOptions> Lookups { get; set; }

		[Usage(ApplicationAlias = "xrm")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Associate the Accounts and Leads contained in the AcccountLeads.xml file",
                    new AssociateToolOptions { File = "AccountLeads.xml", ConnectionName = "Url=http://myCrmServer/myorg" });
            }
        }
    }
}