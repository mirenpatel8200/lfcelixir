using System;
using System.Linq;
using System.Web.Mvc;
using Elixir.Contracts.Interfaces;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.Views;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class ResourcesController : BaseController
    {
        private readonly IResourcesProcessor _resourcesProcessor;
        private readonly IArticlesProcessor _articlesProcessor;
        private readonly IBlogPostsProcessor _blogsProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public ResourcesController(
            IResourcesProcessor resourcesProcessor,
            IArticlesProcessor articlesProcessor,
            IBlogPostsProcessor blogsProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _resourcesProcessor = resourcesProcessor;
            _articlesProcessor = articlesProcessor;
            _blogsProcessor = blogsProcessor;
            _settingsProcessor = settingsProcessor;
        }

        // GET: /resources/<name>
        public ActionResult Index(string name = null)
        {
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType,
                HeadNavigationType.MemberView);

            if (name == null)
                return View();

            var resource = _resourcesProcessor.GetResourceByUrlName(name);
            if (resource == null)
            {
                throw new ContentNotFoundException(
                    "The page with the specified name does not exist.");
            }

            if (name.Equals("404", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 404;
            else if (name.Equals("500", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 500;

            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription,
                resource.ResourceDescriptionPublic);
            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

            var bannerName = AppConstants.DefaultBannerName;
            if (resource.PrimaryTopic != null && !string.IsNullOrWhiteSpace(resource.PrimaryTopic.BannerImageFileName))
            {
                bannerName = resource.PrimaryTopic.BannerImageFileName;
            }

            var ogImageName = AppConstants.DefaultBannerNameSocial;
            if (resource.PrimaryTopic != null && !string.IsNullOrWhiteSpace(resource.PrimaryTopic.SocialImageFilename))
            {
                ogImageName = resource.PrimaryTopic.SocialImageFilename;
            }
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImageName);

            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerName);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
            {
                AltText = $"{resource.ResourceName} information page banner",
                ImgTitle = resource.ResourceName
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

            ResourceViewModel resourceViewModel = new ResourceViewModel();
            resourceViewModel.Resource = resource;

            if (resource.ParentResourceID.HasValue && resource.ParentResourceID.Value > 0)
            {
                var parent = _resourcesProcessor.GetResourceById(
                    resource.ParentResourceID.Value);
                resourceViewModel.Parent = parent;
            }

            resourceViewModel.ArticlesMentioningResource =
                _articlesProcessor.GetArticlesMentioningResource(resource.Id.Value).ToList();
            resourceViewModel.ArticlesDisplayFormat = ArticlesDisplayFormat.AsNews;

            resourceViewModel.ChildResourcesOfTypePerson = _resourcesProcessor.
                GetChildResources(resource.Id.Value,
                ResourceTypes.Person.ToDatabaseValues().First())
                .Where(cr => cr.IsHiddenPublic == false)
                .ToList();

            resourceViewModel.ChildResourcesOfTypeCreation = _resourcesProcessor.
                GetChildResources(resource.Id.Value,
                ResourceTypes.Creation.ToDatabaseValues().First())
                .Where(cr => cr.IsHiddenPublic == false)
                .ToList();

            resourceViewModel.RelatedBlogPosts = _blogsProcessor.
                GetBlogPostsMentioningResource(resource.Id.Value)
                .OrderByDescending(b => b.PublishedUpdatedDT ?? b.PublishedOnDT)
                .ToList();

            resourceViewModel.MentionedResources = _resourcesProcessor.
                GetResourcesMentionedInResource(resource.Id.Value)
                .OrderBy(r => r.ResourceName).ToList();

            resourceViewModel.ResourcesMentioningThis = _resourcesProcessor.
                GetResourcesMentioningResource(resource.Id.Value)
                .OrderBy(r => r.ResourceName).ToList();

            return View(resourceViewModel);
        }
    }
}