using CommandLine;

namespace DynamicsDataTools
{
    class CommonOptions
    {
        [Option("connection", Required = true)]
        public string ConnectionName { get; set; }

        [Option("debug-brk", HelpText = "Launches the debugger before running the selected command")]
        public bool DebugBreak { get; set; }

    }
}
