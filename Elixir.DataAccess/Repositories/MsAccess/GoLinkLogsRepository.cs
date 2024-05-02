using System;
using System.Collections.Generic;
using System.Data.OleDb;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class GoLinkLogsRepository : AbstractRepository<GoLinkLog>, IGoLinkLogsRepository
    {
        public override void Insert(GoLinkLog entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameGoLinkLogs} (CreatedDT, GoLinkID, IPAddress) " +
                    $"VALUES (@CreatedDT, @GoLinkID, @IPAddress)");

                command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.Created));
                command.Parameters.AddWithValue("@GoLinkID", entity.GoLinkId);
                command.Parameters.AddWithValue("@IPAddress", SafeGetStringValue(entity.IPAddress));

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving GoLinkLog to the database");
            }
        }

        public override void Update(GoLinkLog entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<GoLinkLog> GetAll()
        {
            var sql = "SELECT gll.GoLinkLogID as 0, gll.CreatedDT as 1, gll.GoLinkID as 2, gll.IPAddress as 3," +
                      " gl.GoLinkID as 4, gl.GoLinkTitle, gl.IsDeleted, gl.ShortCode, gl.DestinationURL, gl.IsBookLink, gl.IsAffiliateLink, gl.IsEnabled," +
                      " gl.NotesInternal, gl.CreatedDT, gl.UpdatedDT" +
                      $" FROM {TableNameGoLinkLogs} gll" +
                      $" LEFT JOIN {TableNameGoLinks} gl ON gl.GoLinkID = gll.GoLinkID";

            return QueryAllData(sql, list =>
            {
                var goLinkLog = MapGoLinkLog(0);
                var goLink = MapGoLink(4);
                goLinkLog.GoLinkTitle = goLink.GoLinkTitle;
                goLinkLog.GoLinkShortCode = goLink.ShortCode;
                list.Add(goLinkLog);
            });
        }

        public IEnumerable<GoLinkLog> GetLogs(int limit)
        {
            var sql = $"SELECT TOP {limit} gll.GoLinkLogID as 0, gll.CreatedDT as 1, gll.GoLinkID as 2, gll.IPAddress as 3," +
                      $" gl.GoLinkID as 4, gl.GoLinkTitle, gl.IsDeleted, gl.ShortCode, gl.DestinationURL, gl.IsBookLink, gl.IsAffiliateLink, gl.IsEnabled," +
                      $" gl.NotesInternal, gl.CreatedDT, gl.UpdatedDT" +
                      $" FROM {TableNameGoLinkLogs} gll" +
                      $" LEFT JOIN {TableNameGoLinks} gl ON gl.GoLinkID = gll.GoLinkID" +
                      $" ORDER BY gll.CreatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var goLinkLog = MapGoLinkLog(0);
                var goLink = MapGoLink(4);
                goLinkLog.GoLinkTitle = goLink.GoLinkTitle;
                goLinkLog.GoLinkShortCode = goLink.ShortCode;
                list.Add(goLinkLog);
            });
        }
    }
}
