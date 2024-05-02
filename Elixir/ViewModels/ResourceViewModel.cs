using Elixir.Models;
using Elixir.ViewModels.Enums;
using System.Collections.Generic;

namespace Elixir.ViewModels
{
    public class ResourceViewModel
    {
        public Resource Resource { get; set; }

        public Resource Parent { get; set; }

        public List<Article> ArticlesMentioningResource { get; set; }

        public List<Resource> ChildResourcesOfTypePerson { get; set; }

        public List<Resource>  ChildResourcesOfTypeCreation { get; set; }

        public List<BlogPost> RelatedBlogPosts { get; set; }

        public List<Resource> MentionedResources { get; set; }

        public List<Resource> ResourcesMentioningThis { get; set; }

        public ArticlesDisplayFormat ArticlesDisplayFormat { get; set; }
    }
}