using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Contracts.Interfaces;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.ViewModels.SubjectNews;
using Elixir.Views;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class SubjectNewsController : Controller
    {
        private readonly IArticlesProcessor _articlesProcessor;
        private readonly IWebPagesProcessor _webPagesProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public SubjectNewsController(
            IArticlesProcessor articlesProcessor,
            IWebPagesProcessor webPagesProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _articlesProcessor = articlesProcessor;
            _webPagesProcessor = webPagesProcessor;
            _settingsProcessor = settingsProcessor;
        }

        [Route("page/{webPageUrlName}/news")]
        public ActionResult Index(string webPageUrlName, string display = "")
        {
            bool all = (display == "all");
            if (string.IsNullOrWhiteSpace(webPageUrlName) || webPageUrlName.Equals("home", StringComparison.OrdinalIgnoreCase))
                throw new ContentNotFoundException("WebPage with requested URL name does not exist.");

            var webPage = _webPagesProcessor.GetWebPageByUrlName(webPageUrlName, (int)EnumWebPageType.Page);
            if (webPage == null)
                throw new ContentNotFoundException("WebPage with requested URL name does not exist.");
            if (webPage.IsDeleted)
                throw new ContentNotFoundException("Requested WebPage is deleted.");
            if (!webPage.IsEnabled)
                throw new ContentNotFoundException("Requested WebPage is disabled.");

            var vm = new SubjectNewsViewModel
            {
                // Title = $"{webPage.WebPageName} News",
                // Description = $"Latest news and articles regarding {webPage.WebPageName}",
                LinkToPageText = $"{webPage.WebPageName} Information",
                LinkToPageUrl = Url.Action("Index", "WebPageVisual", new { name = webPage.UrlName })
            };
            if (all)
            {
                vm.Title = $"{webPage.WebPageName} News Archive";
                vm.Description = $"Archive of news and articles regarding {webPage.WebPageName}";
            }
            else
            {
                vm.Title = $"{webPage.WebPageName} News";
                vm.Description = $"Latest news and articles regarding {webPage.WebPageName}";
            }

            var relatedArticles = new List<Article>();
            if (all)
            {
                relatedArticles = _articlesProcessor.GetWebPageRelatedArticles(
                    webPage.Id.Value, null, null).ToList();
            }
            else
            {
                relatedArticles = _articlesProcessor.GetWebPageRelatedArticles(
                    webPage.Id.Value, 20, DateTime.Now.AddYears(-2)).ToList();
                int countAll = _articlesProcessor
                    .CountWebPageRelatedArticles(webPage.Id.Value, null);
                vm.CountAllRelatedArticles = countAll;
            }

            if (relatedArticles.Count < 3)
                relatedArticles = _articlesProcessor.GetWebPageRelatedArticles(webPage.Id.Value, 3).ToList();

            vm.RecentRelatedArticles = relatedArticles.Select(x => new ArticleModel(x));

            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

            var bannerImageName = webPage.GetBannerImageName(AppConstants.DefaultBannerName);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.PublicImagesFolderServerPath + bannerImageName);

            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);
            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, vm.Description);

            var ogImagePath = webPage.GetSocialImageName(AppConstants.DefaultBannerNameNews);
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImagePath);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
            {
                AltText = webPage.IsSubjectPage ? $"{webPage.WebPageTitle} news page banner" : "Live Forever Club News",
                ImgTitle = $"{webPage.WebPageTitle} News"
            });

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
