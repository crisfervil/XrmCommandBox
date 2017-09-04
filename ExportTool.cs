using log4net;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicsDataTools
{
    class ExportTool
    {
        private readonly ILog _log;

        public ExportTool(ILog log)
        {
            _log = log;
        }

        public void Run(ExportOptions options)
        {
            _log.Info("Running Export tool...");

            _log.Debug("Creating connection...");
            var crmService = new ConnectionBuilder().GetConnection(options.ConnectionName);

            _log.Debug("Executing query...");
            var foundRecords = crmService.RetrieveMultiple(GetAllRecordsQuery(options.EntityName));
            _log.Info($"{foundRecords.Entities.Count} records found");

            // Save records to an Xml file
            _log.Debug("Peparing file...");
            var extension = System.IO.Path.GetExtension(options.File);
            IExporter exporter = GetExporter(extension);
            exporter.Export(foundRecords.Entities, options.File);

            _log.Info("Completed");
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
