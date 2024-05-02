using System;
using System.Linq;
using System.Text;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories
{
    public class OrmTools
    {
        private readonly string _inputSql;
        public OrmTools(string inputSql)
        {
            _inputSql = inputSql;
        }

        /// <summary>
        /// Adds sorting clause to specified SQL if it's not present.
        /// Input string is not changed.
        /// If ORDER BY is present then the exception will be thrown.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Updated copy of string.</returns>
        public string AppendSorting(OrmSortingContext ctx)
        {
            if (_inputSql.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) != -1)
                throw new ArgumentException("Input sql already has ORDER BY clause.");

            var sb = new StringBuilder(_inputSql);
            sb.Append(" ORDER BY ");

            var sf = ctx.SortingFields.ToArray();
            if (ctx.TableNames != null)
                AppendTableNames(sf, ctx.TableNames);

            for (var i = 0; i < sf.Length; i++)
            {
                sb.Append(sf[i]);
                sb.Append(" ");
                sb.Append(DirectionToSql(ctx.SortingDirections[i]));
                if (i != sf.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts SortDirection to SQL direction of sorting.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private string DirectionToSql(SortDirection direction)
        {
            return direction == SortDirection.Ascending ? "ASC" : "DESC";
        }

        /// <summary>
        /// Appends elements from tableName to sortingFields accordingly.
        /// </summary>
        /// <param name="sortingFields"></param>
        /// <param name="tableNames"></param>
        private void AppendTableNames(string[] sortingFields, string[] tableNames)
        {
            for (var i = 0; i < sortingFields.Length; i++)
                sortingFields[i] = $"{tableNames[i]}.{sortingFields[i]}";
        }
    }
}
