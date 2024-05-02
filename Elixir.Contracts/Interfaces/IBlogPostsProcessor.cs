using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IBlogPostsProcessor
    {
        IEnumerable<BlogPost> GetAll();

        IEnumerable<BlogPost> GetLatest(int count = 5);

        BlogPost GetByUrlName(string urlName);

        IEnumerable<int> GetPublishingYears();

        IEnumerable<BlogPost> GetByYear(int year);

        IEnumerable<BlogPost> Get100Posts(AdminBlogPostsSortOrder sortField, SortDirection sortDirection);

        BlogPost FindNextBlogPost(DateTime moment, int? postBaseId = null);

        BlogPost FindPrevBlogPost(DateTime moment, int? postBaseId = null);

        BlogPost FindRelatedPost(BlogPost postBase, RelatedBlogType type);

        void CreateBlogPost(BlogPost newPost);

        BlogPost GetById(int id);

        //BlogPost GetByIdForDisplay(int id);

        void UpdateBlogPost(BlogPost blogPost, bool isSignificantChange = false, bool isFromAdminScreens = true);

        void DeleteBlogPost(int blogPostId);

        int PopulateUrlNamesAndTitles();

        IEnumerable<BlogPost> GetRelatedBlogPosts(int webPageId, int? maxCount);

        //WebPage GetPrimaryWebPage(int blogPostId);

        WebPage GetPrimaryWebPage(BlogPost blogPost);
        WebPage GetSecondaryWebPage(BlogPost blogPost);

        IEnumerable<BlogPost> Search(List<string> termsOrPhrases, bool all);

        void PopulateResourcesMentioned(BlogPost blogPost);

        IEnumerable<BlogPost> GetBlogPostsMentioningResource(int resourceId);

        IEnumerable<BlogPost> GetBlogPostsRelatedByTopic(int blogPostId);

         IEnumerable<BlogPost> GetLatestBlogsInLastXMonths(int count = 3, int months = 3);
    }
}
