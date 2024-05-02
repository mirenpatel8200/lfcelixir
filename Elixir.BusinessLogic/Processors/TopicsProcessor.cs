using System.Collections.Generic;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.BusinessLogic.Processors
{
    public class TopicsProcessor : ITopicsProcessor
    {
        private readonly ITopicsRepository _topicsRepository;

        public TopicsProcessor(ITopicsRepository topicsRepository)
        {
            _topicsRepository = topicsRepository;
        }

        private IEnumerable<Topic> GetTopics(TopicSortOrder sortField, SortDirection sortDirection, int? count)
        {
            var sortFields = new[] { sortField, TopicSortOrder.TopicID };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };

            var allBookSections = count == null
                ? _topicsRepository.GetAll(sortFields, sortDirections)
                : _topicsRepository.GetN(count.Value, sortFields, sortDirections);

            #region Sorting ?
            //if (sortDirection == SortDirection.Ascending)
            //{
            //    switch (sortField)
            //    {
            //        case TopicSortOrder.Id:
            //            allBookSections = allBookSections.OrderBy(x => x.Id);
            //            break;
            //        case TopicSortOrder.Name:
            //            allBookSections = allBookSections.OrderBy(x => x.TopicName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortField), sortField, null);
            //    }
            //}
            //else if (sortDirection == SortDirection.Descending)
            //{
            //    switch (sortField)
            //    {
            //        case TopicSortOrder.Id:
            //            allBookSections = allBookSections.OrderByDescending(x => x.Id);
            //            break;
            //        case TopicSortOrder.Name:
            //            allBookSections = allBookSections.OrderByDescending(x => x.TopicName);
            //            break;
            //        default:
            //            throw new ArgumentOutOfRangeException(nameof(sortField), sortField, null);
            //    }
            //}
            #endregion

            return allBookSections;
        }
        public IEnumerable<Topic> GetFilteredTopics(string filter, TopicSortOrder sortField, SortDirection sortDirection, bool includeAllFields)
        {
            var sortFields = new[] { sortField, TopicSortOrder.TopicName };
            var sortDirections = new[] { sortDirection, SortDirection.Ascending };
            IEnumerable<Topic> topics;
            if (string.IsNullOrEmpty(filter))
            {
                topics = Get100Topics(sortField, sortDirection);
            }
            else
            {
                topics = _topicsRepository.GetFiltered(filter, sortFields, sortDirections, includeAllFields);
            }

            return topics;
        }

        public IEnumerable<Topic> GetAllTopics(TopicSortOrder sortOrder, SortDirection sortDirection) => GetTopics(sortOrder, sortDirection, null);

        public IEnumerable<Topic> Get100Topics(TopicSortOrder sortOrder, SortDirection sortDirection) => GetTopics(sortOrder, sortDirection, 100);

        public Topic GetTopicById(int id)
        {
            return _topicsRepository.GetById(id);
        }

        public void CreateTopic(Topic topic)
        {
            _topicsRepository.Insert(topic);
        }

        public void UpdateTopic(Topic model)
        {
            _topicsRepository.Update(model);
        }

        public bool TopicExists(Topic topic)
        {
            var foundTopic = _topicsRepository.GetByName(topic.TopicName);

            return foundTopic != null && topic.Id != foundTopic.Id;
        }
    }
}
