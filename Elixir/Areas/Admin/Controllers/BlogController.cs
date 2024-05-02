using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Attributes;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Json;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor, Roles.Blogger)]
    public class BlogController : BaseController
    {
        private readonly IBlogPostsProcessor _blogPostsProcessor;
        private readonly ITopicsProcessor _topicsProcessor;

        public BlogController(IBlogPostsProcessor blogPostsProcessor, ITopicsProcessor topicsProcessor)
        {
            _blogPostsProcessor = blogPostsProcessor;
            _topicsProcessor = topicsProcessor;
        }

        // GET: Admin/Blog
        public ActionResult Index(AdminBlogPostsSortOrder sortBy = AdminBlogPostsSortOrder.BlogPostID, SortDirection direction = SortDirection.Descending)
        {
            if (!Enum.IsDefined(typeof(AdminBlogPostsSortOrder), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");

            var blogs = _blogPostsProcessor.Get100Posts(sortBy, direction).Select(x => new AdminBlogPostModel(x));

            return SortableListView<AdminBlogPostsSortOrder, AdminBlogPostsViewModel, AdminBlogPostModel>(blogs, sortBy, direction);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var moment = DateTime.Now;
            var nextBlogPost = _blogPostsProcessor.FindNextBlogPost(moment);
            var prevBlogPost = _blogPostsProcessor.FindPrevBlogPost(moment);

            var vm = new CreateBlogPostViewModel()
            {
                Model = new AdminBlogPostModel()
                {
                    IsEnabled = true,
                    NextBlogPostUrlName = nextBlogPost?.UrlName,
                    NextBlogPostTitle = nextBlogPost?.BlogPostTitle,
                    PreviousBlogPostUrlName = prevBlogPost?.UrlName,
                    PreviousBlogPostTitle = prevBlogPost?.BlogPostTitle,
                    PrimaryTopicId = 0,
                    SecondaryTopicId = 0,
                    PublishedOnDT = DateTime.Now
                }
            };

            vm.AddSelectList(nameof(vm.Model.PrimaryTopic), GetSelectItems());
            vm.AddSelectList(nameof(vm.Model.SecondaryTopic), GetSelectItems());

            return View(vm);
        }

        [HttpPost]
        public ActionResult Create(CreateBlogPostViewModel vm)
        {
            vm.AddSelectList(nameof(vm.Model.PrimaryTopic), GetSelectItems());
            vm.AddSelectList(nameof(vm.Model.SecondaryTopic), GetSelectItems());

            if (ModelState.IsValid)
            {
                if (vm.Model.PrimaryTopicId.GetValueOrDefault() == 0)
                {
                    ModelState.AddModelError($"{nameof(vm.Model)}.{nameof(vm.Model.PrimaryTopicId)}", "Primary Topic is required.");
                    return View(vm);
                }

                var blogPost = new BlogPost()
                {
                    PreviousBlogPostUrlName = vm.Model.PreviousBlogPostUrlName,
                    PreviousBlogPostTitle = vm.Model.PreviousBlogPostTitle,
                    NextBlogPostUrlName = vm.Model.NextBlogPostUrlName,
                    NextBlogPostTitle = vm.Model.NextBlogPostTitle,
                    BlogPostTitle = vm.Model.BlogPostTitle,
                    ContentMain = vm.Model.ContentMain,
                    UrlName = vm.Model.UrlName,
                    IsEnabled = vm.Model.IsEnabled,
                    PrimaryTopicId = vm.Model.PrimaryTopicId,
                    SecondaryTopicId = vm.Model.SecondaryTopicId,
                    NotesInternal = vm.Model.NotesInternal,
                    PublishedOnDT = vm.Model.PublishedOnDT,
                    SocialImageFilename = vm.Model.SocialImageFilename,
                    OrgsMentioned = vm.Model.OrgsMentioned,
                    OrgsMentionedString = vm.Model.OrgsMentionedString,
                    PeopleMentioned = vm.Model.PeopleMentioned,
                    PeopleMentionedString = vm.Model.PeopleMentionedString,
                    CreationsMentioned = vm.Model.CreationsMentioned,
                    CreationsMentionedString = vm.Model.CreationsMentionedString,
                    BlogPostDescriptionPublic = vm.Model.BlogPostDescriptionPublic,
                    ThumbnailImageFilename=vm.Model.ThumbnailImageFilename
                };

                try
                {
                    _blogPostsProcessor.CreateBlogPost(blogPost);
                    return RedirectToAction("Index");
                }
                catch (ModelValidationException mve)
                {
                    ModelState.AddModelError("", mve.Message);
                }
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var blogPost = _blogPostsProcessor.GetById(id);
            _blogPostsProcessor.PopulateResourcesMentioned(blogPost);
            if (blogPost.IsDeleted)
                throw new InvalidOperationException("Unable to edit deleted record.");

            var nextBlogPost = _blogPostsProcessor.FindRelatedPost(blogPost, RelatedBlogType.Next);
            var prevBlogPost = _blogPostsProcessor.FindRelatedPost(blogPost, RelatedBlogType.Prev);

            var vm = new EditBlogPostViewModel()
            {
                Model = new AdminBlogPostModel(blogPost)
                {
                    NextBlogPostUrlName = nextBlogPost?.UrlName,
                    NextBlogPostTitle = nextBlogPost?.BlogPostTitle,
                    PreviousBlogPostUrlName = prevBlogPost?.UrlName,
                    PreviousBlogPostTitle = prevBlogPost?.BlogPostTitle
                }
            };

            vm.AddSelectList(nameof(vm.Model.PrimaryTopic), GetSelectItems(blogPost.PrimaryTopicId.GetValueOrDefault()));
            vm.AddSelectList(nameof(vm.Model.SecondaryTopic), GetSelectItems(blogPost.SecondaryTopicId.GetValueOrDefault()));

            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit(EditBlogPostViewModel vm)
        {
            vm.AddSelectList(nameof(vm.Model.PrimaryTopic), GetSelectItems());
            vm.AddSelectList(nameof(vm.Model.SecondaryTopic), GetSelectItems(vm.Model.SecondaryTopicId.GetValueOrDefault()));

            if (ModelState.IsValid)
            {
                if (vm.Model.PrimaryTopicId.GetValueOrDefault() == 0)
                {
                    ModelState.AddModelError($"{nameof(vm.Model)}.{nameof(vm.Model.PrimaryTopicId)}", "Primary Topic is required.");
                    return View(vm);
                }
                try
                {
                    _blogPostsProcessor.UpdateBlogPost(vm.Model, vm.Model.IsSignificantChange);

                    return RedirectToAction("Index");
                }
                catch (ModelValidationException mve)
                {
                    ModelState.AddModelError("", mve.Message);
                }
            }

            return View(vm);
        }

        public ActionResult Delete(int id)
        {
            _blogPostsProcessor.DeleteBlogPost(id);

            return RedirectToAction("Index");
        }

        public ActionResult PopulateUrlNamesAndTitles()
        {
            JsonActionResult res;

            try
            {
                var count = _blogPostsProcessor.PopulateUrlNamesAndTitles();
                res = new JsonActionResult(true, $"URL names and Titles of {count} blog posts were populated successfully.");
            }
            catch (Exception e)
            {
                res = new JsonActionResult(false, e.Message);
            }

            return Json(res, JsonRequestBehavior.AllowGet);
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

            result.Insert(0, new SelectListItem { Text = "None", Value = "0" });

            var itemToSelect = result.FirstOrDefault(x => x.Value.Equals(selectedId.ToString()));

            if (itemToSelect != null)
                itemToSelect.Selected = true;

            return result;
        }
    }
}