using System;

namespace Elixir.Helpers
{
    public static class DateTimeUtils
    {
        public static string FormatDdMmYyyy(this DateTime dateTime, bool monthAsNumber = true)
        {
            return monthAsNumber ? dateTime.ToString("dd-MM-yyyy") : dateTime.ToString("dd-MMM-yyyy");
        }

        public static string FormatDdMmYyyyHHmmS(this DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static string FormatYyyyMmDd(this DateTime dateTime)
        {
            return dateTime.ToString("yyy-MM-dd");
        }

        public static string FormatDdMmmYyyy(this DateTime dateTime)
        {
            return dateTime.ToString("dd-MMM-yyyy");
        }

        public static string ToLongFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public static string ToLongFormatNoZone(this DateTime dateTime)
        {
            var dateLongFormat = dateTime.ToLongFormat();
            if(!dateLongFormat.EndsWith("+00:00"))
                dateLongFormat = dateTime.ToString("yyyy-MM-ddTHH:mm:ss") +"+00:00";

            return dateLongFormat;
        }
    }
}