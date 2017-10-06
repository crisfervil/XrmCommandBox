using System;
using CommandLine;
using log4net;
using log4net.Config;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xrm.Sdk;
using XrmCommandBox.Tools;

namespace XrmCommandBox
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static IOrganizationService _crmService = null;

        public static void Main(string[] args)
        {
            try
            {

                Parser.Default.ParseArguments<ExportToolOptions, ImportToolOptions, DeleteToolOptions>(args)
                    .MapResult((ImportToolOptions opts) => RunImportAndReturnExitCode(opts),
                                (ExportToolOptions opts) => RunExportAndReturnExitCode(opts),
                                (DeleteToolOptions opts) => RunDeleteAndReturnExitCode(opts),
                                HandleErrors);

            }
            catch (Exception ex)
            {
                if(ex.InnerException!= null) Log.Error(ex.InnerException);
                Log.Error($"Unexpected error: {ex.Message}");
                Log.Error(ex);
                Environment.Exit(-1);
            }
        }

        private static void ConfigureLog(CommonOptions options)
        {

            var logLevel = Enum.GetName(typeof(LogLevels), options.LogLevel);
            logLevel = logLevel?.ToUpper();

            BasicConfigurator.Configure();

            // http://geekswithblogs.net/rakker/archive/2007/08/22/114900.aspx
            var repositories = LogManager.GetAllRepositories();

            //Configure all loggers to be at the speified level.
            foreach (var repository in repositories)
            {
                repository.Threshold = repository.LevelMap[logLevel];
                var hier = (log4net.Repository.Hierarchy.Hierarchy)repository;
                var loggers = hier.GetCurrentLoggers();
                foreach (var logger in loggers)
                {
                    ((log4net.Repository.Hierarchy.Logger)logger).Level = hier.LevelMap[logLevel];
                }
            }

            //Configure the root logger.
            var logHierarchy = (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository();
            var rootLogger = logHierarchy.Root;
            rootLogger.Level = logHierarchy.LevelMap[logLevel];

        }

        private static void InitConnection(CommonOptions options)
        {
            _crmService = new ConnectionBuilder().GetConnection(options.ConnectionName);
        }

        private static void Init(CommonOptions options)
        {
            if (options.DebugBreak)
            {
                System.Diagnostics.Debugger.Launch();
            }

            ConfigureLog(options);

            new CommandOptionsSerializer().Deserialize(options);

            InitConnection(options);
        }

        private static int RunImportAndReturnExitCode(ImportToolOptions opts)
        {
            Init(opts);
            new ImportTool(_crmService).Run(opts);
            return 0;
        }

        private static int RunExportAndReturnExitCode(ExportToolOptions opts)
        {
            Init(opts);
            new ExportTool(_crmService).Run(opts);
            return 0;
        }

        private static int RunDeleteAndReturnExitCode(DeleteToolOptions opts)
        {
            Init(opts);
            new DeleteTool(_crmService).Run(opts);
            return 0;
        }

        private static int HandleErrors(IEnumerable<Error> errors)
        {
            return -1;
        }
    }
}
