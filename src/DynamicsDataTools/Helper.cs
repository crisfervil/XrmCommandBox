using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataTools
{
    public static class Helper
    {
        public static IList<T> GetObjectInstances<T>(Object[][] parameterValues)
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
        public static IList<Type> GetTypes<T>(Type[] constructorParameterTypes)
        {
            var found = new List<Type>();
            var existingAssemblies = Assembly.GetExecutingAssembly().GetExportedTypes();

            var validTypes = existingAssemblies.Where(x => x.GetInterfaces().Contains(typeof(T)));

            foreach (var validType in validTypes)
            {
                var constructor = validType.GetConstructor(constructorParameterTypes);
                if (constructor != null)
                {
                    found.Add(validType);
                }
            }

            return found;
        }
    }
}
