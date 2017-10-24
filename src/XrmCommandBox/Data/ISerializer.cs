namespace XrmCommandBox.Data
{
    public interface ISerializer
    {
        string Extension { get; }
        void Serialize(DataTable data, string fileName);
        DataTable Deserialize(string fileName);
    }
}