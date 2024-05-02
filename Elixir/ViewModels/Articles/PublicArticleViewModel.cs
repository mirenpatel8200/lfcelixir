using Elixir.Areas.Admin.Models;
using Elixir.Models;
using System.Collections.Generic;

namespace Elixir.ViewModels.Articles
{
    public class PublicArticleViewModel
    {
        public ArticleModel Article { get; set; }
        public string PrimaryWebPageTitle { get; set; }
        public string PrimaryWebPageUrlName { get; set; }

        public string SecondaryWebPageTitle { get; set; }
        public string SecondaryWebPageUrlName { get; set; }

        public bool HasPrimaryPage => !string.IsNullOrWhiteSpace(PrimaryWebPageUrlName) && !string.IsNullOrWhiteSpace(PrimaryWebPageTitle);
        public bool HasSecondaryPage => !string.IsNullOrWhiteSpace(SecondaryWebPageUrlName) && !string.IsNullOrWhiteSpace(SecondaryWebPageTitle);

        public bool TopicsHaveSamePrimaryWebPage { get; set; }
        
        public List<Resource> ResourcesMentioned { get; set; }
    }
}