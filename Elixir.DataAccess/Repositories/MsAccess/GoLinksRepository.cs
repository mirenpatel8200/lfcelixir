using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class GoLinksRepository : AbstractRepository<GoLink>, IGoLinksRepository
    {
        public override void Insert(GoLink entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameGoLinks} (GoLinkTitle, IsDeleted, ShortCode, DestinationURL, IsBookLink, IsAffiliateLink, IsEnabled, NotesInternal, CreatedDT, UpdatedDT) " +
                    $"VALUES (@GoLinkTitle, @IsDeleted, @ShortCode, @DestinationURL, @IsBookLink, @IsAffiliateLink, @IsEnabled, @NotesInternal, @CreatedDT, @UpdatedDT)");

                command.Parameters.AddWithValue("@GoLinkTitle", SafeGetStringValue(entity.GoLinkTitle));
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@ShortCode", SafeGetStringValue(entity.ShortCode));
                command.Parameters.AddWithValue("@DestinationURL", SafeGetStringValue(entity.DestinationUrl));
                command.Parameters.AddWithValue("@IsBookLink", entity.IsBookLink);
                command.Parameters.AddWithValue("@IsAffiliateLink", entity.IsAffiliateLink);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);

                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.Created));
                command.Parameters.AddWithValue("@UpdatedDT", entity.Updated.HasValue ? GetDateWithoutMilliseconds(entity.Updated.Value) : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving GoLink to the database");
            }
        }

        public override void Update(GoLink entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameGoLinks} SET GoLinkTitle = @GoLinkTitle, IsDeleted = @IsDeleted, ShortCode = @ShortCode," +
                        $" DestinationURL = @DestinationURL, IsBookLink = @IsBookLink, IsAffiliateLink = @IsAffiliateLink," +
                        $" IsEnabled = @IsEnabled, NotesInternal = @NotesInternal, UpdatedDT = @UpdatedDT" +
                    $" WHERE GoLinkID = {entity.Id}");

                command.Parameters.AddWithValue("@GoLinkTitle", SafeGetStringValue(entity.GoLinkTitle));
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@ShortCode", SafeGetStringValue(entity.ShortCode));
                command.Parameters.AddWithValue("@DestinationURL", SafeGetStringValue(entity.DestinationUrl));
                command.Parameters.AddWithValue("@IsBookLink", entity.IsBookLink);
                command.Parameters.AddWithValue("@IsAffiliateLink", entity.IsAffiliateLink);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                //command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.Created));
                command.Parameters.AddWithValue("@UpdatedDT", entity.Updated.HasValue ? GetDateWithoutMilliseconds(entity.Updated.Value) : (object)DBNull.Value);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing GoLink in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameGoLinks} SET IsDeleted = 1 WHERE GoLinkID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop product in the database");
            }
        }

        public override IEnumerable<GoLink> GetAll()
        {
            var sql = $"SELECT gl.GoLinkID as 0, gl.GoLinkTitle as 1, gl.IsDeleted as 2, gl.ShortCode as 3," +
                      $" gl.DestinationURL as 4, gl.IsBookLink as 5, gl.IsAffiliateLink as 6, gl.IsEnabled as 7," +
                      $" gl.NotesInternal as 8, gl.CreatedDT as 9, gl.UpdatedDT as 10" +
                      $" FROM {TableNameGoLinks} gl";
            return QueryAllData(sql, list =>
            {
                var goLink = MapGoLink(0);
                list.Add(goLink);
            });
        }

        public IEnumerable<GoLink> GetN(int count, GoLinksSortOrder[] sortFields, SortDirection[] sortDirections)
        {
            var sql = $"SELECT TOP {count} gl.GoLinkID as 0, gl.GoLinkTitle as 1, gl.IsDeleted as 2, gl.ShortCode as 3," +
                      $" gl.DestinationURL as 4, gl.IsBookLink as 5, gl.IsAffiliateLink as 6, gl.IsEnabled as 7," +
                      $" gl.NotesInternal as 8, gl.CreatedDT as 9, gl.UpdatedDT as 10" +
                      $" FROM {TableNameGoLinks} gl" +
                      $" WHERE gl.IsDeleted = FALSE";

            var tableNames = Enumerable.Repeat("gl", sortFields.Length);
            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()), sortDirections, tableNames);

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, list =>
            {
                var goLink = MapGoLink(0);
                list.Add(goLink);
            });
        }

        public GoLink GetByShortUrl(string shortUrl)
        {
            var sql = $"SELECT gl.GoLinkID as 0, gl.GoLinkTitle as 1, gl.IsDeleted as 2, gl.ShortCode as 3," +
                      $" gl.DestinationURL as 4, gl.IsBookLink as 5, gl.IsAffiliateLink as 6, gl.IsEnabled as 7," +
                      $" gl.NotesInternal as 8, gl.CreatedDT as 9, gl.UpdatedDT as 10" +
                      $" FROM {TableNameGoLinks} gl" +
                      $" WHERE gl.IsDeleted = FALSE AND LCase(gl.ShortCode) = @ShortCode";
            return QueryAllData(sql, list =>
            {
                var goLink = MapGoLink(0);
                list.Add(goLink);
            }, new Dictionary<string, object>() { { "ShortCode", shortUrl.ToLower() } }).FirstOrDefault();
        }

        public GoLink GetById(int id)
        {
            var sql = $"SELECT gl.GoLinkID as 0, gl.GoLinkTitle as 1, gl.IsDeleted as 2, gl.ShortCode as 3," +
                      $" gl.DestinationURL as 4, gl.IsBookLink as 5, gl.IsAffiliateLink as 6, gl.IsEnabled as 7," +
                      $" gl.NotesInternal as 8, gl.CreatedDT as 9, gl.UpdatedDT as 10" +
                      $" FROM {TableNameGoLinks} gl" +
                      $" WHERE gl.GoLinkID = {id}";
            return QueryAllData(sql, list =>
            {
                var goLink = MapGoLink(0);
                list.Add(goLink);
            }).FirstOrDefault();
        }

        public bool IsNonDeletedGoLinkExists(string shortCode, int? excludeGoLinkId)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = excludeGoLinkId.HasValue
                     ? $"SELECT COUNT(*) FROM {TableNameGoLinks} gl WHERE gl.GoLinkID <> @ExcludeId AND gl.IsDeleted = 0 AND LCase(gl.ShortCode) = @ShortCode"
                     : $"SELECT COUNT(*) FROM {TableNameGoLinks} gl WHERE gl.IsDeleted = 0 AND LCase(gl.ShortCode) = @ShortCode";

                var command = MsAccessDbManager.CreateCommand(sql);

                if (excludeGoLinkId.HasValue)
                    command.Parameters.AddWithValue("@ExcludeId", excludeGoLinkId.Value);
                command.Parameters.AddWithValue("@ShortCode", shortCode);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
