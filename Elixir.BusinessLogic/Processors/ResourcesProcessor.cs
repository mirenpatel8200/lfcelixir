using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;
using System.Linq;
using Elixir.BusinessLogic.Core.Utils;

namespace Elixir.BusinessLogic.Processors
{
    public class ResourcesProcessor : IResourcesProcessor
    {
        private readonly IResourcesRepository _resourcesRepository;

        public ResourcesProcessor(IResourcesRepository resourcesRepository)
        {
            _resourcesRepository = resourcesRepository;
        }

        public int CreateResource(Resource resource)
        {
            var timeOfAdd = DateTime.Now;
            resource.CreatedDT = timeOfAdd;
            resource.UpdatedDT = timeOfAdd;
            resource.RemoveSocialMediaLeadingSymbols();
            resource.ExternalUrl = resource.ExternalUrl.StripTrackingParameters();
            resource.ResourceName = resource.ResourceName.Trim();

            _resourcesRepository.Insert(resource);

            //var added = _resourcesRepository.GetAll().FirstOrDefault(x =>
            //    x.ResourceName.Equals(resource.ResourceName) &&
            //    DateEqualsRoughly(x.CreatedDT, timeOfAdd));

            var added = GetResourceByUrlName(resource.UrlName);

            if (added == null)
                throw new InvalidOperationException("Unable to find Resource that is supposed to be added.");

            UpdateMentionedResources(added.Id.Value, resource.OrgsMentionedString, ResourceTypes.Organisation.ToDatabaseValues().First());
            UpdateMentionedResources(added.Id.Value, resource.PeopleMentionedString, ResourceTypes.Person.ToDatabaseValues().First());
            UpdateMentionedResources(added.Id.Value, resource.CreationsMentionedString, ResourceTypes.Creation.ToDatabaseValues().First());

            return added.Id.Value;
        }

        //private bool DateEqualsRoughly(DateTime a, DateTime b)
        //{
        //    return a.Date.Equals(b.Date) &&
        //        a.Hour.Equals(b.Hour) &&
        //        a.Minute.Equals(b.Minute) &&
        //        a.Second.Equals(b.Second);
        //}

        public void DeleteResource(int resourceId)
        {
            _resourcesRepository.Delete(resourceId);
        }

        public IEnumerable<Resource> SearchResource(string term, ResourceTypes resourceTypes, int? maxCount = null, bool shortMatch = false)
        {
            if (string.IsNullOrEmpty(term))
                term = "";
            return _resourcesRepository.SearchResource(term, resourceTypes, maxCount, shortMatch);
        }

        public IEnumerable<Resource> Search(List<string> terms, bool all = false)
        {
            return _resourcesRepository.Search(terms, all);
        }

        public IEnumerable<Resource> Get100Resources(ResourcesSortOrder sortField,
            SortDirection sortDirection,
            string filter)
        {
            var sortFields = new[] { sortField, ResourcesSortOrder.ResourceID };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };

            return _resourcesRepository.GetN(100, sortFields, sortDirections, filter);

            //if (sortField == ResourcesSortOrder.ResourceID)
            //{
            //    list = (sortDirection == SortDirection.Ascending) ?
            //        list.OrderBy(res => res.Id) :
            //        list.OrderByDescending(res => res.Id);
            //}
            //else if (sortField == ResourcesSortOrder.ResourceName)
            //{
            //    list = (sortDirection == SortDirection.Ascending) ?
            //        list.OrderBy(res => res.ResourceName) :
            //        list.OrderByDescending(res => res.ResourceName);
            //}

            //return list;
        }

        public Resource GetResourceById(int id)
        {
            return _resourcesRepository.GetResourceById(id);
        }

        public void UpdateResource(Resource resource)
        {
            resource.UpdatedDT = DateTime.Now;
            resource.RemoveSocialMediaLeadingSymbols();
            resource.ExternalUrl = resource.ExternalUrl.StripTrackingParameters();
            resource.ResourceName = resource.ResourceName.Trim();

            _resourcesRepository.Update(resource);

            if (resource.IsOrgsMentionedChanged)
            {
                UpdateMentionedResources(resource.Id.Value, resource.OrgsMentionedString, ResourceTypes.Organisation.ToDatabaseValues().First());
            }
            if (resource.IsPeopleMentionedChanged)
            {
                UpdateMentionedResources(resource.Id.Value, resource.PeopleMentionedString, ResourceTypes.Person.ToDatabaseValues().First());
            }
            if (resource.IsCreationsMentionedChanged)
            {
                UpdateMentionedResources(resource.Id.Value, resource.CreationsMentionedString, ResourceTypes.Creation.ToDatabaseValues().First());
            }
        }

        public Resource GetResourceByUrlName(string urlName)
        {
            return _resourcesRepository.GetResourceByUrlName(urlName);
            //if (resource != null && resource.IsEnabled && !resource.IsDeleted &&
            //    !resource.IsHiddenPublic)
            //{
            //    return resource;
            //}

            //return null;
        }

        public IEnumerable<Resource> GetChildResources(int parentId, int resourceType)
        {
            return _resourcesRepository.GetChildResources(parentId, resourceType);
        }

