using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataTools.Data
{
    public static class Extensions
    {
        public static DataTable AsDataTable(this EntityCollection records)
        {
            var data = new DataTable() {  Name=records.EntityName };

            foreach (var recordData in records.Entities)
            {
                var attrValues = new Dictionary<string, object>();
                foreach (var recordAttr in recordData.Attributes)
                {
                    attrValues.Add(recordAttr.Key, Convert(recordAttr.Value));
                }
                data.Add(attrValues);
            }
            return data;
        }

        private static object Convert(object value)
        {
            var retVal = value;

            if(value is EntityReference)
            {
                var er = (EntityReference)value;
                retVal = new EntityReferenceValue() { LogicalName=er.LogicalName, Name=er.Name, Value=er.Id };
            }
            else if (value is Money)
            {
                retVal = ((Money)value).Value;
            }

            // TODO add more data type converters

            return retVal;
        }
    }
}
