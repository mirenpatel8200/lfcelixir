namespace Elixir.Models.Utils
{
    public enum PdfPageType
    {
        FrontChapter,
        TableOfContents,
        BodyChapter,
        Section,
        Tip,
        BackChapter,
        None
    }

    public class TableOfContentItem
    {
        public int PageNumber { get; set; }

        public PdfPageType Type { get; set; }

        public string DisplayName { get; set; }

        public int? DatabaseId { get; set; }

        public TableOfContentItem(int pageNumber, PdfPageType type, string displayName, int? databaseId)
        {
            PageNumber = pageNumber;
            Type = type;
            DisplayName = displayName;
            DatabaseId = databaseId;
        }
    }
}