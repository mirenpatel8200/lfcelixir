namespace Elixir.Models
{
    public class UrlLink
    {
        public UrlLink()
        {
            
        }

        public UrlLink(string text, string url)
        {
            Url = url;
            Text = text;
        }

        public string Text { get; set; }
        public string Url { get; set; }
    }
}