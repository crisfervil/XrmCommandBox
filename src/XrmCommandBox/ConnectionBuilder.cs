using System;
using System.Configuration;
using System.Xml.Linq;
using System.Linq;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace XrmCommandBox
{
    internal class ConnectionBuilder
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ConnectionBuilder));

        public IOrganizationService GetConnection(string connection)
        {
            if (connection == null) throw new ArgumentException("connection parameter missing");

            _log.Info("Connecting to CRM...");

            ConnectionStringSettings connStrSetting = null;
            string connStrValue = null;

            // Try to get the connection string from a connections.config file in the current path
            var connectionsPath = "connections.config";
            connectionsPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, connectionsPath);
            if (System.IO.File.Exists(connectionsPath))
            {
                // the connections.config file exists, so try to read the connectionstring from there
                _log.Debug($"Trying to get the connection from current dir connections.config");

                var connectionsStringDoc = XDocument.Load(connectionsPath);
                var connsQry = from c in connectionsStringDoc.Descendants("add")
                               where c.Attribute("name")?.Value == connection
                            select c.Attribute("connectionString")?.Value;
                connStrValue = connsQry.FirstOrDefault();
            }

            if (connStrValue == null)
            {
                _log.Debug($"Trying to get the connection from XrmCommandBox AppSettings...");
                connStrSetting = ConfigurationManager.ConnectionStrings[connection];
                if (connStrSetting != null)
                {
                    _log.Debug($"{connection} found");
                    connStrValue = connStrSetting.ConnectionString;
                }
            }

            // connection not found in conf files. Assume the full conn string was passed
            if (connStrValue == null)
            {
                connStrValue = connection;
            }

            var client = new CrmServiceClient(connStrValue);

            if (!client.IsReady || client.LastCrmException != null || !string.IsNullOrEmpty(client.LastCrmError))
            {
                var url = client.CrmConnectOrgUriActual != null ? client.CrmConnectOrgUriActual.ToString() : "CRM";
                throw new Exception($"Error when connecting to {url} - {client.LastCrmError}", client.LastCrmException);
            }

            _log.Info($"Connected to: {client.CrmConnectOrgUriActual}");

            return client.OrganizationWebProxyClient ?? (IOrganizationService) client.OrganizationServiceProxy;
        }
    }
}