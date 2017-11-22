using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            var updatedWebResources = new List<Guid>();
            foreach (var fileToUpdate in _filesToUpdate)
            {
                var webResourceName = fileToUpdate.Substring(Environment.CurrentDirectory.Length+1);
                webResourceName = webResourceName.Replace("\\","/");
                _log.Info($"Updating {webResourceName}...");

                var webresourceId = GetId(webResourceName);
                if (webresourceId.HasValue)
                {
                    Update(webresourceId.Value, webResourceName, fileToUpdate);
                    updatedWebResources.Add(webresourceId.Value);
                }
                else
                {
                    _log.Info($"{webResourceName} doesn't exist in the environment");
                }
            }

            if (updatedWebResources.Count > 0) {
                _log.Info("Publishing customizations...");
                PublishAllCustomizations(updatedWebResources);
                _log.Info("Done. Waiting for more changes....");
            }

            _filesToUpdate.Clear();
        }

        private void PublishAllCustomizations(IEnumerable<Guid> webResourcesIds)
        {

            var webResourcesXml = webResourcesIds.Select(x => $"<webresource>{{{x}}}</webresource>").ToList();

            var importExportXml = $"<importexportxml>" +
                                        "<webresources>" + 
                                        string.Join(Environment.NewLine,webResourcesXml) + 
                                        "</webresources>" +
                                    "</importexportxml>";
            _log.Debug(importExportXml);
            _crmService.Execute(new PublishXmlRequest() { ParameterXml = importExportXml });
        }

        private void Update(Guid webResourceId, string webResourceName, string filePath)
        {
            var content = GetContent(filePath);
            var webResource = new Entity("webresource", webResourceId);
            webResource["content"] = content;
            _log.Debug($"Updating {webResourceName}:{webResourceId}");
            _crmService.Update(webResource);
            _log.Debug("done");
        }

        private object GetContent(string filePath)
        {
            byte[] byteContent = File.ReadAllBytes(filePath);
            var strContent = Convert.ToBase64String(byteContent);
            return strContent;
        }

        private Guid? GetId(string webresourceName)
        {
            var qry = new QueryByAttribute("webresource");
            qry.ColumnSet = new ColumnSet("webresourceid");
            qry.Attributes.Add("name");
            qry.Values.Add(webresourceName);
            var found = _crmService.RetrieveMultiple(qry);

            if (found.Entities.Count > 1) _log.Error("More than one webresource with that name");
            return found.Entities.Count == 1 ? found.Entities[0].Id :  default(Guid?);
        }

        private void File_Changed(object sender, FileSystemEventArgs e)
        {
            _log.Debug($"Change detected: {e.ChangeType} : {e.FullPath}");
            if (!_filesToUpdate.Contains(e.FullPath))
            {
                _filesToUpdate.Add(e.FullPath);
            }
        }

    }
}