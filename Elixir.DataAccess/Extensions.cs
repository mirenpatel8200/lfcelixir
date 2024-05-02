using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess
{
    public static class Extensions
    {
        public static bool AllMatch(this bool[] array)
        {
            foreach (var item in array)
            {
                if (item == false) return false;
            }
            return true;
        }
    }
}
