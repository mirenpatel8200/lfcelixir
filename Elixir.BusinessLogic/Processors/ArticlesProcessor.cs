using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class ArticlesProcessor : IArticlesProcessor
    {
        private readonly IArticlesRepository _articlesRepository;
        private readonly IResourcesRepository _resourcesRepository;
        
        public ArticlesProcessor(IArticlesRepository articlesRepository, IResourcesRepository resourcesRepository)
        {
            _articlesRepository = articlesRepository;
            _resourcesRepository = resourcesRepository;
        }

        public IEnumerable<Article> Get100Articles(ArticlesSortField sortField, SortDirection sortDirection, int TopicFilter)
        {
            var sortFields = new[] {sortField, ArticlesSortField.ArticleTitle};
            var sortDirections = new[] {sortDirection, SortDirection.Ascending};

            var allArticles = _articlesRepository.GetN(100, sortFields, sortDirections, TopicFilter);

            return allArticles;
        }

        public IEnumerable<Article> GetFilteredArticles(string filter, ArticlesSortField sortField, SortDirection sortDirection, bool IncludeAllFields=true, int TopicFilter = 0)
        {
            var sortFields = new[] { sortField, ArticlesSortField.ArticleTitle };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };
            IEnumerable<Article> articles;
            if (string.IsNullOrEmpty(filter))
            {
                articles = Get100Articles(sortField, sortDirection, TopicFilter);
            }
            else
            {
                articles = _articlesRepository.GetFiltered(filter, sortFields, sortDirections, IncludeAllFields, TopicFilter);
            }
            
            return articles;
        }

        private void ValidateUrlName(string urlName, int? excludeArticleId = null)
        {
            var exists = _articlesRepository.IsNonDeletedArticleExists(urlName, excludeArticleId);
            if (exists)
                throw new ModelValidationException("Article with specified UrlName already exists.");
        }

        public int CreateArticle(Article article)
        {
            ValidateUrlName(article.UrlName);
            ValidateResource(article);
            if (!string.IsNullOrEmpty(article.BulletPoints))
            {
                article.BulletPoints = article.BulletPoints.Trim();
                article.BulletPoints = Regex.Replace(
                    article.BulletPoints, @"[\r\n]{2,}", "\r\n");
            }
                
            var now = DateTime.Now;
            article.Created = now;
            article.Updated = now;

            //var tempId = 000;
            //article.Id = tempId;
            //var oldHashCode = article.CalculateIdHashCode();
            //article.IdHashCode = oldHashCode;

            if(!string.IsNullOrEmpty(article.PublisherUrl))
                article.PublisherUrl = article.PublisherUrl.Trim();
            article.PublisherUrl = article.PublisherUrl.StripAnchors();
            article.PublisherUrl = article.PublisherUrl.StripTrackingParameters();

            _articlesRepository.Insert(article);
            
            var addedArticle = _articlesRepository.GetArticleByUrlName(article.UrlName);
            if (addedArticle == null)
                throw new InvalidOperationException("Unable to find article that was added.");

            //addedArticle.IdHashCode = addedArticle.CalculateIdHashCode();
            //_articlesRepository.Update(addedArticle);

            //if (addedArticle.IdHashCode.Equals(oldHashCode, StringComparison.Ordinal))
            //    throw new InvalidOperationException("Unable to construct new Article hash instead of old one.");

            UpdateMentionedResources(addedArticle.Id.Value, article.OrgsMentionedString, ResourceTypes.Organisation.ToDatabaseValues().First());
            UpdateMentionedResources(addedArticle.Id.Value, article.PeopleMentionedString, ResourceTypes.Person.ToDatabaseValues().First());
            UpdateMentionedResources(addedArticle.Id.Value, article.CreationsMentionedString, ResourceTypes.Creation.ToDatabaseValues().First());

            return addedArticle.Id.Value;
        }
        
        public Article GetArticleById(int id)
        {
            return _articlesRepository.GetArticleById(id);
        }

        public Article GetArticleByUrlName(string urlName)
        {
            return _articlesRepository.GetArticleByUrlName(urlName);
        }

        private void ValidateResource(Article article)
        {
            if (article.PublisherResourceId.HasValue)
            {
                var resource = _resourcesRepository.GetResourceById(article.PublisherResourceId.Value);
                if(resource == null)
                    throw new ModelValidationException("Unable to find publisher resource linked to model by its id.");
            }

            if (article.ReporterResourceId.HasValue)
            {
                var resource = _resourcesRepository.GetResourceById(article.ReporterResourceId.Value);
                if(resource == null)
                    throw new ModelValidationException("Unable to find reporter resource linked to model by its id.");
            }
        }

        public void UpdateArticle(Article article)
        {
            if (!string.IsNullOrEmpty(article.BulletPoints))
            {
                article.BulletPoints = article.BulletPoints.Trim();
                article.BulletPoints = Regex.Replace(
                    article.BulletPoints, @"[\r\n]{2,}", "\r\n");
            }
            
            ValidateUrlName(article.UrlName, article.Id);
            ValidateResource(article);

            article.Updated = DateTime.Now;
            if (!string.IsNullOrEmpty(article.PublisherUrl))
                article.PublisherUrl = article.PublisherUrl.Trim();
            article.PublisherUrl = article.PublisherUrl.StripAnchors();
            article.PublisherUrl = article.PublisherUrl.StripTrackingParameters();

            _articlesRepository.Update(article);

            if (article.IsOrgsMentionedChanged)
            {
                UpdateMentionedResources(article.Id.Value, article.OrgsMentionedString, ResourceTypes.Organisation.ToDatabaseValues().First());
            }
            if (article.IsPeopleMentionedChanged)
            {
                UpdateMentionedResources(article.Id.Value, article.PeopleMentionedString, ResourceTypes.Person.ToDatabaseValues().First());
            }
            if (article.IsCreationsMentionedChanged)
            {
                UpdateMentionedResources(article.Id.Value, article.CreationsMentionedString, ResourceTypes.Creation.ToDatabaseValues().First());
            }
        }

        public void DeleteArticle(int articleId)
        {
            _articlesRepository.Delete(articleId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topicIds"></param>
        /// <param name="sortField"></param>
        /// <param name="sortDirection"></param>
        /// <param name="maxNumberOfItems">when 0, retrieve all data found. Otherwise, top <paramref name="maxNumberOfItems"/> items.</param>
        /// <returns></returns>
        //public IEnumerable<Article> GetArticlesByTopics(IEnumerable<int> topicIds, ArticlesSortField sortField = ArticlesSortField.ArticleDate, SortDirection sortDirection = SortDirection.Descending, int maxNumberOfItems = 0)
        //{
        //    var topicIdsList = topicIds.ToList();

        //    bool ArticleSelectionFunc(Article a)
        //    {
        //        if (!a.IsEnabled)
        //            return false;

        //        var condA = a.PrimaryTopic.Id.HasValue && topicIdsList.Contains(a.PrimaryTopic.Id.Value);
        //        var condB = a.SecondaryTopic != null && a.SecondaryTopic.Id.HasValue &&
        //                    topicIdsList.Contains(a.SecondaryTopic.Id.Value);

        //        return condA || condB;
        //    }

        //    var articles = _articlesRepository.GetAll().Where(ArticleSelectionFunc);

        //    switch (sortField)
        //    {
        //        case ArticlesSortField.ArticleDate:
        //            {
        //                articles = sortDirection == SortDirection.Descending
        //                    ? articles.OrderByDescending(a => a.ArticleDate)
        //                    : articles.OrderBy(a => a.ArticleDate);
        //                break;
        //            }
        //        case ArticlesSortField.DnPublisherName:
        //            {
        //                articles = sortDirection == SortDirection.Descending
        //                    ? articles.OrderByDescending(a => a.DnPublisherName)
        //                    : articles.OrderBy(a => a.DnPublisherName);
        //                break;
        //            }
        //        case ArticlesSortField.ArticleTitle:
        //            {
        //                articles = sortDirection == SortDirection.Descending
        //                    ? articles.OrderByDescending(a => a.Title)
        //                    : articles.OrderBy(a => a.Title);
        //                break;
        //            }
        //        default:
        //            break;
        //    }

        //    if (maxNumberOfItems != 0)
        //    {
        //        articles = articles.Take(maxNumberOfItems);
        //    }

        //    return articles;
        //}

        public IEnumerable<Article> GetWebPageRelatedArticles(int webPageId, int? maxCount, DateTime? startingArticleDate)
        {
            return _articlesRepository.GetWebPageRelatedArticles(webPageId, maxCount, startingArticleDate).Select(PreprocessArticle);
        }

        public IEnumerable<Article> GetLatestArticles(int count = 10)
        {
            return _articlesRepository.GetLatestArticles(count).Select(PreprocessArticle);
        }

        public string RecalculateIdHashes()
        {
            var log = "";
            void WriteLog(string text)
            {
                log += $"{DateTime.Now}: {text}<br />";
            }

            WriteLog("Started.");

            var articles = _articlesRepository.GetAll().ToList();

            var failedList = new List<int>();
            var total = articles.Count;
            WriteLog($"Total database articles count is {total}.");

            for (var i = 0; i < articles.Count; i++)
            {
                var article = articles[i];
                try
                {
                    article.IdHashCode = article.CalculateIdHashCode();
                    // WriteLog($"Hash {i}/{total} calculated: {article.IdHashCode}");
                }
                catch (Exception)
                {
                    failedList.Add(article.Id.Value);
                    WriteLog($"Unable to calculated hash for article #{article.Id}.");
                }
            }

            WriteLog("Hashes calculated.");

            for (var i = 0; i < articles.Count; i++)
            {
                var article = articles[i];
                _articlesRepository.Update(article);
                //if(i % 100 == 0)
                //    Debug.WriteLine($"Saved {i} of {total}.");
            }

            WriteLog("Saved to database.");

            WriteLog($"Failed {failedList.Count}.");
            if (failedList.Any())
                WriteLog($"Failed articles: {string.Join(",", failedList)}");

            return log;
        }  

        public string ConvertArticleUrlNames()
        {
            var log = "";
            void WriteLog(string text)
            {
                log += $"{DateTime.Now}: {text}<br />";
            }

            WriteLog("Started.");
            var failedList = new List<int>();

            var articles = _articlesRepository.GetAll().ToList();

            for (int i = 0; i < articles.Count; i++)
            {
                var article = articles[i];
                if (string.IsNullOrWhiteSpace(article.Title))
                {
                    WriteLog($"Article {i} title is empty.");
                    failedList.Add(article.Id.Value);
                }

                var articleUrlName = TextUtils.ConvertToUrlName(article.Title);

                article.UrlName = articleUrlName;
            }

            WriteLog("URL names populated.");

            foreach (var article in articles)
            {
                _articlesRepository.Update(article);
            }

            WriteLog("Saved to database.");

            WriteLog($"Failed {failedList.Count}.");
            if (failedList.Any())
                WriteLog($"Failed articles: {string.Join(",", failedList)}");

            return log;
        }

        public Article GetArticleByHash(string hash)
        {
            var article = _articlesRepository.GetArticleByHash(hash);
            return article;
        }

        private Article PreprocessArticle(Article article)
        {
            article.ProcessSummary();
            return article;
        }

        public IEnumerable<Article> Search(List<string> terms, bool all)
        {
            var articles = _articlesRepository.SearchSQL(terms, all);
           
            articles = articles.OrderByDescending(a => a.ArticleDate);

            return articles; 
        }

        private void UpdateMentionedResources(int articleId, string resourcesMentioned, int resourceType)
        {
            _articlesRepository.DeleteMentionedResources(articleId, resourceType);
            
            int resourceId;
            if (string.IsNullOrEmpty(resourcesMentioned))
                return;

            var resourceTags = resourcesMentioned.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var resourceIds = new List<int>();
            for (int i = 0; i < resourceTags.Length; i++)
            {
                resourceId = ExtractIdFromResourceTag(resourceTags[i]);
                if (resourceId != -1 && resourceIds.Contains(resourceId) == false)
                    resourceIds.Add(resourceId);
            }
            
            _articlesRepository.InsertMentionedResources(articleId, resourceIds, resourceType);
        }
        
        public void PopulateResourcesMentioned(Article article)
        {
            var organisationsMentioned = _resourcesRepository.GetMentionedResources(
                article.Id.Value, ResourceTypes.Organisation.ToDatabaseValues().First()).
                OrderBy(o => o.MentionedOrder);
            article.OrgsMentionedString = organisationsMentioned.ToTagsFormat();

            var peopleMentioned = _resourcesRepository.GetMentionedResources(
                article.Id.Value, ResourceTypes.Person.ToDatabaseValues().First()).
                OrderBy(p => p.MentionedOrder);
            article.PeopleMentionedString = peopleMentioned.ToTagsFormat();

            var creationsMentioned = _resourcesRepository.GetMentionedResources(
                article.Id.Value, ResourceTypes.Creation.ToDatabaseValues().First()).
                OrderBy(c => c.MentionedOrder);
            article.CreationsMentionedString = creationsMentioned.ToTagsFormat();
        }

        public IEnumerable<Article> GetArticlesMentioningResource(int resourceId)
        {
            return _articlesRepository.GetArticlesMentioningResource(resourceId);
        }

        private int ExtractIdFromResourceTag(string tagText)
        {
            int resourceId, beginId, endId;
            beginId = tagText.IndexOf("[");
            endId = tagText.IndexOf("]");
            if (beginId == -1 || endId == -1) return -1;
            string idString = tagText.Substring(beginId + 1,
                endId - beginId - 1);

            if (int.TryParse(idString, out resourceId) == true)
                return resourceId;

            return -1;
        }

        public Article GetArticleByPublisherUrl(string publisherUrl, int? idToExclude = null)
        {
            return _articlesRepository.GetArticleByPublisherUrl(publisherUrl, idToExclude);
        }

        public int CountWebPageRelatedArticles(int webpageId, DateTime? startArticleDate)
        {
            return _articlesRepository.CountWebPageRelatedArticles(webpageId, startArticleDate);
        }
    }
}

