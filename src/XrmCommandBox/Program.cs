using System;
using System.Diagnostics;
using System.Reflection;
using CommandLine;
using log4net;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            // Don't add any code to the Main function. It's just a wrapper
            var returnValue = Run(args);
            Environment.Exit(returnValue);
        }

        public static int Run(string[] args)
        {
            var returnValue = 0;
            try
            {
                // handle debugging
                if (Array.IndexOf(args, "--debug-brk") != -1)
                    Debugger.Launch();

                // Configure Debug Listeners
                Debug.Listeners.AddRange(new TraceListener[] {new TextWriterTraceListener(Console.Out)});

                // Get Available Commands
                var commandTypes = Helper.GetTypesWithAttribute(typeof(VerbAttribute));

                // Parse commands
                var parsedResult = Parser.Default.ParseArguments(args, commandTypes);

                // The command was successfully parsed
                if (parsedResult.Tag == ParserResultType.Parsed)
                {
                    // Get the command handler for this command
                    var parsedCommand = (Parsed<object>) parsedResult;
                    var commandOptions = parsedCommand.Value;
                    var commandOptionsType = commandOptions.GetType();
                    var handlerAttr = commandOptionsType.GetCustomAttribute<HandlerAttribute>();

                    var crmCommandCommonOptions = commandOptions as CrmCommonOptions;
                    if (crmCommandCommonOptions != null)
                    {
                        // Configure Services. Services are instances of objects that need to be injected in the constructors of the tools
                        // By doing this, we allow tools to optionally have common shared objects passed in the constructor
                        Func<object> getConnection = () => crmCommandCommonOptions.GetConnection();
                        if (!Helper.ServicesMap.ContainsKey(typeof(IOrganizationService)))
                            Helper.ServicesMap.Add(typeof(IOrganizationService), getConnection);
                    }

                    var commandCommonOptions = commandOptions as CommonOptions;
                    if (commandCommonOptions != null)
                    {
                        // Configure the logging
                        commandCommonOptions.SetLogLevel();

                        // Configure parameter defined in the config file
                        new CommandOptionsSerializer().Deserialize(commandCommonOptions);
                    }

                    // create instance of the tool to run
                    var toolInstance = Helper.CreateInstance(handlerAttr.HandlerType);

                    // run it                    
                    Helper.RunTool(toolInstance, commandOptions);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) Log.Error(ex.InnerException);
                Log.Error($"Unexpected error: {ex.Message}");
                Log.Error(ex);
                returnValue = -1;
            }

            return returnValue;
        }
    }
}