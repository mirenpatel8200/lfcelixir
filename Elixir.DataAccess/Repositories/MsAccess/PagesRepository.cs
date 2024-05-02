using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class PagesRepository : AbstractRepository<BookPage>, IPagesRepository
    {
        public override void Insert(BookPage entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameBookPages} (BookPageName, BookSectionID, DisplayOrder, LifeExtension40, IsIncluded, Notes, Author, Status, BookPageDescription, Cost, Difficulty, ImageFilename, Tips, Resources, ResearchPapers) " +
                    $"VALUES (@BookPageName, @BookSectionID, @DisplayOrder, @LifeExtension40, @IsIncluded, @Notes, @Author, @Status, @BookPageDescription, @Cost, @Difficulty, @ImageFilename, @Tips, @Resources, @ResearchPapers)");

                command.Parameters.AddWithValue("@BookPageName", entity.BookPageName);
                command.Parameters.AddWithValue("@BookSectionID", entity.BookSection.Id);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@LifeExtension40", entity.LifeExtension40);
                command.Parameters.AddWithValue("@IsIncluded", entity.IsIncluded);
                command.Parameters.AddWithValue("@Notes", entity.Notes);
                command.Parameters.AddWithValue("@Author", SafeGetStringValue(entity.Author));
                command.Parameters.AddWithValue("@Status", SafeGetStringValue(entity.Status));
                command.Parameters.AddWithValue("@BookPageDescription", SafeGetStringValue(entity.BookPageDescription));

                if (entity.Cost.HasValue)
                    command.Parameters.AddWithValue("@Cost", entity.Cost);
                else
                    command.Parameters.AddWithValue("@Cost", DBNull.Value);

                if (entity.Difficulty.HasValue)
                    command.Parameters.AddWithValue("@Difficulty", entity.Difficulty.Value);
                else
                    command.Parameters.AddWithValue("@Difficulty", DBNull.Value);

                command.Parameters.AddWithValue("@ImageFilename", SafeGetStringValue(entity.ImageFilename));
                command.Parameters.AddWithValue("@Tips", SafeGetStringValue(entity.Tips));
                command.Parameters.AddWithValue("@Resources", SafeGetStringValue(entity.Resources));
                command.Parameters.AddWithValue("@ResearchPapers", SafeGetStringValue(entity.ResearchPapers));

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Page to the database");
            }
        }

        public override void Update(BookPage entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameBookPages} SET BookPageName = @BookPageName, BookSectionID = @BookSectionID, DisplayOrder = @DisplayOrder," +
                    "LifeExtension40 = @LifeExtension40, IsIncluded = @IsIncluded, Notes = @Notes, " +
                    "Author = @Author, Status = @Status, BookPageDescription = @BookPageDescription, Cost = @Cost, " +
                    "Difficulty = @Difficulty, ImageFilename = @ImageFilename, " +
                    "Tips = @Tips, Resources = @Resources, ResearchPapers = @ResearchPapers " +
                    $"WHERE BookPageID = {entity.Id}");

                command.Parameters.AddWithValue("@BookPageName", entity.BookPageName);
                command.Parameters.AddWithValue("@BookSectionID", entity.BookSection.Id);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@LifeExtension40", entity.LifeExtension40);
                command.Parameters.AddWithValue("@IsIncluded", entity.IsIncluded);
                command.Parameters.AddWithValue("@Notes", entity.Notes);
                command.Parameters.AddWithValue("@Author", SafeGetStringValue(entity.Author));
                command.Parameters.AddWithValue("@Status", SafeGetStringValue(entity.Status));
                command.Parameters.AddWithValue("@BookPageDescription", SafeGetStringValue(entity.BookPageDescription));

                if (entity.Cost.HasValue)
                    command.Parameters.AddWithValue("@Cost", entity.Cost);
                else
                    command.Parameters.AddWithValue("@Cost", DBNull.Value);

                if (entity.Difficulty.HasValue)
                    command.Parameters.AddWithValue("@Difficulty", entity.Difficulty.Value);
                else
                    command.Parameters.AddWithValue("@Difficulty", DBNull.Value);

                command.Parameters.AddWithValue("@ImageFilename", SafeGetStringValue(entity.ImageFilename));
                command.Parameters.AddWithValue("@Tips", SafeGetStringValue(entity.Tips));
                command.Parameters.AddWithValue("@Resources", SafeGetStringValue(entity.Resources));
                command.Parameters.AddWithValue("@ResearchPapers", SafeGetStringValue(entity.ResearchPapers));

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
                    $"DELETE FROM {TableNameBookPages} WHERE BookPageID = @BookPageID");

                command.Parameters.AddWithValue("@BookPageID", id);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during Section deleting from the database");
            }
        }

        public override IEnumerable<BookPage> GetAll()
        {
            var sql = $"SELECT bp.BookPageID as 0, bp.BookPageName as 1, bp.BookSectionID as 2, bp.DisplayOrder as 3," +
                $" bp.IsIncluded as 4, bp.BookPageDescription as 5, bp.LifeExtension40 as 6, bp.Cost as 7, bp.Difficulty as 8," +
                $" bp.ImageFilename as 9, bp.Status as 10, bp.Notes as 11, bp.Author as 12, bp.Tips as 13, bp.Resources as 14," +
                $" bp.ResearchPapers as 15, bp.PageFirst as 16, bp.PageLast as 17, bs.BookSectionID as 18," +
                $" bs.BookSectionName as 19, bs.DisplayOrder as 20, bs.IsIncluded as 21" +
                $" FROM (({TableNameBookPages} bp)" +
                $" LEFT JOIN {TableNameBookSections} bs ON bs.BookSectionID = bp.BookSectionID)";

            return QueryAllData(sql, list =>
            {
                var bookPage = MapBookPage(0);
                if (GetTableValue<int?>(2).HasValue)
                {
                    bookPage.BookSection = MapBookSection(18);
                }
                list.Add(bookPage);
            });

            //using (MsAccessDbManager = new MsAccessDbManager())
            //{
            //    DataReader = MsAccessDbManager.CreateCommand(
            //        $"SELECT p.BookPageID, p.BookPageName, s.BookSectionID, "+
            //        "s.BookSectionName, s.DisplayOrder, s.IsIncluded, p.DisplayOrder, " + 
            //        "p.LifeExtension40, p.IsIncluded, p.Notes, p.Author, p.Status, " +
            //        $"p.BookPageDescription, p.ImageFilename, p.Cost, "+

            //        "p.Difficulty, p.Tips, p.Resources, p.ResearchPapers, p.PageFirst " + 
            //        $"FROM {TableNameBookPages} p LEFT JOIN {TableNameBookSections} s ON " +
            //        $"p.BookSectionID = s.BookSectionID").ExecuteReader();

            //    while (DataReader.Read())
            //    {
            //        int pId = GetTableValue<int>(0);
            //        String pPageName = GetTableValue<String>(1);
            //        int? sId = GetTableValue<int?>(2); 
            //        String sSectionName = GetTableValue<String>(3);
            //        int? sDisplayOrder = GetTableValue<int?>(4);    
            //        bool? sIsIncluded = GetTableValue<bool?>(5); 
            //        int pDisplayOrder = GetTableValue<int>(6); 
            //        int pLifeExtension40 = GetTableValue<int>(7);
            //        bool pIsIncluded = GetTableValue<bool>(8);
            //        String pNotes = GetTableValue<String>( 9);
            //        String pDescription = GetTableValue<String>(12);
            //        String pImageFilename = GetTableValue<String>(13);
            //        int? pCost = GetTableValue<int?>(14);

            //        int? pDifficulty = GetTableValue<int?>(15);
            //        string pTips = GetTableValue<String>(16);
            //        string pResources = GetTableValue<String>(17);
            //        string pResearchPapers = GetTableValue<String>(18);
            //        int? pageFirst = GetTableValue<int>(19);

            //        BookPage bookPage = new BookPage(pPageName)
            //        {
            //            Id = pId,
            //            BookPageName = pPageName,
            //            DisplayOrder = pDisplayOrder,
            //            LifeExtension40 = pLifeExtension40,
            //            IsIncluded = pIsIncluded,
            //            Notes = pNotes,
            //            Author = GetTableValue<String>(10),
            //            Status = GetTableValue<String>(11),
            //            BookPageDescription = pDescription,
            //            ImageFilename = pImageFilename,
            //            Cost = pCost,
            //            Difficulty = pDifficulty,
            //            Tips = pTips, 
            //            Resources = pResources,
            //            ResearchPapers = pResearchPapers,
            //            PageFirst = pageFirst
            //        };

            //        if (sId.HasValue && sDisplayOrder.HasValue && sIsIncluded.HasValue)
            //        {
            //            BookSection bookSection = new BookSection(sSectionName)
            //            {
            //                Id = sId,
            //                BookSectionName = sSectionName,
            //                DisplayOrder = sDisplayOrder.Value,
            //                IsIncluded = sIsIncluded.Value
            //            };

            //            bookPage.BookSection = bookSection;
            //        }

            //        yield return bookPage;
            //    }
            //}
        }

        public IEnumerable<BookPage> GetAllPages(BookPagesSortOrder sortOrder, SortDirection sortDirection)
        {
            var sql = $"SELECT bp.BookPageID as 0, bp.BookPageName as 1, bp.BookSectionID as 2, bp.DisplayOrder as 3," +
               $" bp.IsIncluded as 4, bp.BookPageDescription as 5, bp.LifeExtension40 as 6, bp.Cost as 7, bp.Difficulty as 8," +
               $" bp.ImageFilename as 9, bp.Status as 10, bp.Notes as 11, bp.Author as 12, bp.Tips as 13, bp.Resources as 14," +
               $" bp.ResearchPapers as 15, bp.PageFirst as 16, bp.PageLast as 17, bs.BookSectionID as 18," +
               $" bs.BookSectionName as 19, bs.DisplayOrder as 20, bs.IsIncluded as 21" +
               $" FROM (({TableNameBookPages} bp)" +
               $" LEFT JOIN {TableNameBookSections} bs ON bs.BookSectionID = bp.BookSectionID)" +
               $" ORDER BY bp.{sortOrder.ToString()} {(sortDirection == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var bookPage = MapBookPage(0);
                if (GetTableValue<int?>(2).HasValue)
                {
                    bookPage.BookSection = MapBookSection(18);
                }
                list.Add(bookPage);
            });
            //return GetAll();
        }

        public IEnumerable<BookPage> GetIncludedPages(BookPagesSortOrder sortOrder, SortDirection sortDirection)
        {
            var sql = $"SELECT bp.BookPageID as 0, bp.BookPageName as 1, bp.BookSectionID as 2, bp.DisplayOrder as 3," +
               $" bp.IsIncluded as 4, bp.BookPageDescription as 5, bp.LifeExtension40 as 6, bp.Cost as 7, bp.Difficulty as 8," +
               $" bp.ImageFilename as 9, bp.Status as 10, bp.Notes as 11, bp.Author as 12, bp.Tips as 13, bp.Resources as 14," +
               $" bp.ResearchPapers as 15, bp.PageFirst as 16, bp.PageLast as 17, bs.BookSectionID as 18," +
               $" bs.BookSectionName as 19, bs.DisplayOrder as 20, bs.IsIncluded as 21" +
               $" FROM (({TableNameBookPages} bp)" +
               $" LEFT JOIN {TableNameBookSections} bs ON bs.BookSectionID = bp.BookSectionID)" +
               $" WHERE bp.IsIncluded = TRUE" +
               $" ORDER BY bp.{sortOrder.ToString()} {(sortDirection == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var bookPage = MapBookPage(0);
                if (GetTableValue<int?>(2).HasValue)
                {
                    bookPage.BookSection = MapBookSection(18);
                }
                list.Add(bookPage);
            });
            //return GetAll().Where(x => x.IsIncluded == true);
        }

        public BookPage GetPageById(int id)
        {
            var sql = $"SELECT bp.BookPageID as 0, bp.BookPageName as 1, bp.BookSectionID as 2, bp.DisplayOrder as 3," +
               $" bp.IsIncluded as 4, bp.BookPageDescription as 5, bp.LifeExtension40 as 6, bp.Cost as 7, bp.Difficulty as 8," +
               $" bp.ImageFilename as 9, bp.Status as 10, bp.Notes as 11, bp.Author as 12, bp.Tips as 13, bp.Resources as 14," +
               $" bp.ResearchPapers as 15, bp.PageFirst as 16, bp.PageLast as 17, bs.BookSectionID as 18," +
               $" bs.BookSectionName as 19, bs.DisplayOrder as 20, bs.IsIncluded as 21" +
               $" FROM (({TableNameBookPages} bp)" +
               $" LEFT JOIN {TableNameBookSections} bs ON bs.BookSectionID = bp.BookSectionID)" +
               $" WHERE bp.BookPageID = {id}";

            return QueryAllData(sql, list =>
            {
                var bookPage = MapBookPage(0);
                if (GetTableValue<int?>(2).HasValue)
                {
                    bookPage.BookSection = MapBookSection(18);
                }
                list.Add(bookPage);
            }).FirstOrDefault();
            //TODO: redo in SQL to be more efficient
            //return GetAll().FirstOrDefault(p => p.Id == id);
        }

        public void SetPageFirstPageLastPage(int id, int firstPage, int lastPage)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameBookPages} SET PageFirst = @PageFirst, PageLast = @PageLast " +
                    $"WHERE BookPageID = {id}");

                command.Parameters.AddWithValue("@PageFirst", firstPage);
                command.Parameters.AddWithValue("@PageLast", lastPage);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("SetPageFirstPageLastPage: Error during editing Page in the database");
            }
        }

        public BookPage GetPageByManualPageNumber(int pageNumber)
        {
            var sql = $"SELECT bp.BookPageID as 0, bp.BookPageName as 1, bp.BookSectionID as 2, bp.DisplayOrder as 3," +
               $" bp.IsIncluded as 4, bp.BookPageDescription as 5, bp.LifeExtension40 as 6, bp.Cost as 7, bp.Difficulty as 8," +
               $" bp.ImageFilename as 9, bp.Status as 10, bp.Notes as 11, bp.Author as 12, bp.Tips as 13, bp.Resources as 14," +
               $" bp.ResearchPapers as 15, bp.PageFirst as 16, bp.PageLast as 17, bs.BookSectionID as 18," +
               $" bs.BookSectionName as 19, bs.DisplayOrder as 20, bs.IsIncluded as 21" +
               $" FROM (({TableNameBookPages} bp)" +
               $" LEFT JOIN {TableNameBookSections} bs ON bs.BookSectionID = bp.BookSectionID)" +
               $" WHERE bp.PageFirst = {pageNumber}";

            return QueryAllData(sql, list =>
            {
                var bookPage = MapBookPage(0);
                if (GetTableValue<int?>(2).HasValue)
                {
                    bookPage.BookSection = MapBookSection(18);
                }
                list.Add(bookPage);
            }).FirstOrDefault();
            //using (MsAccessDbManager = new MsAccessDbManager())
            //{
            //    DataReader = MsAccessDbManager
            //        .CreateCommand(
            //            "SELECT p.BookPageID, p.BookPageName, s.BookSectionID, s.BookSectionName, s.DisplayOrder, s.IsIncluded, " +
            //            "p.DisplayOrder, p.LifeExtension40, p.IsIncluded, p.Notes, p.Author, p.Status, p.BookPageDescription, " +
            //            "p.ImageFilename, p.Cost, p.Difficulty, p.Tips, p.Resources, p.ResearchPapers, p.PageFirst " +
            //            $"FROM {TableNameBookPages} p LEFT JOIN {TableNameBookSections} s ON p.BookSectionID = s.BookSectionID " +
            //            $"WHERE p.PageFirst = {pageNumber}")
            //        .ExecuteReader();
            //    BookPage bookPage = null;

            //    while (DataReader.Read())
            //    {
            //        int pId = GetTableValue<int>(0);
            //        String pPageName = GetTableValue<String>(1);
            //        int? sId = GetTableValue<int?>(2);
            //        String sSectionName = GetTableValue<String>(3);
            //        int? sDisplayOrder = GetTableValue<int?>(4);
            //        bool? sIsIncluded = GetTableValue<bool?>(5);
            //        int pDisplayOrder = GetTableValue<int>(6);
            //        int pLifeExtension40 = GetTableValue<int>(7);
            //        bool pIsIncluded = GetTableValue<bool>(8);
            //        String pNotes = GetTableValue<String>(9);
            //        String pDescription = GetTableValue<String>(12);
            //        String pImageFilename = GetTableValue<String>(13);
            //        int? pCost = GetTableValue<int?>(14);

            //        int? pDifficulty = GetTableValue<int?>(15);
            //        string pTips = GetTableValue<String>(16);
            //        string pResources = GetTableValue<String>(17);
            //        string pResearchPapers = GetTableValue<String>(18);
            //        int? pageFirst = GetTableValue<int>(19);

            //        bookPage = new BookPage(pPageName)
            //        {
            //            Id = pId,
            //            BookPageName = pPageName,
            //            DisplayOrder = pDisplayOrder,
            //            LifeExtension40 = pLifeExtension40,
            //            IsIncluded = pIsIncluded,
            //            Notes = pNotes,
            //            Author = GetTableValue<String>(10),
            //            Status = GetTableValue<String>(11),
            //            BookPageDescription = pDescription,
            //            ImageFilename = pImageFilename,
            //            Cost = pCost,
            //            Difficulty = pDifficulty,
            //            Tips = pTips,
            //            Resources = pResources,
            //            ResearchPapers = pResearchPapers,
            //            PageFirst = pageFirst
            //        };

            //        if (sId.HasValue && sDisplayOrder.HasValue && sIsIncluded.HasValue)
            //        {
            //            BookSection bookSection = new BookSection(sSectionName)
            //            {
            //                Id = sId,
            //                BookSectionName = sSectionName,
            //                DisplayOrder = sDisplayOrder.Value,
            //                IsIncluded = sIsIncluded.Value
            //            };

            //            bookPage.BookSection = bookSection;
            //        }
            //    }

            //    return bookPage;
            //}
        }

        public IEnumerable<BookPage> GetPagesBySection(int bookSectionId)
        {
            var sql = $"SELECT bp.BookPageID as 0, bp.BookPageName as 1, bp.BookSectionID as 2, bp.DisplayOrder as 3," +
              $" bp.IsIncluded as 4, bp.BookPageDescription as 5, bp.LifeExtension40 as 6, bp.Cost as 7, bp.Difficulty as 8," +
              $" bp.ImageFilename as 9, bp.Status as 10, bp.Notes as 11, bp.Author as 12, bp.Tips as 13, bp.Resources as 14," +
              $" bp.ResearchPapers as 15, bp.PageFirst as 16, bp.PageLast as 17, bs.BookSectionID as 18," +
              $" bs.BookSectionName as 19, bs.DisplayOrder as 20, bs.IsIncluded as 21" +
              $" FROM (({TableNameBookPages} bp)" +
              $" LEFT JOIN {TableNameBookSections} bs ON bs.BookSectionID = bp.BookSectionID)" +
              $" WHERE bp.IsIncluded = TRUE AND bp.BookSectionID = {bookSectionId}" +
              $" ORDER BY bp.DisplayOrder ASC";

            return QueryAllData(sql, list =>
            {
                var bookPage = MapBookPage(0);
                if (GetTableValue<int?>(2).HasValue)
                {
                    bookPage.BookSection = MapBookSection(18);
                }
                list.Add(bookPage);
            });
        }

        public int GetPagesCountByStatus(string status)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = $"SELECT COUNT(*) FROM {TableNameBookPages} bp" +
                $" WHERE bp.Status = @Status";

                var command = MsAccessDbManager.CreateCommand(sql);
                command.Parameters.AddWithValue("@Status", status);
                return (int)command.ExecuteScalar();
            }
        }
    }
}
