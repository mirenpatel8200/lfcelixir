using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class ChaptersRepository : AbstractRepository<Chapter>, IChaptersRepository
    {
        public override void Insert(Chapter entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameBookChapter} (BookChapterName, DisplayOrder, IsIncluded, Notes, ChapterText, ContentPage2, ContentPage3, ContentPage4, ContentPage5, ContentPage6, ContentPage7, ContentPage8, ContentPage9, ContentPage10, BookChapterTypeID, MarginTop, HasBreakInParagraph1, HasBreakInParagraph2, HasBreakInParagraph3, HasBreakInParagraph4, HasBreakInParagraph5, HasBreakInParagraph6, HasBreakInParagraph7, HasBreakInParagraph8, HasBreakInParagraph9, HasBreakInParagraph10) VALUES (@BookChapterName, @DisplayOrder, @IsIncluded, @Notes, @ChapterText, @ContentPage2, @ContentPage3, @ContentPage4, @ContentPage5, @ContentPage6, @ContentPage7, @ContentPage8, @ContentPage9, @ContentPage10, @BookChapterTypeID, @MarginTop, @HasBreakInParagraph1, @HasBreakInParagraph2, @HasBreakInParagraph3, @HasBreakInParagraph4, @HasBreakInParagraph5, @HasBreakInParagraph6, @HasBreakInParagraph7, @HasBreakInParagraph8, @HasBreakInParagraph9, @HasBreakInParagraph10)");

                command.Parameters.AddWithValue("@BookChapterName", SafeGetStringValue(entity.ChapterName));
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@IsIncluded", entity.IsIncluded);
                command.Parameters.AddWithValue("@Notes", SafeGetStringValue(entity.Notes));
                command.Parameters.AddWithValue("@ChapterText", SafeGetStringValue(entity.Text));
                command.Parameters.AddWithValue("@ContentPage2", SafeGetStringValue(entity.ContentPage2));
                command.Parameters.AddWithValue("@ContentPage3", SafeGetStringValue(entity.ContentPage3));
                command.Parameters.AddWithValue("@ContentPage4", SafeGetStringValue(entity.ContentPage4));
                command.Parameters.AddWithValue("@ContentPage5", SafeGetStringValue(entity.ContentPage5));
                command.Parameters.AddWithValue("@ContentPage6", SafeGetStringValue(entity.ContentPage6));
                command.Parameters.AddWithValue("@ContentPage7", SafeGetStringValue(entity.ContentPage7));
                command.Parameters.AddWithValue("@ContentPage8", SafeGetStringValue(entity.ContentPage8));
                command.Parameters.AddWithValue("@ContentPage9", SafeGetStringValue(entity.ContentPage9));
                command.Parameters.AddWithValue("@ContentPage10", SafeGetStringValue(entity.ContentPage10));
                command.Parameters.AddWithValue("@BookChapterTypeID", entity.TypeID);
                command.Parameters.AddWithValue("@MarginTop", entity.MarginTop);
                command.Parameters.AddWithValue("@HasBreakInParagraph1", entity.HasBreakInParagraph1);
                command.Parameters.AddWithValue("@HasBreakInParagraph2", entity.HasBreakInParagraph2);
                command.Parameters.AddWithValue("@HasBreakInParagraph3", entity.HasBreakInParagraph3);
                command.Parameters.AddWithValue("@HasBreakInParagraph4", entity.HasBreakInParagraph4);
                command.Parameters.AddWithValue("@HasBreakInParagraph5", entity.HasBreakInParagraph5);
                command.Parameters.AddWithValue("@HasBreakInParagraph6", entity.HasBreakInParagraph6);
                command.Parameters.AddWithValue("@HasBreakInParagraph7", entity.HasBreakInParagraph7);
                command.Parameters.AddWithValue("@HasBreakInParagraph8", entity.HasBreakInParagraph8);
                command.Parameters.AddWithValue("@HasBreakInParagraph9", entity.HasBreakInParagraph9);
                command.Parameters.AddWithValue("@HasBreakInParagraph10", entity.HasBreakInParagraph10);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Chapter to the database");
            }
        }

        public override void Update(Chapter entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameBookChapter} SET BookChapterName = @BookChapterName, DisplayOrder = @DisplayOrder, IsIncluded = @IsIncluded, Notes = @Notes, ChapterText = @ChapterText, ContentPage2 = @ContentPage2, ContentPage3 = @ContentPage3, ContentPage4 = @ContentPage4, ContentPage5 = @ContentPage5, ContentPage6 = @ContentPage6, ContentPage7 = @ContentPage7, ContentPage8 = @ContentPage8, ContentPage9 = @ContentPage9, ContentPage10 = @ContentPage10, BookChapterTypeID = @BookChapterTypeID, MarginTop = @MarginTop, HasBreakInParagraph1 = @HasBreakInParagraph1, HasBreakInParagraph2 = @HasBreakInParagraph2, HasBreakInParagraph3 = @HasBreakInParagraph3, HasBreakInParagraph4 = @HasBreakInParagraph4, HasBreakInParagraph5 = @HasBreakInParagraph5, HasBreakInParagraph6 = @HasBreakInParagraph6, HasBreakInParagraph7 = @HasBreakInParagraph7, HasBreakInParagraph8 = @HasBreakInParagraph8, HasBreakInParagraph9 = @HasBreakInParagraph9, HasBreakInParagraph10 = @HasBreakInParagraph10 WHERE BookChapterID = {entity.Id}");

                command.Parameters.AddWithValue("@BookChapterName", SafeGetStringValue(entity.ChapterName));
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@IsIncluded", entity.IsIncluded);
                command.Parameters.AddWithValue("@Notes", SafeGetStringValue(entity.Notes));
                command.Parameters.AddWithValue("@ChapterText", SafeGetStringValue(entity.Text));
                command.Parameters.AddWithValue("@ContentPage2", SafeGetStringValue(entity.ContentPage2));
                command.Parameters.AddWithValue("@ContentPage3", SafeGetStringValue(entity.ContentPage3));
                command.Parameters.AddWithValue("@ContentPage4", SafeGetStringValue(entity.ContentPage4));
                command.Parameters.AddWithValue("@ContentPage5", SafeGetStringValue(entity.ContentPage5));
                command.Parameters.AddWithValue("@ContentPage6", SafeGetStringValue(entity.ContentPage6));
                command.Parameters.AddWithValue("@ContentPage7", SafeGetStringValue(entity.ContentPage7));
                command.Parameters.AddWithValue("@ContentPage8", SafeGetStringValue(entity.ContentPage8));
                command.Parameters.AddWithValue("@ContentPage9", SafeGetStringValue(entity.ContentPage9));
                command.Parameters.AddWithValue("@ContentPage10", SafeGetStringValue(entity.ContentPage10));
                command.Parameters.AddWithValue("@BookChapterTypeID", entity.TypeID);
                command.Parameters.AddWithValue("@MarginTop", entity.MarginTop);
                command.Parameters.AddWithValue("@HasBreakInParagraph1", entity.HasBreakInParagraph1);
                command.Parameters.AddWithValue("@HasBreakInParagraph2", entity.HasBreakInParagraph2);
                command.Parameters.AddWithValue("@HasBreakInParagraph3", entity.HasBreakInParagraph3);
                command.Parameters.AddWithValue("@HasBreakInParagraph4", entity.HasBreakInParagraph4);
                command.Parameters.AddWithValue("@HasBreakInParagraph5", entity.HasBreakInParagraph5);
                command.Parameters.AddWithValue("@HasBreakInParagraph6", entity.HasBreakInParagraph6);
                command.Parameters.AddWithValue("@HasBreakInParagraph7", entity.HasBreakInParagraph7);
                command.Parameters.AddWithValue("@HasBreakInParagraph8", entity.HasBreakInParagraph8);
                command.Parameters.AddWithValue("@HasBreakInParagraph9", entity.HasBreakInParagraph9);
                command.Parameters.AddWithValue("@HasBreakInParagraph10", entity.HasBreakInParagraph10);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing Chapter in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"DELETE FROM {TableNameBookChapter} WHERE BookChapterID = @BookChapterID");

                command.Parameters.AddWithValue("@BookChapterID", id);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during Chapter deleting from the database");
            }

        }

        public override IEnumerable<Chapter> GetAll()
        {
            var sql = $"SELECT c.BookChapterID as 0, c.BookChapterName as 1, c.DisplayOrder as 2, c.IsIncluded as 3," +
                      $" c.Notes as 4, c.ChapterText as 5, c.ContentPage2 as 6, c.ContentPage3 as 7, c.ContentPage4 as 8," +
                      $" c.ContentPage5 as 9, c.ContentPage6 as 10, c.ContentPage7 as 11, c.ContentPage8 as 12, c.ContentPage9 as 13," +
                      $" c.ContentPage10 as 14, c.BookChapterTypeID as 15, c.MarginTop as 16, c.HasBreakInParagraph1 as 17," +
                      $" c.HasBreakInParagraph2 as 18, c.HasBreakInParagraph3 as 19, c.HasBreakInParagraph4 as 20," +
                      $" c.HasBreakInParagraph5 as 21, c.HasBreakInParagraph6 as 22, c.HasBreakInParagraph7 as 23," +
                      $" c.HasBreakInParagraph8 as 24, c.HasBreakInParagraph9 as 25, c.HasBreakInParagraph10 as 26," +
                      $" c.PageFirst as 27, c.PageLast as 28" +
                      $" FROM {TableNameBookChapter} c";

            return QueryAllData(sql, list =>
            {
                var chapter = MapChapter(0);
                list.Add(chapter);
            });


            //using (MsAccessDbManager = new MsAccessDbManager())
            //{
            //    DataReader = MsAccessDbManager.CreateCommand(
            //        $"SELECT BookChapterID, BookChapterName, DisplayOrder, IsIncluded, Notes, " + 
            //        "ChapterText, ContentPage2, ContentPage3, ContentPage4, ContentPage5, ContentPage6, " + 
            //        "ContentPage7, ContentPage8, ContentPage9, ContentPage10, BookChapterTypeID, " + 
            //        "MarginTop, HasBreakInParagraph1, HasBreakInParagraph2, HasBreakInParagraph3, " + 
            //        "HasBreakInParagraph4, HasBreakInParagraph5, HasBreakInParagraph6, " + 
            //        "HasBreakInParagraph7, HasBreakInParagraph8, HasBreakInParagraph9, HasBreakInParagraph10, " +
            //        "PageFirst, PageLast " +
            //        $"FROM {TableNameBookChapter}").ExecuteReader();

            //    while (DataReader.Read())
            //    {
            //        int id = GetTableValue<int>(0);
            //        String chapterName =  GetTableValue<String>(1);
            //        int displayOrder = GetTableValue<int>(2);
            //        bool isIncluded = GetTableValue<bool>(3);
            //        string notes = GetTableValue<String>(4);
            //        string text = GetTableValue<String>(5);

            //        string contentPage2 = GetTableValue<String>(6);
            //        string contentPage3 = GetTableValue<String>(7);
            //        string contentPage4 = GetTableValue<String>(8);
            //        string contentPage5 = GetTableValue<String>(9);
            //        string contentPage6 = GetTableValue<String>(10);
            //        string contentPage7 = GetTableValue<String>(11);
            //        string contentPage8 = GetTableValue<String>(12);
            //        string contentPage9 = GetTableValue<String>(13);
            //        string contentPage10 = GetTableValue<String>(14);

            //        int typeId = GetTableValue<int>(15);
            //        decimal marginTop = GetTableValue<decimal>(16);

            //        bool hasBreakInP1 = GetTableValue<bool>(17);
            //        bool hasBreakInP2 = GetTableValue<bool>(18);
            //        bool hasBreakInP3 = GetTableValue<bool>(19);
            //        bool hasBreakInP4 = GetTableValue<bool>(20);
            //        bool hasBreakInP5 = GetTableValue<bool>(21);
            //        bool hasBreakInP6 = GetTableValue<bool>(22);
            //        bool hasBreakInP7 = GetTableValue<bool>(23);
            //        bool hasBreakInP8 = GetTableValue<bool>(24);
            //        bool hasBreakInP9 = GetTableValue<bool>(25);
            //        bool hasBreakInP10 = GetTableValue<bool>(26);

            //        int pageFirst = GetTableValue<int>(27);
            //        int pageLast = GetTableValue<int>(28);

            //        Chapter chapter = new Chapter();
            //        chapter.Id = id;
            //        chapter.ChapterName = chapterName;
            //        chapter.DisplayOrder = displayOrder;
            //        chapter.IsIncluded = isIncluded;
            //        chapter.Notes = notes;
            //        chapter.Text = text;
            //        chapter.ContentPage2 = contentPage2;
            //        chapter.ContentPage3 = contentPage3;
            //        chapter.ContentPage4 = contentPage4;
            //        chapter.ContentPage5 = contentPage5;
            //        chapter.ContentPage6 = contentPage6;
            //        chapter.ContentPage7 = contentPage7;
            //        chapter.ContentPage8 = contentPage8;
            //        chapter.ContentPage9 = contentPage9;
            //        chapter.ContentPage10 = contentPage10;
            //        chapter.TypeID = typeId;
            //        chapter.MarginTop = marginTop;
            //        chapter.HasBreakInParagraph1 = hasBreakInP1;
            //        chapter.HasBreakInParagraph2 = hasBreakInP2;
            //        chapter.HasBreakInParagraph3 = hasBreakInP3;
            //        chapter.HasBreakInParagraph4 = hasBreakInP4;
            //        chapter.HasBreakInParagraph5 = hasBreakInP5;
            //        chapter.HasBreakInParagraph6 = hasBreakInP6;
            //        chapter.HasBreakInParagraph7 = hasBreakInP7;
            //        chapter.HasBreakInParagraph8 = hasBreakInP8;
            //        chapter.HasBreakInParagraph9 = hasBreakInP9;
            //        chapter.HasBreakInParagraph10 = hasBreakInP10;
            //        chapter.PageFirst = pageFirst;
            //        chapter.PageLast = pageLast;

            //        yield return chapter;
            //    }
            //}
        }

        //public Chapter GetByName(string chapterName)
        //{
        //    return GetAll().FirstOrDefault(x => x.ChapterName.Equals(chapterName));
        //}

        public IEnumerable<Chapter> GetAllChapters(ChaptersSortOrder sortOrder, SortDirection sortDirection)
        {
            var sql = $"SELECT c.BookChapterID as 0, c.BookChapterName as 1, c.DisplayOrder as 2, c.IsIncluded as 3," +
                      $" c.Notes as 4, c.ChapterText as 5, c.ContentPage2 as 6, c.ContentPage3 as 7, c.ContentPage4 as 8," +
                      $" c.ContentPage5 as 9, c.ContentPage6 as 10, c.ContentPage7 as 11, c.ContentPage8 as 12, c.ContentPage9 as 13," +
                      $" c.ContentPage10 as 14, c.BookChapterTypeID as 15, c.MarginTop as 16, c.HasBreakInParagraph1 as 17," +
                      $" c.HasBreakInParagraph2 as 18, c.HasBreakInParagraph3 as 19, c.HasBreakInParagraph4 as 20," +
                      $" c.HasBreakInParagraph5 as 21, c.HasBreakInParagraph6 as 22, c.HasBreakInParagraph7 as 23," +
                      $" c.HasBreakInParagraph8 as 24, c.HasBreakInParagraph9 as 25, c.HasBreakInParagraph10 as 26," +
                      $" c.PageFirst as 27, c.PageLast as 28" +
                      $" FROM {TableNameBookChapter} c" +
                      $" ORDER BY c.{sortOrder.ToString()} {(sortDirection == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var chapter = MapChapter(0);
                list.Add(chapter);
            });
        }

        public Chapter GetChapterById(int id)
        {
            var sql = $"SELECT c.BookChapterID as 0, c.BookChapterName as 1, c.DisplayOrder as 2, c.IsIncluded as 3," +
                      $" c.Notes as 4, c.ChapterText as 5, c.ContentPage2 as 6, c.ContentPage3 as 7, c.ContentPage4 as 8," +
                      $" c.ContentPage5 as 9, c.ContentPage6 as 10, c.ContentPage7 as 11, c.ContentPage8 as 12, c.ContentPage9 as 13," +
                      $" c.ContentPage10 as 14, c.BookChapterTypeID as 15, c.MarginTop as 16, c.HasBreakInParagraph1 as 17," +
                      $" c.HasBreakInParagraph2 as 18, c.HasBreakInParagraph3 as 19, c.HasBreakInParagraph4 as 20," +
                      $" c.HasBreakInParagraph5 as 21, c.HasBreakInParagraph6 as 22, c.HasBreakInParagraph7 as 23," +
                      $" c.HasBreakInParagraph8 as 24, c.HasBreakInParagraph9 as 25, c.HasBreakInParagraph10 as 26," +
                      $" c.PageFirst as 27, c.PageLast as 28" +
                      $" FROM {TableNameBookChapter} c" +
                      $" WHERE c.BookChapterID = {id}";

            return QueryAllData(sql, list =>
            {
                var chapter = MapChapter(0);
                list.Add(chapter);
            }).FirstOrDefault();
        }

        public IEnumerable<Chapter> GetChaptersByType(ChapterType type, ChapterIncluded included)
        {
            var sql = $"SELECT c.BookChapterID as 0, c.BookChapterName as 1, c.DisplayOrder as 2, c.IsIncluded as 3," +
                     $" c.Notes as 4, c.ChapterText as 5, c.ContentPage2 as 6, c.ContentPage3 as 7, c.ContentPage4 as 8," +
                     $" c.ContentPage5 as 9, c.ContentPage6 as 10, c.ContentPage7 as 11, c.ContentPage8 as 12, c.ContentPage9 as 13," +
                     $" c.ContentPage10 as 14, c.BookChapterTypeID as 15, c.MarginTop as 16, c.HasBreakInParagraph1 as 17," +
                     $" c.HasBreakInParagraph2 as 18, c.HasBreakInParagraph3 as 19, c.HasBreakInParagraph4 as 20," +
                     $" c.HasBreakInParagraph5 as 21, c.HasBreakInParagraph6 as 22, c.HasBreakInParagraph7 as 23," +
                     $" c.HasBreakInParagraph8 as 24, c.HasBreakInParagraph9 as 25, c.HasBreakInParagraph10 as 26," +
                     $" c.PageFirst as 27, c.PageLast as 28" +
                     $" FROM {TableNameBookChapter} c";
           
            if (included == ChapterIncluded.All)
            {
                sql += $" WHERE c.BookChapterTypeID = {(int)type}";
                //return GetAll().Where(c => c.TypeID == (int)type);
            }
            else
            {
                bool isIncluded = included == ChapterIncluded.IncludedOnly;
                sql += $" WHERE c.BookChapterTypeID = {(int)type} AND c.IsIncluded = {isIncluded}";
                //return GetAll().Where(c => c.TypeID == (int)type && c.IsIncluded == isIncluded);
            }
            return QueryAllData(sql, list =>
            {
                var chapter = MapChapter(0);
                list.Add(chapter);
            });
        }

        public void SetChapterFirstPageLastPage(int chapterId, int firstPage, int lastPage)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                   $"UPDATE {TableNameBookChapter} SET PageFirst = @PageFirst, PageLast = @PageLast WHERE BookChapterID = {chapterId}");

                command.Parameters.AddWithValue("@PageFirst", firstPage);
                command.Parameters.AddWithValue("@PageLast", lastPage);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("SetChapterFirstPageAndLastPage: Error during editing Chapter in the database");
            }
        }

        public Chapter GetChapterByManualPageNumber(int pageNumber)
        {
            //var chapter = GetAll().FirstOrDefault(c =>
            //    (c.PageFirst < pageNumber && c.PageLast > pageNumber) ||
            //    c.PageFirst == pageNumber || c.PageLast == pageNumber);
            ////testeaza cu 5 - should get 0 rezultate

            //return chapter;
            var sql = $"SELECT c.BookChapterID as 0, c.BookChapterName as 1, c.DisplayOrder as 2, c.IsIncluded as 3," +
                      $" c.Notes as 4, c.ChapterText as 5, c.ContentPage2 as 6, c.ContentPage3 as 7, c.ContentPage4 as 8," +
                      $" c.ContentPage5 as 9, c.ContentPage6 as 10, c.ContentPage7 as 11, c.ContentPage8 as 12, c.ContentPage9 as 13," +
                      $" c.ContentPage10 as 14, c.BookChapterTypeID as 15, c.MarginTop as 16, c.HasBreakInParagraph1 as 17," +
                      $" c.HasBreakInParagraph2 as 18, c.HasBreakInParagraph3 as 19, c.HasBreakInParagraph4 as 20," +
                      $" c.HasBreakInParagraph5 as 21, c.HasBreakInParagraph6 as 22, c.HasBreakInParagraph7 as 23," +
                      $" c.HasBreakInParagraph8 as 24, c.HasBreakInParagraph9 as 25, c.HasBreakInParagraph10 as 26," +
                      $" c.PageFirst as 27, c.PageLast as 28" +
                      $" FROM {TableNameBookChapter} c" +
                      $" WHERE (c.PageFirst < {pageNumber} AND c.PageLast > {pageNumber}) OR c.PageFirst = {pageNumber} OR c.PageLast = {pageNumber}";

            return QueryAllData(sql, list =>
            {
                var chapter = MapChapter(0);
                list.Add(chapter);
            }).FirstOrDefault();
        }
    }
}
