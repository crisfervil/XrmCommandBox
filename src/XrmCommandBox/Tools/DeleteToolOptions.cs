using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace XrmCommandBox.Tools
{
    [Verb("delete", HelpText = "Delete all the records returned by the specified query")]
    [Handler(typeof(DeleteTool))]
    public class DeleteToolOptions : CommonOptions
    {
        [Option("fetch-query", HelpText = "Fetch query to retrieve the records to delete")]
        public string FetchQuery { get; set; }
    }
}
