using CommandLine;

namespace DynamicsDataTools
{
    class CommonOptions
    {
        [Option("debug-brk", HelpText = "Launches the debugger before running the selected command")]
        public bool DebugBreak { get; set; }

    }
}
