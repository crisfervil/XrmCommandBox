using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace XrmCommandBox
{
    public static class XrmExtensions
    {
        public static EntityMetadata GetMetadata(this IOrganizationService service, string entityName, EntityFilters filters = EntityFilters.Attributes)
        {
            var request = new RetrieveEntityRequest
            {
                EntityFilters = filters,
                LogicalName = entityName
            };
            var response = (RetrieveEntityResponse)service.Execute(request);
            return response.EntityMetadata;
        }
    }
}
