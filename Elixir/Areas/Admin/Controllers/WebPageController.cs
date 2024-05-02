using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Areas.Admin.Views.WebPage;
using Elixir.Attributes;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Controllers;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Microsoft.AspNet.Identity;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class WebPageController : BaseController
    {
        private readonly IWebPagesProcessor _webPagesProcessor;
        private readonly ITopicsProcessor _topicsProcessor;
        private readonly IWebPageTypesRepository _webPageTypesRepository;

        public WebPageController(IWebPagesProcessor webPagesProcessor, ITopicsProcessor topicsProcessor, IWebPageTypesRepository webPageTypesRepository)
        {
            _webPagesProcessor = webPagesProcessor;
            _topicsProcessor = topicsProcessor;
            _webPageTypesRepository = webPageTypesRepository;
        }

        public ActionResult Index
            (WebPagesSortOrder sortBy = WebPagesSortOrder.WebPageName,
            SortDirection direction = SortDirection.Ascending,
            string filter = "",
            int pageTypeId = 0
            )
        {
            var webPage = _webPagesProcessor.GetNFilteredWebPages(100, false, filter, pageTypeId, sortBy, direction).Select(p => new WebPageModel(p));
            return SortableListView<WebPagesSortOrder, WebPagesViewModel, WebPageModel>(webPage, sortBy, direction);
        }

        public ActionResult Editor()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateWebPageViewModel();

            viewModel.DisplayOrder = 1;
            viewModel.IsEnabled = true;
            viewModel.IsSubjectPage = false;
            viewModel.AvailableParentPages = GetWebPagesForParentPage();
            viewModel.AvailableWebPageTypes = GetAllWebpageTypes();
            viewModel.PrimaryTopics = GetTopicsDropdownItems();
            viewModel.SecondaryTopics = GetTopicsDropdownItems();
            viewModel.PublishedOnDT = DateTime.Now;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateWebPageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //map the string id to an int
                //viewModel.Model.ParentID = viewModel.Model.ParentID;
                try
                {
                    int userId;
                    if (User.Identity.IsAuthenticated)
                    {
                        userId = int.Parse(User.Identity.GetUserId());
                        viewModel.CreatedByUserId = userId;
                        viewModel.UpdatedByUserId = userId;
                    }
                    viewModel = TrimmingData(viewModel);
                    _webPagesProcessor.CreateWebPage(viewModel);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException mve)
                {
                    ModelState.AddModelError("", mve.Message);

                    viewModel.AvailableParentPages = GetWebPagesForParentPage();
                    viewModel.PrimaryTopics = GetTopicsDropdownItems();
                    viewModel.SecondaryTopics = GetTopicsDropdownItems();
                    viewModel.AvailableWebPageTypes = GetAllWebpageTypes();
                }
            }
            else
            {
                viewModel.AvailableParentPages = GetWebPagesForParentPage();
                viewModel.DisplayOrder = 1;
                viewModel.IsEnabled = true;
                viewModel.IsSubjectPage = false;
                viewModel.PrimaryTopics = GetTopicsDropdownItems();
                viewModel.SecondaryTopics = GetTopicsDropdownItems();
                viewModel.AvailableWebPageTypes = GetAllWebpageTypes();
                viewModel.PublishedOnDT = DateTime.Now;
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                throw new ArgumentException("Incorrect id of Page.", nameof(id));

            var viewModel = new EditWebPageViewModel();

            var webPage = _webPagesProcessor.GetWebPageById(id.Value);

            if (webPage == null || webPage.IsDeleted)
                throw new ContentNotFoundException("The WebPage with specified id does not exist.");

            viewModel.EntityToViewModel(webPage);
            viewModel.AvailableParentPages = GetWebPagesForParentPage(webPage.Id.Value);
            viewModel.PrimaryTopics = GetTopicsDropdownItems(webPage.PrimaryTopicID);
            viewModel.SecondaryTopics = GetTopicsDropdownItems(webPage.SecondaryTopicID.GetValueOrDefault());
            viewModel.AvailableWebPageTypes = GetAllWebpageTypes();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditWebPageViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Check if WebPage is not deleted.
                // TODO: AY - this checks should me moved to Business Logic.
                if (viewModel.IsDeleted || _webPagesProcessor.GetWebPageById(viewModel.Id.Value).IsDeleted)
                    throw new InvalidOperationException("Unable to update WebPage because it is deleted.");

                try
                {
                    int userId;
                    if (User.Identity.IsAuthenticated)
                    {
                        userId = int.Parse(User.Identity.GetUserId());
                        viewModel.UpdatedByUserId = userId;
                    }
                    viewModel = TrimmingData(viewModel);
                    //check ParentID stuff
                    _webPagesProcessor.UpdateWebPage(viewModel, viewModel.IsSignificantChange);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException mve)
                {
                    ModelState.AddModelError("", mve.Message);
                }
            }

            viewModel.AvailableParentPages = GetWebPagesForParentPage(viewModel.Id.Value);
            viewModel.PrimaryTopics = GetTopicsDropdownItems();
            viewModel.SecondaryTopics = GetTopicsDropdownItems();
            viewModel.AvailableWebPageTypes = GetAllWebpageTypes();

            return View(viewModel);
        }

        // TODO: AY - maybe get rid of Nullables?
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                throw new ArgumentException("No id provided");

            // TODO: AY - this checks should me moved to Business Logic.
            if (_webPagesProcessor.GetWebPageById(id.Value).IsDeleted)
                throw new InvalidOperationException("Unable to delete WebPage because it is deleted.");

            _webPagesProcessor.DeleteWebPage(id.Value);

            return RedirectToAction("Index");
        }

        private IEnumerable<KeyValuePair<int, string>> GetWebPagesForParentPage(int idToExclude = -1)
        {
            var types = _webPageTypesRepository.GetAll();

            var list = new List<KeyValuePair<int, string>>();

            foreach (var webPage in _webPagesProcessor.GetAllWebPages())
            {
                //don't allow to add self as parent?
                if (webPage.Id.Value == idToExclude) continue;

                var type = types.FirstOrDefault(t =>
                    t.WebPageTypeID == webPage.TypeID).WebPageTypeName;
                string detailedParent = $"[{type}]" + webPage.WebPageName;

                list.Add(new KeyValuePair<int, string>(webPage.Id.Value, detailedParent));
            }

            return list.OrderBy(x => x.Value);
        }

        public IEnumerable<KeyValuePair<int, string>> GetAllWebpageTypes()
        {
            var allTypes = _webPageTypesRepository.GetAll()
                .OrderBy(x => x.WebPageTypeName);
            var list = new List<KeyValuePair<int, string>>();

            foreach (var type in allTypes)
            {
                list.Add(new KeyValuePair<int, string>(type.WebPageTypeID, type.WebPageTypeName));
            }
            return list.OrderBy(x => x.Value);
        }

        private IEnumerable<SelectListItem> GetTopicsDropdownItems(int selectedId = 0)
        {
            var topics = _topicsProcessor.GetAllTopics(TopicSortOrder.TopicName, SortDirection.Ascending);
            var result = new List<SelectListItem>();

            foreach (var topic in topics)
            {
                var listItem = new SelectListItem();
                listItem.Text = topic.TopicName;
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

        private dynamic TrimmingData(dynamic viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.WebPageName))
                viewModel.WebPageName = viewModel.WebPageName.Trim();
            if (!string.IsNullOrEmpty(viewModel.WebPageTitle))
                viewModel.WebPageTitle = viewModel.WebPageTitle.Trim();
            if (!string.IsNullOrEmpty(viewModel.MetaDescription))
                viewModel.MetaDescription = viewModel.MetaDescription.Trim();
            if (!string.IsNullOrEmpty(viewModel.BannerImageFileName))
                viewModel.BannerImageFileName = viewModel.BannerImageFileName.Trim();
            if (!string.IsNullOrEmpty(viewModel.SocialImageFileName))
                viewModel.SocialImageFileName = viewModel.SocialImageFileName.Trim();
            if (!string.IsNullOrEmpty(viewModel.NotesInternal))
                viewModel.NotesInternal = viewModel.NotesInternal.Trim();
            return viewModel;
        }
    }
}