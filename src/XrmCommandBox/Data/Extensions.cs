using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

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
                    var found = metadata.Attributes
                        .Where(attr => string.Compare(attr.LogicalName, recordAttr.Key,
                                           StringComparison.OrdinalIgnoreCase) == 0)
                        .ToList();
                    var attrMetadata = found.Count > 0 ? found[0] : null;
                    if (attrMetadata != null)
                    {
                        entityRecord[recordAttr.Key] =
                            GetAttrValue(recordAttr.Key, recordAttr.Value, record, attrMetadata);
                        if (string.Compare(recordAttr.Key, metadata.PrimaryIdAttribute,
                                StringComparison.OrdinalIgnoreCase) == 0)
                            entityRecord.Id = entityRecord.GetAttributeValue<Guid>(recordAttr.Key);
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
            var retVal = attrValue;
            if (attrValue != null)
            {
                var strAttrValue = attrValue as string;
                if (attrMetadata.AttributeType == AttributeTypeCode.Lookup)
                {
                    var referenceGuid = strAttrValue != null ? Guid.Parse(strAttrValue) : (Guid) attrValue;

                    // try to find the attr type
                    var lookupType = record[$"{attrName}.type"];
                    if (lookupType != null)
                        retVal = new EntityReference((string) lookupType, referenceGuid);
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.Money)
                {
                    var moneyValue = strAttrValue != null ? decimal.Parse(strAttrValue) : (decimal) attrValue;
                    retVal = new Money(moneyValue);
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.Picklist)
                {
                    var optionValue = strAttrValue != null ? int.Parse(strAttrValue) : (int) attrValue;
                    retVal = new OptionSetValue(optionValue);
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
                {
                    var idValue = strAttrValue != null ? Guid.Parse(strAttrValue) : (Guid) attrValue;
                    retVal = idValue;
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.Integer)
                {
                    var intValue = strAttrValue != null ? int.Parse(strAttrValue) : (int) attrValue;
                    retVal = intValue;
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.Decimal)
                {
                    var decimalValue = strAttrValue != null ? decimal.Parse(strAttrValue) : (decimal) attrValue;
                    retVal = decimalValue;
                }
                else if (attrMetadata.AttributeType == AttributeTypeCode.String)
                {
                    var strValue = (string) attrValue;
                    retVal = strValue;
                }
                else
                {
                    Log.Debug(
                        $"Could not convert attribute {attrName} value {attrValue} entity {attrMetadata.EntityLogicalName}");
                }
            }

            return retVal;
        }

        public static DataTable AsDataTable(this EntityCollection records)
        {
            var data = new DataTable {Name = records.EntityName};

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

            if (value is EntityReference)
                retVal = ((EntityReference) value).Id;
            else if (value is Money)
                retVal = ((Money) value).Value;
            else if (value is OptionSetValue)
                retVal = ((OptionSetValue)value).Value;
            else if (value is AliasedValue)
                retVal = Convert(((AliasedValue)value).Value);

            // TODO add more data type converters

            return retVal;
        }
    }
}