using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrmCommandBox.Data
{
	public class ExcelSerializer : ISerializer
	{
		public string Extension => ".xlsx";

		public DataTable Deserialize(string fileName, string sheetName)
		{
			DataTable dataTable = null;

			using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
			{
				using (var reader = ExcelReaderFactory.CreateReader(stream))
				{
					var sheetRead = false;
					do
					{
						if (string.Compare(reader.Name, sheetName, true) == 0)
						{
							dataTable = ReadTable(reader);
							dataTable.Name = sheetName;
							sheetRead = true;
						}

					} while (!sheetRead && reader.NextResult());
				}
			}

			return dataTable;
		}

		private DataTable ReadTable(IExcelDataReader reader)
		{
			DataTable data = new DataTable();
			var isFirst = true;
			var columnIndexes = new List<string>();

			while (reader.Read())
			{
				if (isFirst) // assuming first rows contains column names
				{
					for (var i = 0; i < reader.FieldCount; i++)
					{
						var columnName = Convert.ToString(reader.GetString(i));
						columnName = string.IsNullOrEmpty(columnName) ? $"Column{i + 1}" : columnName;
						columnIndexes.Add(columnName);
					}

					isFirst = false;
				}
				else
				{
					var dataRow = new Dictionary<string, object>();
					for (var i=0; i< columnIndexes.Count; i++)
					{
						var colName = columnIndexes[i];
						var colValue = reader.GetValue(i)?.ToString();
						if (colValue != null)
						{
							dataRow.Add(colName, colValue);
						}
					}
					if (dataRow.Count > 0) // skip empty rows
					{
						data.Add(dataRow);
					} 
				}
			}
			return data;
		}

		public void Serialize(DataTable data, string fileName)
		{
			throw new NotImplementedException();
		}
	}
}
