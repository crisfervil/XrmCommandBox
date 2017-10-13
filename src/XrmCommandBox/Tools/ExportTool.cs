﻿using System;
using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using XrmCommandBox.Data;

namespace XrmCommandBox.Tools
{
    public class ExportTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ExportTool));

        public ExportTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(ExportToolOptions options)
        {
            _log.Info("Running Export Tool...");

            ValidateOptions(options);

            _log.Debug("Executing query...");
            var foundRecords = GetRecords(options);
            _log.Info($"{foundRecords.Entities.Count} records found");

            // Convert to a data table
            var recordsTable = foundRecords.AsDataTable();

            // set a default file name
            if (string.IsNullOrEmpty(options.File))
                options.File = $"{foundRecords.EntityName}.xml";

            var serializer = new DataTableSerializer();
            serializer.Serialize(recordsTable, options.File, options.RecordNumber);

            _log.Info("Done!");
        }

        private EntityCollection GetRecords(ExportToolOptions options)
        {
            EntityCollection foundRecords = null;

            if (!string.IsNullOrEmpty(options.EntityName))
            {
                var qry = GetAllRecordsQuery(options.EntityName, options.PageSize, options.Page);
                foundRecords = _crmService.RetrieveMultiple(qry);
            }
            else if (!string.IsNullOrEmpty(options.FetchQuery))
            {
                var qry = new FetchExpression(options.FetchQuery);
                foundRecords = _crmService.RetrieveMultiple(qry);
            }
            return foundRecords;
        }

        private void ValidateOptions(ExportToolOptions options)
        {
            if (string.IsNullOrEmpty(options.FetchQuery) && string.IsNullOrEmpty(options.EntityName))
                throw new Exception("Either the entity or the fetch-query options are required");
        }

        private QueryBase GetAllRecordsQuery(string entityName, int pageSie, int page)
        {
            return new QueryExpression(entityName)
            {
                ColumnSet = new ColumnSet(true), // retrieve all columns
                PageInfo =
                {
                    PageNumber = page,
                    Count = pageSie
                }
            };
        }
    }
}