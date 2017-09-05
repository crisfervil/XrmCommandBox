using log4net;
using Microsoft.Xrm.Sdk;

namespace DynamicsDataTools
{
    public interface IExporter
    {
        string Extension { get; }
        void Export(DataCollection<Entity> data, string fileName);
    }
}