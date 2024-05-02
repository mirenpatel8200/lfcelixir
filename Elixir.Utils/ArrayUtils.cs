using System;
using System.Collections.Generic;

namespace Elixir.Utils
{
    public static class ArrayUtils
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> list, Func<T, T> func)
        {
            if(func == null)
                throw new ArgumentNullException(nameof(func));

            foreach (var e in list)
            {
                yield return func(e);
            }
        }
    }
}
