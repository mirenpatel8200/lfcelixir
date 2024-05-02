using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using System.Web;
using Elixir.Models.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Attributes;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor, Roles.Writer)]
    public class ArticleController : BaseController
    {
        private readonly IArticlesProcessor _articlesProcessor;
        private readonly ITopicsProcessor _topicsProcessor;
        private readonly IResourcesRepository _resourcesRepository;

        public ArticleController(IArticlesProcessor articlesProcessor, ITopicsProcessor topicsProcessor,
            IResourcesRepository resourcesRepository)
        {
            _articlesProcessor = articlesProcessor;
            _topicsProcessor = topicsProcessor;
            _resourcesRepository = resourcesRepository;
        }

        public ActionResult Index(ArticlesSortField sortBy = ArticlesSortField.ArticleID,
            SortDirection direction = SortDirection.Descending,
            string filter = "", string includeAllFields = "", int TopicFilter = 0)
        {
            var _includeAllFields = false;
            if (!string.IsNullOrEmpty(includeAllFields) && includeAllFields.ToLower() == "on")
            {
                _includeAllFields = true;
            }

            if (!Enum.IsDefined(typeof(ArticlesSortField), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");

            var articles = _articlesProcessor.GetFilteredArticles(filter, sortBy, direction, _includeAllFields, TopicFilter).Select(x => new ArticleModel(x));

            ViewBag.topics = _topicsProcessor.GetAllTopics(TopicSortOrder.TopicName, SortDirection.Ascending);
            return SortableListView<ArticlesSortField, ArticlesViewModel, ArticleModel>(articles, sortBy, direction);
        }

        #region Database Scripts

        [HttpGet]
        public ActionResult UrlName()
        {
            return Content(
                "Click on the link to start article UrlNames population.<br /><a href=\"/admin/article/StartUrlNamesUpdating\">Start populating</a>");
        }

        [HttpGet]
        public ActionResult StartUrlNamesUpdating()
        {
            var result = _articlesProcessor.ConvertArticleUrlNames();
            return Content(result);
        }

        [HttpGet]
        public ActionResult HashArticles()
        {
            return Content(
                "Click on the link to start article hashes recalculation.<br /><a href=\"/admin/article/StartHashArticles\">Start hashing</a>");
        }

        [HttpGet]
        public ActionResult StartHashArticles()
        {
            var result = _articlesProcessor.RecalculateIdHashes();
            return Content(result);
        }

        #endregion

        [HttpGet]
        public ActionResult Create()
        {
            var viewModel = new CreateArticleViewModel
            {
                Model = new ArticleModel()
            };

            viewModel.Model.IsEnabled = true;
            viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopic), GetSelectItems());
            viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopic), GetSelectItems());

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateArticleViewModel viewModel, string method)
        {
            viewModel.Model = UpdateTopicObjects(viewModel.Model);
            if (ModelState.IsValid)
            {
                viewModel.Model.ArticleDate = viewModel.Model.ArticleDateNullable.Value; // Already checked in ModelState.IsValid.

                try
                {
                    try
                    {
                        viewModel.Model.DeconstructPublisherWithId();
                        viewModel.Model.DeconstructReporterWithId();
                    }
                    catch (Exception e)
                    {
                        throw new ModelValidationException(e.Message, e);
                    }

                    SetCreatedBy(viewModel.Model);
                    SetUpdatedBy(viewModel.Model);
                    var isUrlUnique = VerifyDuplicatesUrl(viewModel.Model.PublisherUrl);
                    if (isUrlUnique == false)
                    {
                        throw new ModelValidationException("This Publisher URL already exists in the database. Please check for any typos or if other article already has this URL, to avoid producing a duplicate article.");
                    }
                    var newArticleId = _articlesProcessor.CreateArticle(viewModel.Model);

                    if (method.Equals(viewModel.SubmitName_Create, StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Index");

                    if (method.Equals(viewModel.SubmitName_CreateAndSocial, StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("CreatePost", "Social", new { entityType = EntityType.Article, entityId = newArticleId });
                }
                catch (ModelValidationException mve)
                {
                    ModelState.AddModelError("", mve.Message);
                }
            }

            if (viewModel.IsSelectsListEmpty)
            {
                var primaryItems = GetSelectItems(viewModel.Model.PrimaryTopic.Id.Value);
                var secondaryItems = GetSelectItems();

                viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopic), primaryItems);
                viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopic), secondaryItems);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                throw new ContentNotFoundException("Incorrect id of Article.");

            var viewModel = new EditArticleViewModel();

            var article = _articlesProcessor.GetArticleById(id.Value);

            _articlesProcessor.PopulateResourcesMentioned(article);

            if (article == null)
                throw new ContentNotFoundException("The Article with specified id does not exist.");

            viewModel.Model = new ArticleModel(article)
            {
                ArticleDateNullable = article.ArticleDate
            };

            var primaryItems = GetSelectItems(article.PrimaryTopic.Id.Value);
            var secondaryItems = GetSelectItems();

            viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopic), primaryItems);
            viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopic), secondaryItems);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditArticleViewModel viewModel, string method)
        {
            viewModel.Model = UpdateTopicObjects(viewModel.Model);
            if (ModelState.IsValid)
            {
                viewModel.Model.ArticleDate = viewModel.Model.ArticleDateNullable.Value; // Already checked in ModelState.IsValid.

                try
                {
                    viewModel.Model.DeconstructPublisherWithId();
                    viewModel.Model.DeconstructReporterWithId();
                    SetUpdatedBy(viewModel.Model);

                    var isUrlUnique = VerifyDuplicatesUrl(viewModel.Model.PublisherUrl, viewModel.Model.Id);
                    if (isUrlUnique == false)
                    {
                        throw new ModelValidationException("This Publisher URL already exists in the database. Please check for any typos or if other article already has this URL, to avoid producing a duplicate article.");
                    }

                    _articlesProcessor.UpdateArticle(viewModel.Model);

                    if (method.Equals(viewModel.SubmitName_Save, StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("Index");

                    if (method.Equals(viewModel.SubmitName_SaveAndSocial, StringComparison.OrdinalIgnoreCase))
                        return RedirectToAction("CreatePost", "Social", new { entityType = EntityType.Article, entityId = viewModel.Model.Id.Value });
                }
                catch (ModelValidationException mve)
                {
                    ModelState.AddModelError("", mve.Message);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            if (viewModel.IsSelectsListEmpty)
            {
                var primaryItems = GetSelectItems(viewModel.Model.PrimaryTopic.Id.Value);
                var secondaryItems = GetSelectItems();

                viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopic), primaryItems);
                viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopic), secondaryItems);
            }

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id.HasValue == false)
                throw new ContentNotFoundException("Incorrect id of Article");

            _articlesProcessor.DeleteArticle(id.Value);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult MakeUrlName(string rawName)
        {
            var urlName = TextUtils.ConvertToUrlName(rawName);
            return Json(new { urlName = urlName }, JsonRequestBehavior.AllowGet);
        }

        private ArticleModel UpdateTopicObjects(ArticleModel article)
        {
            article.PrimaryTopic = new Topic()
            {
                Id = article.PrimaryTopicID
            };

            article.SecondaryTopic = new Topic()
            {
                Id = article.SecondaryTopicID
            };

            return article;
        }

        private IEnumerable<SelectListItem> GetSelectItems(int selectedId = 0)
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

        #region Created By / Updated By Fields
        private void SetCreatedBy(ArticleModel model)
        {
            var accessUserManager = Request.GetOwinContext().Get<AccessUserManager>();
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.CreatedByUserId = authedUserId;
        }
        private void SetUpdatedBy(ArticleModel model)
        {
            var accessUserManager = Request.GetOwinContext().Get<AccessUserManager>();
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.UpdatedByUserId = authedUserId;
        }
        #endregion

        [HttpGet]
        public ActionResult VerifyDuplicates(string url, int? idToExclude = null)
        {
            bool result = VerifyDuplicatesUrl(url, idToExclude);

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
        }

        public bool VerifyDuplicatesUrl(string url, int? idToExclude = null)
        {
            if (string.IsNullOrEmpty(url))
                return true;
            url = url.UrlFullCleanup();
            var existingArticle = _articlesProcessor.GetArticleByPublisherUrl(url, idToExclude);
            if (existingArticle != null)
                return false;
            return true;
        }
    }
}