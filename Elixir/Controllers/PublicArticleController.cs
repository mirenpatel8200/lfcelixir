using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Contracts.Interfaces;
using Elixir.Helpers;
using Elixir.Models.Exceptions;
using Elixir.Models.Identity;
using Elixir.Models.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Articles;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class PublicArticleController : Controller
    {
        private readonly IArticlesProcessor _articlesProcessor;
        private readonly IWebPagesProcessor _webPagesProcessor;
        private readonly IResourcesProcessor _resourcesProcessor;
        private readonly IAuditLogsProcessor _auditLogsProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public PublicArticleController(
            IArticlesProcessor articlesProcessor,
            IWebPagesProcessor webPagesProcessor,
            IResourcesProcessor resourcesProcessor,
            IAuditLogsProcessor auditLogsProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _articlesProcessor = articlesProcessor;
            _webPagesProcessor = webPagesProcessor;
            _resourcesProcessor = resourcesProcessor;
            _auditLogsProcessor = auditLogsProcessor;
            _settingsProcessor = settingsProcessor;
        }

        //public ActionResult UpdateHashes()
        //{
        //    _articlesProcessor.RecalculateIdHashes();
        //    return Content("Done");
        //}

        [Route("article/{name}")]
        public ActionResult Index(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ContentNotFoundException("Article's name has to be specified.");

            // Track old links
            // If hash is NULL then link is new
            var article = _articlesProcessor.GetArticleByUrlName(name);

            if (article != null)
            {
                if (article.IsDeleted)
                    throw new ContentNotFoundException("Article with requested name is deleted.");
                if (!article.IsEnabled)
                    throw new ContentNotFoundException("Article with requested name is disabled.");

                var articleM = new ArticleModel(article);

                var vm = new PublicArticleViewModel()
                {
                    Article = articleM,
                    ResourcesMentioned = _resourcesProcessor
                        .GetResourcesMentionedInArticle(article.Id.Value)
                        .Where(rm => rm.IsHiddenPublic == false)
                        .OrderBy(rm => rm.ResourceName)
                        .ToList()
                };

                if (article.PrimaryTopic.PrimaryWebPageId.HasValue)
                {
                    var primaryWebPage = _webPagesProcessor.GetWebPageById(article.PrimaryTopic.PrimaryWebPageId.Value);
                    if (primaryWebPage == null)
                        throw new ContentNotFoundException("Unable to find primary web page by PrimaryTopic.PrimaryWebPageId.");

                    vm.PrimaryWebPageTitle = primaryWebPage.WebPageTitle;
                    vm.PrimaryWebPageUrlName = primaryWebPage.UrlName;
                }

                if (article.SecondaryTopic != null &&
                    article.SecondaryTopic.PrimaryWebPageId.HasValue &&
                    article.SecondaryTopic.PrimaryWebPageId != 0)
                {
                    var secondaryWebPage = _webPagesProcessor.GetWebPageById(article.SecondaryTopic.PrimaryWebPageId.Value);
                    if (secondaryWebPage == null)
                        throw new ContentNotFoundException("Unable to find secondary web page by SecondaryTopic.PrimaryWebPageId.");

                    vm.SecondaryWebPageTitle = secondaryWebPage.WebPageTitle;
                    vm.SecondaryWebPageUrlName = secondaryWebPage.UrlName;
                }

                if (article.PrimaryTopic.PrimaryWebPageId.HasValue &&
                    article.SecondaryTopic != null &&
                    article.SecondaryTopic.PrimaryWebPageId.HasValue &&
                    article.SecondaryTopic.PrimaryWebPageId != 0 &&
                    article.PrimaryTopic.PrimaryWebPageId == article.SecondaryTopic.PrimaryWebPageId)
                {
                    vm.TopicsHaveSamePrimaryWebPage = true;
                }

                // Always show Member navbar when looking at public WebPages.
                ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

                ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

                var ogImageName = articleM.GetOgImageName();
                ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImageName);
                // ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, ogImageName); CHANGED BY AC 10/Aug/19

                var bannerName = AppConstants.DefaultBannerNameNewsNew;
                if (article.PrimaryTopic != null && !string.IsNullOrWhiteSpace(article.PrimaryTopic.BannerImageFileName))
                {
                    bannerName = article.PrimaryTopic.BannerImageFileName;
                }

                ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerName);
                ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, $"Key points summary of {vm.Article.DnPublisherName} article. {HttpUtility.UrlDecode(vm.Article.Summary)}");
                ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
                {
                    AltText = (article != null && article.PrimaryTopic != null && !string.IsNullOrEmpty(article.PrimaryTopic.TopicName) ? article.PrimaryTopic.TopicName : "") + " Article page banner",
                    ImgTitle = (article != null && !string.IsNullOrEmpty(article.Title) ? article.Title : "")
                });

                var currentHost = Request.Url.GetLeftPart(UriPartial.Authority);

                string articleStructuredDataScript = ScriptUtils.FillFormatForArticle(ScriptUtils.ArticleScriptFormat,
                    article.ArticleDate, article.Title, article.DnPublisherName, article.Summary,
                    article.SocialImageFilename, article.DnReporterName, article.DnPublisherName, currentHost);

                articleStructuredDataScript = HttpUtility.HtmlEncode(articleStructuredDataScript);

                ViewData.AddOrUpdateValue(ViewDataKeys.ArticleScript, articleStructuredDataScript);

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

            //If hash is NOT NULL then link is old
            var requestedArticle = _articlesProcessor.GetArticleByHash(name);

            if (requestedArticle == null)
                throw new ContentNotFoundException("Article with requested name does not exist.");
            if (requestedArticle.IsDeleted)
                throw new ContentNotFoundException("Article with requested hash is deleted.");
            if (!requestedArticle.IsEnabled)
                throw new ContentNotFoundException("Article with requested hash is disabled.");

            // Audit log to track the article links
            var userId = 0;
            var ipAddress = Request.UserHostAddress;
            if (Request.IsAuthenticated)
            {
                var userManager = Request.GetOwinContext().Get<AccessUserManager>();
                var authedUser = userManager.FindById(int.Parse(User.Identity.GetUserId()));
                userId = authedUser.Id;
            }
            var notesLog = $"Hashcode={requestedArticle.IdHashCode};Referrer={(Request.UrlReferrer != null ? Request.UrlReferrer.OriginalString.ToLower() : "")}";
            _auditLogsProcessor.ArticleLinkTrackLog(userId, ipAddress, (int)requestedArticle.Id, notesLog);
            //Need 301 redirect
            return RedirectToActionPermanent("Index", new { name = requestedArticle.UrlName });
        }
    }
}