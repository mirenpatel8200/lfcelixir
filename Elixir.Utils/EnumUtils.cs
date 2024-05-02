using System;
using System.Collections.Generic;
using System.Linq;

namespace Elixir.Utils
{
    public static class EnumUtils
    {
        /// <summary>
        /// Checks if Flagged enum has more than 1 flag.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static bool HasMultipleFlags(this Enum enumValue)
        {
            return enumValue.GetFlags().Count() > 1;
        }

        /// <summary>
        /// Returns collection of enum values which are set in specified enum value.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> GetFlags(this Enum enumValue)
        {
            return Enum.GetValues(enumValue.GetType()).Cast<Enum>().Where(enumValue.HasFlag);
        }
    }
}
