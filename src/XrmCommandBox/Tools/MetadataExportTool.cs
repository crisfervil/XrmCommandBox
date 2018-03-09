using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using XrmCommandBox.Data;

namespace XrmCommandBox.Tools
{
    /// <summary>
    /// Export metadata information
    /// </summary>
    public class MetadataExportTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ImportTool));

        public MetadataExportTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(MetadataExportToolOptions options)
        {
            var sw = Stopwatch.StartNew();

            _log.Info("Running Metadata Export Tool...");

            _log.Debug("Querying metadata...");
            var metadata = _crmService.GetMetadata(options.Entity, EntityFilters.Attributes | EntityFilters.Entity | EntityFilters.Relationships /* TODO: Allow filtering in parameters*/);
            if (metadata == null)
            {
                throw new Exception($"{options.Entity} entity metadata not found");
            }

            // export the metadata
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented; // TODO: add this as a parameter

            using (StreamWriter fsw = new StreamWriter(options.File))
            using (JsonWriter writer = new JsonTextWriter(fsw))
            {
                serializer.Serialize(writer, metadata);
                fsw.Close();
            }

            sw.Stop();
            _log.Info($"Done! {options.Entity} metadata successfully exported in {sw.Elapsed.TotalSeconds.ToString("0.00")} seconds.");
        }
    }
}