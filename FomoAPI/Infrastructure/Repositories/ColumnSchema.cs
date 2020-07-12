using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Data class for RepositoryExtensions for column schema info
    /// </summary>
    public class ColumnSchema<T>
    {

        public string ColumnName { get;}

        public Type Type { get; }

        public int MaxLength { get; }

        /// <summary>
        /// Method to return the property from the type T object/class
        /// </summary>
        public Func<T, object> PropertyAccessor { get;}

        public ColumnSchema(string columnName, Type type, Func<T, object> propertyAccessor, int maxLength = -1)
        {
            ColumnName = columnName;
            Type = type;
            PropertyAccessor = propertyAccessor;
            MaxLength = maxLength;
        }
    }
}

