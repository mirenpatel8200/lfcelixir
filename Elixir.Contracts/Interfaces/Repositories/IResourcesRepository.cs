using Elixir.Models;
using System.Collections.Generic;
using Elixir.Models.Enums;
using System;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IResourcesRepository : IRepository<Resource>
    {
        Resource GetResourceById(int id);

        IEnumerable<Resource> GetN(int count, ResourcesSortOrder[] sortFields, SortDirection[] sortDirections,
            string filter);
        IEnumerable<Resource> SearchResource(string term, ResourceTypes resourceTypes, int? maxCount = null, bool shortMatch = false);

        IEnumerable<Resource> GetMentionedResources(int articleId, int? resourceType = null, EntityType rootEntity = EntityType.Article);

        Resource GetResourceByUrlName(string urlName);

        IEnumerable<Resource> GetChildResources(int parentId, int resourceType);

        //IEnumerable<Resource> GetResourcesByTopicsAndMediaType(int primaryTopicId, int? secondaryTopicId, ResourceMediaTypes mediaTypes, int count = 10);

        IEnumerable<Resource> GetWebPageReleatedResources(int webPageId, int? maxCount, DateTime? resourceUpdatedDate);

        Resource GetResourceByExternalUrl(string url, int? idToExclude = null);

        void DeleteMentionedResources(int resourceId, int resourceType);

        void InsertMentionedResources(int resourceId, List<int> resourceIds, int resourceType);

        IEnumerable<Resource> GetResourcesMentioningResource(int resourceId);

        IEnumerable<Resource> Search(List<string> terms, bool all);
        IEnumerable<Resource> GetEventResources();
        IEnumerable<Resource> GetLatestResources(int limit, ResourcesSortOrder sortField, SortDirection sortDirections);
    }
}
