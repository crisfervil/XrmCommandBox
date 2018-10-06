using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using XrmCommandBox.Data;
using XrmCommandBox.Tools.Common;

namespace XrmCommandBox.Tools.DataLoader
{
    public class DataLoaderTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(DataLoaderTool));

        public DataLoaderTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(DataLoaderToolOptions options)
        {
            var sw = Stopwatch.StartNew();
            int recordCount = 0, createdCount = 0, updatedCount = 0, errorsCount = 0, progress = 0;
            var serializer = new DataSetSerializer();

            _log.Info("Running DataLoader Tool...");
            if (options.MappingOptions == null || options.MappingOptions.Count() == 0) throw new Exception("Mappings not defined");

            _log.Info($"Reading {options.File} file...");

            var dataset = serializer.Deserialize(options.File);

			if (dataset == null) throw new Exception("Unexpected error reading file");

			_log.Info($"{dataset.Tables.Count} tables read");

            foreach (System.Data.DataTable dataTable in dataset.Tables)
            {
                int dtCreatedCount = 0, dtUpdatedCount = 0, dtErrorsCount = 0;
                ProcessDataSet(dataTable, options, out dtCreatedCount, out dtUpdatedCount, out dtErrorsCount);
            }

            sw.Stop();
            _log.Info($"Done! Processed {recordCount} records in {sw.Elapsed.TotalSeconds.ToString("0.00")} seconds. Created: {createdCount}. Updated: {updatedCount}. Errors: {errorsCount}");
        }


        private void ProcessDataSet(System.Data.DataTable dataTable, DataLoaderToolOptions options, out int createdCount, out int updatedCount, out int errorCount)
        {
            int recordCount = 0;
            int progress = 0;

            createdCount = 0;
            updatedCount = 0;
            errorCount = 0;

            // find the options for this datatable
            var mappingOptions = options.MappingOptions.FirstOrDefault(x => string.Compare(x.TableName, dataTable.TableName, true) == 0);

            if (mappingOptions != null)
            {
                _log.Debug($"Querying metadata of entity {mappingOptions.EntityName}...");
                var metadata = _crmService.GetMetadata(mappingOptions.EntityName);

                _log.Info("Processing records...");
                var records = dataTable.AsEntityCollection(metadata, mappingOptions);

                foreach (var entityRecord in records.Entities)
                {
                    try
                    {
                        recordCount++;
                        progress = (int)Math.Round(((decimal)recordCount / records.Entities.Count) * 100); // calculate the progress percentage
                        _log.Info($"{entityRecord.LogicalName} {recordCount} of {records.Entities.Count} : {entityRecord.Id} ({progress}%)");

                        // figure out if the record exists, in order to decide to create or update it
                        var recordId = Utilities.GetExistingRecordId(_crmService, entityRecord.LogicalName, entityRecord, mappingOptions.MatchAttributes?.ToList(), metadata);
                        _log.Debug($"RecordId: {recordId}");

                        if (recordId != null)
                        {
                            // the record exists, so update it
                            _log.Info($"Updating record: {recordId}...");
                            entityRecord[metadata.PrimaryIdAttribute] = recordId.Value;
                            entityRecord.Id = recordId.Value;
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
                        errorCount++;
                        _log.Error(ex);
                        if (!options.ContinueOnError) throw;
                    }
                }
            }
            else
            {
                _log.Info($"No mappings found for table {dataTable.TableName}");
            }
        }

    }
}