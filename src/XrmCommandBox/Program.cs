using System;
using CommandLine;
using log4net;
using log4net.Config;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xrm.Sdk;
using XrmCommandBox.Tools;

namespace XrmCommandBox
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            try
            {

                // handle debugging
                if (Array.IndexOf(args, "--debug-brk") != -1)
                {
                    System.Diagnostics.Debugger.Break();
                }

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

                    var commandCommonOptions = commandOptions as CommonOptions;
                    if (commandCommonOptions != null)
                    {
                        // Configure Services. Services are instances of objects that need to be injected in the constructors of the tools
                        // By doing this, we allow tools to optionally have common shared objects passed in the constructor
                        Func<object> getConnection = () => commandCommonOptions.GetConnection();
                        Helper.ServicesMap.Add(typeof(IOrganizationService), getConnection);

                        // Configure the logging
                        commandCommonOptions.ConfigureLog();
                    }

                    // create instance of the tool to run
                    var toolInstance = Helper.CreateInstance(handlerAttr.HandlerType);

                    // run it                    
                    Helper.RunTool(toolInstance, commandOptions);
                }
            }
            catch (Exception ex)
            {
                if(ex.InnerException!= null) Log.Error(ex.InnerException);
                Log.Error($"Unexpected error: {ex.Message}");
                Log.Error(ex);
                Environment.Exit(-1);
            }
        }

    }
}
