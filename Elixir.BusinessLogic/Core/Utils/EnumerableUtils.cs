using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Core.Utils
{
    public static class EnumerableUtils
    {
        public static IEnumerable<TItem> SortByEnum<TItem, TEnum>(
            this IEnumerable<TItem> list,
            TEnum sortBy,
            SortDirection sortByDirection)
        {
            var fieldName = sortBy.ToString();
            var param = typeof(TItem).GetProperties()
                .FirstOrDefault(x => x.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));

            if (param == null)
                throw new ArgumentException("Unable to find Enum field in the object.");
            var isNullable = param.PropertyType.Name.IndexOf("nullable", StringComparison.OrdinalIgnoreCase) != -1;

            //if ()
            //{
            //    param = param.PropertyType.GetProperty("Value");
            //}

            var orderByParameter = Expression.Parameter(typeof(TItem));
            var orderByExpression = Expression.Lambda<Func<TItem, object>>(
                isNullable
                    ? Expression.PropertyOrField(Expression.PropertyOrField(orderByParameter, fieldName), "Value")
                    : Expression.PropertyOrField(orderByParameter, fieldName),
                orderByParameter
            ).Compile();

            switch (sortByDirection)
            {
                case SortDirection.Ascending:
                    return list.OrderBy(orderByExpression);
                case SortDirection.Descending:
                    return list.OrderByDescending(orderByExpression);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortByDirection), sortByDirection, null);
            }
        }

        public static IEnumerable<TItem> SortByEnum<TItem, TEnum>(
            this IEnumerable<TItem> list,
            TEnum sortBy,
            SortDirection sortByDirection,
            TEnum thenBy,
            SortDirection thenByDirection)
        {



            return list;
        }
    }
}