        public IEnumerable<Resource> GetResourcesMentionedInArticle(int articleId)
        {
            return _resourcesRepository.GetMentionedResources(articleId);
        }

        public IEnumerable<Resource> GetResourcesMentionedInBlogpost(int blogpostId)
        {
            return _resourcesRepository.GetMentionedResources(blogpostId, rootEntity: EntityType.BlogPost);
        }

        //public IEnumerable<Resource> GetResourcesByTopicAndMediaType(int primaryTopicId, int? secondaryTopicId, ResourceMediaTypes mediaTypes = null, int count = 10)
        //{
        //    return _resourcesRepository.GetResourcesByTopicsAndMediaType(primaryTopicId, secondaryTopicId, mediaTypes, count);
        //}

        public IEnumerable<Resource> GetWebPageReleatedResources(int webPageId, int? maxCount = null, DateTime? resourceUpdatedDate = null)
        {
            return _resourcesRepository.GetWebPageReleatedResources(webPageId, maxCount, resourceUpdatedDate);
        }

        public Resource GetResourceByExternalUrl(string url, int? idToExclude)
        {
            return _resourcesRepository.GetResourceByExternalUrl(url, idToExclude);
        }

        private void UpdateMentionedResources(int resourceId, string resourcesMentioned, int resourceType)
        {
            _resourcesRepository.DeleteMentionedResources(resourceId, resourceType);

            int rId;
            if (string.IsNullOrEmpty(resourcesMentioned))
                return;

            var resourceTags = resourcesMentioned.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var resourceIds = new List<int>();
            for (int i = 0; i < resourceTags.Length; i++)
            {
                rId = ExtractIdFromResourceTag(resourceTags[i]);
                if (rId != -1 && resourceIds.Contains(rId) == false)
                    resourceIds.Add(rId);
            }

            _resourcesRepository.InsertMentionedResources(resourceId, resourceIds, resourceType);
        }

        //public void PopulateResourcesMentioned(Article article)
        //{
        //    var organisationsMentioned = _resourcesRepository.GetMentionedResources(
        //        article.Id.Value, ResourceTypes.Organisation.ToDatabaseValues().First()).
        //        OrderBy(o => o.MentionedOrder);
        //    article.OrgsMentionedString = organisationsMentioned.ToTagsFormat();

        //    var peopleMentioned = _resourcesRepository.GetMentionedResources(
        //        article.Id.Value, ResourceTypes.Person.ToDatabaseValues().First()).
        //        OrderBy(p => p.MentionedOrder);
        //    article.PeopleMentionedString = peopleMentioned.ToTagsFormat();

        //    var creationsMentioned = _resourcesRepository.GetMentionedResources(
        //        article.Id.Value, ResourceTypes.Creation.ToDatabaseValues().First()).
        //        OrderBy(c => c.MentionedOrder);
        //    article.CreationsMentionedString = creationsMentioned.ToTagsFormat();
        //}


        private int ExtractIdFromResourceTag(string tagText)
        {
            int resourceId, beginId, endId;
            beginId = tagText.IndexOf("[");
            endId = tagText.IndexOf("]");
            if (beginId == -1 || endId == -1) return -1;
            string idString = tagText.Substring(beginId + 1,
                endId - beginId - 1);

            if (int.TryParse(idString, out resourceId) == true)
                return resourceId;

            return -1;
        }

        public void PopulateResourcesMentioned(Resource resource)
        {
            var organisationsMentioned = _resourcesRepository.GetMentionedResources(
                resource.Id.Value, ResourceTypes.Organisation.ToDatabaseValues().First(),
                EntityType.Resource).
                OrderBy(o => o.MentionedOrder);
            resource.OrgsMentionedString = organisationsMentioned.ToTagsFormat();

            var peopleMentioned = _resourcesRepository.GetMentionedResources(
                resource.Id.Value, ResourceTypes.Person.ToDatabaseValues().First(),
                EntityType.Resource).
                OrderBy(p => p.MentionedOrder);
            resource.PeopleMentionedString = peopleMentioned.ToTagsFormat();

            var creationsMentioned = _resourcesRepository.GetMentionedResources(
                resource.Id.Value, ResourceTypes.Creation.ToDatabaseValues().First(),
                EntityType.Resource).
                OrderBy(c => c.MentionedOrder);
            resource.CreationsMentionedString = creationsMentioned.ToTagsFormat();

        }

        public IEnumerable<Resource> GetResourcesMentionedInResource(int resourceId)
        {
            return _resourcesRepository.GetMentionedResources(
                resourceId, null, EntityType.Resource);
        }

        public IEnumerable<Resource> GetResourcesMentioningResource(int resourceId)
        {
            return _resourcesRepository.GetResourcesMentioningResource(resourceId);
        }

        public IEnumerable<Resource> GetEventResources()
        {
            return _resourcesRepository.GetEventResources();
        }

        public IEnumerable<Resource> GetLatestResources(int limit, ResourcesSortOrder sortField, SortDirection sortDirections)
        {
            return _resourcesRepository.GetLatestResources(limit, sortField, sortDirections);
        }
    }
}
