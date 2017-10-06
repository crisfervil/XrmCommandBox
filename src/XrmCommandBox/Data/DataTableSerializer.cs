using log4net;
using System;
using System.IO;
using System.Linq;

namespace XrmCommandBox.Data
{
    public class DataTableSerializer
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(DataTableSerializer));

        public void Serialize(DataTable data, string fileName, bool addRecordNumber = false)
        {
            var extension = Path.GetExtension(fileName);
            var serializer = GetSerializer(extension);
            serializer.Serialize(data,fileName, addRecordNumber);
        }

        public DataTable Deserialize(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var serializer = GetSerializer(extension);
            var data = serializer.Deserialize(fileName);
            return data;
        }

        private ISerializer GetSerializer(string extension)
        {
            var emptyObjectArray = new object[] { };
            var arrayWithLogOnly = new object[] { _log };

            var serializers = Helper.GetObjectInstances<ISerializer>(new object[][] { emptyObjectArray, arrayWithLogOnly });

            var found = serializers.Where(x => x.Extension == extension).ToList();
            if (!found.Any()) throw new Exception($"No exporter found for extension {extension}");
            if (found.Count > 1) throw new Exception($"Too many exporters found for extension {extension}");
            return found[0];
        }

    }
}
