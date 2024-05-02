using System.Collections.Generic;
namespace Elixir.Models
{
    public class SearchResult
    {
        public string TermsSearched { get; set; }

        public List<Article> ArticlesFound { get; set; }

        public List<WebPage> WebPagesFound { get; set; }

        public List<BlogPost> BlogPostsFound { get; set; }

        public List<Resource> ResourcesFound { get; set; }

        public bool DisplayAll { get; set; }

        public bool ErrorOnSearch { get; set; }

        public string ErrorMessage { get; set; }

        public SearchResult()
        {
            TermsSearched = "";
            ArticlesFound = new List<Article>();
            WebPagesFound = new List<WebPage>();
            BlogPostsFound = new List<BlogPost>();
            ResourcesFound = new List<Resource>();
        }
    }
}