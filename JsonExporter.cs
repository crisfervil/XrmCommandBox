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
        private string _extension = ".json";

        public string Extension { get { return _extension;  } }

        public void Export(DataCollection<Entity> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
