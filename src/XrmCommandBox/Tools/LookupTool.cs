using System;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XrmCommandBox.Data;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;

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
            int errorsCount = 0, recordCount = 0;
            var serializer = new DataTableSerializer();

            _log.Info("Running Lookup Tool...");

            _log.Debug("Querying metadata...");
            var metadata = _crmService.GetMetadata(options.EntityName);

            _log.Info("Reading file...");
            var dataTable = serializer.Deserialize(options.File);

            IList<string> matchAttributes = options.MatchAttributes.ToList();
            IList<string> matchColumns = options.MatchColumns.ToList();

            foreach (var record in dataTable)
            {
                try
                {
                    _log.Info($"Record {++recordCount} of {dataTable.Count}");

                    // create query to run against crm
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

                    var foundRecords = _crmService.RetrieveMultiple(qry);
                    if (foundRecords.Entities.Count == 0)
                    {
                        throw new Exception("No data found");
                    }
                    if (foundRecords.Entities.Count > 1)
                    {
                        throw new Exception("Too many records found");
                    }

                    var recordId = foundRecords.Entities[0].Id;
                    record[options.Column] = recordId;
                }
                catch (Exception ex)
                {
                    errorsCount++;
                    _log.Error(ex);
                    if (!options.ContinueOnError) throw;
                }
            }

            _log.Info($"Processed {recordCount} records. {errorsCount} errors");

            _log.Info("Saving file...");

            // TODO: Make addRecordNumber dynamic
            serializer.Serialize(dataTable, options.File, true);

            _log.Info("Done!");
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