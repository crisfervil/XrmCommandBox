using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace DynamicsDataTools
{
    public class JsonExporter : IExporter
    {
        public string Extension { get; } = ".json";

        public void Export(DataCollection<Entity> data, string fileName, bool addRecordNumber)
        {
            throw new NotImplementedException();
        }
    }
}
