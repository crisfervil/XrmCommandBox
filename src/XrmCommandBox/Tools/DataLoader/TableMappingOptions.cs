using System.Collections.Generic;

namespace XrmCommandBox.Tools.DataLoader
{
    public class TableMappingOptions
    {
        public string TableName { get; internal set; }
        public string EntityName { get; internal set; }

        public IEnumerable<ColumnMappingOptions> ColumnMappings;

        public IEnumerable<string> MatchAttributes { get; set; }
    }
}