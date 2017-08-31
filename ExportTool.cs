using System;
using System.IO;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DynamicsDataTools
{
    class ExportTool
    {
        public void Run(ExportOptions options)
        {
            IOrganizationService crmService = new ConnectionBuilder().GetConnection(options.ConnectionName);
            var foundRecords = crmService.RetrieveMultiple(GetAllRecordsQuery(options.EntityName));
            if (foundRecords != null)
            {
                // Save records to an Xml file
                Save(foundRecords.Entities, options.File);
            }
        }

        private void Save(DataCollection<Entity> data, string fileName)
        {
            System.Xml.Serialization.XmlSerializer ser = new XmlSerializer(typeof(DataCollection<Entity>));
            using (var sw = new System.IO.StreamWriter(fileName))
            {
                ser.Serialize(sw,data);
                sw.Close();
            }
        }

        private QueryBase GetAllRecordsQuery(string entityName)
        {
            return new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(true), // retrieve all columns
            };
        }
    }
}
