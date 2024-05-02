using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.BusinessLogic.Processors
{
    public class WebPageXTopicsProcessor : IWebPageXTopicProcessor
    {
        private IWebPageXTopicRepository _webPageXTopicRepository;
        public WebPageXTopicsProcessor(IWebPageXTopicRepository repository)
        {
            _webPageXTopicRepository = repository;
        }

        //public void CreateWebPageXTopic(WebPageXTopic wpxt)
        //{
        //    _webPageXTopicRepository.Insert(wpxt);
        //}

        //public void DeleteWebPageXTopic(int id)
        //{
        //    _webPageXTopicRepository.Delete(id);
        //}

        //public IEnumerable<WebPageXTopic> GetAllWebPageXTopic()
        //{
        //    return _webPageXTopicRepository.GetAll();
        //}

        //public WebPageXTopic GetWebPageXTopicById(int id)
        //{
        //    return _webPageXTopicRepository.GetWebPageXTopicById(id);
        //}

        //public IEnumerable<WebPageXTopic> GetWebPageXTopicByTopic(int topicId)
        //{
        //    return _webPageXTopicRepository.GetWebPageXTopicByTopic(topicId);
        //}

        //public IEnumerable<WebPageXTopic> GetWebPageXTopicByWebPage(int webpageId, bool includeChildPagesTopics)
        //{
        //    return _webPageXTopicRepository.GetWebPageXTopicByWebPage(webpageId);
        //}

        //public void UpdateWebPageXTopic(WebPageXTopic wpxt)
        //{
        //    _webPageXTopicRepository.Update(wpxt);
        //}
    }
}
