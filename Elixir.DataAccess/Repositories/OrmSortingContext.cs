using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories
{
    /// <summary>
    /// Contains all information needed for sorting using OrmTools class.
    /// </summary>
    public class OrmSortingContext
    {
        public OrmSortingContext(IEnumerable<string> sortingFields, IEnumerable<SortDirection> sortingDirections)
        {
            SortingFields = sortingFields.ToArray();
            SortingDirections = sortingDirections.ToArray();

            if (SortingFields.Length != SortingDirections.Length)
                throw new ArgumentException("The number of fields is not equal to number of sort directions.");
        }

        public OrmSortingContext(IEnumerable<string> sortingFields, IEnumerable<SortDirection> sortingDirections, IEnumerable<string> tableNames) : this(sortingFields, sortingDirections)
        {
            TableNames = tableNames.ToArray();

            if (TableNames.Length != SortingFields.Length)
                throw new ArgumentException("The number of table names is not equal to number of fields.");
        }

        /// <summary>
        /// Fields that are to be used for sorting.
        /// </summary>
        public string[] SortingFields { get; }
        /// <summary>
        /// Sorting orders corresponding to each sorting field in SortingFields array.
        /// </summary>
        public SortDirection[] SortingDirections { get; }
        /// <summary>
        /// Table names corresponding to each sorting field in SortingFields array.
        /// </summary>
        public string[] TableNames { get; }

        /// <summary>
        /// Replaces a field with new field in SortingFields array. In addition it is possible to replace associated table name.
        /// Helpful when sorting field is not included in a single table, i.e. it is joined from another table.
        /// </summary>
        /// <param name="oldField"></param>
        /// <param name="newField"></param>
        /// <param name="newTableName"></param>
        public void ReplaceField(string oldField, string newField, string newTableName = null)
        {
            var index = Array.FindIndex(SortingFields, s => s.Equals(oldField, StringComparison.OrdinalIgnoreCase));
            if (index != -1)
            {
                SortingFields[index] = newField;
                if (newTableName != null)
                    TableNames[index] = newTableName;
            }
        }
    }
}
