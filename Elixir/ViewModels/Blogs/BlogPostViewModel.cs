using Elixir.Helpers;
using Elixir.Models;
using System;
using System.Collections.Generic;

namespace Elixir.ViewModels.Blogs
{
    public class BlogPostViewModel
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }

        public string PreviousUrlName { get; set; }
        public string NextUrlName { get; set; }

        public bool HasPreviousPage => !string.IsNullOrWhiteSpace(PreviousUrlName);
        public bool HasNextPage => !string.IsNullOrWhiteSpace(NextUrlName);

        public DateTime CreatedDt { get; set; }
        public DateTime? UpdatedDt { get; set; }

        public string PrimaryWebPageName { get; set; }
        public string PrimaryWebPageUrlName { get; set; }
        public bool PrimaryWebPageExists { get; set; }

        public string SecondaryWebPageName { get; set; }
        public string SecondaryWebPageUrlName { get; set; }
        public bool SecondaryWebPageExists { get; set; }

        public bool TopicsHaveSamePrimaryWebPage { get; set; }

        public DateTime PublicCreatedOn { get; set; }
        public DateTime? PublicUpdatedOn { get; set; }

        public string PreviousBlogPostTitle { get; set; }
        public string NextBlogPostTitle { get; set; }

        public List<Resource> MentionedResources { get; set; }

        public List<BlogPost> RelatedBlogPosts { get; set; }

        public string PublishedUpdatedInfo
        {
            get
            {
                string pu = "Published " + this.PublicCreatedOn.FormatDdMmmYyyy();
                if (this.PublicUpdatedOn.HasValue) // originally was only populated when blog post updated - Apr20: now always populated
                {
                    if (this.PublicCreatedOn.Date != this.PublicUpdatedOn.Value.Date)
                        pu += $" (last updated {this.PublicUpdatedOn.Value.FormatDdMmmYyyy()})";
                }
                return pu;
            }
        }   
    }
}