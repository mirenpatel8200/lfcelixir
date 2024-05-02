using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces
{
    public interface IResourcesProcessor
    {
        IEnumerable<Resource> Get100Resources(ResourcesSortOrder sortField = ResourcesSortOrder.ResourceID,
            SortDirection sortDirection = SortDirection.Ascending, string filter = "");

        int CreateResource(Resource resource);

        Resource GetResourceById(int id);

        void UpdateResource(Resource resource);

        void DeleteResource(int resourceId);

        IEnumerable<Resource> SearchResource(string term, ResourceTypes resourceTypes = ResourceTypes.Organisation, int? maxCount = null, bool shortMatch = false);

        Resource GetResourceByUrlName(string urlName);

        IEnumerable<Resource> GetChildResources(int parentId, int resourceType);

        IEnumerable<Resource> GetResourcesMentionedInArticle(int articleId);

        IEnumerable<Resource> GetResourcesMentionedInBlogpost(int blogpostId);

        IEnumerable<Resource> GetResourcesMentionedInResource(int resourceId);

        IEnumerable<Resource> GetResourcesMentioningResource(int resourceId);

        //IEnumerable<Resource> GetResourcesByTopicAndMediaType(int primaryTopicId, int? secondaryTopicId, ResourceMediaTypes mediaTypes = null, int count = 10);

        IEnumerable<Resource> GetWebPageReleatedResources(int webPageId, int? maxCount = null, DateTime? resourceUpdatedDate = null);

        Resource GetResourceByExternalUrl(string url, int? idToExclude = null);

        void PopulateResourcesMentioned(Resource resource);

        IEnumerable<Resource> Search(List<string> terms, bool all);
        IEnumerable<Resource> GetEventResources();
        IEnumerable<Resource> GetLatestResources(int limit, ResourcesSortOrder sortField, SortDirection sortDirections);
    }
}
