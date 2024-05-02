namespace Elixir.Models.Utils
{
    public class UpdateQueueFirstLastPage
    {
        public int EntityId { get; set; }

        public PdfPageType Type { get; set; }

        public int PageFirst { get; set; }

        public int PageLast { get; set; }
        
    }
}