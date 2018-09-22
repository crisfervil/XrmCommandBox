using System;
using System.IO;
using System.Linq;
using log4net;

namespace XrmCommandBox.Data
{
    public class DataTableSerializer
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(DataTableSerializer));

        public void Serialize(DataTable data, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var serializer = GetSerializer(extension);
            serializer.Serialize(data, fileName);
        }

        public DataTable Deserialize(string fileName, string fileOptions)
        {
            var extension = Path.GetExtension(fileName);
            var serializer = GetSerializer(extension);
            var data = serializer.Deserialize(fileName, fileOptions);
            return data;
        }

        private ISerializer GetSerializer(string extension)
        {
            var emptyObjectArray = new object[] { };
            var arrayWithLogOnly = new object[] {_log};

            var serializers = Helper.GetObjectInstances<ISerializer>(new[] {emptyObjectArray, arrayWithLogOnly});

            var found = serializers.Where(x => x.Extension == extension).ToList();
            if (!found.Any()) throw new Exception($"No exporter found for extension {extension}");
            if (found.Count > 1) throw new Exception($"Too many exporters found for extension {extension}");
            return found[0];
        }
    }
}