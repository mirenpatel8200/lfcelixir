using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Elixir.Helpers
{
    public class HtmlUtils
    {
        public static HtmlComponentList ExtractHTMLElementsPAndH(string text)
        {
            HtmlComponentList list = new HtmlComponentList();
            if (string.IsNullOrEmpty(text))
            {
                list.JustPlainText = true;
                return list;
            }
            
            int startH3 = text.IndexOf("<h3>");
            int startH4 = text.IndexOf("<h4>");
            int startP = text.IndexOf("<p>");

            if (startH3 < 0 && startH4 < 0 && startP < 0)
            {
                list.JustPlainText = true;
                return list;
            }
            #region H3
            while (startH3 != -1)
            {
                int endH3 = text.IndexOf("</h3>", startH3);
                if (endH3 == -1)
                {
                    //badly formatted html -> ignore, add as plain text
                    list.JustPlainText = true;
                    return list;
                }
                string element = text.Substring(startH3, endH3 + "</h3>".Length - startH3);
                string innerText = element.Substring("<h3>".Length, element.Length - ("<h3>".Length + "</h3>".Length));
                H3 h3 = new H3 { Text = innerText, StartIndex = startH3 };

                list.Elements.Add(h3);
                startH3 = text.IndexOf("<h3>", endH3 + 4);
            }
            #endregion

            #region H4
            while (startH4 != -1)
            {
                int endH4 = text.IndexOf("</h4>", startH4);
                if (endH4 == -1)
                {
                    list.JustPlainText = true;
                    return list;
                }
                string element = text.Substring(startH4, endH4 - startH4 + 5);
                string innerText = element.Substring(4, element.Length - 9);
                H4 h4 = new H4 { Text = innerText, StartIndex = startH4 };

                list.Elements.Add(h4);
                startH4 = text.IndexOf("<h4>", endH4 + 4);
            }
            #endregion

            #region P
            while (startP != -1)
            {
                int endP = text.IndexOf("</p>", startP);
                if (endP == -1)
                {
                    list.JustPlainText = true;
                    return list;
                }
                string element = text.Substring(startP, endP - startP + 4);
                string innerText = element.Substring(3, element.Length - 7);
                innerText = innerText.Trim();
                innerText = innerText.Replace("\r\n", String.Empty);
                bool hasBreaks = innerText.IndexOf("<br>") > 0;
                P p = new P
                {
                    Text = innerText,
                    HasBreaks = hasBreaks,
                    StartIndex = startP
                };

                list.Elements.Add(p);

                startP = text.IndexOf("<p>", endP + 3);
            }
            #endregion

            list.Elements =
                list.Elements.OrderBy(e => e.StartIndex).ToList();

            return list;

        }

        public static HtmlComponentList ExtractHTMLElements(string text)
        {
            HtmlComponentList list = new HtmlComponentList();
            if (string.IsNullOrEmpty(text))
            {
                list.JustPlainText = true;
                return list;
            }
            int startStrong = text.IndexOf("<strong>");
            int startP = text.IndexOf("<p>");
            int startH2 = text.IndexOf("<h2>");
            int startSup = text.IndexOf("<sup>");
            int startPWithStyle = -1;
            int startImage = -1;
            string pStylePattern = @"<p style\s?=\s?[""]([a-z-:;]+)[""]";
            //If you want floating images, please specify first src, THEN float value.
            string imgPattern = @"<img src\s?=\s?[""]([a-zA-Z0-9+-_]+).(jpg|png)[""](\sfloat\s?=\s?[""](left|right)[""])?\s?/>";

            MatchCollection pStyleMatches = Regex.Matches(text, pStylePattern);
            MatchCollection imgMatches = Regex.Matches(text, imgPattern);

            #region Initial checks
            int psIndex = 0, iIndex = 0;
            if (pStyleMatches.Count > 0)
            {
                startPWithStyle = pStyleMatches[0].Index;
                psIndex++;
            }
            if(imgMatches.Count > 0)
            {
                startImage = imgMatches[0].Index;
                iIndex++;
            }
            
            if (startStrong < 0 && startP < 0 && startPWithStyle < 0 && startH2 < 0 && startImage < 0)
            {
                list.JustPlainText = true;
                return list;
            }
            #endregion

            #region Parsing <strong> tag
            while (startStrong != -1)
            {
                int endStrong = text.IndexOf("</strong>", startStrong);
                if (endStrong == -1)
                {
                    //tag started, but not closed. => ignore markup, add text plain
                    list.JustPlainText = true;
                    return list;
                }
                
                //only if independent tag here     
                int neighborPLeftStart = text.LastIndexOf("<p", startStrong);
                int neighborPRightEnd = text.IndexOf("</p>", endStrong);
                if (neighborPLeftStart >= 0 && neighborPRightEnd >= 0)
                {
                    int neighborPLeftEnd = text.IndexOf("</p>", neighborPLeftStart);
                    int neighborPRightStart = text.LastIndexOf("<p", neighborPRightEnd);

                    if ((neighborPLeftStart < 0 && neighborPLeftEnd < 0 &&
                        neighborPRightStart < 0 && neighborPRightStart < 0) ||
                        (neighborPLeftStart == neighborPRightStart &&
                        neighborPRightEnd == neighborPLeftEnd))
                    {
                        startStrong = text.IndexOf("<strong>", endStrong + 3);
                        continue;
                    }
                }

                string element = text.Substring(startStrong, endStrong - startStrong + 8 + 1);
                string innerText = element.Substring(8, element.Length - 17);
                Strong s = new Strong { Text = innerText, StartIndex = startStrong };

                list.Elements.Add(s);
                startStrong = text.IndexOf("<strong>", endStrong + 3);
            }
            #endregion

            #region Parsing <p> tag
            while (startP != -1 || startPWithStyle != -1)
            {
                if(startP!= -1)
                {
                    int endP = text.IndexOf("</p>", startP);
                    if (endP == -1)
                    {
                        list.JustPlainText = true;
                        return list;
                    }
                    string element = text.Substring(startP, endP - startP + 4);
                    string innerText = element.Substring(3, element.Length - 7);
                    if(innerText != " ")
                        innerText = innerText.Trim();
                    innerText = innerText.Replace("\r\n", string.Empty);
                    bool hasBreaks = innerText.IndexOf("<br>") > 0;
                    bool hasStrongChunks = innerText.IndexOf("<strong>") >= 0;
                    bool hasItalicChunks = innerText.IndexOf("<em>") >= 0;
                    P p = new P
                    {
                        Text = innerText,
                        HasBreaks = hasBreaks,
                        HasStrongChunks = hasStrongChunks,
                        HasItalicChunks = hasItalicChunks,
                        StartIndex = startP                     
                    };

                    list.Elements.Add(p);

                    startP = text.IndexOf("<p>", endP + 3);
                }
                if(startPWithStyle != -1)
                {
                    string alignValue = null;
                    int endP = text.IndexOf("</p>", startPWithStyle);
                    if (endP == -1)
                    {
                        list.JustPlainText = true;
                        return list;
                    }

                    string quote= @"""";

                    int quoteStart = text.IndexOf(quote, startPWithStyle);
                    int quoteEnd = text.IndexOf(quote, quoteStart+1);
                    if (quoteStart < 0 || quoteEnd < 0)
                    {
                        list.JustPlainText = true;
                        return list;
                    }
                    string pStyle = text.Substring(quoteStart, quoteEnd - quoteStart);
                    int textAlignIndex = pStyle.IndexOf("text-align");
                    int columnIndex = pStyle.IndexOf(":", textAlignIndex);
                    int semiColumnIndex = pStyle.IndexOf(";");
                    if(textAlignIndex >= 0 && columnIndex >= 0)
                    {
                        if (semiColumnIndex < 0)
                            alignValue = pStyle.Substring(columnIndex + 1);
                        else
                            alignValue = pStyle.Substring(columnIndex + 1, semiColumnIndex - columnIndex - 1);    
                    }
                    Align align = Align.Left;
                    if (alignValue.Equals("center"))
                        align = Align.Center;
                    if (alignValue.Equals("right"))
                        align = Align.Right;
                    
                    int endOfOpenTag = text.IndexOf(">", startPWithStyle);
                    string innerText = text.Substring(endOfOpenTag + 1, 
                        endP - endOfOpenTag - 1 );
                    innerText = innerText.Trim();
                    innerText = innerText.Replace("\r\n", String.Empty);
                    bool hasBreaks = innerText.IndexOf("<br>") > 0;
                    bool hasStrongChunks = innerText.IndexOf("<strong>") >= 0;
                    bool hasItalicChunks = innerText.IndexOf("<em>") >= 0;
                    
                    P p = new P
                    {
                        Text = innerText,
                        HasBreaks = hasBreaks,
                        HasStrongChunks = hasStrongChunks,
                        HasItalicChunks = hasItalicChunks,
                        StartIndex = startPWithStyle,
                        Align = align
                    };

                    list.Elements.Add(p);
                                  
                    if (pStyleMatches.Count > psIndex)
                    {
                        startPWithStyle = pStyleMatches[psIndex].Index;
                        psIndex++;
                    }
                    else
                        startPWithStyle = -1;
                }
            }
            #endregion

            #region Parsing <h2> tag
            while (startH2 != -1)
            {
                int endH2 = text.IndexOf("</h2>", startH2);
                if (endH2 == -1)
                {
                    //badly formatted html -> ignore, add as plain text
                    list.JustPlainText = true;
                    return list;
                }
                
                string innerText = text.Substring(startH2 +"<h2>".Length, 
                    endH2 - startH2 - "<h2>".Length );
                H2 h2 = new H2 { Text = innerText, StartIndex = startH2 };

                list.Elements.Add(h2);
                startH2 = text.IndexOf("<h2>", endH2 + "</h2>".Length);
            }
            #endregion

            #region Parsing <img /> tag
            while (startImage != -1)
            {
                int endImage = text.IndexOf("/>", startImage);
                int srcIndex = text.IndexOf("src", startImage);
                int floatIndex = text.IndexOf("float", startImage);
                string quote = @"""";

                int quoteStart = text.IndexOf(quote, srcIndex);
                int quoteEnd = text.IndexOf(quote, quoteStart + 1);

                string source = text.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
                string floatValue = null;
                if (floatIndex >= 0)
                {
                    quoteStart = text.IndexOf(quote, floatIndex);
                    quoteEnd = text.IndexOf(quote, quoteStart + 1);
                    floatValue = text.Substring(quoteStart + 1, quoteEnd - quoteStart - 1);
                }
                
                Image img = new Image
                {
                    StartIndex = startImage,
                    Src = source,
                    Align = Align.Center
                };
                if (floatValue != null && floatValue == "left")
                    img.Align = Align.Left;
                if (floatValue != null && floatValue == "right")
                    img.Align = Align.Right;


                list.Elements.Add(img);

                if (imgMatches.Count > iIndex)
                {
                    startImage = imgMatches[iIndex].Index;
                    iIndex++;
                }
                else
                    startImage = -1;
            }
            #endregion

            list.Elements = list.Elements.OrderBy(e => e.StartIndex).ToList();
            
            return list;
        }
        
        /// <summary>
        /// https://stackoverflow.com/a/785743/10027214
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public string GetContentFromHtml(string html)
        {
            var stepOne = Regex.Replace(html, "<[^>]*(>|$)", string.Empty);
            var stepTwo = WebUtility.HtmlDecode(stepOne);
            var stepThree = Regex.Replace(stepTwo, "<[^>]*(>|$)", string.Empty);
            return stepThree;
        }
    }
}