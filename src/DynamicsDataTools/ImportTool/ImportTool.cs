using log4net;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DynamicsDataTools.ImportTool
{
    public class ImportTool
    {
        private readonly ILog _log;
        private readonly IOrganizationService _crmService;

        public ImportTool(ILog log, IOrganizationService service)
        {
            _log = log;
            _crmService = service;
        }

        public void Run(ImportOptions options)
        {
            var extension = Path.GetExtension(options.File);
            IDataReader reader = GetReader(extension);

            // Read the file and get the records to import
            var records = reader.Read(options.File);
        }

        private IDataReader GetReader(string extension)
        {
            var exporters = GetAvailableReaders();
            var found = exporters.Where(x => x.Extension == extension).ToList();
            if (!found.Any()) throw new Exception($"No exporter found for extension {extension}");
            if (found.Count > 1) throw new Exception($"Too many exporters found for extension {extension}");
            return found[0];
        }

        private IList<IDataReader> GetAvailableReaders()
        {
            var emptyObjectArray = new object[] { };
            var arrayWithLogOnly = new object[] { _log };

            var readers = GetObjectInstances<IDataReader>(new object[][] { emptyObjectArray, arrayWithLogOnly });

            return readers;
        }

        private IList<T> GetObjectInstances<T>(Object[][] parameterValues)
        {
            IList<T> foundInstances = new List<T>();
            foreach (var parameterSet in parameterValues)
            {
                // Get all the types in item (is an object array)
                var types = parameterSet.Select(x => x.GetType());
                var typesArr = types.ToArray();

                // Get all the types in the assembly implementing T with a constructor of the parameterset type
                var foundTypes = GetTypes<T>(typesArr);

                // create an instance of each type found
                var typeInstances = foundTypes.Select(x => (T)x.GetConstructor(typesArr).Invoke(parameterSet));

                foreach (var typeInstance in typeInstances)
                {
                    foundInstances.Add(typeInstance);
                }
            }

            return foundInstances;
        }


        /// <summary>
        /// Returns all the types with public constructors with parameters of the specified types
        /// </summary>
        /// <typeparam name="T">Interface that must be implemented by the returned type</typeparam>
        /// <param name="constructorParameterTypes"></param>
        /// <returns></returns>
        private IList<Type> GetTypes<T>(Type[] constructorParameterTypes)
        {

            var found = new List<Type>();
            var existingAssemblies = Assembly.GetExecutingAssembly().GetExportedTypes();

            var validTypes = existingAssemblies.Where(x => x.GetInterfaces().Contains(typeof(T)));

            foreach (var validType in validTypes)
            {
                var constructor = validType.GetConstructor(constructorParameterTypes);
                if(constructor != null)
                {
                    found.Add(validType);
                }
            }

            return found;
        }

    }
}
