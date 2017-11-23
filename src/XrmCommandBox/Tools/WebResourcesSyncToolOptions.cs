using CommandLine;
using System.Collections.Generic;

namespace XrmCommandBox.Tools
{
    [Verb("wr-sync", HelpText = "Synchronizes Web Resoources with local directories")]
    [Handler(typeof(WebResourcesSyncTool))]
    public class WebResourcesSyncToolOptions : CrmCommonOptions
    {
        /// <summary>
        /// The tool will monitor any changes in the current directory and when a file changes will update CRM accordingly. 
        /// If there is a web resource named like the relative path of the file, then it will be updated with the file contents. 
        /// If it does not exist then a new web resource will be created. 
        /// Only valid file extensions will be synchronized (*.html, *.css, *.js, *.xml, *.png, *.jpg, *.gif, *.xap, *.xsl, *.ico)
        /// </summary>
        [Option('w', "watch", HelpText = "Initiates the command in watch mode")]
        public bool Watch { get; set; }

        [Option('n', "name-filters", HelpText = "Only the web resources beggining with thesee filters are going to be included")]
        public IEnumerable<string> NamePrefixes { get; set; }
    }
}