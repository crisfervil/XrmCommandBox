using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmCommandBox.Tools
{
    [Verb("delete", HelpText = "Delete all the records returned by the specified query")]
    public class DeleteToolOptions : CommonOptions
    {
        [Option("fetchquery", HelpText = "Fetch query to retrieve the records to delete")]
        public string FetchQuery { get; set; }

    }
}
