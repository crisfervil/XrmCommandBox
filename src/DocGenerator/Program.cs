using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine;
using Newtonsoft.Json;
using CommandLine.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

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
            public List<CommandExample> Examples { get; set; }
            public string ApplicationAlias { get; set; }
        }
        

        class CommandOption
        {
            public string LongName { get; set; }
            public string ShortName { get; set; }
            public string HelpText { get; set; }
            public string Summary { get; set; }
            public string Remarks { get; set; }
            public string Example { get; set; }
            public bool Hidden { get; set; }
            public bool Required { get; set; }
        }

        class CommandExample
        {
            public string ApplicationAlias { get; set; }
            public string HelpText { get; set; }
            public List<CommandExampleParam> Values { get; set; }
        }

        class CommandExampleParam
        {
            public bool IsDefault { get; set; }
            public string OptionLongName { get; set; }
            public string ParamValue { get; set; }
        }

        static void Main(string[] args)
        {

            // the first parameter contains the assembly with the command options
            var assemblyPath = Path.GetFullPath(args[0]);
            var outputPath = args.Length>1? Path.GetFullPath(args[1]) : "doc.json";
            Console.WriteLine(assemblyPath);

            // try to load the documentation xml
            XDocument xmlDoc = null;
            var xmlDocFile = Path.ChangeExtension(assemblyPath, "xml");
            if (File.Exists(xmlDocFile))
            {
                xmlDoc = XDocument.Load(xmlDocFile,LoadOptions.PreserveWhitespace);
            }

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
                var optionAttrs = options.Select(x => new { Property=x, OptionAttribute= x.GetCustomAttribute<OptionAttribute>() });

                foreach (var commandOption in optionAttrs)
                {

                    var newCommandOption = new CommandOption()
                    {
                        ShortName = commandOption.OptionAttribute.ShortName,
                        LongName = commandOption.OptionAttribute.LongName,
                        HelpText = commandOption.OptionAttribute.HelpText,
                        Hidden = commandOption.OptionAttribute.Hidden,
                        Required = commandOption.OptionAttribute.Required
                    };

                    // add xml info
                    if(xmlDoc != null)
                    {
                        var docXPathRemarksSelector = $"doc/members/member[@name='P:{commandOption.Property.DeclaringType.FullName}.{commandOption.Property.Name}']/remarks";
                        var remarksNode = xmlDoc.XPathSelectElement(docXPathRemarksSelector);
                        if (remarksNode != null)
                        {
                            newCommandOption.Remarks = String.Concat(remarksNode.Nodes());
                        }

                        var docXPathExampleSelector = $"doc/members/member[@name='P:{commandOption.Property.DeclaringType.FullName}.{commandOption.Property.Name}']/example";
                        var exampleNode = xmlDoc.XPathSelectElement(docXPathExampleSelector);
                        if (exampleNode != null)
                        {
                            newCommandOption.Example = String.Concat(exampleNode.Nodes());
                        }
                    }

                    commandInfo.Options.Add(newCommandOption);
                }

                var usageProperties = optionType.GetProperties().Where(x => x.GetCustomAttribute(typeof(UsageAttribute)) != null).ToList();
                
                if(usageProperties.Count > 0)
                {
                    var usageProperty = usageProperties[0];

                    var usageAttr = usageProperty.GetCustomAttribute<UsageAttribute>();
                    commandInfo.ApplicationAlias = usageAttr.ApplicationAlias;
                    commandInfo.Examples = new List<CommandExample>();

                    var commandExamples = (IEnumerable<Example>)usageProperty.GetMethod.Invoke(null, null);

                    foreach (var example in commandExamples)
                    {
                        var commandExample = new CommandExample() { ApplicationAlias = commandInfo.ApplicationAlias, Values = new List<CommandExampleParam>(), HelpText = example.HelpText };
                        foreach (var optionAttr in optionAttrs)
                        {
                            // get the property vaue
                            var propValue = optionAttr.Property.GetMethod.Invoke(example.Sample, null);
                            if (propValue != null)
                            {
                                var isDefault = optionAttr.OptionAttribute.Default != null && optionAttr.OptionAttribute.Default.ToString() == propValue.ToString();
                                commandExample.Values.Add(new CommandExampleParam { OptionLongName = optionAttr.OptionAttribute.LongName, ParamValue = propValue.ToString(), IsDefault=isDefault });
                            }
                        }
                        commandInfo.Examples.Add(commandExample);
                    }
                }

                doc.Commands.Add(commandInfo);
            }

            File.WriteAllText(outputPath, JsonConvert.SerializeObject(doc, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings() { NullValueHandling=NullValueHandling.Ignore }), Encoding.Default);
        }
    }
}
