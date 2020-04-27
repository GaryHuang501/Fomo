using System;
using System.Collections.Generic;
using System.Data;

namespace FomoAPI.Infrastructure.Repositories
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Extension method to convert IDictionary to a datatable. KVP is flattened into a row.
        /// </summary>
        /// <typeparam name="K">Type of the Key</typeparam>
        /// <typeparam name="T">Type of Value</typeparam>
        /// <param name="dict">Dictionary to convert</param>
        /// <param name="keyName">Name of dictionary Key</param>
        /// <param name="columnName">Name of Column</param>
        /// <returns>DataTable with each KVP as a row.</returns>
       public static DataTable ToDataTable<K,V>(this IDictionary<K, V> dict, string keyName, string columnName)
        {
            var dataTable = new DataTable();

            dataTable.Columns.Add(new DataColumn
                {
                    DataType = typeof(K),
                    ColumnName = keyName
                });

            dataTable.Columns.Add(new DataColumn
                {
                    DataType = typeof(V),
                    ColumnName = columnName
                });

            foreach (var kvp in dict)
            {
                var row = dataTable.NewRow();
                row[keyName] = kvp.Key;
                row[columnName] = kvp.Value;
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
