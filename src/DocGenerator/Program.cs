using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using Newtonsoft.Json;

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
        }
        

        class CommandOption
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
            public string Description { get; set; }
            public string Summary { get; set; }
            public string Remarks { get; set; }
        }

        static void Main(string[] args)
        {

            // the first parameter contains the assembly with the command options
            var assemblyPath = System.IO.Path.GetFullPath(args[0]);
            Console.WriteLine(assemblyPath);

            var assembly =  Assembly.LoadFile(assemblyPath);

            var optionTypes =  assembly.ExportedTypes.Where(t => t.GetCustomAttribute(typeof(VerbAttribute)) != null);

            var doc = new Documentation() { Commands = { } };

            foreach (var optionType in optionTypes)
            {
                var verbAttrs = optionType.GetCustomAttribute<VerbAttribute>();
                var commandInfo = new CommandInfo { Name = verbAttrs.Name,
                                                    HelpText = verbAttrs.HelpText };
                doc.Commands.Add(commandInfo);
            }

            System.IO.File.WriteAllText("doc.json", JsonConvert.SerializeObject(doc, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling=NullValueHandling.Ignore }), Encoding.UTF8);
        }
    }
}
