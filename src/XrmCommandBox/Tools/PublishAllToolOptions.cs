using CommandLine;

namespace XrmCommandBox.Tools
{
    [Verb("publish-all", HelpText = "Publishes all existing customizations in the environment")]
    [Handler(typeof(PublishAllTool))]
    public class PublishAllToolOptions : CrmCommonOptions
    {
    }
}