using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elixir.Models.Enums;
using Elixir.Utils;
using System.Data.OleDb;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class CountryRepository : AbstractRepository<Country>, ICountryRepository
    {
        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override void Insert(Country entity)
        {
            throw new NotImplementedException();
        }
        public override IEnumerable<Country> GetAll()
        {
            var sql = $"SELECT c.CountryAutoID as 0, c.CountryName as 1, c.ContinentCode as 2, c.CountryID as 3" +
                      $" FROM {TableNameCountry} c";

            return QueryAllData(sql, list =>
            {
                var country = MapCountry(0);
                list.Add(country);
            });
        }

        public Country GetCountryById(int id)
        {
            var sql = $"SELECT c.CountryAutoID as 0, c.CountryName as 1, c.ContinentCode as 2, c.CountryID as 3" +
                      $" FROM {TableNameCountry} c" +
                      $" WHERE c.CountryAutoID = {id}";

            return QueryAllData(sql, list =>
            {
                var country = MapCountry(0);
                list.Add(country);
            }).FirstOrDefault();
        }
        public IEnumerable<Country> SearchCountry(string term, int? maxCount = null)
        {
            term = StringUtils.FixSqlLikeClause(term);
            var sql = $"SELECT {(maxCount.HasValue ? "TOP " + maxCount.Value : "")} " +
               $" CountryAutoID, CountryName, ContinentCode, CountryID" +
               $" FROM {TableNameCountry}  where CountryName like '%" + term + "%'";

            sql += " ORDER BY CountryName";
            return QueryAllData(sql, list =>
            {
                var resource = MapCountry(0);
                list.Add(resource);
            });
        }
        public IEnumerable<Country> SearchCountryByName(string term, int? maxCount = null)
        {
            term = StringUtils.FixSqlLikeClause(term);
            var sql = $"SELECT {(maxCount.HasValue ? "TOP " + maxCount.Value : "")} " +
               $" CountryAutoID, CountryName, ContinentCode, CountryID" +
               $" FROM {TableNameCountry}  where CountryName like '" + term + "'";

            sql += " ORDER BY CountryName";
            return QueryAllData(sql, list =>
            {
                var resource = MapCountry(0);
                list.Add(resource);
            });
        }
        public override void Update(Country entity)
        {
            throw new NotImplementedException();
        }
    }
    //    public class CountryRepository : AbstractRepository<Country>, ICountryRepository
    //    {
    //        public IEnumerable<Country> SearchCountry(string term, int? maxCount = null);
    //        {
    //            term = StringUtils.FixSqlLikeClause(term);


    //            var sql = $"SELECT {(maxCount.HasValue ? "TOP " + maxCount.Value : "")} ResourceID, ResourceName, IsDeleted, ExternalUrl, FacebookHandle, TwitterHandle, ResourceTypeId," +
    //               $" CountryID, CountryName, ContinentCode" +
    //               $" FROM {TableNameCountry}  ";



    //        sql += " ORDER BY ResourceName";

    //        return QueryAllData(sql, list =>
    //        {
    //            var resource = MapResource(0);
    //            list.Add(resource);
    //        });
    //    }

    //    }

    //}
}
