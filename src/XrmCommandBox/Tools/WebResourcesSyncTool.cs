using log4net;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace XrmCommandBox.Tools
{
    /// <summary>
    /// This tool uses the Associate request to create relationships between 2 entity records
    /// </summary>
    public class WebResourcesSyncTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ImportTool));

        private List<string> _filesToUpdate = new List<string>();

        public WebResourcesSyncTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(WebResourcesSyncToolOptions options)
        {
            var sw = Stopwatch.StartNew();

            _log.Info("Running WebResources Sync Tool...");

            if (options.Watch)
            {

                FileSystemWatcher watcher = new FileSystemWatcher
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                    Path = Environment.CurrentDirectory,
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true
                };
                watcher.Changed += File_Changed;

                _log.Info("Waiting for changes....");
                _log.Info("Ctrl + C to end the program");

                while (true)
                {
                    UpdateFiles();
                    System.Threading.Thread.Sleep(1000);
                }

            }
            sw.Stop();
            _log.Info($"Done!");
        }


        private void UpdateFiles()
        {
            foreach (var fileToUpdate in _filesToUpdate)
            {
                _log.Info($"Updating {fileToUpdate}...");
                
            }

            _filesToUpdate.Clear();
        }

        private void File_Changed(object sender, FileSystemEventArgs e)
        {
            _log.Info($"Change detected: {e.ChangeType} : {e.FullPath}");
            if (!_filesToUpdate.Contains(e.FullPath))
            {
                _filesToUpdate.Add(e.FullPath);
            }
        }

    }
}