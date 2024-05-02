using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class SectionsRepository : AbstractRepository<BookSection>, ISectionsRepository
    {
        public IEnumerable<BookSection> GetAllSections(BookSectionsSortOrder sortOrder, SortDirection sortDirection)
        {
            var sql = $"SELECT bs.BookSectionID as 0, bs.BookSectionName as 1, bs.DisplayOrder as 2, bs.IsIncluded as 3" +
                      $" FROM {TableNameBookSections} bs" +
                      $" ORDER BY bs.{sortOrder.ToString()} {(sortDirection == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var bookSection = MapBookSection(0);
                list.Add(bookSection);
            });
        }

        public IEnumerable<BookSection> GetIncludedSections(BookSectionsSortOrder sortOrder, SortDirection sortDirection)
        {
            var sql = $"SELECT bs.BookSectionID as 0, bs.BookSectionName as 1, bs.DisplayOrder as 2, bs.IsIncluded as 3" +
                     $" FROM {TableNameBookSections} bs" +
                     $" WHERE bs.IsIncluded = TRUE" +
                     $" ORDER BY bs.{sortOrder.ToString()} {(sortDirection == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var bookSection = MapBookSection(0);
                list.Add(bookSection);
            });
        }

        public BookSection GetSectionById(int id)
        {
            var sql = $"SELECT bs.BookSectionID as 0, bs.BookSectionName as 1, bs.DisplayOrder as 2, bs.IsIncluded as 3" +
                      $" FROM {TableNameBookSections} bs" +
                      $" WHERE bs.BookSectionID = {id}";

            return QueryAllData(sql, list =>
            {
                var bookSection = MapBookSection(0);
                list.Add(bookSection);
            }).FirstOrDefault();
        }

        public bool SectionExists(int id)
        {
            return GetSectionById(id) != null;
        }

        public int GetAllSectionsCount()
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = $"SELECT COUNT(*) FROM {TableNameBookSections}";

                var command = MsAccessDbManager.CreateCommand(sql);
                return (int)command.ExecuteScalar();
            }
        }

        public int GetIncludedSectionsCount()
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = $"SELECT COUNT(*) FROM {TableNameBookSections} bs WHERE bs.IsIncluded = TRUE";

                var command = MsAccessDbManager.CreateCommand(sql);
                return (int)command.ExecuteScalar();
            }
        }

        public override void Insert(BookSection entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameBookSections} (BookSectionName, DisplayOrder, IsIncluded) VALUES (@BookSectionName, @DisplayOrder, @IsIncluded)");

                command.Parameters.AddWithValue("@BookSectionName", entity.BookSectionName);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@IsIncluded", entity.IsIncluded);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Section to the database");
            }
        }

        public override void Update(BookSection entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand($"UPDATE {TableNameBookSections} SET BookSectionName = @BookSectionName, DisplayOrder = @DisplayOrder, IsIncluded = @IsIncluded WHERE BookSectionID = {entity.Id}");

                command.Parameters.AddWithValue("@BookSectionName", entity.BookSectionName);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@IsIncluded", entity.IsIncluded);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing Section in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"DELETE FROM {TableNameBookSections} WHERE BookSectionID = @BookSectionID");

                command.Parameters.AddWithValue("@BookSectionID", id);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during Section deleting from the database");
            }
        }

        public override IEnumerable<BookSection> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
