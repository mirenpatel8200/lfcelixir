using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface ITopicsRepository : IRepository<Topic>
    {
        Topic GetById(int id);
        Topic GetByName(string topicName);
        IEnumerable<Topic> GetN(int count, TopicSortOrder[] sortFields, SortDirection[] sortDirections);
        IEnumerable<Topic> GetAll(TopicSortOrder[] sortFields, SortDirection[] sortDirections);
        IEnumerable<Topic> GetFiltered(string filter, TopicSortOrder[] sortFields, SortDirection[] sortDirections, bool includeAllFields);
    }
}
