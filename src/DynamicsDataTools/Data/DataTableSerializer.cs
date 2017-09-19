using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataTools.Data
{
    public class DataTableSerializer
    {
        private ILog _log;

        public DataTableSerializer(ILog log)
        {
            this._log = log;
        }

        public static void Serialize(DataTable data, string fileName)
        {

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

            var serializers = Extensions.GetObjectInstances<ISerializer>(new object[][] { emptyObjectArray, arrayWithLogOnly });

            var found = serializers.Where(x => x.Extension == extension).ToList();
            if (!found.Any()) throw new Exception($"No exporter found for extension {extension}");
            if (found.Count > 1) throw new Exception($"Too many exporters found for extension {extension}");
            return found[0];
        }

    }
}
