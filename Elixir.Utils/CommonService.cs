using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Utils
{
    public static class CommonService
    {
        public static string ElixcriptToSimpleText(string text, List<string> roles)
        {
            while (text.Contains("[LFC:"))
            {
                var substring = text.Substring(text.IndexOf("[LFC:"));
                var elixcript = substring.Substring(0, substring.IndexOf("]") + 1);
                if (!string.IsNullOrEmpty(elixcript))
                {
                    var elixcriptArray = elixcript.Substring(elixcript.IndexOf("Text:") + 5);
                    var alternativeText = "";
                    var mainText = "";
                    if (elixcriptArray.IndexOf('|') > 0)
                    {
                        var displayTextArray = elixcriptArray.Split('|');
                        if (displayTextArray != null && displayTextArray.Length > 1)
                        {
                            mainText = displayTextArray[0];
                            alternativeText = displayTextArray[1];
                        }
                    }
                    else
                        mainText = elixcriptArray;
                    var loggedInRoleExistsInElixcript = false;
                    if (roles != null && roles.Any())
                    {
                        foreach (var role in roles)
                        {
                            if (elixcript.ToLower().Contains(role.ToLower()))
                            {
                                loggedInRoleExistsInElixcript = true;
                            }
                        }
                    }
                    if (loggedInRoleExistsInElixcript)
                    {
                        mainText = mainText
                            .Replace("]", "");
                        text = text.Replace(elixcript, mainText);
                    }
                    else
                    {
                        alternativeText = alternativeText
                          .Replace("]", "");
                        text = text.Replace(elixcript, alternativeText);
                    }
                }
            }
            return text;
        }
    }
}
