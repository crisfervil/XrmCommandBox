using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XrmCommandBox
{
    public static class Helper
    {
        public static Dictionary<Type, Delegate> ServicesMap = new Dictionary<Type, Delegate>();

        /// <summary>
        ///     Returns all the exported types containing the specified attribute
        /// </summary>
        public static Type[] GetTypesWithAttribute(Type attrType)
        {
            return Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(t => t.GetCustomAttribute(attrType) != null)
                .ToArray();
        }

        public static IList<T> GetObjectInstances<T>(object[][] parameterValues)
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
                var typeInstances = foundTypes.Select(x => (T) x.GetConstructor(typesArr)?.Invoke(parameterSet));

                foreach (var typeInstance in typeInstances)
                    foundInstances.Add(typeInstance);
            }

            return foundInstances;
        }

        public static object CreateInstance(Type handlerType)
        {
            object instance;
            var noParamsConstructor = handlerType.GetConstructor(Type.EmptyTypes);
            if (noParamsConstructor != null)
            {
                instance = noParamsConstructor.Invoke(null);
            }
            else
            {
                var constructors = handlerType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                // take the first available constructor
                var constructor = constructors[0];
                var constructorParameters = constructor.GetParameters();
                var constructorParameterValues = new List<object>();
                foreach (var constructorParameter in constructorParameters)
                {
                    var paramValueBuilder = ServicesMap[constructorParameter.ParameterType];
                    var paramValue = paramValueBuilder.DynamicInvoke();
                    constructorParameterValues.Add(paramValue);
                }

                instance = constructor.Invoke(constructorParameterValues.ToArray());
            }

            return instance;
        }


        /// <summary>
        ///     Returns all the types with public constructors with parameters of the specified types
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
                    found.Add(validType);
            }

            return found;
        }

        public static void RunTool(object toolInstance, object options)
        {
            toolInstance.GetType().GetMethod("Run")?.Invoke(toolInstance, new[] {options});
        }
    }
}