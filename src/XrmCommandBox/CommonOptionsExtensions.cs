using System;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox
{
    public static class CommonOptionsExtensions
    {
        public static IOrganizationService GetConnection(this CommonOptions options)
        {
            return new ConnectionBuilder().GetConnection(options.ConnectionName);
        }

        public static void SetLogLevel(this CommonOptions options)
        {
            var logLevel = Enum.GetName(typeof(LogLevels), options.LogLevel);
            logLevel = logLevel?.ToUpper();

            // http://geekswithblogs.net/rakker/archive/2007/08/22/114900.aspx
            var repositories = LogManager.GetAllRepositories();

            //Configure all loggers to be at the speified level.
            foreach (var repository in repositories)
            {
                repository.Threshold = repository.LevelMap[logLevel];
                var hier = (Hierarchy) repository;
                var loggers = hier.GetCurrentLoggers();
                foreach (var logger in loggers)
                    ((Logger) logger).Level = hier.LevelMap[logLevel];
            }

            //Configure the root logger.
            var logHierarchy = (Hierarchy) LogManager.GetRepository();
            var rootLogger = logHierarchy.Root;
            rootLogger.Level = logHierarchy.LevelMap[logLevel];
        }
    }
}