using log4net;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Diagnostics;
using System.Xml;
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
            var sw = Stopwatch.StartNew();
            _log.Info("Running Export Tool...");

            ValidateOptions(options);

            _log.Info("Executing query...");
            var foundRecords = GetRecords(options);
            _log.Info($"{foundRecords.Entities.Count} records found");

            // Convert to a data table
            var recordsTable = foundRecords.AsDataTable(options.RecordNumber);

            // set a default file name
            if (string.IsNullOrEmpty(options.File))
                options.File = $"{foundRecords.EntityName}.xml";

            _log.Info("Saving file...");
            var serializer = new DataTableSerializer();
            serializer.Serialize(recordsTable, options.File);

            sw.Stop();
            _log.Info($"Done! Exported {recordsTable.Count} {recordsTable.Name} records to {options.File} in {sw.Elapsed.TotalSeconds.ToString("0.00")} seconds");
        }

        private EntityCollection GetRecords(ExportToolOptions options)
        {
            EntityCollection foundRecords = null;

            if (!string.IsNullOrEmpty(options.EntityName))
            {
                _log.Debug("Entity name specified");
                var qry = GetAllRecordsQuery(options.EntityName, options.PageSize, options.Page);
                foundRecords = _crmService.RetrieveMultiple(qry);
            }
            else if (!string.IsNullOrEmpty(options.FetchQuery))
            {
                _log.Debug("Fetch Query specified");
                foundRecords = ExecuteFetchQuery(options, foundRecords);

            }
            return foundRecords;
        }

        private EntityCollection ExecuteFetchQuery(ExportToolOptions options, EntityCollection foundRecords)
        {
            var qry = new FetchExpression(options.FetchQuery);
            bool pageSpecified = FetchXmlPageSpecified(options.FetchQuery);
            int currentPage = 1;
            _log.Debug($"Page specified: {pageSpecified}");

            while (true)
            {
                var pageRecords = _crmService.RetrieveMultiple(qry);
                if (foundRecords == null)
                {
                    // initialize
                    foundRecords = pageRecords;
                }
                else
                {
                    foundRecords.Entities.AddRange(pageRecords.Entities);
                }

                if (pageRecords.MoreRecords && !pageSpecified)
                {
                    // increase the page number
                    var updatedQry = SetFetchXmlPagingAttributes(options.FetchQuery, ++currentPage, pageRecords.PagingCookie);
                    qry = new FetchExpression(updatedQry);
                    _log.Info($"More records found. Querying page {currentPage}...");
                }
                else
                {
                    _log.Debug("No more monkeys jumping in the bed!");
                    break;
                }
            }

            return foundRecords;
        }

        private string SetFetchXmlPagingAttributes(string fetchQuery, int pageNumber, string pagingCookie)
        {
            var xml = new XmlDocument();
            xml.LoadXml(fetchQuery);

            XmlAttribute pageAttr = xml.DocumentElement.Attributes["page"];
            if (pageAttr == null)
            {
                pageAttr = xml.CreateAttribute("page");
                xml.DocumentElement.Attributes.Append(pageAttr);
            }
            pageAttr.Value = pageNumber.ToString();

            XmlAttribute pagingCookieAttr = xml.DocumentElement.Attributes["paging-cookie"];
            if (pagingCookieAttr == null)
            {
                pagingCookieAttr = xml.CreateAttribute("paging-cookie");
                xml.DocumentElement.Attributes.Append(pagingCookieAttr);
            }
            pagingCookieAttr.Value = pagingCookie;


            return xml.OuterXml;
        }

        private bool FetchXmlPageSpecified(string fetchQuery)
        {
            var xml = new XmlDocument();
            xml.LoadXml(fetchQuery);
            return xml.DocumentElement.Attributes["page"] != null;
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