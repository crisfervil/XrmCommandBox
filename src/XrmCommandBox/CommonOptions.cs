using CommandLine;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox
{
    public class CommonOptions
    {
        [Option('c', "connection", Required = true, HelpText ="Connection string, or name of a connection string to use")]
        public string ConnectionName { get; set; }

        public IOrganizationService Connection { get; set; }

        [Option('b', "debug-brk", HelpText = "Launches the debugger before running the selected command")]
        public bool DebugBreak { get; set; }

        [Option('f', "config-file", HelpText = "Xml file containing the command options")]
        public string ConfigFile { get; set; }

        [Option('e', "continue-on-error", HelpText = "Continue if there's an error while processing the command")]
        public bool ContinueOnError { get; set; }

        [Option('l', "log-level", HelpText = "Sets the current logging output. Can be Debug, Info, Error")]
        public LogLevels LogLevel { get; set; } = LogLevels.Info;
    }
}
