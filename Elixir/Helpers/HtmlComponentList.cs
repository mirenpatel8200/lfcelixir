using System.Collections.Generic;

namespace Elixir.Helpers
{
    public class HtmlComponentList
    {
        public HtmlComponentList()
        {
            Elements = new List<HtmlTag>();
        }
        
        public bool JustPlainText { get; set; }

        public List<HtmlTag> Elements { get; set; }
    }

    public class Image : HtmlTag
    {
        public string Src { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }
        
        public Align Align { get; set; }
    }

    public class P : HtmlTag
    {
        public bool HasBreaks { get; set; }

        public bool HasStrongChunks { get; set; }

        public Align Align { get; set; }

        public bool HasItalicChunks { get; set; }
    }

    public class H2 : HtmlTag { }

    public class H3 : HtmlTag { }

    public class H4 : HtmlTag { }

    public class Strong : HtmlTag { }

    public class HtmlTag
    {
        public string Text { get; set; }

        public int StartIndex { get; set; }

        public bool IsAdded { get; set; }
    }

    public enum Align
    {
        Left,
        Center,
        Right
    }
}