using CommandLine;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox
{
    public class CommonOptions
    {
        [Option('b', "debug-brk", HelpText = "Launches the debugger before running the selected command", Hidden = true, Default = false)]
        public bool DebugBreak { get; set; }

        [Option('f', "config-file", HelpText = "Xml file containing the command options")]
        public string ConfigFile { get; set; }

        [Option('l', "log-level", HelpText = "Sets the current logging output. Can be Debug, Info, Error", Default = LogLevels.Info )]
        public LogLevels LogLevel { get; set; } = LogLevels.Info;
    }
}