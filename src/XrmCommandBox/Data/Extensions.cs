using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;
using System.Linq;
using System;
using System.Workflow.Activities;
using log4net;
using XrmCommandBox.Tools;

namespace XrmCommandBox.Data
{
    public static class Extensions
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(Extensions));

        public static EntityCollection AsEntityCollection(this DataTable dataTable, EntityMetadata metadata)
        {
            var records = new EntityCollection();

            foreach (var record in dataTable)
            {
                var entityRecord = new Entity(dataTable.Name);

                foreach (var recordAttr in record)
                {
                    // Query the metadata to get the attribute value types
                    var found = metadata.Attributes.Where(attr => attr.SchemaName == recordAttr.Key).ToList();
                    var attrMetadata = found.Count > 0 ? found[0] : null;
                    if (attrMetadata != null)
                    {
                        entityRecord[recordAttr.Key] = GetAttrValue(recordAttr.Key, recordAttr.Value, record, attrMetadata);
                    }
                    else
                    {
                        Log.Debug($"Attr {recordAttr.Key} not found in entity {dataTable.Name}. Skipping");
                    }
                }

                records.Entities.Add(entityRecord);
            }

            return records;
        }

        private static object GetAttrValue(string attrName, object attrValue, Dictionary<string, object> record, AttributeMetadata attrMetadata)
        {
            object retVal = attrValue;
            if (attrValue != null)
            {
                var strAttrValue = attrValue as string;
                if (attrMetadata.AttributeType == AttributeTypeCode.Lookup)
                {
                    Guid referenceGuid = strAttrValue != null ? Guid.Parse(strAttrValue) : (Guid) attrValue; 

                    // try to find the attr type
                    var lookupType = record[$"{attrName}.type"];
                    if (lookupType != null)
                    {
                        retVal = new EntityReference((string)lookupType,referenceGuid);
                    }                
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.Money)
                {
                    decimal moneyValue = strAttrValue != null ? decimal.Parse(strAttrValue) : (decimal) attrValue;
                    retVal = new Money(moneyValue);
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.Picklist)
                {
                    int optionValue = strAttrValue != null ? int.Parse(strAttrValue) : (int)attrValue;
                    retVal = new OptionSetValue(optionValue);
                }                
            }

            return retVal;
        }

        public static DataTable AsDataTable(this EntityCollection records)
        {
            var data = new DataTable() {  Name=records.EntityName };

            foreach (var recordData in records.Entities)
            {
                var attrValues = new Dictionary<string, object>();
                foreach (var recordAttr in recordData.Attributes)
                {
                    attrValues.Add(recordAttr.Key, Convert(recordAttr.Value));
                    AddAdditionalValues(recordAttr.Key, attrValues, recordAttr.Value);
                }
                data.Add(attrValues);
            }
            return data;
        }

        private static void AddAdditionalValues(string attrName, Dictionary<string, object> record, object value)
        {
            var erValue = value as EntityReference;
            if (erValue != null)
            {
                record.Add($"{attrName}.name", erValue.Name);
                record.Add($"{attrName}.type", erValue.LogicalName);
            }
        }

        private static object Convert(object value)
        {
            var retVal = value;

            if(value is EntityReference)
            {
                retVal = ((EntityReference)value).Id;
            }
            else if (value is Money)
            {
                retVal = ((Money)value).Value;
            }
            else if (value is OptionSetValue)
            {
                retVal = ((OptionSetValue)value).Value;
            }

            // TODO add more data type converters

            return retVal;
        }
    }
}
