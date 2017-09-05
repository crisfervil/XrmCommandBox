using log4net;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.Xrm.Sdk;

namespace DynamicsDataTools
{
    class ExportTool : ToolBase
    {
        private readonly ILog _log;

        public ExportTool(ILog log)
        {
            _log = log;
        }

        public void Run(ExportOptions options)
        {
            base.Run(options);

            _log.Info("Running Export tool...");

            ValidateOptions(options);

            _log.Debug("Creating connection...");
            var crmService = new ConnectionBuilder().GetConnection(options.ConnectionName);

            _log.Debug("Executing query...");
            var foundRecords = GetRecords(options, crmService);
            _log.Info($"{foundRecords.Entities.Count} records found");

            // Save records to a file
            _log.Debug("Peparing file...");
            if (string.IsNullOrEmpty(options.File))
            {
                options.File = !string.IsNullOrEmpty(options.FetchFile) ? $"{Path.GetFileNameWithoutExtension(options.FetchFile)}_data.xml" : $"{options.EntityName}.xml";
            }
            var extension = System.IO.Path.GetExtension(options.File);
            IExporter exporter = GetExporter(extension);
            exporter.Export(foundRecords.Entities, options.File);

            _log.Info("Completed");
        }

        private EntityCollection GetRecords(ExportOptions options, IOrganizationService service)
        {
            EntityCollection foundRecords = null;
            if (!string.IsNullOrEmpty(options.EntityName))
            {
                foundRecords = service.RetrieveMultiple(GetAllRecordsQuery(options.EntityName));
            }
            else if (!string.IsNullOrEmpty(options.FetchFile))
            {
                foundRecords = service.RetrieveMultiple(GetFetchQuery(options.FetchFile));
            }
            return foundRecords;
        }

        private void ValidateOptions(ExportOptions options)
        {
            if (string.IsNullOrEmpty(options.FetchFile) && string.IsNullOrEmpty(options.EntityName))
            {
                throw new Exception("Either the entityname or the fetchfile options are required");
            }
        }

        private QueryBase GetFetchQuery(string fileName)
        {
            // read xml file
            var xml = new XmlDocument();
            xml.Load(fileName);

            if (xml.DocumentElement == null || xml.DocumentElement.Name != "fetch")
            {
                throw new Exception("Invalid xml document. The first node in the document must be a fetch");
            }

            return new FetchExpression(xml.DocumentElement.OuterXml);
        }

        private QueryBase GetAllRecordsQuery(string entityName)
        {
            return new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(true), // retrieve all columns
            };
        }

        private IExporter GetExporter(string extension)
        {
            var exporters = GetAvailableExporters();
            var found=exporters.Where(x=> x.Extension==extension).ToList();
            if (!found.Any()) throw new Exception($"No exporter found for extension {extension}");
            if (found.Count > 1) throw new Exception($"Too many exporters found for extension {extension}");
            return found[0];
        }


        private IList<IExporter> GetAvailableExporters()
        {
            var found = new List<IExporter>();
            var existingAssemblies = Assembly.GetExecutingAssembly().GetExportedTypes();
            var exporters = existingAssemblies.Where(x => x.GetInterfaces().Contains(typeof(IExporter)));

            foreach (var exporter in exporters)
            {
                _log.Debug($"Exporter: {exporter.Name}");

                var logConstructor = exporter.GetConstructor(new[] { typeof(ILog) });
                var noParamsConstructor = exporter.GetConstructor(new Type[] {});
                IExporter instance = null;
                if ( logConstructor != null)
                {
                    _log.Debug("ILog constructor");
                    instance = (IExporter)logConstructor.Invoke(new object[] {_log});
                } 
                else if (noParamsConstructor != null)
                {
                    _log.Debug("No param constructor");
                    instance = (IExporter)noParamsConstructor.Invoke(new object[] { });
                }

                if (instance != null)
                {
                    found.Add(instance);
                }
            }

            return found;
        }
    }
}
