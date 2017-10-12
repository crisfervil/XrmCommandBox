using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System;

namespace XrmCommandBox.Data
{
    public static class Extensions
    {

        public static EntityCollection AsEntityCollection(this DataTable dataTable)
        {
            var records = new EntityCollection();

            foreach (var record in dataTable)
            {
                var entityRecord = new Entity(dataTable.Name);

                foreach (var recordAttr in record)
                {
                    // TODO: query the metadata to get the attribute value types
                    entityRecord[recordAttr.Key] = recordAttr.Value;
                }

                records.Entities.Add(entityRecord);
            }

            return records;
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
