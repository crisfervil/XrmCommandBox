using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace XrmCommandBox.Tools
{
    [Verb("metadata-export", HelpText = "Metadata Export tool for dynamics 365")]
    [Handler(typeof(MetadataExportTool))]
    public class MetadataExportToolOptions : CrmCommonOptions
    {
        [Option('f', "file", HelpText = "File name where to export the data to")]
        public string File { get; set; }

        [Option('e', "entity", HelpText = "Name of the entity which metadata you want to export")]
        public string Entity { get; set; }

        [Usage(ApplicationAlias = "xrm")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Exports the Account metadata to the accounts.json file",
                    new MetadataExportToolOptions { Entity = "account", File = "AccountLeads.xml", ConnectionName = "Url=http://myCrmServer/myorg" });
            }
        }
    }
}