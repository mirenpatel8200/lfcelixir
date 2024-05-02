using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class WebPagesRepository : AbstractRepository<WebPage>, IWebPagesRepository
    {
        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"DELETE FROM {TableNameWebPages} WHERE WebPageID = @WebPageID");

                command.Parameters.AddWithValue("@WebPageID", id);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting WebPage from the database");
            }
        }

        public override IEnumerable<WebPage> GetAll()
        {
            var sql = "SELECT wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                      " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)" +
                      $" WHERE wp.IsDeleted = FALSE";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(39);
                webPage.TypeName = GetTableValue<string>(40);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            });
        }

        public IEnumerable<WebPage> GetNFilter(int count, bool includeDeleted, string filter, int pageTypeId, WebPagesSortOrder sortOrder, SortDirection sortDirection)
        {
            var sql = $"SELECT TOP {count} wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                      " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29, t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39, t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)";

            var filterSql = string.Empty;

            if (!string.IsNullOrEmpty(filter.Trim()))
            {
                filterSql += " wp.WebPageName LIKE '%@filter%'";
                filterSql = filterSql.Replace("@filter", filter);
            }
            if (pageTypeId > 0)
            {
                if (!string.IsNullOrEmpty(filterSql))
                {
                    filterSql += " AND wp.TypeID=" + pageTypeId;
                }
                else
                {
                    filterSql += " wp.TypeID=" + pageTypeId;
                }

            }
            if (!string.IsNullOrEmpty(filterSql))
            {
                if (includeDeleted)
                    sql += " WHERE " + filterSql;
                else
                    sql += " WHERE wp.IsDeleted = FALSE AND " + filterSql;
            }
            else
            {
                if (!includeDeleted)
                    sql += " WHERE wp.IsDeleted = FALSE";
            }

            //Sorting Sql builder
            var sortingSql = $" ORDER BY wp.{sortOrder.ToString()} {(sortDirection == SortDirection.Ascending ? "ASC" : "DESC")}";
            sql += sortingSql;

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(41);
                webPage.TypeName = GetTableValue<string>(42);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            });
        }
        //public IEnumerable<WebPage> GetAllWebPages()
        //{
        //    return GetAll();
        //}

        public IEnumerable<WebPage> GetAllShopWebPages()
        {
            var sql = "SELECT wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                     " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)" +
                      $" WHERE wp.TypeID = {(int)EnumWebPageType.Shop} AND wp.IsDeleted = FALSE";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(39);
                webPage.TypeName = GetTableValue<string>(40);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            });
        }

        public override void Insert(WebPage entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameWebPages} (UrlName, WebPageName, IsDeleted, WebPageTitle, ContentMain, ParentID, IsSubjectPage, DisplayOrder, IsEnabled, NotesInternal, BannerImageFileName, CreatedDT, UpdatedDT, SocialImageFileName, MetaDescription, PrimaryTopicID, SecondaryTopicID, PublishedOnDT, TypeID,UpdatedByUserId,CreatedByUserId) " +
                    $"VALUES (@UrlName, @WebPageName, @IsDeleted, @WebPageTitle, @ContentMain, @ParentID, @IsSubjectPage, @DisplayOrder, @IsEnabled, @NotesInternal, @BannerImageFileName, @CreatedDT, @UpdatedDT, @SocialImageFileName, @MetaDescription, @PrimaryTopicID, @SecondaryTopicID, @PublishedOnDT, @TypeID,@UpdatedByUserId,@CreatedByUserId)");

                command.Parameters.AddWithValue("@UrlName", SafeGetStringValue(entity.UrlName));
                command.Parameters.AddWithValue("@WebPageName", SafeGetStringValue(entity.WebPageName));
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@WebPageTitle", SafeGetStringValue(entity.WebPageTitle));
                command.Parameters.AddWithValue("@ContentMain", SafeGetStringValue(entity.ContentMain));

                command.Parameters.AddWithValue("@ParentID", entity.ParentID.HasValue ? entity.ParentID : (object)DBNull.Value);

                command.Parameters.AddWithValue("@IsSubjectPage", entity.IsSubjectPage);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));

                command.Parameters.AddWithValue("@BannerImageFileName", SafeGetStringValue(entity.BannerImageFileName));
                command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.CreatedDateTime));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedDateTime.HasValue ? GetDateWithoutMilliseconds(entity.UpdatedDateTime.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SocialImageFileName", SafeGetStringValue(entity.SocialImageFileName));
                command.Parameters.AddWithValue("@MetaDescription", SafeGetStringValue(entity.MetaDescription));
                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopicID);
                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopicID.HasValue ? entity.SecondaryTopicID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PublishedOnDT", GetDateWithoutMilliseconds(entity.PublishedOnDT));
                command.Parameters.AddWithValue("@TypeID", entity.TypeID);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId);
                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedByUserId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving WebPage to the database");
            }
        }

        public override void Update(WebPage entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameWebPages} SET UrlName = @UrlName, WebPageName = @WebPageName, DisplayOrder = @DisplayOrder," +
                    "IsDeleted = @IsDeleted, WebPageTitle = @WebPageTitle, ContentMain = @ContentMain, " +
                    "ParentID = @ParentID, IsSubjectPage = @IsSubjectPage, IsEnabled = @IsEnabled, " +
                    "NotesInternal = @NotesInternal, BannerImageFileName = @BannerImageFileName, " +
                    "UpdatedDT = @UpdatedDT, SocialImageFileName = @SocialImageFileName, MetaDescription = @MetaDescription, " +
                    "PrimaryTopicID = @PrimaryTopicID, SecondaryTopicID = @SecondaryTopicID, " +
                    "PublishedOnDT = @PublishedOnDT, PublishedUpdatedDT = @PublishedUpdatedDT, TypeID = @TypeID, " +
                    "UpdatedByUserId = @UpdatedByUserId " +
                    $"WHERE WebPageID = {entity.Id}");

                command.Parameters.AddWithValue("@UrlName", entity.UrlName);
                command.Parameters.AddWithValue("@WebPageName", entity.WebPageName);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder);
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@WebPageTitle", entity.WebPageTitle);
                command.Parameters.AddWithValue("@ContentMain", entity.ContentMain);

                command.Parameters.AddWithValue("@ParentID", entity.ParentID.HasValue ? entity.ParentID : (object)DBNull.Value);

                command.Parameters.AddWithValue("@IsSubjectPage", entity.IsSubjectPage);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));

                command.Parameters.AddWithValue("@BannerImageFileName", SafeGetStringValue(entity.BannerImageFileName));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedDateTime.HasValue ? GetDateWithoutMilliseconds(entity.UpdatedDateTime.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SocialImageFileName", SafeGetStringValue(entity.SocialImageFileName));
                command.Parameters.AddWithValue("@MetaDescription", SafeGetStringValue(entity.MetaDescription));
                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopicID);
                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopicID.HasValue ? entity.SecondaryTopicID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PublishedOnDT", GetDateWithoutMilliseconds(entity.PublishedOnDT));
                command.Parameters.AddWithValue("@PublishedUpdatedDT",
                    entity.PublishedUpdatedDT.HasValue ?
                    GetDateWithoutMilliseconds(entity.PublishedUpdatedDT.Value) :
                    (object)DBNull.Value);
                command.Parameters.AddWithValue("@TypeID", entity.TypeID);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing WebPage in the database");
            }
        }

        public WebPage GetWebPageById(int id)
        {
            var sql = "SELECT wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                      " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)" +
                      $" WHERE wp.WebPageID = {id}";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(39);
                webPage.TypeName = GetTableValue<string>(40);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            }).FirstOrDefault();
        }

        public WebPage GetWebPageByUrlName(string urlName, int webPageTypeId)
        {
            var sql = "SELECT wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                      " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)" +
                      $" WHERE LCASE(wp.UrlName) = @UrlName AND wp.TypeID = @TypeID AND wp.IsDeleted = 0 AND wp.IsEnabled = TRUE";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(41);
                webPage.TypeName = GetTableValue<string>(42);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            }, new Dictionary<string, object>() { { "UrlName", urlName.ToLower() }, { "TypeID", webPageTypeId } }).FirstOrDefault();
        }


        public IEnumerable<WebPage> GetWebPagesChildren(int parentId)
        {
            var sql = "SELECT wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                      " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)" +
                      $" WHERE wp.ParentID = {parentId} AND wp.IsEnabled = TRUE AND wp.TypeID = {(int)EnumWebPageType.Page}" +
                      $" ORDER BY wp.DisplayOrder ASC";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(39);
                webPage.TypeName = GetTableValue<string>(40);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            });
        }

        /// <summary>
        /// Checks if non-deleted webpage with specified UrlName exists, given Id of page that should be excluded.
        /// </summary>
        /// <param name="urlName"></param>
        /// <param name="excludeId"></param>
        /// <returns></returns>
        public bool UrlNameExists(string urlName, int webPageTypeId, int? excludeId)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = excludeId.HasValue
                     ? $"SELECT COUNT(*) FROM {TableNameWebPages} wp WHERE wp.WebPageId <> @ExcludeId AND wp.IsDeleted = 0 AND wp.UrlName = @UrlName AND wp.TypeID = @TypeID"
                     : $"SELECT COUNT(*) FROM {TableNameWebPages} wp WHERE wp.IsDeleted = 0 AND wp.UrlName = @UrlName AND wp.TypeID = @TypeID";

                var command = MsAccessDbManager.CreateCommand(sql);

                if (excludeId.HasValue)
                    command.Parameters.AddWithValue("@ExcludeId", excludeId.Value);
                command.Parameters.AddWithValue("@UrlName", urlName);
                command.Parameters.AddWithValue("@TypeID", webPageTypeId);

                return (int)command.ExecuteScalar() > 0;
            }
        }

        public int GetWebPageType(int id)
        {
            int typeId = 0;
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbDataReader dataReader = MsAccessDbManager.CreateCommand(
                    $"SELECT TypeID FROM {TableNameWebPages} WHERE WebPageID = {id}").ExecuteReader();

                while (dataReader.Read())
                {
                    typeId = dataReader.GetInt32(0);
                }
            }
            return typeId;
        }

        public IEnumerable<WebPage> SearchSQL(List<string> terms, bool all = false)
        {
            var parameters = new Dictionary<string, object>();
            var sql = $"SELECT {(all ? "" : "TOP 10")} wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                     " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40, " +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID) " +
                      " WHERE wp.IsDeleted = FALSE AND wp.IsEnabled = TRUE";

            if (terms.Count() > 0)
            {
                string whereClause = "";
                string regexPunctuation = "% [().,;:-_!/?" + "\"" + "]";
                for (int i = 0; i < terms.Count; i++)
                {
                    string term = terms[i], alias = $"term{i}";
                    string condition = SqlHelper.ConditionWebPageContains(alias);
                    whereClause += $" AND {condition}";

                    parameters.Add($"@{alias}", $"% {term}%");
                    parameters.Add($"@start{alias}", $"{term}%");
                    parameters.Add($"@punctuation{alias}", regexPunctuation + term + " %");

                }

                sql += whereClause;
            }

            sql += " ORDER BY wp.PublishedUpdatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(41);
                webPage.TypeName = GetTableValue<string>(42);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);

            }, parameters);
        }

        public void DeleteWebPage(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameWebPages} SET IsDeleted = 1 WHERE WebPageID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop product in the database");
            }
        }

        public IEnumerable<WebPage> GetWebPageSiblings(int webPageId)
        {
            var sql = "SELECT wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                      " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)" +
                      $" WHERE wp.IsEnabled = TRUE AND wp.IsDeleted = FALSE AND wp.IsSubjectPage = TRUE AND wp.ParentID = {webPageId}" +
                      $" ORDER BY wp.WebPageTitle ASC";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(39);
                webPage.TypeName = GetTableValue<string>(40);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            });
        }

        public IEnumerable<WebPage> GetAllWebPagesForSiteMapGenerator()
        {
            var sql = "SELECT wp.WebPageID as 0, wp.UrlName as 1, wp.WebPageName as 2, wp.IsDeleted as 3," +
                      " wp.WebPageTitle as 4, wp.ContentMain as 5, wp.ParentID as 6, wp.IsSubjectPage as 7," +
                      " wp.DisplayOrder as 8, wp.IsEnabled as 9, wp.NotesInternal as 10," +
                      " wp.CreatedDT as 11, wp.UpdatedDT as 12, wp.BannerImageFileName as 13," +
                      " wp.SocialImageFileName as 14, wp.MetaDescription as 15, " +
                      " wp.PrimaryTopicID as 16, wp.SecondaryTopicID as 17," +
                      " wp.PublishedOnDT as 18, wp.PublishedUpdatedDT as 19, wp.TypeID as 20, " +
                      " t1.TopicID as 21, t1.TopicName as 22, t1.DescriptionInternal as 23, t1.PrimaryWebPageId as 24, t1.NotesInternal as 25, t1.SocialImageFilename as 26," +
                      " t1.SocialImageFilenameNews as 27, t1.BannerImageFileName as 28, t1.Hashtags as 29,  t1.ThumbnailImageFilename as 30," +
                      " t2.TopicID as 31, t2.TopicName as 32, t2.DescriptionInternal as 33, t2.PrimaryWebPageId as 34, t2.NotesInternal as 35," +
                      " t2.SocialImageFilename as 36, t2.SocialImageFilenameNews as 37, t2.BannerImageFileName as 38, t2.Hashtags as 39 , t2.ThumbnailImageFilename as 40," +
                      " wpp.WebPageName as 41, wpt.WebPageTypeName as 42 " +
                      $" FROM (((({TableNameWebPages} wp" +
                      $" LEFT JOIN {TableNameTopics} t1 ON wp.PrimaryTopicID = t1.TopicID)" +
                      $" LEFT JOIN {TableNameTopics} t2 ON wp.SecondaryTopicID = t2.TopicID)" +
                      $" LEFT JOIN {TableNameWebPages} wpp ON wp.ParentID = wpp.WebPageID)" +
                      $" LEFT JOIN {TablenameWebPageTypes} wpt ON wp.TypeID = wpt.WebPageTypeID)" +
                      $" WHERE wp.IsDeleted = FALSE AND wp.IsEnabled = TRUE" +
                      $" ORDER BY wp.UrlName ASC";

            return QueryAllData(sql, list =>
            {
                var webPage = MapWebPage(0);
                webPage.ParentName = GetTableValue<string>(39);
                webPage.TypeName = GetTableValue<string>(40);

                if (GetTableValue<int?>(21) != null)
                    webPage.PrimaryTopic = MapTopic(21);
                if (GetTableValue<int?>(31) != null)
                    webPage.SecondaryTopic = MapTopic(31);

                list.Add(webPage);
            });
        }
    }
}
