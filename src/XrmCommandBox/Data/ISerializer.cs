namespace XrmCommandBox.Data
{
    public interface ISerializer
    {
        string Extension { get; }
        void Serialize(DataTable data, string fileName, bool addRecordNumber = false);
        DataTable Deserialize(string fileName);
        DataTable Deserialize(string fileName, out bool containsRecordNumber);
    }
}