using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.Helpers
{
    public static class OnlineManualHelper
    {
        public const int LastManualPage = 172;

        public static bool IsChapterPage(this int pageNumber)
        {
            return (pageNumber > 0 && pageNumber <= 67) || pageNumber >= 169 && pageNumber <= 171;
        }

        public static bool IsTipPage(this int pageNumber)
        {
            return pageNumber >= 68 && pageNumber <= 168;
        }
    }
}