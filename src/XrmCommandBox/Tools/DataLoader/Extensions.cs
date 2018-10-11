using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmCommandBox.Tools.DataLoader
{
    public static class Extensions
    {
        public static EntityCollection AsEntityCollection(this System.Data.DataTable dataTable, EntityMetadata metadata, TableMappingOptions mappingOptions)
        {
            throw new NotImplementedException();
        }
    }
}
