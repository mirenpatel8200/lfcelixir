using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class WebPageXTopicRepository : AbstractRepository<WebPageXTopic>, IWebPageXTopicRepository
    {
        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<WebPageXTopic> GetAll()
        {
            //var sql = $"SELECT wpxt.WebPageTopicId as 0, wpxt.TopicId as 1, wpxt.WebPageId as 2" +
            //   $" FROM {TableNameWebPageTopic} wpxt";

            //return QueryAllData(sql, list =>
            //{
            //    var webPageXTopic = MapWebPageXTopic(0);
            //    list.Add(webPageXTopic);
            //});
            throw new NotImplementedException();
        }

        public override void Insert(WebPageXTopic entity)
        {
            throw new NotImplementedException();
        }

        //public bool IsWebPageLinkedToTopic(int webPageId, int topicId)
        //{
        //    var sql = $"SELECT wpxt.WebPageTopicId as 0, wpxt.TopicId as 1, wpxt.WebPageId as 2" +
        //       $" FROM {TableNameWebPageTopic} wpxt" +
        //       $" WHERE wpxt.WebPageId = {webPageId} AND wpxt.TopicId = {topicId}";

        //    var data = QueryAllData(sql, list =>
        //    {
        //        var webPageXTopic = MapWebPageXTopic(0);
        //        list.Add(webPageXTopic);
        //    }).FirstOrDefault();
        //    return data != null;
        //}

        public override void Update(WebPageXTopic entity)
        {
            throw new NotImplementedException();
        }

        //public IEnumerable<WebPageXTopic> GetWebPageXTopicByWebPage(int webPageId)
        //{
        //    var sql = $"SELECT wpxt.WebPageTopicId as 0, wpxt.TopicId as 1, wpxt.WebPageId as 2" +
        //       $" FROM {TableNameWebPageTopic} wpxt" +
        //       $" WHERE wpxt.WebPageId = {webPageId}";

        //    return QueryAllData(sql, list =>
        //    {
        //        var webPageXTopic = MapWebPageXTopic(0);
        //        list.Add(webPageXTopic);
        //    });
        //}

        //public IEnumerable<WebPageXTopic> GetWebPageXTopicByTopic(int topicId)
        //{
        //    var sql = $"SELECT wpxt.WebPageTopicId as 0, wpxt.TopicId as 1, wpxt.WebPageId as 2" +
        //       $" FROM {TableNameWebPageTopic} wpxt" +
        //       $" WHERE wpxt.TopicId = {topicId}";

        //    return QueryAllData(sql, list =>
        //    {
        //        var webPageXTopic = MapWebPageXTopic(0);
        //        list.Add(webPageXTopic);
        //    });
        //}

        //public WebPageXTopic GetWebPageXTopicById(int id)
        //{
        //    var sql = $"SELECT wpxt.WebPageTopicId as 0, wpxt.TopicId as 1, wpxt.WebPageId as 2" +
        //       $" FROM {TableNameWebPageTopic} wpxt" +
        //       $" WHERE wpxt.WebPageTopicId = {id}";

        //    return QueryAllData(sql, list =>
        //    {
        //        var webPageXTopic = MapWebPageXTopic(0);
        //        list.Add(webPageXTopic);
        //    }).FirstOrDefault();
        //}
    }
}
