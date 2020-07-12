using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

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


        /// <summary>
        /// Extension Method to convert enumerable to data table based off column schema <see cref="ColumnSchema{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of object to convert</typeparam>
        /// <param name="enumerable">Enumerabl of objects</param>
        /// <param name="columns"><see cref="ColumnSchema{T}"/>"/></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable, params ColumnSchema<T>[] columns)
        {
            var dataTable = new DataTable();

            foreach(var col in columns)
            {
                dataTable.Columns.Add(new DataColumn
                {
                    DataType = col.Type,
                    ColumnName = col.ColumnName,
                    MaxLength = col.MaxLength
                });
            }

            foreach(var obj in enumerable)
            {
                var row =  dataTable.NewRow();

                foreach(var col in columns)
                {
                    row[col.ColumnName] = col.PropertyAccessor(obj);
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static async Task<AutoRollBackTransaction> BeginAutoRollBackTransactionAsync(this SqlConnection connection)
        {
            var autoRollBackTransaction = new AutoRollBackTransaction();
            return await autoRollBackTransaction.BeginTransactionAsync(connection);
        }      
    }
}
