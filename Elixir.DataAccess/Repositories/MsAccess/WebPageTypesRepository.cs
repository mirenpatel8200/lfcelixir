using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class WebPageTypesRepository : AbstractRepository<WebPageType>, IWebPageTypesRepository
    {
        public override IEnumerable<WebPageType> GetAll()
        {
            var sql = $"SELECT wpt.WebPageTypeID as 0, wpt.WebPageTypeName as 1 FROM {TablenameWebPageTypes} wpt";

            return QueryAllData(sql, list =>
            {
                var webPageType = MapWebPageType(0);
                list.Add(webPageType);
            });
        }

        //public WebPageType GetType(int id)
        //{
        //    var sql = $"SELECT wpt.WebPageTypeID as 0, wpt.WebPageTypeName as 1 FROM {TablenameWebPageTypes} wpt" +
        //              $" WHERE wpt.WebPageTypeID = {id}";

        //    return QueryAllData(sql, list =>
        //    {
        //        var webPageType = MapWebPageType(0);
        //        list.Add(webPageType);
        //    }).FirstOrDefault();
        //}

        public override void Insert(WebPageType entity)
        {
            throw new NotImplementedException();
        }
        public override void Update(WebPageType entity)
        {
            throw new NotImplementedException();
        }
        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

    }
}
