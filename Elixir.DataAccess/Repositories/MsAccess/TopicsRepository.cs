using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class TopicsRepository : AbstractRepository<Topic>, ITopicsRepository
    {
        public override void Insert(Topic entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameTopics} (TopicName, DescriptionInternal, PrimaryWebPageId, NotesInternal, SocialImageFilename, SocialImageFilenameNews, BannerImageFileName,Hashtags,ThumbnailImageFilename) " +
                    $"VALUES (@TopicName, @DescriptionInternal, @PrimaryWebPageId, @NotesInternal, @SocialImageFilename, @SocialImageFilenameNews, @BannerImageFileName, @Hashtags, @ThumbnailImageFilename)");
                //TODO: add PrimaryWebPageId in access table

                command.Parameters.AddWithValue("@TopicName", entity.TopicName);
                command.Parameters.AddWithValue("@DescriptionInternal", SafeGetStringValue(entity.DescriptionInternal));
                command.Parameters.AddWithValue("@PrimaryWebPageId", entity.PrimaryWebPageId);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@SocialImageFilename", SafeGetStringValue(entity.SocialImageFilename));
                command.Parameters.AddWithValue("@SocialImageFilenameNews", SafeGetStringValue(entity.SocialImageFilenameNews));
                command.Parameters.AddWithValue("@BannerImageFileName", SafeGetStringValue(entity.BannerImageFileName));
                command.Parameters.AddWithValue("@Hashtags", SafeGetStringValue(entity.Hashtags));
                command.Parameters.AddWithValue("@SocialImageFilenameNews", SafeGetStringValue(entity.ThumbnailImageFilename));

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Topic to the database");
            }
        }

        public override void Update(Topic entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameTopics} SET TopicName = @TopicName, DescriptionInternal = @DescriptionInternal, PrimaryWebPageId = @PrimaryWebPageId, NotesInternal = @NotesInternal," +
                    $" SocialImageFilename = @SocialImageFilename, SocialImageFilenameNews = @SocialImageFilenameNews, " +
                    $" BannerImageFileName = @BannerImageFileName, " +
                    $" Hashtags = @Hashtags , ThumbnailImageFilename = @ThumbnailImageFilename " +
                    $"WHERE TopicID = {entity.Id}");

                command.Parameters.AddWithValue("@TopicName", entity.TopicName);
                command.Parameters.AddWithValue("@DescriptionInternal", SafeGetStringValue(entity.DescriptionInternal));
                command.Parameters.AddWithValue("@PrimaryWebPageId", entity.PrimaryWebPageId);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@SocialImageFilename", SafeGetStringValue(entity.SocialImageFilename));
                command.Parameters.AddWithValue("@SocialImageFilenameNews", SafeGetStringValue(entity.SocialImageFilenameNews));
                command.Parameters.AddWithValue("@BannerImageFileName", SafeGetStringValue(entity.BannerImageFileName));
                command.Parameters.AddWithValue("@Hashtags", SafeGetStringValue(entity.Hashtags));
                command.Parameters.AddWithValue("@ThumbnailImageFilename", SafeGetStringValue(entity.ThumbnailImageFilename));

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing Topic in the database");
            }
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Topic> GetAll()
        {
            var sql = "SELECT t.TopicID as 0, t.TopicName as 1, t.DescriptionInternal as 2," +
                " t.PrimaryWebPageId as 3, t.NotesInternal as 4, t.SocialImageFilename as 5," +
                " t.SocialImageFilenameNews as 6, t.BannerImageFileName as 7, t.Hashtags as 8,  t.ThumbnailImageFilename as 9," +
                " wp.WebPageID as 10, wp.UrlName as 11, wp.WebPageName as 12, wp.IsDeleted as 13, " +
                " wp.WebPageTitle as 14, wp.ContentMain as 15, wp.ParentID as 16, wp.IsSubjectPage as 17, " +
                " wp.DisplayOrder as 18, wp.IsEnabled as 19, wp.NotesInternal as 20, " +
                " wp.CreatedDT as 21, wp.UpdatedDT as 22, wp.BannerImageFileName as 23, " +
                " wp.SocialImageFileName as 24, wp.MetaDescription as 25, " +
                " wp.PrimaryTopicID as 26, wp.SecondaryTopicID as 27, " +
                " wp.PublishedOnDT as 28, wp.PublishedUpdatedDT as 29, " +
                " wp.TypeID as 30" +
                $" FROM {TableNameTopics} t " +
                $" LEFT JOIN {TableNameWebPages} wp ON t.PrimaryWebPageId = wp.WebPageID";

            return QueryAllData(sql, list =>
            {
                var topic = MapTopic(0);

                if (GetTableValue<int?>(3).HasValue)
                    topic.PrimaryWebPage = MapWebPage(10);

                list.Add(topic);
            });
        }

        public Topic GetById(int id)
        {
            var sql = "SELECT t.TopicID as 0, t.TopicName as 1, t.DescriptionInternal as 2," +
                " t.PrimaryWebPageId as 3, t.NotesInternal as 4, t.SocialImageFilename as 5," +
                " t.SocialImageFilenameNews as 6, t.BannerImageFileName as 7, t.Hashtags as 8,  t.ThumbnailImageFilename as 9," +
                " wp.WebPageID as 10, wp.UrlName as 11, wp.WebPageName as 12, wp.IsDeleted as 13, " +
                " wp.WebPageTitle as 14, wp.ContentMain as 15, wp.ParentID as 16, wp.IsSubjectPage as 17, " +
                " wp.DisplayOrder as 18, wp.IsEnabled as 19, wp.NotesInternal as 20, " +
                " wp.CreatedDT as 21, wp.UpdatedDT as 22, wp.BannerImageFileName as 23, " +
                " wp.SocialImageFileName as 24, wp.MetaDescription as 25, " +
                " wp.PrimaryTopicID as 26, wp.SecondaryTopicID as 27, " +
                " wp.PublishedOnDT as 28, wp.PublishedUpdatedDT as 29, " +
                " wp.TypeID as 30" +
                $" FROM {TableNameTopics} t " +
                $" LEFT JOIN {TableNameWebPages} wp ON t.PrimaryWebPageId = wp.WebPageID" +
                $" WHERE t.TopicID = {id}";

            return QueryAllData(sql, list =>
            {
                var topic = MapTopic(0);

                if (GetTableValue<int?>(3).HasValue)
                    topic.PrimaryWebPage = MapWebPage(10);

                list.Add(topic);
            }).FirstOrDefault();
        }

        public Topic GetByName(string topicName)
        {
            var sql = "SELECT t.TopicID as 0, t.TopicName as 1, t.DescriptionInternal as 2," +
               " t.PrimaryWebPageId as 3, t.NotesInternal as 4, t.SocialImageFilename as 5," +
               " t.SocialImageFilenameNews as 6, t.BannerImageFileName as 7, t.Hashtags as 8,  t.ThumbnailImageFilename as 9," +
               " wp.WebPageID as 10, wp.UrlName as 11, wp.WebPageName as 12, wp.IsDeleted as 13, " +
               " wp.WebPageTitle as 14, wp.ContentMain as 15, wp.ParentID as 16, wp.IsSubjectPage as 17, " +
               " wp.DisplayOrder as 18, wp.IsEnabled as 19, wp.NotesInternal as 20, " +
               " wp.CreatedDT as 21, wp.UpdatedDT as 22, wp.BannerImageFileName as 23, " +
               " wp.SocialImageFileName as 24, wp.MetaDescription as 25, " +
               " wp.PrimaryTopicID as 26, wp.SecondaryTopicID as 27, " +
               " wp.PublishedOnDT as 28, wp.PublishedUpdatedDT as 29, " +
               " wp.TypeID as 30" +
               $" FROM {TableNameTopics} t " +
               $" LEFT JOIN {TableNameWebPages} wp ON t.PrimaryWebPageId = wp.WebPageID" +
               $" WHERE t.TopicName = {topicName}";

            return QueryAllData(sql, list =>
            {
                var topic = MapTopic(0);

                if (GetTableValue<int?>(3).HasValue)
                    topic.PrimaryWebPage = MapWebPage(10);

                list.Add(topic);
            }).FirstOrDefault();
        }

        public IEnumerable<Topic> GetAll(TopicSortOrder[] sortFields, SortDirection[] sortDirections)
        {
            var sql = "SELECT t.TopicID as 0, t.TopicName as 1, t.DescriptionInternal as 2, t.PrimaryWebPageId as 3," +
                      " t.NotesInternal as 4, t.SocialImageFilename as 5, t.SocialImageFilenameNews as 6," +
                      $" t.BannerImageFileName as 7, t.Hashtags as 8,  t.ThumbnailImageFilename as 9," +
                      $" wp.WebPageID as 10, wp.UrlName as 11, wp.WebPageName as 12, wp.IsDeleted as 13, " +
                      $" wp.WebPageTitle as 14, wp.ContentMain as 15, wp.ParentID as 16, wp.IsSubjectPage as 17, " +
                      $" wp.DisplayOrder as 18, wp.IsEnabled as 19, wp.NotesInternal as 20, " +
                      $" wp.CreatedDT as 21, wp.UpdatedDT as 22, wp.BannerImageFileName as 23, " +
                      $" wp.SocialImageFileName as 24, wp.MetaDescription as 25, " +
                      $" wp.PrimaryTopicID as 26, wp.SecondaryTopicID as 27, " +
                      $" wp.PublishedOnDT as 28, wp.PublishedUpdatedDT as 29, " +
                      $" wp.TypeID as 30 " +
                      $" FROM {TableNameTopics} t" +
                      $" LEFT JOIN {TableNameWebPages} wp ON t.PrimaryWebPageId = wp.WebPageID";

            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()), sortDirections, Enumerable.Repeat("t", sortFields.Length));

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, list =>
            {
                var topic = MapTopic(0);

                if (GetTableValue<int?>(3).HasValue)
                    topic.PrimaryWebPage = MapWebPage(10);

                list.Add(topic);
            });
        }

        public IEnumerable<Topic> GetN(int count, TopicSortOrder[] sortFields, SortDirection[] sortDirections)
        {
            var sql = $"SELECT TOP {count} t.TopicID as 0, t.TopicName as 1, t.DescriptionInternal as 2," +
                      $" t.PrimaryWebPageId as 3, t.NotesInternal as 4, t.SocialImageFilename as 5, t.SocialImageFilenameNews as 6, " +
                      $" t.BannerImageFileName as 7, t.Hashtags as 8,  t.ThumbnailImageFilename as 9," +
                      $" wp.WebPageID as 10, wp.UrlName as 11, wp.WebPageName as 12, wp.IsDeleted as 13, " +
                      $" wp.WebPageTitle as 14, wp.ContentMain as 15, wp.ParentID as 16, wp.IsSubjectPage as 17, " +
                      $" wp.DisplayOrder as 18, wp.IsEnabled as 19, wp.NotesInternal as 20, " +
                      $" wp.CreatedDT as 21, wp.UpdatedDT as 22, wp.BannerImageFileName as 23, " +
                      $" wp.SocialImageFileName as 24, wp.MetaDescription as 25, " +
                      $" wp.PrimaryTopicID as 26, wp.SecondaryTopicID as 27, " +
                      $" wp.PublishedOnDT as 28, wp.PublishedUpdatedDT as 29, " +
                      $" wp.TypeID as 30 " +
                      $" FROM {TableNameTopics} t" +
                      $" LEFT JOIN {TableNameWebPages} wp ON t.PrimaryWebPageId = wp.WebPageID";

            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()), sortDirections, Enumerable.Repeat("t", sortFields.Length));

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, list =>
            {
                var topic = MapTopic(0);

                if (GetTableValue<int?>(3).HasValue)
                    topic.PrimaryWebPage = MapWebPage(10);

                list.Add(topic);
            });
        }

        public IEnumerable<Topic> GetFiltered(string filter, TopicSortOrder[] sortFields, SortDirection[] sortDirections, bool includeAllFields)
        {
            bool filterExists = !string.IsNullOrEmpty(filter);
            var parameters = new Dictionary<string, object>();
            var sql = "SELECT t.TopicID as 0, t.TopicName as 1, t.DescriptionInternal as 2, t.PrimaryWebPageId as 3," +
                      " t.NotesInternal as 4, t.SocialImageFilename as 5, t.SocialImageFilenameNews as 6," +
                      $" t.BannerImageFileName as 7, t.Hashtags as 8,  t.ThumbnailImageFilename as 9, " +
                      $" wp.WebPageID as 10, wp.UrlName as 11, wp.WebPageName as 12, wp.IsDeleted as 13, " +
                      $" wp.WebPageTitle as 14, wp.ContentMain as 15, wp.ParentID as 16, wp.IsSubjectPage as 17, " +
                      $" wp.DisplayOrder as 18, wp.IsEnabled as 19, wp.NotesInternal as 20, " +
                      $" wp.CreatedDT as 21, wp.UpdatedDT as 22, wp.BannerImageFileName as 23, " +
                      $" wp.SocialImageFileName as 24, wp.MetaDescription as 25, " +
                      $" wp.PrimaryTopicID as 26, wp.SecondaryTopicID as 27, " +
                      $" wp.PublishedOnDT as 28, wp.PublishedUpdatedDT as 29, " +
                      $" wp.TypeID as 30 " +
                      $" FROM {TableNameTopics} t" +
                      $" LEFT JOIN {TableNameWebPages} wp ON t.PrimaryWebPageId = wp.WebPageID";
            if (filterExists)
            {
                if (includeAllFields)
                {
                    sql += " WHERE t.TopicName LIKE '%{{filter}}%' or t.DescriptionInternal LIKE '%{{filter}}%' ";
                    sql = sql.Replace("{{filter}}", filter);
                }
                else
                {
                    sql += " WHERE t.TopicName LIKE '%{{filter}}%'";
                    sql = sql.Replace("{{filter}}", filter);
                }
            }

            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()), sortDirections, Enumerable.Repeat("t", sortFields.Length));

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, (list =>
            {
                var topic = MapTopic(0);

                if (GetTableValue<int?>(3).HasValue)
                    topic.PrimaryWebPage = MapWebPage(10);

                list.Add(topic);
            }), parameters);
        }
    }
}
