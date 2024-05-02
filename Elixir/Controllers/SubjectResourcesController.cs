using Elixir.Areas.Admin.Models;
using Elixir.Contracts.Interfaces;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class SubjectResourcesController : Controller
    {
        private readonly IWebPagesProcessor _webPagesProcessor;
        private readonly IResourcesProcessor _resourcesProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public SubjectResourcesController(
            IWebPagesProcessor webPagesProcessor,
            IResourcesProcessor resourcesProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _webPagesProcessor = webPagesProcessor;
            _resourcesProcessor = resourcesProcessor;
            _settingsProcessor = settingsProcessor;
        }

        [Route("page/{webPageUrlName}/resources")]
        public ActionResult Index(string webPageUrlName, string display = "")
        {
            var all = display == "all";
            if (string.IsNullOrWhiteSpace(webPageUrlName) || webPageUrlName.Equals("home", StringComparison.OrdinalIgnoreCase))
                throw new ContentNotFoundException("WebPage with requested URL name does not exist.");

            var webPage = _webPagesProcessor.GetWebPageByUrlName(webPageUrlName, (int)EnumWebPageType.Page);
            if (webPage == null)
                throw new ContentNotFoundException("WebPage with requested URL name does not exist.");
            if (webPage.IsDeleted)
                throw new ContentNotFoundException("Requested WebPage is deleted.");
            if (!webPage.IsEnabled)
                throw new ContentNotFoundException("Requested WebPage is disabled.");

            var vm = new SubjectResourcesViewModel
            {
                Title = $"{webPage.WebPageTitle} Resources",
                WebPageTitle = webPage.WebPageTitle,
                Description = $"Creations, people and organisations regarding {webPage.WebPageTitle}",
                LinkToPageText = $"{webPage.WebPageTitle} Information",
                LinkToPageUrl = Url.Action("Index", "WebPageVisual", new { name = webPage.UrlName }),
                IntroText = $"The Live Forever Club collates details of as many people, organisation and things regarding {webPage.WebPageTitle}. Click through on any resource below to discover more about it."
            };

            var limitedRelatedResources = new List<Resource>();
            limitedRelatedResources = _resourcesProcessor.GetWebPageReleatedResources(webPage.Id.Value, 40, DateTime.Now.AddYears(-2)).ToList();

            var relatedResources = new List<Resource>();
            if (all)
                relatedResources = _resourcesProcessor.GetWebPageReleatedResources(webPage.Id.Value).ToList();
            else
            {
                relatedResources = limitedRelatedResources;
                vm.CountOfRelatedResources = _resourcesProcessor.GetWebPageReleatedResources(webPage.Id.Value).ToList().Count;
            }

            if (relatedResources.Count < 5)
                relatedResources = _resourcesProcessor.GetWebPageReleatedResources(webPage.Id.Value, 5).ToList();

            vm.RecentRelatedResources = relatedResources;

            if (vm.RecentRelatedResources != null && vm.RecentRelatedResources.Count > 0)
            {
                foreach (var resource in vm.RecentRelatedResources)
                {
                    resource.IsVisible = true;
                }
            }

            vm.CreationResources = vm.RecentRelatedResources.Where(x => x.ResourceTypeId == (int)ResourceTypes.Creation.ToDatabaseValues().First()).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();

            if (vm.CreationResources != null && vm.CreationResources.Count > 0)
            {
                vm.Applications = new List<Resource>(vm.CreationResources.Where(x => x.IsApplication).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Audio = new List<Resource>(vm.CreationResources.Where(x => x.IsAudio).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Books = vm.CreationResources.Where(x => x.IsBook).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Competitions = new List<Resource>(vm.CreationResources.Where(x => x.IsCompetition).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Education = new List<Resource>(vm.CreationResources.Where(x => x.IsEducation).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Events = new List<Resource>(vm.CreationResources.Where(x => x.IsEvent).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Films = new List<Resource>(vm.CreationResources.Where(x => x.IsFilm).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Information = new List<Resource>(vm.CreationResources.Where(x => x.IsInformation).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Products = new List<Resource>(vm.CreationResources.Where(x => x.IsProduct).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Research = new List<Resource>(vm.CreationResources.Where(x => x.IsResearch).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());
                vm.Videos = new List<Resource>(vm.CreationResources.Where(x => x.IsVideo).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList());

                vm.CreationResources = SetVisibilityOfResources(vm.Applications, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Audio, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Books, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Competitions, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Education, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Events, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Films, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Information, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Products, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Research, vm.CreationResources);
                vm.CreationResources = SetVisibilityOfResources(vm.Videos, vm.CreationResources);

                if (all && !Request.IsAuthenticated)
                {
                    vm.CreationResources = SetDisabilityOfResources(limitedRelatedResources, vm.CreationResources);
                    vm.Applications = SetDisabilityOfResources(limitedRelatedResources, vm.Applications);
                    vm.Audio = SetDisabilityOfResources(limitedRelatedResources, vm.Audio);
                    vm.Books = SetDisabilityOfResources(limitedRelatedResources, vm.Books);
                    vm.Competitions = SetDisabilityOfResources(limitedRelatedResources, vm.Competitions);
                    vm.Education = SetDisabilityOfResources(limitedRelatedResources, vm.Education);
                    vm.Events = SetDisabilityOfResources(limitedRelatedResources, vm.Events);
                    vm.Films = SetDisabilityOfResources(limitedRelatedResources, vm.Films);
                    vm.Information = SetDisabilityOfResources(limitedRelatedResources, vm.Information);
                    vm.Products = SetDisabilityOfResources(limitedRelatedResources, vm.Products);
                    vm.Research = SetDisabilityOfResources(limitedRelatedResources, vm.Research);
                    vm.Videos = SetDisabilityOfResources(limitedRelatedResources, vm.Videos);
                }
            }

            vm.PeopleResources = vm.RecentRelatedResources.Where(x => x.ResourceTypeId == (int)ResourceTypes.Person.ToDatabaseValues().First()).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();

            if (vm.PeopleResources != null && vm.PeopleResources.Count > 0)
            {
                vm.Academics = vm.PeopleResources.Where(x => x.IsAcademic).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Advocates = vm.PeopleResources.Where(x => x.IsAdvocate).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Artists = vm.PeopleResources.Where(x => x.IsArtist).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Authors = vm.PeopleResources.Where(x => x.IsAuthor).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.CompanyRepresentatives = vm.PeopleResources.Where(x => x.IsCompanyRep).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.HealthcareProfessionals = vm.PeopleResources.Where(x => x.IsHealthPro).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Journalists = vm.PeopleResources.Where(x => x.IsJournalist).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Politicians = vm.PeopleResources.Where(x => x.IsPolitician).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();

                vm.PeopleResources = SetVisibilityOfResources(vm.Academics, vm.PeopleResources);
                vm.PeopleResources = SetVisibilityOfResources(vm.Advocates, vm.PeopleResources);
                vm.PeopleResources = SetVisibilityOfResources(vm.Artists, vm.PeopleResources);
                vm.PeopleResources = SetVisibilityOfResources(vm.Authors, vm.PeopleResources);
                vm.PeopleResources = SetVisibilityOfResources(vm.CompanyRepresentatives, vm.PeopleResources);
                vm.PeopleResources = SetVisibilityOfResources(vm.HealthcareProfessionals, vm.PeopleResources);
                vm.PeopleResources = SetVisibilityOfResources(vm.Journalists, vm.PeopleResources);
                vm.PeopleResources = SetVisibilityOfResources(vm.Politicians, vm.PeopleResources);

                if (all && !Request.IsAuthenticated)
                {
                    vm.PeopleResources = SetDisabilityOfResources(limitedRelatedResources, vm.PeopleResources);
                    vm.Academics = SetDisabilityOfResources(limitedRelatedResources, vm.Academics);
                    vm.Advocates = SetDisabilityOfResources(limitedRelatedResources, vm.Advocates);
                    vm.Artists = SetDisabilityOfResources(limitedRelatedResources, vm.Artists);
                    vm.Authors = SetDisabilityOfResources(limitedRelatedResources, vm.Authors);
                    vm.CompanyRepresentatives = SetDisabilityOfResources(limitedRelatedResources, vm.CompanyRepresentatives);
                    vm.HealthcareProfessionals = SetDisabilityOfResources(limitedRelatedResources, vm.HealthcareProfessionals);
                    vm.Journalists = SetDisabilityOfResources(limitedRelatedResources, vm.Journalists);
                    vm.Politicians = SetDisabilityOfResources(limitedRelatedResources, vm.Politicians);
                }
            }

            vm.OrganisationResources = vm.RecentRelatedResources.Where(x => x.ResourceTypeId == (int)ResourceTypes.Organisation.ToDatabaseValues().First()).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();

            if (vm.OrganisationResources != null && vm.OrganisationResources.Count > 0)
            {
                vm.Academia = vm.OrganisationResources.Where(x => x.IsAcademia).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Companies = vm.OrganisationResources.Where(x => x.IsCompany).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.HealthcareOrganisations = vm.OrganisationResources.Where(x => x.IsHealthOrg).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Institutes = vm.OrganisationResources.Where(x => x.IsInstitute).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Journals = vm.OrganisationResources.Where(x => x.IsJournal).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();
                vm.Publisher = vm.OrganisationResources.Where(x => x.IsPublisher).OrderBy(x => x.ResourceName).Select(x => (Resource)x.Clone()).ToList();

                vm.OrganisationResources = SetVisibilityOfResources(vm.Academia, vm.OrganisationResources);
                vm.OrganisationResources = SetVisibilityOfResources(vm.Companies, vm.OrganisationResources);
                vm.OrganisationResources = SetVisibilityOfResources(vm.HealthcareOrganisations, vm.OrganisationResources);
                vm.OrganisationResources = SetVisibilityOfResources(vm.Institutes, vm.OrganisationResources);
                vm.OrganisationResources = SetVisibilityOfResources(vm.Journals, vm.OrganisationResources);
                vm.OrganisationResources = SetVisibilityOfResources(vm.Publisher, vm.OrganisationResources);

                if (all && !Request.IsAuthenticated)
                {
                    vm.OrganisationResources = SetDisabilityOfResources(limitedRelatedResources, vm.OrganisationResources);
                    vm.Academia = SetDisabilityOfResources(limitedRelatedResources, vm.Academia);
                    vm.Companies = SetDisabilityOfResources(limitedRelatedResources, vm.Companies);
                    vm.HealthcareOrganisations = SetDisabilityOfResources(limitedRelatedResources, vm.HealthcareOrganisations);
                    vm.Institutes = SetDisabilityOfResources(limitedRelatedResources, vm.Institutes);
                    vm.Journals = SetDisabilityOfResources(limitedRelatedResources, vm.Journals);
                    vm.Publisher = SetDisabilityOfResources(limitedRelatedResources, vm.Publisher);
                }
            }
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);
            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);
            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, vm.Description);
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + webPage.GetSocialImageName(AppConstants.DefaultBannerNameNews));
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.PublicImagesFolderServerPath + webPage.GetBannerImageName(AppConstants.DefaultBannerName));
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
            {
                AltText = webPage.IsSubjectPage ? $"{webPage.WebPageTitle} resources page banner" : "Live Forever Club Resources",
                ImgTitle = $"{webPage.WebPageTitle} Resources"
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

        public List<Resource> SetVisibilityOfResources(List<Resource> categoryResources, List<Resource> typeResources)
        {
            if (categoryResources != null && categoryResources.Count > 2 && typeResources != null && typeResources.Count > 0)
            {
                foreach (var categoryResource in categoryResources)
                {
                    var resource = typeResources.Where(x => x.Id == categoryResource.Id).SingleOrDefault();
                    if (resource != null)
                        resource.IsVisible = false;
                }
            }
            return typeResources;
        }

        public List<Resource> SetDisabilityOfResources(List<Resource> source, List<Resource> destination)
        {
            if (source != null && source.Count > 0 && destination != null && destination.Count > 0)
            {
                foreach (var destinationResource in destination)
                {
                    var resource = source.Where(x => x.Id == destinationResource.Id).SingleOrDefault();
                    if (resource == null)
                        destinationResource.IsDisableLink = true;
                    else
                        destinationResource.IsDisableLink = false;
                }
            }
            return destination;
        }
    }
}