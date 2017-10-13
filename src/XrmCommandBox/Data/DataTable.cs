using System.Collections.Generic;

namespace XrmCommandBox.Data
{
    /// <summary>
    ///     A DataTable is an object where every row is a dictionary of column names and values
    /// </summary>
    public class DataTable : List<Dictionary<string, object>>
    {
        public string Name { get; set; }
    }
}