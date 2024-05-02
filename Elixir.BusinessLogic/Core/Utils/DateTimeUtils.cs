using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.BusinessLogic.Core.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime ToTimeZero(this DateTime dt)
        {
            return new DateTime(dt.Date.Year, dt.Date.Month, dt.Date.Day, 0, 0, 0);
        }
    }
}
