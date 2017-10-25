using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace XrmCommandBox
{
    public static class XrmExtensions
    {
        public static EntityMetadata GetMetadata(this IOrganizationService service, string entityName)
        {
            var request = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Attributes|EntityFilters.Entity|EntityFilters.Relationships,
                LogicalName = entityName
            };
            var response = (RetrieveEntityResponse)service.Execute(request);
            return response.EntityMetadata;
        }
    }
}
