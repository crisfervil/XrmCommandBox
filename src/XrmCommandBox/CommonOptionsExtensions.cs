using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Microsoft.Xrm.Sdk;

namespace XrmCommandBox
{
    public static class CommonOptionsExtensions
    {
        public static IOrganizationService GetConnection(this CommonOptions options)
        {
            return new ConnectionBuilder().GetConnection(options.ConnectionName);
        }

        public static void ConfigureLog(this CommonOptions options)
        {
            var logLevel = Enum.GetName(typeof(LogLevels), options.LogLevel);
            logLevel = logLevel?.ToUpper();

            BasicConfigurator.Configure(); // Configure the log from app.config file

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
    }
}
