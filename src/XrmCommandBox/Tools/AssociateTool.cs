using log4net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using XrmCommandBox.Data;

namespace XrmCommandBox.Tools
{
    /// <summary>
    /// This tool uses the Associate request to create relationships between 2 entity records
    /// </summary>
    public class AssociateTool
    {
        private readonly IOrganizationService _crmService;
        private readonly ILog _log = LogManager.GetLogger(typeof(ImportTool));

        public AssociateTool(IOrganizationService service)
        {
            _crmService = service;
        }

        public void Run(AssociateToolOptions options)
        {
            var sw = Stopwatch.StartNew();
            int recordCount = 0, errorsCount = 0, progress = 0, createdCount = 0;
            var serializer = new DataTableSerializer();

            _log.Info("Running Associate Tool...");

            _log.Info($"Reading {options.File} file...");
            var dataTable = serializer.Deserialize(options.File);

            // TODO: allow to specify this in the tool options
            var relationshipIntersectEntityName = dataTable.Name;

            _log.Info($"{dataTable.Count} {dataTable.Name} n:n relationships read");

            _log.Debug("Querying metadata...");
            var metadata = _crmService.GetMetadata(dataTable.Name, EntityFilters.Attributes | EntityFilters.Entity | EntityFilters.Relationships);
            if (metadata == null)
            {
                throw new Exception($"{relationshipIntersectEntityName} entity metadata not found");
            }

            _log.Debug("Validating metadata...");
            if (metadata.IsIntersect == null || metadata.IsIntersect.Value == false)
            {
                throw new Exception($"{dataTable.Name} is not a valid n:n relationship");
            }

            _log.Debug("Getting relationship attributes...");
            var relationshipMetadata = metadata.ManyToManyRelationships.Where(x => x.IntersectEntityName == dataTable.Name).ToList().First();

            _log.Info($"Relationship between {relationshipMetadata.Entity1LogicalName} and {relationshipMetadata.Entity2LogicalName} ({relationshipMetadata.SchemaName})");

            // TODO: Allow to specify this in the tool options
            var moniker1AttrName = relationshipMetadata.Entity1IntersectAttribute;
            var moniker2AttrName = relationshipMetadata.Entity2IntersectAttribute;

            foreach (var relationshipRecord in dataTable)
            {
                try
                {
                    recordCount++;
                    progress = (int)Math.Round(((decimal)recordCount / dataTable.Count) * 100); // calculate the progress percentage
                    _log.Info($"Record {recordCount} of {dataTable.Count} : ({progress}%)");

                    var moniker1Guid = Guid.Parse((string)relationshipRecord[moniker1AttrName]);
                    var moniker2Guid = Guid.Parse((string)relationshipRecord[moniker2AttrName]);

                    _log.Debug($"{moniker1Guid} : {moniker2Guid}");

                    var request = new AssociateEntitiesRequest
                    {
                        Moniker1 = new EntityReference(relationshipMetadata.Entity1LogicalName, moniker1Guid),
                        Moniker2 = new EntityReference(relationshipMetadata.Entity2LogicalName, moniker2Guid),
                        RelationshipName = relationshipMetadata.SchemaName
                    };

                    _crmService.Execute(request);
                    createdCount++;
                }
                catch (FaultException ex)
                {
                    if (ex.Message == "Cannot insert duplicate key.")
                    {
                        errorsCount++;
                        _log.Error("The relationship between the records already exists");
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    errorsCount++;
                    _log.Error(ex);
                    if (!options.ContinueOnError) throw;
                }
            }

            sw.Stop();
            _log.Info($"Done! Processed {recordCount} {relationshipMetadata.SchemaName} relationship records in {sw.Elapsed.TotalSeconds.ToString("0.00")} seconds. Created: {createdCount}. Errors: {errorsCount}");
        }

    }
}