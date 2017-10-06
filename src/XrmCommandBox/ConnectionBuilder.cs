using System;
using System.Net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using log4net;

namespace XrmCommandBox
{
    class ConnectionBuilder
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ConnectionBuilder));

        public IOrganizationService GetConnection(string connection)
        {

            _log.Info("Connecting to CRM...");
 
            // The connection can be a connection name in the app.config file or a connection string
            var connStr = System.Configuration.ConfigurationManager.ConnectionStrings[connection];
            var connStrValue = connection;
            if (connStr != null)
            {
                _log.Debug($"Using ConnectionString {connection}");
                connStrValue = connStr.ConnectionString;
            }

            var client = new CrmServiceClient(connStrValue);

            if (!client.IsReady || client.LastCrmException!=null || !string.IsNullOrEmpty(client.LastCrmError))
            {
                var url = client.CrmConnectOrgUriActual != null ? client.CrmConnectOrgUriActual.ToString() : "CRM";
                throw new Exception($"Error when connecting to {url} - {client.LastCrmError}", client.LastCrmException);
            }

            _log.Info($"Connected to: {client.CrmConnectOrgUriActual}");

            return client.OrganizationWebProxyClient ?? (IOrganizationService)client.OrganizationServiceProxy;

        }
    }
}