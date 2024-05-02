using System.Collections.Generic;
using Elixir.Areas.Admin.Models;

namespace Elixir.ViewModels.SubjectNews
{
    public class SubjectNewsViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public IEnumerable<ArticleModel> RecentRelatedArticles { get; set; }
        public string LinkToPageUrl { get; set; }
        public string LinkToPageText { get; set; }
        public int CountAllRelatedArticles { get; set; }
    }
}