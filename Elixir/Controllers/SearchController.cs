using Elixir.Contracts.Interfaces;
using Elixir.Models;
using Elixir.Models.Utils;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly IArticlesProcessor _articlesProcessor;
        private readonly IWebPagesProcessor _webpagesProcessor;
        private readonly IBlogPostsProcessor _blogsProcessor;
        private readonly ISearchLogsProcessor _searchLogsProcessor;
        private readonly IResourcesProcessor _resourcesProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public SearchController(
            IArticlesProcessor articlesProcessor,
            IWebPagesProcessor webpagesProcessor,
            IBlogPostsProcessor blogsProcessor,
            ISearchLogsProcessor searchLogsProcessor,
            IResourcesProcessor resourcesProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _articlesProcessor = articlesProcessor;
            _webpagesProcessor = webpagesProcessor;
            _blogsProcessor = blogsProcessor;
            _searchLogsProcessor = searchLogsProcessor;
            _resourcesProcessor = resourcesProcessor;
            _settingsProcessor = settingsProcessor;
        }

        [ValidateInput(true)]
        public ActionResult Index(string query = "", string display = "")
        {
            SearchResult result = new SearchResult();
            try
            {
                bool all = !string.IsNullOrEmpty(display) && display.Trim().CleanAndMarkPhrases().ToLower() == "all";
                if (!string.IsNullOrEmpty(query))
                {
                    query = query.Trim();
                }
                result.DisplayAll = all;

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

                if (query == "")
                {
                    ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription,
                        "Search for information on the Live Forever Club website");
                    ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);
                    ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath,
                        AppConstants.FileSystem.WebPageBannerImageFolderServerPath +
                        AppConstants.DefaultBannerName);

                    return View(result);
                }

                query = query.CleanAndMarkPhrases();

                var terms = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (terms.Length > 10)
                    terms = terms.Take(10).ToArray();
                var termsOrPhrases = CheckForPhrases(terms);

                result.TermsSearched = query;

                if (all)
                {
                    ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription,
                           query + " news, blogs and resources in the Live Forever Club archive");
                }
                else
                {
                    ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription,
                        query + " news, blogs and resources on the Live Forever Club");
                }
                ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName,
                    "/" + AppConstants.FileSystem.PublicImagesFolderName +
                    "/" + AppConstants.ImageSearchResults);

                ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);
                ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath,
                    AppConstants.FileSystem.WebPageBannerImageFolderServerPath +
                    AppConstants.BannerImageSearchResults);

                var articles = _articlesProcessor.Search(termsOrPhrases, all);
                var webPages = _webpagesProcessor.Search(termsOrPhrases, all);
                var blogPosts = _blogsProcessor.Search(termsOrPhrases, all);
                var resources = _resourcesProcessor.Search(termsOrPhrases, all);

                result.ArticlesFound = articles.ToList();
                result.WebPagesFound = webPages.ToList();
                result.BlogPostsFound = blogPosts.ToList();
                result.ResourcesFound = resources.ToList();

                var wordsCount = query.Split(new char[] { ' ', '+' }, StringSplitOptions.RemoveEmptyEntries).Count();

                _searchLogsProcessor.Log(DateTime.Now, Request.UserHostAddress, query, wordsCount);

                return View(result);
            }
            catch (Exception ex)
            {
                result.ErrorOnSearch = true;
                result.ErrorMessage = $"Ex. Message: {ex.Message}\nInner Ex. Message:{ex.InnerException?.Message}\nStack trace:{ex.StackTrace}";

                return View(result);
            }
        }

        private List<string> CheckForPhrases(string[] items)
        {
            List<string> result = new List<string>();
            foreach (var item in items)
            {
                if (item.Contains("+"))
                    result.Add(item.Replace("+", " "));
                else
                    result.Add(item);
            }

            return result;
        }
    }
}