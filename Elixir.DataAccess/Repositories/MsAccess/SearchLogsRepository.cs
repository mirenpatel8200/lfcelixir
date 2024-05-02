using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class SearchLogsRepository : AbstractRepository<SearchLog>, ISearchLogsRepository
    {
        public override void Insert(SearchLog entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameSearchLogs} (CreatedDateTime, IPAddress, Search, WordCount) " +
                    $"VALUES (@CreatedDateTime, @IPAddress, @Search, @WordCount)");

                command.Parameters.AddWithValue("@CreatedDateTime", GetDateWithoutMilliseconds(entity.Created));
                command.Parameters.AddWithValue("@IPAddress", entity.IPAddress);
                command.Parameters.AddWithValue("@Search", SafeGetStringValue(entity.Search));
                command.Parameters.AddWithValue("@WordCount", entity.WordCount);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving SearchLog to the database");
            }
        }

        public override void Update(SearchLog entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<SearchLog> GetAll()
        {
            var sql =
                "SELECT sl.SearchLogID as 0, sl.CreatedDateTime as 1, sl.IPAddress as 3," +
                " sl.Search as 4, sl.WordCount as 5" +
                $" FROM {TableNameSearchLogs} sl";

            return QueryAllData(sql, list =>
            {
                var searchLog = MapSearchLog(0);
                
                list.Add(searchLog);
            });
        }

        public IEnumerable<SearchLog> GetLogs(int limit)
        {
            var sql = $"SELECT TOP {limit} sl.SearchLogID as 0, sl.CreatedDateTime as 1, sl.IPAddress as 3," +
                      $" sl.Search as 4, sl.WordCount as 5" +
                      $" FROM {TableNameSearchLogs} sl" +
                      $" ORDER BY sl.CreatedDateTime DESC";

            return QueryAllData(sql, list =>
            {
                var searchLog = MapSearchLog(0);
                list.Add(searchLog);
            });
        }
    }
}
