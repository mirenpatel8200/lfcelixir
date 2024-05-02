using System;
using System.Text;

namespace Elixir.Utils
{
    public static class NumericUtils
    {
        /// <summary>
        /// Converts a 0...675 number to base26 padded equivalent.
        /// Single-digit numbers will be padded with 0.
        /// For example values 00..09 will be represented as values from 'aa' to 'aj' respectively.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string ToSimple2CharsBase26(this int i)
        {
            if(i < 0 || i > 675)
                throw new ArgumentException("Input number should be in range from 0 to 99.", nameof(i));

            var first = i / 26;
            var second = i % 26;
            var sb = new StringBuilder(2);
            sb.Append(first.ToABasedLetter());
            sb.Append(second.ToABasedLetter());

            return sb.ToString();
        }

        public static char ToABasedLetter(this int i) => (char) (i + 'a');
    }
}
