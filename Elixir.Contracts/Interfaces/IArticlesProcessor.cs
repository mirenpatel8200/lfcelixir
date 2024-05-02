using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IArticlesProcessor
    {
        [Obsolete("Too many args - consider refactoring.")]
        IEnumerable<Article> Get100Articles(ArticlesSortField sortField = ArticlesSortField.ArticleID, SortDirection sortDirection = SortDirection.Ascending, int TopicFilter = 0);
        IEnumerable<Article> GetFilteredArticles(string filter, ArticlesSortField sortBy = ArticlesSortField.ArticleID, SortDirection direction = SortDirection.Ascending, bool IncludeAllFields = false, int TopicFilter = 0);
        int CreateArticle(Article article);
        Article GetArticleById(int id);
        Article GetArticleByUrlName(string urlName);
        void UpdateArticle(Article article);
        void DeleteArticle(int articleId);

        //[Obsolete("Too many args - refactor!")]
        //IEnumerable<Article> GetArticlesByTopics(IEnumerable<int> topicIds, ArticlesSortField sortField = ArticlesSortField.ArticleDate, SortDirection sortDirection = SortDirection.Descending, int maxNumberOfItems = 0);

        IEnumerable<Article> GetWebPageRelatedArticles(int webPageId, int? maxCount = 20, DateTime? startingArticleDate = null);

        //[Obsolete("Re-implement to SQL.")]
        IEnumerable<Article> GetLatestArticles(int count = 10);

        string RecalculateIdHashes();
        string ConvertArticleUrlNames();

        Article GetArticleByHash(string hash);

        IEnumerable<Article> Search(List<string> terms, bool all);

        void PopulateResourcesMentioned(Article article);

        IEnumerable<Article> GetArticlesMentioningResource(int resourceId);

        Article GetArticleByPublisherUrl(string publisherUrl, int? idToExclude = null);

        int CountWebPageRelatedArticles(int webpageId, DateTime? startArticleDate);
    }
}
