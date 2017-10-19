using CommandLine;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox
{
    public class CrmCommonOptions : CommonOptions
    {
        [Option('c', "connection", Required = true,
            HelpText = "Connection string, or name of a connection string to use")]
        public string ConnectionName { get; set; }

        public IOrganizationService Connection { get; set; }
    }
}