using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using XrmCommandBox.Data;

namespace XrmCommandBox.Tools
{
    public class ImportTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ImportTool));

        public ImportTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(ImportToolOptions options)
        {
            var sw = Stopwatch.StartNew();
            int recordCount = 0, createdCount = 0, updatedCount = 0, errorsCount = 0, progress = 0;
            var serializer = new DataTableSerializer();

            _log.Info("Running Import Tool...");

            _log.Info($"Reading {options.File} file...");
            var dataTable = serializer.Deserialize(options.File);
            _log.Info($"{dataTable.Count} {dataTable.Name} records read");

            _log.Debug("Querying metadata...");
            var metadata = _crmService.GetMetadata(dataTable.Name);

            _log.Info("Processing records...");
            var records = dataTable.AsEntityCollection(metadata);

            foreach (var entityRecord in records.Entities)
            {
                try
                {
                    recordCount++;
                    progress = (int)Math.Round(((decimal)recordCount / records.Entities.Count) * 100); // calculate the progress percentage
                    _log.Info($"{entityRecord.LogicalName} {recordCount} of {records.Entities.Count} : {entityRecord.Id} ({progress}%)");

                    // figure out if the record exists, in order to decide to create or update it
                    var recordId = GetRecordId(entityRecord.LogicalName, entityRecord,
                        options.MatchAttributes?.ToList(), metadata);
                    _log.Debug($"RecordId: {recordId}");

                    if (recordId != null)
                    {
                        // the record exists, so update it
                        _log.Info($"Updating record: {recordId}...");
                        entityRecord[metadata.PrimaryIdAttribute] = recordId.Value;
                        _crmService.Update(entityRecord);
                        _log.Info("Record updated successfully");
                        updatedCount++;
                    }
                    else
                    {
                        // the record doesn't exist, so create it
                        _log.Info("Creating record....");
                        recordId = _crmService.Create(entityRecord);
                        _log.Info($"Record created successfully: Guid {recordId}");
                        createdCount++;
                    }
                }
                catch (Exception ex)
                {
                    errorsCount++;
                    _log.Error(ex);
                    if (!options.ContinueOnError) throw;
                }
            }

            sw.Stop();
            _log.Info($"Done! Processed {recordCount} {dataTable.Name} records in {sw.Elapsed.TotalSeconds} seconds. Created: {createdCount}. Updated: {updatedCount}. Errors: {errorsCount}");
        }

        private Guid? GetRecordId(string entityName, Entity entityRecord, IList<string> matchAttributes, EntityMetadata entityMetadata)
        {
            Guid? recordGuid = null;

            var qry = GetMatchQuery(entityName, entityRecord, matchAttributes, entityMetadata);

            var foundRecords = _crmService.RetrieveMultiple(qry);

            if (foundRecords.Entities.Count > 0)
            {
                if (foundRecords.Entities.Count > 1)
                    throw new Exception("Too many records found");

                recordGuid = foundRecords.Entities[0].GetAttributeValue<Guid>(entityMetadata.PrimaryIdAttribute);
            }

            return recordGuid;
        }

        private QueryBase GetMatchQuery(string entityName, Entity entityRecord, IList<string> matchAttributes, EntityMetadata entityMetadata)
        {
            if (matchAttributes == null || matchAttributes.Count == 0)
            {
                // set the id attribute as match attribute
                var attrId = entityMetadata.PrimaryIdAttribute;

                matchAttributes = new[] {attrId};
            }

            var qry = new QueryByAttribute
            {
                EntityName = entityName,
                ColumnSet = new ColumnSet(entityMetadata.PrimaryIdAttribute)
            };

            qry.Attributes.AddRange(matchAttributes);

            foreach (var attrName in matchAttributes)
            {
                var filterAttrValue = entityRecord.Contains(attrName) ? GetFilterValue(entityRecord[attrName]) : null;
                qry.Values.Add(filterAttrValue);
            }

            return qry;
        }

        private object GetFilterValue(object attributeValue)
        {
            var filterValue = attributeValue;

            if (attributeValue is EntityReference)
            {
                var attrValueReference = (EntityReference) attributeValue;
                filterValue = attrValueReference.Id;
            }
            else if (attributeValue is OptionSetValue)
            {
                var attrValueOptionset = (OptionSetValue) attributeValue;
                filterValue = attrValueOptionset.Value;
            }

            return filterValue;
        }

    }
}