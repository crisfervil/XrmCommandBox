using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmCommandBox.Tools.Common
{
    public static class Utilities
    {
        internal static Guid? GetExistingRecordId(IOrganizationService service, string entityName, Entity entityRecord, IList<string> matchAttributes, EntityMetadata entityMetadata)
        {
            Guid? recordGuid = null;

            var qry = GetMatchQuery(entityName, entityRecord, matchAttributes, entityMetadata);

            var foundRecords = service.RetrieveMultiple(qry);

            if (foundRecords.Entities.Count > 0)
            {
                if (foundRecords.Entities.Count > 1)
                    throw new Exception("Too many records found");

                recordGuid = foundRecords.Entities[0].Id;
            }

            return recordGuid;
        }

        private static QueryBase GetMatchQuery(string entityName, Entity entityRecord, IList<string> matchAttributes, EntityMetadata entityMetadata)
        {
            if (matchAttributes == null || matchAttributes.Count == 0)
            {
                // set the id attribute as match attribute
                var attrId = entityMetadata.PrimaryIdAttribute;

                matchAttributes = new[] { attrId };
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

        private static object GetFilterValue(object attributeValue)
        {
            var filterValue = attributeValue;

            if (attributeValue is EntityReference)
            {
                var attrValueReference = (EntityReference)attributeValue;
                filterValue = attrValueReference.Id;
            }
            else if (attributeValue is OptionSetValue)
            {
                var attrValueOptionset = (OptionSetValue)attributeValue;
                filterValue = attrValueOptionset.Value;
            }

            return filterValue;
        }
    }
}
