using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IArticlesRepository : IRepository<Article>
    {
        /// <summary>
        /// Gets all articles without any filtering.
        /// </summary>
        /// <returns></returns>
        Article GetArticleById(int id);

        /// <summary>
        /// Checks if article with specified UrlName exists.
        /// </summary>
        /// <param name="urlName">The name of article to be checked.</param>
        /// <returns></returns>
        //bool IsArticleExists(string urlName);

        /// <summary>
        /// Checks if non-deleted article with specified UrlName exists given an article id which should not be checked.
        /// </summary>
        /// <param name="urlName">The name of article to be checked.</param>
        /// <param name="excludeArticleId">Id of article that should not be checked.</param>
        /// <returns></returns>
        bool IsNonDeletedArticleExists(string urlName, int? excludeArticleId = null);

        /// <summary>
        /// Gets an article with specified urlName.
        /// </summary>
        /// <param name="urlName"></param>
        /// <returns></returns>
        Article GetArticleByUrlName(string urlName);

        /// <summary>
        /// Returns list of related articles sorted by date (most recent first).
        /// Disabled and Deleted articles are excluded.
        /// </summary>
        /// <param name="webPageId"></param>
        /// <param name="maxCount">Maximum number of records, null if shouldn't be limited.</param>
        /// <param name="startingArticleDate">Starting article date, null if shouldn't be limited.</param>
        /// <returns></returns>
        IEnumerable<Article> GetWebPageRelatedArticles(int webPageId, int? maxCount, DateTime? startingArticleDate);

        /// <summary>
        /// Returns article by specified hash.
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Article GetArticleByHash(string hash);

        /// <summary>
        /// Returns N Article records.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="sortFields"></param>
        /// <param name="sortDirections"></param>
        /// <returns></returns>
        IEnumerable<Article> GetN(int count, ArticlesSortField[] sortFields, SortDirection[] sortDirections,int TopicFilter);

        IEnumerable<Article> GetFiltered(string filter, ArticlesSortField[] sortFields, SortDirection[] sortDirections, bool IncludeAllFields,int TopicFilter);
        
        IEnumerable<Article> SearchSQL(List<string> terms, bool all);

        /// <summary>
        /// Gets mentioned resources in article and inserts them in ArticleResource table
        /// </summary>
        /// <param name="articleId">The main article ID</param>
        /// <param name="resourceIds">The list of the mentioned resources' IDs</param>
        /// <returns></returns>
        void InsertMentionedResources(int articleId, List<int> resourceIds, int resourceType);

        void DeleteMentionedResources(int articleId, int resourceType);

        IEnumerable<Article> GetArticlesMentioningResource(int resourceId);

        Article GetArticleByPublisherUrl(string publisherUrl, int? idToExclude = null);

        int CountWebPageRelatedArticles(int webpageId, DateTime? startArticleDate);
        IEnumerable<Article> GetLatestArticles(int limit);
    }
}
