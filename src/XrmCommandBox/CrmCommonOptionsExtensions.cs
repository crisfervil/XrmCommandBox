using System;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox
{
    public static class CrmCommonOptionsExtensions
    {
        public static IOrganizationService GetConnection(this CrmCommonOptions options)
        {
            return new ConnectionBuilder().GetConnection(options.ConnectionName);
        }
    }
}