using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace DynamicsDataTools
{
    class ConnectionBuilder
    {
        public IOrganizationService GetConnection(string connection)
        {
            // The connection can be a connection name in the app.config file or a connection string
            var connStr = System.Configuration.ConfigurationManager.ConnectionStrings[connection];
            var connStrValue = connection;
            if (connStr != null)
            {
                connStrValue = connStr.ConnectionString;
            }

            return new CrmServiceClient(connStrValue);

        }
    }
}