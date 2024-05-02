using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Models.Json;
using Elixir.Utils.View;
using System.Web;
using Elixir.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Attributes;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Researcher)]
    public class ResourceController : BaseController
    {
        private readonly IResourcesProcessor _resourcesProcessor;
        private readonly ITopicsProcessor _topicsProcessor;
        private readonly ICountryProcessor _countryProcessor;
        public ResourceController(IResourcesProcessor resourcesProcessor,
            ITopicsProcessor topicsProcessor,
            ICountryProcessor countryProcessor)
        {
            _resourcesProcessor = resourcesProcessor;
            _topicsProcessor = topicsProcessor;
            _countryProcessor = countryProcessor;
        }

        // GET: Admin/Resource
        public ActionResult Index(
            ResourcesSortOrder sortBy = ResourcesSortOrder.ResourceID,
            SortDirection direction = SortDirection.Descending,
            string filter = "")
        {
            if (!Enum.IsDefined(typeof(ResourcesSortOrder), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");

            var resources = _resourcesProcessor.Get100Resources(sortBy, direction, filter).Select(r => new ResourceModel(r));
            var viewModel = new ResourcesViewModel()
            {
                CurrentFilter = filter
            };

            return SortableListView(resources, sortBy, direction, viewModel);
        }

        #region Create Resource

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateResourceViewModel
            {
                Model = new ResourceModel()
            };
            viewModel.AddSelectList(nameof(viewModel.Model.ResourceTypeId), GetAllResourceTypes());
            viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopicID), GetTopicsDropdownItems());
            viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopicID), GetTopicsDropdownItems());

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateResourceViewModel viewModel)
        {
            try
            {
                #region check for subtype selection
                if (viewModel!=null && viewModel.Model!=null)
                {
                    if (!(viewModel.Model.IsAcademia || viewModel.Model.IsCompany ||
                    viewModel.Model.IsHealthOrg || viewModel.Model.IsInstitute ||
                    viewModel.Model.IsJournal || viewModel.Model.IsPublisher ||
                    viewModel.Model.IsAcademic || viewModel.Model.IsAdvocate ||
                    viewModel.Model.IsArtist || viewModel.Model.IsAuthor ||
                    viewModel.Model.IsCompanyRep || viewModel.Model.IsHealthPro ||
                    viewModel.Model.IsJournalist || viewModel.Model.IsPolitician ||
                    viewModel.Model.IsApplication || viewModel.Model.IsAudio ||
                    viewModel.Model.IsBook || viewModel.Model.IsCompetition ||
                    viewModel.Model.IsEducation || viewModel.Model.IsEvent ||
                    viewModel.Model.IsFilm || viewModel.Model.IsInformation ||
                    viewModel.Model.IsProduct || viewModel.Model.IsResearch ||
                    viewModel.Model.IsVideo)
                    )
                    {
                        ModelState.AddModelError("Model.IsAcademia", "Select at least one sub type!");
                    }
                }
                
                #endregion
                if (ModelState.IsValid)
                {
                    
                    viewModel = TrimmingData(viewModel);
                    viewModel.Model.DeconstructParentResourceWithId();
                    SetCreatedBy(viewModel.Model);
                    SetUpdatedBy(viewModel.Model);

                    var duplicateResource = _resourcesProcessor.GetResourceByUrlName(viewModel.Model.UrlName);
                    if (duplicateResource != null)
                    {
                        throw new ModelValidationException("Resource with specified Url Name already exists.");
                    }

                    var isUrlUnique = VerifyDuplicatesUrl(viewModel.Model.ExternalUrl);
                    if (isUrlUnique == false)
                    {
                        throw new ModelValidationException("This External URL already exists in the database. Please check for any typos or if other resource already has this URL, to avoid producing a duplicate content.");
                    }
                    //Find Country Id For Text
                    if (!string.IsNullOrEmpty(viewModel.Model.CountryNameWithId) && !string.IsNullOrEmpty(viewModel.Model.CountryNameWithId.Trim()))
                    {
                        var CountryName = viewModel.Model.CountryNameWithId.Trim();
                        var Result = _countryProcessor.SearchCountryByName(CountryName, 1);
                        //if (Result != null)
                        //{
                        //    viewModel.Model.CountryId = Result.FirstOrDefault().CountryId;
                        //}
                        if (Result != null && Result.Count() > 0)
                        {
                            viewModel.Model.CountryId = Result.FirstOrDefault().CountryID;
                        }
                        else
                        {
                            ModelState.AddModelError("Model.CountryNameWithId", "Please select a country from the search results!");
                            return View(viewModel);
                        }
                    }

                    _resourcesProcessor.CreateResource(viewModel.Model);
                    return RedirectToAction("Index");
                }
            }
            catch (ModelValidationException mve)
            {
                ModelState.AddModelError("", mve.Message);
            }
            finally
            {
                viewModel.AddSelectList(nameof(viewModel.Model.ResourceTypeId), GetAllResourceTypes());
                viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopicID), GetTopicsDropdownItems());
                viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopicID), GetTopicsDropdownItems());
            }
            return View(viewModel);

        }

        #endregion

        #region Edit Resource

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                throw new ContentNotFoundException("Incorrect id of Resource.");

            var viewModel = new EditResourceViewModel();

            var resource = _resourcesProcessor.GetResourceById(id.Value);
            if (resource == null)
                throw new ContentNotFoundException("The Resource with specified id does not exist.");

            _resourcesProcessor.PopulateResourcesMentioned(resource);

            viewModel.Model = new ResourceModel(resource);
            viewModel.AddSelectList(nameof(viewModel.Model.ResourceTypeId), GetAllResourceTypes());
            viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopicID), GetTopicsDropdownItems(resource.PrimaryTopicID.GetValueOrDefault()));
            viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopicID), GetTopicsDropdownItems(resource.SecondaryTopicID.GetValueOrDefault()));

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditResourceViewModel viewModel)
        {
            try
            {
                #region check for subtype selection
                if (viewModel != null && viewModel.Model != null)
                {
                    if (!(viewModel.Model.IsAcademia || viewModel.Model.IsCompany ||
                    viewModel.Model.IsHealthOrg || viewModel.Model.IsInstitute ||
                    viewModel.Model.IsJournal || viewModel.Model.IsPublisher ||
                    viewModel.Model.IsAcademic || viewModel.Model.IsAdvocate ||
                    viewModel.Model.IsArtist || viewModel.Model.IsAuthor ||
                    viewModel.Model.IsCompanyRep || viewModel.Model.IsHealthPro ||
                    viewModel.Model.IsJournalist || viewModel.Model.IsPolitician ||
                    viewModel.Model.IsApplication || viewModel.Model.IsAudio ||
                    viewModel.Model.IsBook || viewModel.Model.IsCompetition ||
                    viewModel.Model.IsEducation || viewModel.Model.IsEvent ||
                    viewModel.Model.IsFilm || viewModel.Model.IsInformation ||
                    viewModel.Model.IsProduct || viewModel.Model.IsResearch ||
                    viewModel.Model.IsVideo)
                    )
                    {
                        ModelState.AddModelError("Model.IsAcademia", "Select atleast one sub type!");
                    }
                }

                #endregion
                if (ModelState.IsValid)
                {
                    viewModel = TrimmingData(viewModel);
                    viewModel.Model.DeconstructParentResourceWithId();
                    SetUpdatedBy(viewModel.Model);

                    var duplicateResource = _resourcesProcessor.GetResourceByUrlName(viewModel.Model.UrlName);
                    if (duplicateResource != null && viewModel.Model.Id == duplicateResource.Id)
                    {
                        var isUrlUnique = VerifyDuplicatesUrl(viewModel.Model.ExternalUrl, viewModel.Model.Id);
                        if (isUrlUnique == false)
                        {
                            throw new ModelValidationException("This External URL already exists in the database. Please check for any typos or if other resource already has this URL, to avoid producing a duplicate content.");
                        }
                        //Find Country Id For Text
                        if (!string.IsNullOrEmpty(viewModel.Model.CountryNameWithId) && !string.IsNullOrEmpty(viewModel.Model.CountryNameWithId.Trim()))
                        {
                            var CountryName = viewModel.Model.CountryNameWithId.Trim();
                            var Result = _countryProcessor.SearchCountryByName(CountryName, 1);
                            //if (Result != null)
                            //{
                            //    viewModel.Model.CountryId = Result.FirstOrDefault().CountryId;
                            //}
                            if (Result != null && Result.Count() > 0)
                            {
                                viewModel.Model.CountryId = Result.FirstOrDefault().CountryID;
                            }
                            else
                            {
                                ModelState.AddModelError("Model.CountryNameWithId", "Please select a country from the search results!");
                                return View(viewModel);
                            }
                        }
                        _resourcesProcessor.UpdateResource(viewModel.Model);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw new ModelValidationException("Resource with specified Url Name already exists.");
                    }
                }
            }
            catch (ModelValidationException mve)
            {
                ModelState.AddModelError("", mve.Message);
            }
            finally
            {
                viewModel.AddSelectList(nameof(viewModel.Model.ResourceTypeId), GetAllResourceTypes());
                viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopicID), GetTopicsDropdownItems());
                viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopicID), GetTopicsDropdownItems());
            }

            return View(viewModel);
        }

        #endregion

        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
                throw new ContentNotFoundException("Incorrect id of Resource");

            _resourcesProcessor.DeleteResource(id.Value);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult VerifyDuplicates(string url, int? idToExclude = null)
        {
            bool isUnique = VerifyDuplicatesUrl(url, idToExclude);

            return Json(new { success = isUnique }, JsonRequestBehavior.AllowGet);
        }

        public bool VerifyDuplicatesUrl(string url, int? idToExclude = null)
        {
            bool isUnique = true;
            if (string.IsNullOrEmpty(url))
                return isUnique;
            url = url.UrlFullCleanup();
            var existingResource = _resourcesProcessor.GetResourceByExternalUrl(url, idToExclude);
            if (existingResource != null)
                isUnique = false;

            return isUnique;
        }

        private dynamic TrimmingData(dynamic viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Model.ResourceName))
                viewModel.Model.ResourceName = viewModel.Model.ResourceName.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.ExternalUrl))
                viewModel.Model.ExternalUrl = viewModel.Model.ExternalUrl.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.ResourceDescriptionInternal))
                viewModel.Model.ResourceDescriptionInternal = viewModel.Model.ResourceDescriptionInternal.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.ResourceDescriptionPublic))
                viewModel.Model.ResourceDescriptionPublic = viewModel.Model.ResourceDescriptionPublic.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.FacebookHandle))
                viewModel.Model.FacebookHandle = viewModel.Model.FacebookHandle.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.TwitterHandle))
                viewModel.Model.TwitterHandle = viewModel.Model.TwitterHandle.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.YouTubeUrl))
                viewModel.Model.YouTubeUrl = viewModel.Model.YouTubeUrl.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.LinkedInUrl))
                viewModel.Model.LinkedInUrl = viewModel.Model.LinkedInUrl.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.AmazonProductCode))
                viewModel.Model.AmazonProductCode = viewModel.Model.AmazonProductCode.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.NotesInternal))
                viewModel.Model.NotesInternal = viewModel.Model.NotesInternal.Trim();
            return viewModel;
        }

        #region Utilities for Dropdowns 
        private IEnumerable<SelectListItem> GetAllResourceTypes()
        {
            var list = new List<SelectListItem>();
            var allResourceTypes = Enum.GetValues(typeof(ResourceTypes));
            foreach (var resourceType in allResourceTypes)
            {
                var sli = new SelectListItem
                {
                    Text = resourceType.ToString(),
                    Value = ((ResourceTypes)resourceType).ToDatabaseValues().First().ToString()
                };

                list.Add(sli);
            }

            return list;
        }

        private IEnumerable<SelectListItem> GetTopicsDropdownItems(int selectedId = 0)
        {
            var topics = _topicsProcessor.GetAllTopics(TopicSortOrder.TopicName, SortDirection.Ascending);
            var result = new List<SelectListItem>();

            foreach (var topic in topics)
            {
                var listItem = new SelectListItem { Text = topic.TopicName };

                if (!string.IsNullOrEmpty(topic.DescriptionInternal))
                    listItem.Text += $" ({topic.DescriptionInternal})";
                listItem.Value = topic.Id.Value.ToString();

                result.Add(listItem);
            }

            var itemToSelect = result.FirstOrDefault(x => x.Value.Equals(selectedId.ToString()));

            if (itemToSelect != null)
                itemToSelect.Selected = true;

            return result;
        }
        #endregion

        #region Autocomplete JSON

        private IEnumerable<AutocompleteJson> GetResourcesAutocompleteList(
            ResourceTypes resourceTypes, string term, int? limit = null, bool shortMatch = false)
        {
            var list = _resourcesProcessor.SearchResource(term, resourceTypes, limit, shortMatch).Select(x =>
            {
                var valueText = ViewUtils.FormatAutocompleteResource(
                    x.ResourceName, x.Id.Value);
                var displayText = ViewUtils.FormatAutocompleteResourceDetailed(
                    x.ResourceName, x.Id.Value, x.ResourceDescriptionInternal);
                return new AutocompleteJson(displayText, valueText);
            });

            return list;
        }

        /// <summary>
        /// Gets all resources filtered by resource type and name.
        /// </summary>
        /// <param name="resourceTypes">Comma separated enum names (i.g. "person, unknown").</param>
        /// <param name="term">Filter term.</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FetchAutocomplete(string resourceTypes, string term, string shortMatch = "")
        {
            var resourceTypesFlags = ResourceTypes.Unknown;

            if (!string.IsNullOrWhiteSpace(resourceTypes) && !Enum.TryParse(resourceTypes, true, out resourceTypesFlags))
            {
                var r = new JsonActionResult(false, "Unable to parse resource types.");
                return Json(r, JsonRequestBehavior.AllowGet);
            }

            bool isShortMatch = false;
            if (!string.IsNullOrEmpty(shortMatch))
            {
                bool.TryParse(shortMatch, out isShortMatch);
            }

            var list = GetResourcesAutocompleteList(resourceTypesFlags, term, 10, isShortMatch);

            var result = new JsonActionResult<IEnumerable<AutocompleteJson>>(true, list);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult FetchAutocompleteForTags(string resourceType)
        {
            var resourceTypesFlags = ResourceTypes.Unknown;

            if (!string.IsNullOrWhiteSpace(resourceType) && !Enum.TryParse(resourceType, true, out resourceTypesFlags))
            {
                var r = new JsonActionResult(false, "Unable to parse resource type.");
                return Json(r, JsonRequestBehavior.AllowGet);
            }

            var list = GetResourcesAutocompleteList(resourceTypesFlags, "", 1000);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //[Obsolete("Please use FetchAutocomplete instead of this method. It will be removed soon.")]
        public ActionResult AllAutocomplete(string term, string shortMatch = "")
        {
            bool isShortMatch = false;
            if (!string.IsNullOrEmpty(shortMatch))
            {
                bool.TryParse(shortMatch, out isShortMatch);
            }
            var list = GetResourcesAutocompleteList(ResourceTypes.Organisation, term, 10, isShortMatch);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Obsolete("Please use FetchAutocomplete instead of this method. It will be removed soon.")]
        public ActionResult AllReporterAutocomplete(string term)
        {
            var list = GetResourcesAutocompleteList(ResourceTypes.Person, term, 10);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Created By / Updated By Fields
        private void SetCreatedBy(ResourceModel model)
        {
            var accessUserManager = Request.GetOwinContext().Get<AccessUserManager>();
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.CreatedByUserId = authedUserId;
        }
        private void SetUpdatedBy(ResourceModel model)
        {
            var accessUserManager = Request.GetOwinContext().Get<AccessUserManager>();
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.UpdatedByUserId = authedUserId;
        }
        #endregion
    }
}