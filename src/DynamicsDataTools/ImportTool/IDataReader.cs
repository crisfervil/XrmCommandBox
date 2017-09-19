using DynamicsDataTools.Data;

namespace DynamicsDataTools.ImportTool
{
    public interface IDataReader
    {
        string Extension { get; }
        DataTable Read(string fileName);
    }
}