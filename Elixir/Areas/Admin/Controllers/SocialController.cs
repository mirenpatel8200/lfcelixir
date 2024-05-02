using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Attributes;
using Elixir.BusinessLogic.Exceptions;
using Elixir.BusinessLogic.Processors.SocialPosts;
using Elixir.Contracts.Interfaces;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.Utils.View;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor, Roles.Writer)]
    public class SocialController : Controller
    {
        private readonly IArticlesProcessor _articlesProcessor;
        private readonly ISocialPostsProcessor _socialPostsProcessor;
        private readonly ITopicsProcessor _topicsProcessor;
        private readonly IBlogPostsProcessor _blogPostsProcessor;
        private readonly IResourcesProcessor _resourcesProcessor;

        public SocialController(IArticlesProcessor articlesProcessor,
            ISocialPostsProcessor socialPostsProcessor,
            ITopicsProcessor topicsProcessor,
            IBlogPostsProcessor blogPostsProcessor,
            IResourcesProcessor resourcesProcessor)
        {
            _articlesProcessor = articlesProcessor;
            _socialPostsProcessor = socialPostsProcessor;
            _topicsProcessor = topicsProcessor;
            _blogPostsProcessor = blogPostsProcessor;
            _resourcesProcessor = resourcesProcessor;
        }

        // GET: Admin/Social
        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreatePost(EntityType? entityType, int? entityId)
        {
            var viewModel = new CreatePostViewModel();
            var vm = (SocialPostModel)TempData["spm"];
            if (vm != null)
            {
                viewModel.Model = vm;
                TempData.Remove("spm");
                if (viewModel.IsSelectsListEmpty)
                {
                    IEnumerable<SelectListItem> primaryItems;
                    if (viewModel.Model.PrimaryTopic?.Id != null)
                        primaryItems = GetSelectItems(viewModel.Model.PrimaryTopic.Id.Value);
                    else
                        primaryItems = GetSelectItems();
                    var secondaryItems = GetSelectItems();

                    viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopic), primaryItems);
                    viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopic), secondaryItems);
                }
                return View(viewModel);
            }
            else
            {
                viewModel.Model = new SocialPostModel();
                if (entityType.HasValue && entityId.HasValue)
                {
                    var model = new SocialPostModel();
                    if (entityType.Value == EntityType.Article)
                    {
                        var article = _articlesProcessor.GetArticleById(entityId.Value);
                        _articlesProcessor.PopulateResourcesMentioned(article);
                        if (article == null)
                            throw new ContentNotFoundException("Unable to find article with specified id.");

                        model = new SocialPostModel
                        {
                            EntityType = EntityType.Article,
                            EntityTypeName = EntityType.Article.ToString(),
                            EntityId = article.Id.Value,
                            FirstLine = article.Title,
                            LastLine = article.Summary,
                            DnPublisherName = article.DnPublisherName,
                            KeyOrganisationId = article.PublisherResourceId,
                            KeyOrganisationWithId = ViewUtils.FormatAutocompleteResource(article.DnPublisherName, article.PublisherResourceId),
                            IsSuffixKeyOrganisation = true,
                            DnReporterName = article.DnReporterName,
                            KeyPersonId = article.ReporterResourceId,
                            KeyPersonWithId = ViewUtils.FormatAutocompleteResource(article.DnReporterName, article.ReporterResourceId),
                            OrgsMentioned = article.OrgsMentioned,
                            OrgsMentionedString = article.OrgsMentionedString,
                            PeopleMentioned = article.PeopleMentioned,
                            PeopleMentionedString = article.PeopleMentionedString,
                            CreationsMentioned = article.CreationsMentioned,
                            CreationsMentionedString = article.CreationsMentionedString,
                            IsOrgsMentionedChanged = article.IsOrgsMentionedChanged,
                            IsPeopleMentionedChanged = article.IsPeopleMentionedChanged,
                            IsCreationsMentionedChanged = article.IsCreationsMentionedChanged,
                            PrimaryTopic = article.PrimaryTopic,
                            SecondaryTopic = article.SecondaryTopic,
                            TopicHashtags = $" {string.Join(" ", article.GetHashtags())}",
                            SocialPostDate = article.ArticleDate,
                            ImagePreview = article.GetOgImageName(),
                            BackUrl = Url.Action("Index", "Article", new { area = "Admin" })
                        };
                        var currentUrl = Request.Url.AbsoluteUri;
                        var authority = Request.Url.Authority;
                        var articleHost = currentUrl.Substring(0, currentUrl.IndexOf(authority) + authority.Length);
                        var articleQuery = Url.Action("Index", "PublicArticle", new { area = "", name = article.UrlName });
                        model.UrlName = articleHost + articleQuery;
                    }
                    else if (entityType.Value == EntityType.BlogPost)
                    {
                        var blog = _blogPostsProcessor.GetById(entityId.Value);
                        _blogPostsProcessor.PopulateResourcesMentioned(blog);
                        if (blog == null)
                            throw new ContentNotFoundException("Unable to find blog with specified id.");

                        model = new SocialPostModel
                        {
                            EntityType = EntityType.BlogPost,
                            EntityTypeName = EntityType.BlogPost.ToString(),
                            EntityId = blog.Id.Value,
                            FirstLine = blog.BlogPostTitle,
                            LastLine = blog.BlogPostDescriptionPublic,
                            OrgsMentioned = blog.OrgsMentioned,
                            OrgsMentionedString = blog.OrgsMentionedString,
                            PeopleMentioned = blog.PeopleMentioned,
                            PeopleMentionedString = blog.PeopleMentionedString,
                            CreationsMentioned = blog.CreationsMentioned,
                            CreationsMentionedString = blog.CreationsMentionedString,
                            IsOrgsMentionedChanged = blog.IsOrgsMentionedChanged,
                            IsPeopleMentionedChanged = blog.IsPeopleMentionedChanged,
                            IsCreationsMentionedChanged = blog.IsCreationsMentionedChanged,
                            PrimaryTopic = blog.PrimaryTopic,
                            SecondaryTopic = blog.SecondaryTopic,
                            TopicHashtags = $" {string.Join(" ", blog.GetHashtags())}",
                            SocialPostDate = blog.UpdatedDt,
                            ImagePreview = blog.GetOgImageName(),
                            BackUrl = Url.Action("Index", "Blog", new { area = "Admin" })
                        };
                        var currentUrl = Request.Url.AbsoluteUri;
                        var authority = Request.Url.Authority;
                        var articleHost = currentUrl.Substring(0, currentUrl.IndexOf(authority) + authority.Length);
                        var articleQuery = Url.Action("ViewBlog", "PublicBlog", new { area = "", name = blog.UrlName });
                        model.UrlName = articleHost + articleQuery;
                    }
                    else if (entityType.Value == EntityType.Resource)
                    {
                        var resource = _resourcesProcessor.GetResourceById(entityId.Value);
                        _resourcesProcessor.PopulateResourcesMentioned(resource);
                        if (resource == null)
                            throw new ContentNotFoundException("Unable to find resource with specified id.");

                        model = new SocialPostModel
                        {
                            EntityType = EntityType.Resource,
                            EntityTypeName = EntityType.Resource.ToString(),
                            EntityId = resource.Id.Value,
                            FirstLine = resource.ResourceName,
                            LastLine = resource.ResourceDescriptionPublic,
                            OrgsMentioned = resource.OrgsMentioned,
                            OrgsMentionedString = resource.OrgsMentionedString,
                            PeopleMentioned = resource.PeopleMentioned,
                            PeopleMentionedString = resource.PeopleMentionedString,
                            CreationsMentioned = resource.CreationsMentioned,
                            CreationsMentionedString = resource.CreationsMentionedString,
                            IsOrgsMentionedChanged = resource.IsOrgsMentionedChanged,
                            IsPeopleMentionedChanged = resource.IsPeopleMentionedChanged,
                            IsCreationsMentionedChanged = resource.IsCreationsMentionedChanged,
                            PrimaryTopic = resource.PrimaryTopic,
                            SecondaryTopic = resource.SecondaryTopic,
                            TopicHashtags = $" {string.Join(" ", resource.GetHashtags())}",
                            SocialPostDate = resource.UpdatedDT,
                            ImagePreview = resource.GetOgImageName(),
                            BackUrl = Url.Action("Index", "Resource", new { area = "Admin" })
                        };

                        if (resource.ResourceTypeId == ResourceTypes.Organisation.ToDatabaseValues().First())
                        {
                            model.DnPublisherName = resource.ResourceName;
                            model.KeyOrganisationId = resource.Id;
                            model.KeyOrganisationWithId = ViewUtils.FormatAutocompleteResource(model.DnPublisherName, model.KeyOrganisationId);
                            model.IsSuffixKeyOrganisation = true;
                        }
                        else if (resource.ResourceTypeId == ResourceTypes.Person.ToDatabaseValues().First())
                        {
                            model.DnReporterName = resource.ResourceName;
                            model.KeyPersonId = resource.Id;
                            model.KeyPersonWithId = ViewUtils.FormatAutocompleteResource(model.DnReporterName, model.KeyPersonId);
                        }
                        else if (resource.ResourceTypeId == ResourceTypes.Creation.ToDatabaseValues().First())
                        {
                            model.DnCreationName = resource.ResourceName;
                            model.KeyCreationId = resource.Id;
                            model.KeyCreationWithId = ViewUtils.FormatAutocompleteResource(model.DnCreationName, model.KeyCreationId);
                        }

                        if (resource.ParentResourceID.HasValue)
                        {
                            var parentResource = _resourcesProcessor.GetResourceById(resource.ParentResourceID.Value);
                            if (parentResource != null)
                            {
                                var parentResourceString = $"{parentResource.ResourceName} [{parentResource.Id}]";
                                if (parentResource.ResourceTypeId == ResourceTypes.Organisation.ToDatabaseValues().First())
                                {
                                    if (!string.IsNullOrEmpty(model.OrgsMentionedString))
                                        model.OrgsMentionedString = $"{model.OrgsMentionedString}|{parentResourceString}";
                                    else
                                        model.OrgsMentionedString = parentResourceString;
                                }
                                else if (parentResource.ResourceTypeId == ResourceTypes.Person.ToDatabaseValues().First())
                                {
                                    if (!string.IsNullOrEmpty(model.PeopleMentionedString))
                                        model.PeopleMentionedString = $"{model.PeopleMentionedString}|{parentResourceString}";
                                    else
                                        model.PeopleMentionedString = parentResourceString;
                                }
                                else if (parentResource.ResourceTypeId == ResourceTypes.Creation.ToDatabaseValues().First())
                                {
                                    if (!string.IsNullOrEmpty(model.CreationsMentionedString))
                                        model.CreationsMentionedString = $"{model.CreationsMentionedString}|{parentResourceString}";
                                    else
                                        model.CreationsMentionedString = parentResourceString;
                                }
                            }
                        }

                        var currentUrl = Request.Url.AbsoluteUri;
                        var authority = Request.Url.Authority;
                        var articleHost = currentUrl.Substring(0, currentUrl.IndexOf(authority) + authority.Length);
                        var articleQuery = Url.Action("Index", "Resources", new { area = "", name = resource.UrlName });
                        model.UrlName = articleHost + articleQuery;
                    }

                    if (model.PrimaryTopic?.Id != null)
                        model.PrimaryTopicID = model.PrimaryTopic.Id.Value;

                    if (model.SecondaryTopic?.Id != null)
                        model.SecondaryTopicID = model.SecondaryTopic.Id.Value;

                    // TODO: fix triple call of ComposeSocialPost - it queries same resource record 3 times.
                    model.PostFacebook = _socialPostsProcessor.ComposeSocialPost(SocialNetwork.Facebook, model);
                    model.PostLinkedIn = _socialPostsProcessor.ComposeSocialPost(SocialNetwork.LinkedIn, model);
                    model.PostTwitter = _socialPostsProcessor.ComposeSocialPost(SocialNetwork.Twitter, model);

                    viewModel.Model = model;
                    IEnumerable<SelectListItem> primaryItems;
                    if (model.PrimaryTopic?.Id != null)
                        primaryItems = GetSelectItems(model.PrimaryTopic.Id.Value);
                    else
                        primaryItems = GetSelectItems();
                    viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopic), primaryItems);
                }
                else
                {
                    viewModel.Model.EntityTypeName = "N/A";
                    viewModel.Model.BackUrl = Url.Action("Dashboard", "Social", new { area = "Admin" });
                    var primaryItems = GetSelectItems();
                    viewModel.AddSelectList(nameof(viewModel.Model.PrimaryTopic), primaryItems);
                }
                var secondaryItems = GetSelectItems();
                viewModel.AddSelectList(nameof(viewModel.Model.SecondaryTopic), secondaryItems);
                return View(viewModel);
            }
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

        [HttpPost]
        public ActionResult CreatePost(CreatePostViewModel vm)
        {
            vm.Model.PrimaryTopic = new Topic()
            {
                Id = vm.Model.PrimaryTopicID
            };

            vm.Model.SecondaryTopic = new Topic()
            {
                Id = vm.Model.SecondaryTopicID
            };

            try
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(vm.Model.FirstLine))
                        vm.Model.FirstLine = vm.Model.FirstLine.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.OtherContent))
                        vm.Model.OtherContent = vm.Model.OtherContent.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.LastLine))
                        vm.Model.LastLine = vm.Model.LastLine.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.KeyOrganisationWithId))
                        vm.Model.KeyOrganisationWithId = vm.Model.KeyOrganisationWithId.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.KeyPersonWithId))
                        vm.Model.KeyPersonWithId = vm.Model.KeyPersonWithId.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.TopicHashtags))
                        vm.Model.TopicHashtags = vm.Model.TopicHashtags.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.OmitHashtags))
                        vm.Model.OmitHashtags = vm.Model.OmitHashtags.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.ExtraHashtags))
                        vm.Model.ExtraHashtags = vm.Model.ExtraHashtags.Trim();
                    if (!string.IsNullOrEmpty(vm.Model.UrlName))
                        vm.Model.UrlName = vm.Model.UrlName.Trim();

                    if (string.IsNullOrWhiteSpace(vm.Model.KeyOrganisationWithId))
                    {
                        vm.Model.KeyOrganisationId = null;
                        vm.Model.DnPublisherName = null;
                    }
                    else
                    {
                        var parsed = ViewUtils.ParseAutocompleteResource(vm.Model.KeyOrganisationWithId);

                        vm.Model.KeyOrganisationId = parsed.ResourceId;
                        vm.Model.DnPublisherName = parsed.ResourceName;
                    }

                    if (string.IsNullOrWhiteSpace(vm.Model.KeyPersonWithId))
                    {
                        vm.Model.KeyPersonId = null;
                        vm.Model.DnReporterName = null;
                    }
                    else
                    {
                        var parsed = ViewUtils.ParseAutocompleteResource(vm.Model.KeyPersonWithId);

                        vm.Model.KeyPersonId = parsed.ResourceId;
                        vm.Model.DnReporterName = parsed.ResourceName;
                    }

                    if (string.IsNullOrWhiteSpace(vm.Model.KeyCreationWithId))
                    {
                        vm.Model.KeyCreationId = null;
                        vm.Model.DnCreationName = null;
                    }
                    else
                    {
                        var parsed = ViewUtils.ParseAutocompleteResource(vm.Model.KeyCreationWithId);

                        vm.Model.KeyCreationId = parsed.ResourceId;
                        vm.Model.DnCreationName = parsed.ResourceName;
                    }

                    if (!string.IsNullOrEmpty(vm.Model.OrgsMentionedString))
                        vm.Model.OrgsMentionedString = vm.Model.OrgsMentionedString.Trim(new char[] { '|' });
                    if (!string.IsNullOrEmpty(vm.Model.PeopleMentionedString))
                        vm.Model.PeopleMentionedString = vm.Model.PeopleMentionedString.Trim(new char[] { '|' });
                    if (!string.IsNullOrEmpty(vm.Model.CreationsMentionedString))
                        vm.Model.CreationsMentionedString = vm.Model.CreationsMentionedString.Trim(new char[] { '|' });

                    // TODO: fix triple call of ComposeSocialPost - it queries same resource record 3 times.
                    vm.Model.PostFacebook = _socialPostsProcessor.ComposeSocialPost(SocialNetwork.Facebook, vm.Model);
                    vm.Model.PostLinkedIn = _socialPostsProcessor.ComposeSocialPost(SocialNetwork.LinkedIn, vm.Model);
                    vm.Model.PostTwitter = _socialPostsProcessor.ComposeSocialPost(SocialNetwork.Twitter, vm.Model);
                    TempData["spm"] = vm.Model;
                    if (vm.Model.EntityId.HasValue)
                        return RedirectToAction("CreatePost", "Social", new { entityType = vm.Model.EntityType, entityId = vm.Model.EntityId.Value });
                    else
                        return RedirectToAction("CreatePost", "Social");
                }
            }
            catch (ModelValidationException mve)
            {
                ModelState.AddModelError("", mve.Message);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }
            if (vm.IsSelectsListEmpty)
            {
                IEnumerable<SelectListItem> primaryItems;
                if (vm.Model.PrimaryTopic?.Id != null)
                    primaryItems = GetSelectItems(vm.Model.PrimaryTopic.Id.Value);
                else
                    primaryItems = GetSelectItems();
                var secondaryItems = GetSelectItems();

                vm.AddSelectList(nameof(vm.Model.PrimaryTopic), primaryItems);
                vm.AddSelectList(nameof(vm.Model.SecondaryTopic), secondaryItems);
            }
            return View(vm);
        }

        [HttpGet]
        public string GetTopicHashTags(int? primaryTopicId, int? secondaryTopicId)
        {
            try
            {
                Topic primaryTopic = null;
                Topic secondaryTopic = null;
                if (primaryTopicId.HasValue)
                {
                    primaryTopic = _topicsProcessor.GetTopicById((int)primaryTopicId);
                }
                if (secondaryTopicId.HasValue)
                {
                    secondaryTopic = _topicsProcessor.GetTopicById((int)secondaryTopicId);
                }
                var hashtags = GetHashtags(primaryTopic, secondaryTopic);
                return $" {string.Join(" ", hashtags)}";
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string[] GetHashtags(Topic primaryTopic, Topic secondaryTopic)
        {
            var hashtags = new List<string>();
            var separator = new[] { " " };

            if (primaryTopic != null && !string.IsNullOrWhiteSpace(primaryTopic.Hashtags))
                hashtags.AddRange(primaryTopic.Hashtags.Split(separator, StringSplitOptions.RemoveEmptyEntries));
            if (secondaryTopic != null && !string.IsNullOrWhiteSpace(secondaryTopic.Hashtags))
                hashtags.AddRange(secondaryTopic.Hashtags.Split(separator, StringSplitOptions.RemoveEmptyEntries));

            return hashtags.ToArray();
        }
    }
}