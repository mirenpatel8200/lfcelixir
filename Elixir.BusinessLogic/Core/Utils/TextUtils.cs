using System;
using System.Web;
using System.Globalization;
using System.Linq;
using System.Text;
using Elixir.Models;

namespace Elixir.BusinessLogic.Core.Utils
{
    public static class TextUtils
    {
        /// <summary>
        /// Gets first sentence (defined by "\r\n", "\r", "\n") from string. If there is not such separator in input text, the input text will be returned.
        /// </summary>
        /// <param name="text">Input text which first sentence is to be found.</param>
        /// <returns></returns>
        public static string GetFirstSentence(this string text)
        {
            var parts = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return parts.FirstOrDefault() ?? text;
        }

        public static string StripTrackingParameters(this string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            string resultUrl = url;
            
            string query = url.IndexOf("?") > 0 ? 
                url.Substring(url.IndexOf("?") + 1) : null;

            if (string.IsNullOrEmpty(query) == false)
            {
                var newUrl = url.Substring(0, url.IndexOf("?")) + "?";

                var parameters = HttpUtility.ParseQueryString(query);
                int parametersUsed = 0;
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (parameters.Keys[i].IsTrackingType())
                    {
                        continue;
                    }
                    else
                    {
                        var pair = parameters.Keys[i] + "=" + parameters[i];
                        if (parametersUsed > 0)
                        {
                            newUrl += "&";
                        }  
                        newUrl += pair;
                        parametersUsed++;
                    }
                }
                resultUrl = newUrl;

                if (resultUrl.EndsWith("?"))
                {
                    //all parameters were skipped (tracking), so nothing added in query
                    resultUrl = resultUrl.Substring(0, resultUrl.Length - 1);
                }

                //If any anchor is in URL after the query params, 
                //put it the result
                var indexAnchor = url.LastIndexOf("#");
                var indexLastQueryParam = url.IndexOf(parameters[parameters.Count - 1]);
                if(indexAnchor > indexLastQueryParam)
                {
                    resultUrl += url.Substring(indexAnchor);
                }
            }
            
            return resultUrl;
        }

        public static string StripAnchors(this string url)
        {
            var indexAnchor = url.LastIndexOf("#");
            if (indexAnchor > 0 &&
                    //"#" is not on last position, and is followed by "/"
                    (indexAnchor < (url.Length - 1) &&
                    url[indexAnchor + 1] != '/') ||
                    //"#" is on last position
                    indexAnchor == url.Length - 1)
            {
                url = url.Substring(0, indexAnchor);
            }
            return url;
        }

        public static string TrimUrlToEssential(this string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                url = url.Substring(url.IndexOf("://") + 3);
            }
            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            if (url.StartsWith("www."))
            {
                url = url.Substring(4);
            }
            return url;
        }

        public static string UrlFullCleanup(this string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            url = url.StripAnchors();
            url = url.StripTrackingParameters();
            url = url.TrimUrlToEssential();
            
            return url;
        }

        /// <summary>
        /// Gets 
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        public static void ProcessSummary(this Article article)
        {
            if (string.IsNullOrWhiteSpace(article.Summary) && !string.IsNullOrWhiteSpace(article.BulletPoints))
                article.Summary = article.BulletPoints.GetFirstSentence();
        }

        public static string SubstringSafe(this string text, int startIndex, int length)
        {
            return text.Length <= length ? text : text.Substring(0, length);
        }

        /// <summary>
        /// Gets substring of string until the first non-alphanumeric (and non-slash, non-underscore) character.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Null if text is null, truncated string otherwise.</returns>
        public static string GetAlphanumericValue(this string text)
        {
            if (text == null)
                return null;

            var index = -1;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (!(char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '(' || c == ')'))
                {
                    index = i;
                    break;
                }
            }

            return index == -1 ? text: text.Substring(0, index);
        }

        /// <summary>
        /// Converts specified text to valid URL string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertToUrlName(string text)
        {
            if(text == null)
                throw new ArgumentNullException(nameof(text));

            var sb = new StringBuilder();
            const string toDash = " ,.:;_+=!?|\\/$#%&";

            var filteredText = text
                .ToLower(CultureInfo.InvariantCulture)
                .Replace("'", "")
                .Replace("\"", "");

            foreach (var c in filteredText)
            {
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsSeparator(c) || toDash.IndexOf(c) != -1)
                    sb.Append('-');
                else
                    sb.Append(c);
            }

            return sb.ToString().CollapseRepeated('-').Trim('-');
        }

        /// <summary>
        /// Finds sequentially repeated symbols and changes them to a single one.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="symbol">Symbol which repeated sequence is to be found and replaced with.</param>
        /// <returns></returns>
        public static string CollapseRepeated(this string text, char symbol)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (text.Equals("", StringComparison.OrdinalIgnoreCase))
                return "";

            var sb = new StringBuilder(text[0].ToString());
            for (var i = 1; i < text.Length; i++)
            {
                if (text[i].Equals(symbol))
                {
                    if (text[i] != text[i - 1])
                        sb.Append(text[i]);
                }
                else
                {
                    sb.Append(text[i]);
                }
            }

            return sb.ToString();
        }

        public static bool IsTrackingType(this string queryParameter)
        {
            /// keep list in alphabetical order when adding new entries, so that it is easier to check if exists
            return queryParameter == "_r" ||
                queryParameter == "_ga" ||
                queryParameter == "CMP" ||
                queryParameter == "fbclid" ||
                queryParameter == "feedType" ||
                queryParameter == "feedName" ||
                queryParameter == "fsrc" ||
                queryParameter == "full" ||
                queryParameter == "guccounter" ||
                queryParameter == "mc_cid" || 
                queryParameter == "ocid" ||
                queryParameter == "op" ||
                queryParameter == "originalSubdomain" ||
                queryParameter == "page" ||
                queryParameter == "rm" ||
                queryParameter == "register" ||
                queryParameter == "src" ||
                queryParameter == "sessionId" ||
                 queryParameter == "utm_content" || 
                 queryParameter == "utm_term" ||
                queryParameter == "utm_source" 
               ;

        }

        public static string ToTagsFormat(this IOrderedEnumerable<Resource> resources)
        {
            string result = resources.Count() == 0 ? "" : string.Join("|", 
                resources.Select(o => o.ResourceName + " [" + o.Id + "]").ToList());

            return result;
        }
    }
}
