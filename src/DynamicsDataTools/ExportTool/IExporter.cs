using log4net;
using Microsoft.Xrm.Sdk;

namespace DynamicsDataTools.ExportTools
{
    public interface IExporter
    {
        string Extension { get; }
        void Export(DataCollection<Entity> data, string fileName, bool addRecordNumber);
    }
}