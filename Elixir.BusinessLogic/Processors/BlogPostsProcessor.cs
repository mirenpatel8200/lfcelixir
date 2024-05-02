using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class BlogPostsProcessor : IBlogPostsProcessor
    {
        private readonly IBlogPostsRepository _blogPostsRepository;
        private readonly IWebPagesRepository _webPagesRepository;
        private readonly ITopicsRepository _topicsRepository;
        private readonly IResourcesRepository _resourcesRepository;

        public BlogPostsProcessor(IBlogPostsRepository blogPostsRepository,
            IWebPagesRepository webPagesRepository, ITopicsRepository topicsRepository,
            IResourcesRepository resourcesRepository)
        {
            _blogPostsRepository = blogPostsRepository;
            _webPagesRepository = webPagesRepository;
            _topicsRepository = topicsRepository;
            _resourcesRepository = resourcesRepository;
        }

        public IEnumerable<BlogPost> GetAll()
        {
            return _blogPostsRepository.GetAll();
        }

        public IEnumerable<BlogPost> GetLatest(int count = 5)
        {
            return _blogPostsRepository.GetLatest(count);
        }

        public IEnumerable<BlogPost> GetLatestBlogsInLastXMonths(int count = 3, int months = 3)
        {
            var dateXMonthsAgo = DateTime.Now.AddMonths(-1 * months);
            return _blogPostsRepository.GetLatestBlogsInLastXMonths(count, dateXMonthsAgo);
        }

        public BlogPost GetByUrlName(string urlName)
        {
            return _blogPostsRepository.GetBlogPostByUrlName(urlName);
        }

        public IEnumerable<int> GetPublishingYears()
        {
            return _blogPostsRepository.GetAll().Select(x => x.CreatedDt.Year).Distinct().OrderByDescending(x => x);
        }

        public IEnumerable<BlogPost> GetByYear(int year)
        {
            return _blogPostsRepository.GetEnabledBlogPosts()
                .Where(x => x.PublishedUpdatedDT.Value.Year == year)
                .OrderBy(x => x.PublishedUpdatedDT);
        }

        public IEnumerable<BlogPost> Get100Posts(AdminBlogPostsSortOrder sortField, SortDirection sortDirection)
        {
            var sortFields = new[] { sortField, AdminBlogPostsSortOrder.BlogPostTitle };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };

            var allPosts = _blogPostsRepository.GetN(100, sortFields, sortDirections);

            //if (sortDirection == SortDirection.Ascending)
            //{
            //    switch (sortField)
            //    {
            //        case AdminBlogPostsSortOrder.BlogPostID:
            //            allPosts = allPosts.OrderBy(x => x.BlogPostID).ThenBy(x => x.BlogPostTitle);
            //            break;
            //        case AdminBlogPostsSortOrder.BlogPostTitle:
            //            allPosts = allPosts.OrderBy(x => x.BlogPostTitle);
            //            break;
            //        case AdminBlogPostsSortOrder.UrlName:
            //            allPosts = allPosts.OrderBy(x => x.UrlName);
            //            break;
            //        case AdminBlogPostsSortOrder.IsEnabled:
            //            allPosts = allPosts.OrderBy(x => x.IsEnabled).ThenBy(x => x.BlogPostID);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortField), sortField, null);
            //    }
            //}
            //else if (sortDirection == SortDirection.Descending)
            //{
            //    switch (sortField)
            //    {
            //        case AdminBlogPostsSortOrder.BlogPostID:
            //            allPosts = allPosts.OrderByDescending(x => x.BlogPostID).ThenBy(x => x.BlogPostTitle);
            //            break;
            //        case AdminBlogPostsSortOrder.BlogPostTitle:
            //            allPosts = allPosts.OrderByDescending(x => x.BlogPostTitle);
            //            break;
            //        case AdminBlogPostsSortOrder.UrlName:
            //            allPosts = allPosts.OrderByDescending(x => x.UrlName);
            //            break;
            //        case AdminBlogPostsSortOrder.IsEnabled:
            //            allPosts = allPosts.OrderByDescending(x => x.IsEnabled).ThenBy(x => x.BlogPostID);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortField), sortField, null);
            //    }
            //}

            return allPosts;
        }

        public BlogPost FindRelatedPost(BlogPost postBase, RelatedBlogType type)
        {
            switch (type)
            {
                case RelatedBlogType.Next:
                    return FindNextBlogPost(postBase.CreatedDt, postBase.Id);
                case RelatedBlogType.Prev:
                    return FindPrevBlogPost(postBase.CreatedDt, postBase.Id);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public BlogPost FindNextBlogPost(DateTime moment, int? postBaseId = null)
        {
            return _blogPostsRepository.FindNextBlogPost(moment, postBaseId);
        }

        public BlogPost FindPrevBlogPost(DateTime moment, int? postBaseId = null)
        {
            return _blogPostsRepository.FindPrevBlogPost(moment, postBaseId);
        }

        /// <summary>
        /// Gets a list of blog posts which are newer than specified 'moment'. Last item is the closest to 'moment'.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        //private IEnumerable<BlogPost> FindNextBlogPosts(DateTime moment)
        //{
        //    return GetAll().OrderByDescending(x => x.CreatedDt)
        //        .Where(x => x.CreatedDt > moment && x.IsDeleted == false && x.IsEnabled);
        //}

        /// <summary>
        /// Gets a list of blog posts which are older than specified 'moment'. First item is the closest to 'moment'.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        //private IEnumerable<BlogPost> FindPrevBlogPosts(DateTime moment)
        //{
        //    return GetAll().OrderByDescending(x => x.CreatedDt)
        //        .Where(x => x.CreatedDt <= moment && x.IsDeleted == false && x.IsEnabled);
        //}

        public void CreateBlogPost(BlogPost newPost)
        {
            ValidateUrlName(newPost.UrlName);
            //var postByUrl = GetByUrlName(newPost.UrlName);
            //if (postByUrl != null && postByUrl.IsDeleted == false)
            //    throw new ModelValidationException("Blog post with specified URL already exists.");

            var now = DateTime.Now;

            CheckNextPrevPages(newPost);

            newPost.CreatedDt = now;
            newPost.UpdatedDt = now;
            newPost.PublishedUpdatedDT = null;
            newPost.PublishedOnDT.ToTimeZero();

            //rule 4
            if (newPost.IsEnabled)
            {
                newPost.PublishedUpdatedDT = now;
            }

            _blogPostsRepository.Insert(newPost);

            var inserted = _blogPostsRepository.GetBlogPostByUrlName(newPost.UrlName);

            UpdateMentionedResources(inserted.Id.Value, newPost.OrgsMentionedString, ResourceTypes.Organisation.ToDatabaseValues().First());
            UpdateMentionedResources(inserted.Id.Value, newPost.PeopleMentionedString, ResourceTypes.Person.ToDatabaseValues().First());
            UpdateMentionedResources(inserted.Id.Value, newPost.CreationsMentionedString, ResourceTypes.Creation.ToDatabaseValues().First());
        }

        public BlogPost GetById(int id)
        {
            return _blogPostsRepository.GetBlogPostById(id);
        }

        //public BlogPost GetByIdForDisplay(int id)
        //{
        //    return GetAll().FirstOrDefault(x => x.IsDeleted == false && x.Id == id);
        //}

        public void UpdateBlogPost(BlogPost blogPost, bool isSignificantChange = false, bool isFromAdminScreens = true)
        {
            var prevState = GetById(blogPost.Id.Value);
            if (prevState.IsDeleted)
                throw new InvalidOperationException("Unable to edit deleted blog post.");

            ValidateUrlName(blogPost.UrlName);
            //var postByUrl = GetByUrlName(blogPost.UrlName);
            //if (postByUrl != null && postByUrl.Id != blogPost.Id && postByUrl.IsDeleted == false)
            //    throw new ModelValidationException("Blog post with specified URL already exists.");

            CheckNextPrevPages(blogPost);

            blogPost.CreatedDt = prevState.CreatedDt;
            blogPost.PublishedOnDT.ToTimeZero();
            var now = DateTime.Now;

            blogPost.UpdatedDt = isFromAdminScreens ? now : prevState.UpdatedDt;


            //rule 6
            if (blogPost.IsEnabled && prevState.PublishedUpdatedDT.HasValue == false)
            {
                blogPost.PublishedUpdatedDT = now;
            }

            //preserve existing value in db for PublishedUpdatedDT.
            else
            {
                if (prevState.PublishedUpdatedDT.HasValue)
                {
                    blogPost.PublishedUpdatedDT = prevState.PublishedUpdatedDT;
                }
                else
                {
                    blogPost.PublishedUpdatedDT = null;
                }
            }

            //rule 5
            if (isSignificantChange)
            {
                blogPost.PublishedUpdatedDT = now;
            }

            _blogPostsRepository.Update(blogPost);

            if (blogPost.IsOrgsMentionedChanged)
            {
                UpdateMentionedResources(blogPost.Id.Value, blogPost.OrgsMentionedString, ResourceTypes.Organisation.ToDatabaseValues().First());
            }
            if (blogPost.IsPeopleMentionedChanged)
            {
                UpdateMentionedResources(blogPost.Id.Value, blogPost.PeopleMentionedString, ResourceTypes.Person.ToDatabaseValues().First());
            }
            if (blogPost.IsCreationsMentionedChanged)
            {
                UpdateMentionedResources(blogPost.Id.Value, blogPost.CreationsMentionedString, ResourceTypes.Creation.ToDatabaseValues().First());
            }
        }

        public void DeleteBlogPost(int blogPostId)
        {
            _blogPostsRepository.Delete(blogPostId);
        }

        public int PopulateUrlNamesAndTitles()
        {
            //var blogs = _blogPostsRepository.GetAll()
            //    .Where(x => x.IsDeleted == false && x.IsEnabled)
            //    .OrderBy(x => x.PublishedUpdatedDT ?? x.PublishedOnDT).ToArray();
            var blogs = _blogPostsRepository.GetEnabledBlogPosts().ToArray();
            int postsUpdated = blogs.Length;
            if (blogs.Length == 0)
                return 0;

            blogs[0].PreviousBlogPostUrlName = null;
            blogs[0].NextBlogPostUrlName = blogs[1].UrlName;
            blogs[0].PreviousBlogPostTitle = null;
            blogs[0].NextBlogPostTitle = blogs[1].BlogPostTitle;
            UpdateBlogPost(blogs[0], isFromAdminScreens: false);

            for (int i = 1; i < blogs.Length - 1; i++)
            {
                blogs[i].PreviousBlogPostUrlName = blogs[i - 1].UrlName;
                blogs[i].NextBlogPostUrlName = blogs[i + 1].UrlName;

                blogs[i].PreviousBlogPostTitle = blogs[i - 1].BlogPostTitle;
                blogs[i].NextBlogPostTitle = blogs[i + 1].BlogPostTitle;
                try
                {
                    UpdateBlogPost(blogs[i], isFromAdminScreens: false);
                }
                catch (Exception e)
                {
                    postsUpdated--; continue;
                }
            }

            var last = blogs.Length - 1;

            blogs[last].PreviousBlogPostUrlName = blogs[blogs.Length - 2].UrlName;
            blogs[last].NextBlogPostUrlName = null;
            blogs[last].PreviousBlogPostTitle = blogs[blogs.Length - 2].BlogPostTitle;
            blogs[last].NextBlogPostTitle = null;

            UpdateBlogPost(blogs[last], isFromAdminScreens: false);

            return postsUpdated;
        }

        public IEnumerable<BlogPost> GetRelatedBlogPosts(int webPageId, int? maxCount)
        {
            return _blogPostsRepository.GetTopicRelatedBlogPosts(webPageId, maxCount);
        }

        //public WebPage GetPrimaryWebPage(int blogPostId)
        //{
        //    var blogPost = _blogPostsRepository.GetAll().FirstOrDefault(x => x.Id.Value == blogPostId);
        //    if (blogPost == null)
        //        throw new NullReferenceException("Unable to find blog post with specified Id.");

        //    return GetPrimaryWebPage(blogPost);
        //}

        public WebPage GetPrimaryWebPage(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            if (blogPost.PrimaryTopicId == null)
                return null;

            var primaryTopic = blogPost.PrimaryTopic ?? _topicsRepository.GetById(blogPost.PrimaryTopicId.Value);
            if (primaryTopic == null)
                return null;

            if (primaryTopic.PrimaryWebPageId == null)
                return null;

            var webPage = _webPagesRepository.GetWebPageById(primaryTopic.PrimaryWebPageId.Value);
            if (webPage == null)
                throw new NullReferenceException("Topic's primary web page id points to non-existing web page.");

            return webPage;
        }

        public WebPage GetSecondaryWebPage(BlogPost blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            if (blogPost.SecondaryTopicId == null)
                return null;

            var secondaryTopic = blogPost.SecondaryTopic ?? _topicsRepository.GetById(blogPost.SecondaryTopicId.Value);
            if (secondaryTopic == null)
                return null;

            if (secondaryTopic.PrimaryWebPageId == null)
                return null;

            var webPage = _webPagesRepository.GetWebPageById(secondaryTopic.PrimaryWebPageId.Value);
            if (webPage == null)
                throw new NullReferenceException("Topic's secondary web page id points to non-existing web page.");

            return webPage;
        }

        private void CheckNextPrevPages(BlogPost post)
        {
            if (!string.IsNullOrWhiteSpace(post.NextBlogPostUrlName) && GetByUrlName(post.NextBlogPostUrlName) == null)
                throw new ModelValidationException("Next blog post does not exist.");

            if (!string.IsNullOrWhiteSpace(post.PreviousBlogPostUrlName) && GetByUrlName(post.PreviousBlogPostUrlName) == null)
                throw new ModelValidationException("Prev blog post does not exist.");
        }

        public IEnumerable<BlogPost> Search(List<string> termsOrPhrases, bool all)
        {
            return _blogPostsRepository.SearchSQL(termsOrPhrases, all);
        }

        private void UpdateMentionedResources(int blogPostId, string resourcesMentioned, int resourceType)
        {
            _blogPostsRepository.DeleteMentionedResources(blogPostId, resourceType);

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

            _blogPostsRepository.InsertMentionedResources(blogPostId, resourceIds, resourceType);
        }

        public void PopulateResourcesMentioned(BlogPost blogPost)
        {
            var organisationsMentioned = _resourcesRepository.GetMentionedResources(
                blogPost.Id.Value, ResourceTypes.Organisation.ToDatabaseValues().First(),
                EntityType.BlogPost).OrderBy(o => o.MentionedOrder);
            var peopleMentioned = _resourcesRepository.GetMentionedResources(
                blogPost.Id.Value, ResourceTypes.Person.ToDatabaseValues().First(),
                EntityType.BlogPost).OrderBy(o => o.MentionedOrder);
            var creationsMentioned = _resourcesRepository.GetMentionedResources(
                blogPost.Id.Value, ResourceTypes.Creation.ToDatabaseValues().First(),
                EntityType.BlogPost).OrderBy(o => o.MentionedOrder);

            blogPost.OrgsMentionedString = organisationsMentioned.ToTagsFormat();
            blogPost.PeopleMentionedString = peopleMentioned.ToTagsFormat();
            blogPost.CreationsMentionedString = creationsMentioned.ToTagsFormat();
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

        public IEnumerable<BlogPost> GetBlogPostsMentioningResource(int resourceId)
        {
            return _blogPostsRepository.GetBlogPostsMentioningResource(resourceId);
        }

        public IEnumerable<BlogPost> GetBlogPostsRelatedByTopic(int blogPostId)
        {
            return _blogPostsRepository.GetBlogPostsRelatedByTopic(blogPostId, 5);
        }

        private void ValidateUrlName(string urlName, int? excludeBlogPostId = null)
        {
            var exists = _blogPostsRepository.IsNonDeletedBlogPostExists(urlName, excludeBlogPostId);
            if (exists)
                throw new ModelValidationException("Blog post with specified UrlName already exists.");
        }
    }
}
