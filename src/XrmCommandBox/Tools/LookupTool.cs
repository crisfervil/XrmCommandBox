using System;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XrmCommandBox.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;
using System.Diagnostics;

namespace XrmCommandBox.Tools
{
    public class LookupTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(LookupTool));

        public LookupTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(LookupToolOptions options)
        {
            var sw = Stopwatch.StartNew();
            int errorsCount = 0, recordCount = 0, progress=0;
            var serializer = new DataTableSerializer();

            _log.Info("Running Lookup Tool...");

            _log.Debug("Querying metadata...");
            var metadata = _crmService.GetMetadata(options.EntityName);

            _log.Info($"{options.File} file...");
            var dataTable = serializer.Deserialize(options.File);
            _log.Info($"Read {dataTable.Count} {dataTable.Name} records");

            foreach (var record in dataTable)
            {
                try
                {
                    recordCount++;
                    progress = (int)Math.Round(((decimal)recordCount / dataTable.Count) * 100); // calculate the progress percentage
                    _log.Info($"Looking Up {dataTable.Name} record {recordCount} of {dataTable.Count} ({progress}%)...");

                    if (record.ContainsKey(options.Column)) // if the column is empty, we don't perform the lookup
                    {
                        // create query to run against crm

                        var conditionsStr = GetConditionsString(options,metadata,record);
                        var qry = GetQuery(options, metadata, record);

                        _log.Debug("Executing query...");
                        var foundRecords = _crmService.RetrieveMultiple(qry);

                        string errorMsg = null;

                        if (foundRecords.Entities.Count == 0)
                        {
                            errorMsg = $"No {foundRecords.EntityName} record found with {conditionsStr}";
                        }
                        else if (foundRecords.Entities.Count > 1)
                        {
                            errorMsg = $"{foundRecords.Entities.Count} {foundRecords.EntityName} records found with {conditionsStr}";
                        }
                        else
                        {
                            var recordId = foundRecords.Entities[0].Id;
                            record[options.Column] = recordId;
                            _log.Info($"{foundRecords.EntityName} record found: {recordId}");
                        }

                        // handle errors in a log friendly way
                        if (errorMsg != null)
                        {
                            if (options.ContinueOnError)
                            {
                                errorsCount++;
                                _log.Error(errorMsg);
                            }
                            else
                            {
                                throw new Exception(errorMsg);
                            }
                        }
                    }
                    else
                    {
                        _log.Info($"Column {options.Column} empty. Skipping");
                    }

                }
                catch (Exception ex)
                {
                    errorsCount++;
                    _log.Error(ex);
                    if (!options.ContinueOnError) throw;
                }
            }

            _log.Info("Saving file...");

            serializer.Serialize(dataTable, options.File);

            sw.Stop();
            _log.Info($"Done! Looked Up column {options.Column} in {recordCount} {dataTable.Name} records in {sw.Elapsed.TotalSeconds.ToString("0.00")} seconds. {errorsCount} errors");
        }

        private QueryBase GetQuery(LookupToolOptions options, EntityMetadata metadata, Dictionary<string, object> record)
        {
            IList<string> matchAttributes = options.MatchAttributes.ToList();
            IList<string> matchColumns = options.MatchColumns.ToList();
            var qry = new QueryByAttribute()
            {
                EntityName = options.EntityName,
                ColumnSet = new ColumnSet(metadata.PrimaryIdAttribute)
            };
            qry.Attributes.AddRange(matchAttributes);

            for (var i = 0; i < matchColumns.Count; i++)
            {
                var colName = matchColumns[i];
                // TODO: Validate that the number of items in matchColumns and matchAttributes match
                var attrName = matchAttributes[i];
                var attrMetadata = metadata.Attributes.Where(attr => string.Compare(attr.LogicalName, attrName, StringComparison.OrdinalIgnoreCase) == 0).First();

                var filterAttrValue = record.ContainsKey(colName) ? GetFilterValue(record[colName], attrMetadata) : null;
                qry.Values.Add(filterAttrValue);
            }
            return qry;
        }

        private string GetConditionsString(LookupToolOptions options, EntityMetadata metadata, Dictionary<string, object> record)
        {
            IList<string> matchAttributes = options.MatchAttributes.ToList();
            IList<string> matchColumns = options.MatchColumns.ToList();

            var conditionsStr = new List<string>();
            for (var i = 0; i < matchColumns.Count; i++)
            {
                var colName = matchColumns[i];
                // TODO: Validate that the number of items in matchColumns and matchAttributes match
                var attrName = matchAttributes[i];
                var attrMetadata = metadata.Attributes.Where(attr => string.Compare(attr.LogicalName, attrName, StringComparison.OrdinalIgnoreCase) == 0).First();

                var filterAttrValue = record.ContainsKey(colName) ? GetFilterValue(record[colName], attrMetadata) : null;
                conditionsStr.Add($"{attrName}={filterAttrValue ?? "null"}");
            }

            return String.Join(",",conditionsStr);
        }

        private object GetFilterValue(object attributeValue, AttributeMetadata attrMetadata)
        {
            var filterValue = attributeValue;

            if (attrMetadata.AttributeType == AttributeTypeCode.Integer)
            {
                var attributeValueStr = attributeValue as string;
                filterValue = attributeValueStr != null ? int.Parse(attributeValueStr) : (int)attributeValue;
            }
            else if (attrMetadata.AttributeType == AttributeTypeCode.String || attrMetadata.AttributeType == AttributeTypeCode.Memo)
            {
                var attributeValueStr = attributeValue as string;
                filterValue = attributeValueStr;
            }
            else
            {
                _log.Warn($"GetFilterValue is not handling the convertion of {attrMetadata.AttributeType} data types");
            }

            return filterValue;
        }

    }
}