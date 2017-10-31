using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using CommandLine.Text;

namespace DocGenerator
{
    class Program
    {
        class Documentation
        {
            public List<CommandInfo> Commands { get; set; }
        }

        class CommandInfo
        {
            public string Name { get; set; }
            public string HelpText { get; set; }
            public string Summary { get; set; }
            public string Remarks { get; set; }
            public List<CommandOption> Options { get; set; }
            public List<string> Examples { get; set; }
            public string ApplicationAlias { get; set; }
        }
        

        class CommandOption
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
            public string HelpText { get; set; }
            public string Summary { get; set; }
            public string Remarks { get; set; }
        }

        static void Main(string[] args)
        {

            // the first parameter contains the assembly with the command options
            var assemblyPath = System.IO.Path.GetFullPath(args[0]);
            var outputPath = args.Length>1? System.IO.Path.GetFullPath(args[1]) : "doc.json";
            Console.WriteLine(assemblyPath);

            var assembly =  Assembly.LoadFile(assemblyPath);

            var optionTypes =  assembly.ExportedTypes.Where(t => t.GetCustomAttribute(typeof(VerbAttribute)) != null);

            var doc = new Documentation() { Commands = new List<CommandInfo>() };

            foreach (var optionType in optionTypes)
            {
                var verbAttrs = optionType.GetCustomAttribute<VerbAttribute>();
                var commandInfo = new CommandInfo { Name = verbAttrs.Name,
                                                    HelpText = verbAttrs.HelpText,
                                                    Options = new List<CommandOption>()
                                                    };


                // get the properties with the option attribute
                var options = optionType.GetProperties().Where(x => x.GetCustomAttribute(typeof(OptionAttribute)) != null);
                var optionAttrs = options.Select(x => x.GetCustomAttribute<OptionAttribute>());

                foreach (var optionAttr in optionAttrs)
                {
                    if (!optionAttr.Hidden)
                    {
                        commandInfo.Options.Add(new CommandOption() { ShortName=optionAttr.ShortName, LongName=optionAttr.LongName, HelpText=optionAttr.HelpText });
                    }
                }

                var usageProperties = optionType.GetProperties().Where(x => x.GetCustomAttribute(typeof(UsageAttribute)) != null).ToList();
                
                if(usageProperties.Count > 0)
                {
                    var usageProperty = usageProperties[0];
                    commandInfo.Examples = new List<string>();

                    var usageAttr = usageProperty.GetCustomAttribute<UsageAttribute>();
                    var commandExamples = (IEnumerable<Example>)usageProperty.GetMethod.Invoke(null, null);

                    foreach (var example in commandExamples)
                    {
                        commandInfo.Examples.Add(example.HelpText);
                    }

                }

                doc.Commands.Add(commandInfo);
            }

            System.IO.File.WriteAllText(outputPath, JsonConvert.SerializeObject(doc, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling=NullValueHandling.Ignore }), Encoding.Default);
        }
    }
}
