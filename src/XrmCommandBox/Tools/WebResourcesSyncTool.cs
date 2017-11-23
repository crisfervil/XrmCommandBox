using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentQueue<string> _filesToUpdate = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<Guid> _webResourcesToPublish = new ConcurrentQueue<Guid>();

        enum WebResourceTypes
        {
            WebPage=1,
            ICO=10,
            StyleSheetCSS=2,
            Script=3,
            Data=4,
            PNG=5,
            JPG=6,
            GIF=7,
            Silverlight=8,
            StyleSheetXSL=9
        }

        private readonly Dictionary<string, WebResourceTypes> _extensionMappings = 
            new Dictionary<string, WebResourceTypes> { { ".htm", WebResourceTypes.WebPage},
                                                       { ".html", WebResourceTypes.WebPage},
                                                       { ".ico", WebResourceTypes.ICO},
                                                       { ".css", WebResourceTypes.StyleSheetCSS},
                                                       { ".js", WebResourceTypes.Script},
                                                       { ".xml", WebResourceTypes.Data},
                                                       { ".png", WebResourceTypes.PNG},
                                                       { ".jpg", WebResourceTypes.JPG},
                                                       { ".jpeg", WebResourceTypes.JPG},
                                                       { ".gif", WebResourceTypes.GIF},
                                                       { ".xbap", WebResourceTypes.Silverlight},
                                                       { ".xsl", WebResourceTypes.StyleSheetXSL}};

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
                RunInWatchMode();
            }
            else
            {
                DownloadWebResources(options.NamePrefixes);
            }

            sw.Stop();
            _log.Info($"Done!");
        }

        private void DownloadWebResources(IEnumerable<string> namePrefixes)
        {
            // Get all the web resources in the environment
            _log.Info("Downloading web resources...");
            var webResources = GetWebResources(namePrefixes);
            _log.Info($"{webResources.Entities.Count} files found!");
            Save(webResources, Environment.CurrentDirectory);
        }

        private void Save(EntityCollection webResources, string basePath)
        {
            foreach (var webResource in webResources.Entities)
            {
                // get the content
                var content = (string)webResource["content"];
                var webResourceName = (string)webResource["name"];
                var binaryContent = Convert.FromBase64String(content);
                var filePath = Path.Combine(basePath, webResourceName);
                _log.Info($"Saving {filePath}...");
            }--
        }

        private EntityCollection GetWebResources(IEnumerable<string> namePrefixes)
        {
            var qry = new QueryExpression("webresource") { ColumnSet= new ColumnSet("name","content") };
            var isNotCustomizableCondition = new ConditionExpression() { AttributeName = "iscustomizable", Operator = ConditionOperator.Equal };
            isNotCustomizableCondition.Values.Add(true);
            qry.Criteria.AddCondition(isNotCustomizableCondition);


            // add name prefixes filter
            var namePrefixesFilter = new FilterExpression() { FilterOperator = LogicalOperator.Or };
            foreach (var namePrefix in namePrefixes)
            {
                var namePrefixFilter = new ConditionExpression() { AttributeName="name", Operator=ConditionOperator.BeginsWith };
                namePrefixFilter.Values.Add(namePrefix);
                namePrefixesFilter.AddCondition(namePrefixFilter);
            }

            if (namePrefixesFilter.Conditions.Count > 0)
            {
                qry.Criteria.AddFilter(namePrefixesFilter);
            }

            var webResources = _crmService.RetrieveMultiple(qry);
            return webResources;
        }

        private void RunInWatchMode()
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

        private void UpdateFiles()
        {
            while(!_filesToUpdate.IsEmpty)
            {
                string fileToUpdate = null;
                if(_filesToUpdate.TryDequeue(out fileToUpdate))
                {
                    UpdateCreateWebResource(fileToUpdate);
                }
            }

            if (!_webResourcesToPublish.IsEmpty) {
                _log.Info("Publishing customizations...");
                PublishWebResources();
                _log.Info("Done. Waiting for more changes....");
            }
        }

        private void UpdateCreateWebResource(string fullFilePath)
        {
            var isFile = File.Exists(fullFilePath);

            if (isFile)
            {
                var webResourceName = fullFilePath.Substring(Environment.CurrentDirectory.Length + 1);
                webResourceName = webResourceName.Replace("\\", "/");

                var webresourceId = GetId(webResourceName);
                if (webresourceId.HasValue)
                {
                    Update(webresourceId.Value, webResourceName, fullFilePath);
                }
                else
                {
                    webresourceId = Create(webResourceName, fullFilePath);
                }

                _webResourcesToPublish.Enqueue(webresourceId.Value); // add this to the publish queue
                _log.Info("Done!");
            }
            else
            {
                _log.Debug($"{fullFilePath} is not an existing File");
            }
        }

        private Guid Create(string webResourceName, string filePath)
        {
            var extension = Path.GetExtension(filePath);
            int webResourceType = (int)_extensionMappings[extension];

            var content = GetContent(filePath);

            var webResource = new Entity("webresource");
            webResource["name"] = webResourceName;
            webResource["displayname"] = webResourceName;
            webResource["content"] = content;
            webResource["webresourcetype"] = new OptionSetValue(webResourceType);

            _log.Info($"Creating {webResourceName} type {_extensionMappings[extension]}({webResourceType})...");
            var id = _crmService.Create(webResource);
            return id;
        }

        private void PublishWebResources()
        {

            var webResourcesIds = new List<Guid>();
            while (!_webResourcesToPublish.IsEmpty)
            {
                Guid webResourceId=Guid.Empty;
                if(_webResourcesToPublish.TryDequeue(out webResourceId))
                {
                    webResourcesIds.Add(webResourceId);
                }
            }

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
            _log.Info($"Updating Web Resource {webResourceName}...");
            var content = GetContent(filePath);
            var webResource = new Entity("webresource", webResourceId);
            webResource["content"] = content;
            _crmService.Update(webResource);
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
                _filesToUpdate.Enqueue(e.FullPath);
            }
        }
    }
}