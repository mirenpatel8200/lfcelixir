using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface ITopicsProcessor
    {
        IEnumerable<Topic> GetAllTopics(TopicSortOrder sortOrder, SortDirection sortDirection);
        IEnumerable<Topic> Get100Topics(TopicSortOrder sortOrder, SortDirection sortDirection);
        IEnumerable<Topic> GetFilteredTopics(string filter, TopicSortOrder sortOrder = TopicSortOrder.TopicName, SortDirection direction = SortDirection.Ascending,bool includeAllFields=false);
        Topic GetTopicById(int id);
        void CreateTopic(Topic topic);
        void UpdateTopic(Topic model);
        bool TopicExists(Topic topic);
    }
}
