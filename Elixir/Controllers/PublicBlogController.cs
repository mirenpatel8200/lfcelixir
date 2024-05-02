using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.Contracts.Interfaces;
using Elixir.Helpers;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Blogs;
using Elixir.ViewModels.Enums;
using Elixir.Views;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class PublicBlogController : Controller
    {
        private readonly IBlogPostsProcessor _blogPostsProcessor;
        private readonly IResourcesProcessor _resourcesProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public PublicBlogController(
            IBlogPostsProcessor blogPostsProcessor,
            IResourcesProcessor resourcesProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _blogPostsProcessor = blogPostsProcessor;
            _resourcesProcessor = resourcesProcessor;
            _settingsProcessor = settingsProcessor;
        }

        //Browser URL: "/blog/{name}"
        public ActionResult ViewBlog(string name)
        {
            var filteredName = name.GetAlphanumericValue();

            //Test commit new Repo
            // If the filteredName is empty then it was filtered by the FilterQueryParameter attribute.
            // This means that it contained only a query string for tracking purposes, i.e. "&abcdf".
            if (filteredName == "")
                return ReturnAllBlogs();

            var vm = new BlogPostViewModel();

            var blogPost = _blogPostsProcessor.GetByUrlName(filteredName);
            if (blogPost == null)
                throw new ContentNotFoundException("Blog with specified name is not found.");

            var primaryWebPage = _blogPostsProcessor.GetPrimaryWebPage(blogPost);
            if (primaryWebPage != null)
            {
                vm.PrimaryWebPageExists = true;
                vm.PrimaryWebPageName = primaryWebPage.WebPageName;
                vm.PrimaryWebPageUrlName = primaryWebPage.UrlName;
            }
            var secondaryWebPage = _blogPostsProcessor.GetSecondaryWebPage(blogPost);
            if (secondaryWebPage != null)
            {
                vm.SecondaryWebPageExists = true;
                vm.SecondaryWebPageName = secondaryWebPage.WebPageName;
                vm.SecondaryWebPageUrlName = secondaryWebPage.UrlName;
            }

            if (blogPost.PrimaryTopic.PrimaryWebPageId.HasValue &&
               blogPost.SecondaryTopic != null &&
               blogPost.SecondaryTopic.PrimaryWebPageId.HasValue &&
               blogPost.SecondaryTopic.PrimaryWebPageId != 0 &&
               blogPost.PrimaryTopic.PrimaryWebPageId == blogPost.SecondaryTopic.PrimaryWebPageId)
            {
                vm.TopicsHaveSamePrimaryWebPage = true;
            }


            vm.Title = blogPost.BlogPostTitle;
            vm.HtmlContent = blogPost.ContentMain;
            vm.NextUrlName = blogPost.NextBlogPostUrlName;
            vm.PreviousUrlName = blogPost.PreviousBlogPostUrlName;
            vm.PublicCreatedOn = blogPost.PublishedOnDT;
            vm.PublicUpdatedOn = blogPost.PublishedUpdatedDT;
            vm.PreviousBlogPostTitle = blogPost.PreviousBlogPostTitle;
            vm.NextBlogPostTitle = blogPost.NextBlogPostTitle;

            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            var ogImageName = "life-extension-blog.jpg";
            if (!string.IsNullOrWhiteSpace(blogPost.SocialImageFilename))
                ogImageName = blogPost.SocialImageFilename;
            else if (blogPost.PrimaryTopic != null && !string.IsNullOrWhiteSpace(blogPost.PrimaryTopic.SocialImageFilename))
                ogImageName = blogPost.PrimaryTopic.SocialImageFilename;

            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImageName);
            // ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, ogImageName); CHANGED BY AC 11/AUG/19

            var bannerName = AppConstants.DefaultBannerNameBlog;
            if (blogPost.PrimaryTopic != null && !string.IsNullOrWhiteSpace(blogPost.PrimaryTopic.BannerImageFileName))
            {
                bannerName = blogPost.PrimaryTopic.BannerImageFileName;
            }

            string metaDescription = "HELLO";
            if (string.IsNullOrEmpty(blogPost.BlogPostDescriptionPublic))
                metaDescription = $"{ blogPost.BlogPostTitle }. ";
            else
                metaDescription = $"{blogPost.BlogPostDescriptionPublic}. "; // don't repeat title if have description
            metaDescription += "Blog post from the Live Forever Club.";

            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerName);
            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, metaDescription);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
            {
                AltText = (blogPost != null && blogPost.PrimaryTopic != null && !string.IsNullOrEmpty(blogPost.PrimaryTopic.TopicName) ? blogPost.PrimaryTopic.TopicName : "") + " blog post page banner",
                ImgTitle = (blogPost != null && !string.IsNullOrEmpty(blogPost.BlogPostTitle) ? blogPost.BlogPostTitle : "")
            });

            var currentHost = Request.Url.GetLeftPart(UriPartial.Authority);

            string blogStructuredDataScript = ScriptUtils.FillFormatForBlog(ScriptUtils.BlogpostScriptFormat,
                blogPost.PublishedOnDT, blogPost.PublishedUpdatedDT, blogPost.BlogPostTitle,
                blogPost.BlogPostDescriptionPublic, blogPost.SocialImageFilename, currentHost);

            blogStructuredDataScript = HttpUtility.HtmlEncode(blogStructuredDataScript);

            ViewData.AddOrUpdateValue(ViewDataKeys.BlogPostScript, blogStructuredDataScript);

            var isVisibleNewFlashText = _settingsProcessor.GetSettingsByName("IsVisibleNewFlashText");
            if (isVisibleNewFlashText != null)
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.IsVisibleNewFlashText, isVisibleNewFlashText);
                if (!string.IsNullOrEmpty(isVisibleNewFlashText.PairValue) && Convert.ToBoolean(isVisibleNewFlashText.PairValue))
                {
                    var newFlashText = _settingsProcessor.GetSettingsByName("NewFlashText");
                    if (newFlashText != null)
                        ViewData.AddOrUpdateValue(ViewDataKeys.NewFlashText, newFlashText);
                }
            }

            vm.MentionedResources = _resourcesProcessor
                .GetResourcesMentionedInBlogpost(blogPost.Id.Value)
                .Where(mr => mr.IsHiddenPublic == false)
                .OrderBy(mr => mr.ResourceName).ToList();

            vm.RelatedBlogPosts = _blogPostsProcessor
                .GetBlogPostsRelatedByTopic(blogPost.Id.Value)
                .ToList();

            return View(vm);
        }

        private ActionResult ReturnAllBlogs()
        {
            var vm = new BlogsViewModel
            {
                RecentBlogs = _blogPostsProcessor.GetLatest(),
                ListOfYears = _blogPostsProcessor.GetPublishingYears()
            };

            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + AppConstants.DefaultBannerName);

            var isVisibleNewFlashText = _settingsProcessor.GetSettingsByName("IsVisibleNewFlashText");
            if (isVisibleNewFlashText != null)
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.IsVisibleNewFlashText, isVisibleNewFlashText);
                if (!string.IsNullOrEmpty(isVisibleNewFlashText.PairValue) && Convert.ToBoolean(isVisibleNewFlashText.PairValue))
                {
                    var newFlashText = _settingsProcessor.GetSettingsByName("NewFlashText");
                    if (newFlashText != null)
                        ViewData.AddOrUpdateValue(ViewDataKeys.NewFlashText, newFlashText);
                }
            }
            return View("Index", vm);
        }

        public ActionResult Index()
        {
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            return ReturnAllBlogs();
        }

        //Browser URL: /blog/year/{year}
        public ActionResult ViewByYear(int year)
        {
            var BlogPostsByYears = _blogPostsProcessor.GetByYear(year);
            var vm = new BlogsByYearViewModel
            {
                Year = year,
                BlogPosts = BlogPostsByYears
            };
            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + AppConstants.DefaultBannerName);
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            var isVisibleNewFlashText = _settingsProcessor.GetSettingsByName("IsVisibleNewFlashText");
            if (isVisibleNewFlashText != null)
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.IsVisibleNewFlashText, isVisibleNewFlashText);
                if (!string.IsNullOrEmpty(isVisibleNewFlashText.PairValue) && Convert.ToBoolean(isVisibleNewFlashText.PairValue))
                {
                    var newFlashText = _settingsProcessor.GetSettingsByName("NewFlashText");
                    if (newFlashText != null)
                        ViewData.AddOrUpdateValue(ViewDataKeys.NewFlashText, newFlashText);
                }
            }
            return View(vm);
        }
    }
}