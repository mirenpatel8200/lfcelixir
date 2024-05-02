using System;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Areas.AdminManual.ViewModels;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class TopicController : BaseController
    {
        private readonly ITopicsProcessor _topicsProcessor;
        private readonly IWebPagesProcessor _webPagesProcessor;

        public TopicController(ITopicsProcessor topicsProcessor, IWebPagesProcessor webPagesProcessor)
        {
            _topicsProcessor = topicsProcessor;
            _webPagesProcessor = webPagesProcessor;
        }

        // GET: Topics
        public ActionResult Index(TopicSortOrder sortBy = TopicSortOrder.TopicName, SortDirection direction = SortDirection.Ascending,
            string filter = "", string includeAllFields = "")
        {
            if (!Enum.IsDefined(typeof(TopicSortOrder), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");
            
            var _includeAllFields = false;
            if (!string.IsNullOrEmpty(includeAllFields) && includeAllFields.ToLower() == "on")
            {
                _includeAllFields = true;
            }
            var topics = _topicsProcessor.GetFilteredTopics(filter, sortBy, direction, _includeAllFields).Select(x => new TopicModel(x));

            return SortableListView<TopicSortOrder, TopicsViewModel, TopicModel>(topics, sortBy, direction);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateTopicViewModel();
            var topicModel = new TopicModel();

            viewModel.Model = topicModel;
            var allPages = _webPagesProcessor.GetAllWebPages();

            viewModel.FillAvailableWebPages(allPages);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateTopicViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (_topicsProcessor.TopicExists(viewModel.Model))
                {
                    ModelState.AddModelError(String.Empty, "Topic with such name already exists.");
                }
                else
                {
                    viewModel = TrimmingData(viewModel);
                    _topicsProcessor.CreateTopic(viewModel.Model);

                    return RedirectToAction("Index");
                }
            }

            var allPages = _webPagesProcessor.GetAllWebPages();
            viewModel.FillAvailableWebPages(allPages);

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var viewModel = new EditTopicViewModel();

            if (id == null)
                throw new ArgumentException("Incorrect id of Topic.", nameof(id));

            var topic = _topicsProcessor.GetTopicById(id.Value);
            if (topic == null)
                throw new ContentNotFoundException("The Topic with specified id does not exist.");

            viewModel.Model = new TopicModel(topic);

            var allPages = _webPagesProcessor.GetAllWebPages();
            viewModel.FillAvailableWebPages(allPages, topic.PrimaryWebPageId);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditTopicViewModel viewModel)
        {
            if (ModelState.IsValid && viewModel.Model.PrimaryWebPageId.HasValue)
            {
                if (_topicsProcessor.TopicExists(viewModel.Model))
                {
                    ModelState.AddModelError(String.Empty, "Topic with such name already exists.");
                }
                else
                {
                    viewModel = TrimmingData(viewModel);
                    _topicsProcessor.UpdateTopic(viewModel.Model);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                //in case of sending a null PrimaryWebPageId - We don't need to allow this
                //Users can set it to None (0), but not to null
                if (viewModel.Model.PrimaryWebPageId.HasValue == false)
                {
                    ModelState.AddModelError(String.Empty, "Topic - If you want to remove the Topic's Primary Web Page, please set it to \"None\"");
                }

                var allPages = _webPagesProcessor.GetAllWebPages();

                var topic = _topicsProcessor.GetTopicById(viewModel.Model.Id.Value);
                if (topic == null)
                    throw new ContentNotFoundException("The Topic with specified id does not exist.");

                viewModel.Model = new TopicModel(topic);
                viewModel.FillAvailableWebPages(allPages, topic.PrimaryWebPageId);

                return View(viewModel);
            }

            return View(viewModel);
        }

        private dynamic TrimmingData(dynamic viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Model.TopicName))
                viewModel.Model.TopicName = viewModel.Model.TopicName.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.DescriptionInternal))
                viewModel.Model.DescriptionInternal = viewModel.Model.DescriptionInternal.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.BannerImageFileName))
                viewModel.Model.BannerImageFileName = viewModel.Model.BannerImageFileName.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.SocialImageFilename))
                viewModel.Model.SocialImageFilename = viewModel.Model.SocialImageFilename.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.SocialImageFilenameNews))
                viewModel.Model.SocialImageFilenameNews = viewModel.Model.SocialImageFilenameNews.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.Hashtags))
                viewModel.Model.Hashtags = viewModel.Model.Hashtags.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.NotesInternal))
                viewModel.Model.NotesInternal = viewModel.Model.NotesInternal.Trim();
            return viewModel;
        }
    }
}