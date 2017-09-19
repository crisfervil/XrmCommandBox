using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsDataTools.Data
{
    /// <summary>
    /// A DataTable is an object where every row is a dictionary of column names and values
    /// </summary>
    public class DataTable : List<Dictionary<string,string>>
    {
        public string Name { get; set; }
    }
}
