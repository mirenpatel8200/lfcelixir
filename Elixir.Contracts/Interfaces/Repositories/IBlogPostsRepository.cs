using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IBlogPostsRepository : IRepository<BlogPost>
    {
        IEnumerable<BlogPost> GetN(int count, AdminBlogPostsSortOrder[] sortFields, SortDirection[] sortDirections);

        //void UpdatePublishedUpdatedDate(int id, DateTime date);

        IEnumerable<BlogPost> GetTopicRelatedBlogPosts(int webPageId, int? maxCount);

        IEnumerable<BlogPost> SearchSQL(List<string> terms, bool all);

        void DeleteMentionedResources(int blogPostId, int resourceType);

        void InsertMentionedResources(int blogPostId, List<int> resourceIds, int resourceType);

        BlogPost GetBlogPostByUrlName(string urlName);
        BlogPost GetBlogPostById(int id);

        IEnumerable<BlogPost> GetBlogPostsMentioningResource(int resourceId);

        IEnumerable<BlogPost> GetBlogPostsRelatedByTopic(int blogPostId, int limit);

        IEnumerable<BlogPost> GetLatest(int limit);
        IEnumerable<BlogPost> GetLatestBlogsInLastXMonths(int limit, DateTime dateXMonthsAgo);
        IEnumerable<BlogPost> GetEnabledBlogPosts();
        BlogPost FindNextBlogPost(DateTime moment, int? postBaseId);
        BlogPost FindPrevBlogPost(DateTime moment, int? postBaseId);
        bool IsNonDeletedBlogPostExists(string urlName, int? excludeBlogPostId);
    }
}
