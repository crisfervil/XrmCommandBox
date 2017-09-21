using log4net;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DynamicsDataTools.ImportTool
{
    public class ImportTool
    {
        private readonly ILog _log;
        private readonly IOrganizationService _crmService;

        public ImportTool(ILog log, IOrganizationService service)
        {
            _log = log;
            _crmService = service;
        }

        public void Run(ImportOptions options)
        {
            var extension = Path.GetExtension(options.File);
            IDataReader reader = GetReader(extension);

            // Read the file and get the records to import
            var dataTable = reader.Read(options.File);

            foreach (var row in dataTable)
            {
                var entity = GetEntity(row, dataTable.Name);
                var recordId = _crmService.Create(entity);
            }
        }

        private Entity GetEntity(Dictionary<string,object> record, string entityName)
        {
            var entity = new Entity(entityName);

            foreach (var attrName in record.Keys)
            {
                // Convert this to the type specified in the metadata for the attribute in the entity
                entity[attrName] = record[attrName];
            }

            return entity;
        }

        private IDataReader GetReader(string extension)
        {
            var emptyObjectArray = new object[] { };
            var arrayWithLogOnly = new object[] { _log };

            var readers = Extensions.GetObjectInstances<IDataReader>(new object[][] { emptyObjectArray, arrayWithLogOnly });

            var found = readers.Where(x => x.Extension == extension).ToList();
            if (!found.Any()) throw new Exception($"No exporter found for extension {extension}");
            if (found.Count > 1) throw new Exception($"Too many exporters found for extension {extension}");
            return found[0];
        }
    }
}
