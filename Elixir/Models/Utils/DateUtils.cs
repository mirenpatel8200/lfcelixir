using System;

namespace Elixir.Models.Utils
{
    public static class DateUtils
    {
        public const String DateFormat = "yyyy-MM-dd";

        public static String YearFirstFormat(this DateTime date)
        {
            return date.ToString("yyyy.MM.dd");
        }
    }
}