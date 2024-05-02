using System;
using System.Text.RegularExpressions;

namespace Elixir.Utils.View
{
    public static class ViewUtils
    {
        public static string FormatAutocompleteResource(string resourceName, int? id)
        {
            if (id.HasValue && id.Value != 0)
                return $"{resourceName}[{id}]";

            return $"{resourceName}";
        }
        public static string FormatAutocompleteResourceDetailed(string resourceName, int? id, string description)
        {
            string result = resourceName;
            if (id.HasValue && id.Value != 0)
                result += $" [{id}]";
            if (!string.IsNullOrEmpty(description))
                result += $" ({description})";
            if (result.Length > 110)
                result = result.Substring(0, 110) + "...";

            return result;
        }

        public static string FormatAutocompleteParentResource(string parentResourceName, int? parentResourceId)
        {
            if (parentResourceId.HasValue && parentResourceId.Value != 0)
                return $"{parentResourceName}[{parentResourceId}]";

            return $"{parentResourceName}";
        }

        public static ResourceInfo ParseAutocompleteResource(string resourceWithId)
        {
            var m = Regex.Match(resourceWithId, @"^(?<name>.+?)\s*?(\[(?<id>\d+)\])?$");
            if (!m.Success)
                throw new FormatException($"The resource with Id string has incorrect format.");

            int pId = int.MinValue;

            if (m.Groups["id"].Success)
            {
                if (!int.TryParse(m.Groups["id"].Value, out pId))
                {
                    throw new FormatException($"Unable to parse publisher Id in the Resource with Id string.");
                }
            }

            return new ResourceInfo(m.Groups["name"].Value, pId == int.MinValue ? (int?)null : pId);
        }
    }
}
