using System;

namespace Elixir.Views.Helpers
{
    public static class ViewHelpers
    {
        public static String FormatPagesCountString(int pagesCount)
        {
            return pagesCount == 1 ? $"{pagesCount} page": $"{pagesCount} pages";
        }
    }
}