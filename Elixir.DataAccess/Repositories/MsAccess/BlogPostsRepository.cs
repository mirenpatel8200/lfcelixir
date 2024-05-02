using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class BlogPostsRepository : AbstractRepository<BlogPost>, IBlogPostsRepository
    {
        public override void Insert(BlogPost entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameBlogPost} (BlogPostTitle, UrlName, IsDeleted, ContentMain, PrimaryTopicID," +
                    " SecondaryTopicID, IsEnabled, CreatedDT, UpdatedDT, PreviousBlogPostUrlName, NextBlogPostUrlName, " +
                    " NotesInternal, PublishedOnDT, SocialImageFilename, BlogPostDescriptionPublic," +
                    " PreviousBlogPostTitle, NextBlogPostTitle, PublishedUpdatedDT, ThumbnailImageFilename) VALUES " +
                    "(@BlogPostTitle, @UrlName, @IsDeleted, @ContentMain, @PrimaryTopicID, @SecondaryTopicID, @IsEnabled, @CreatedDT, @UpdatedDT," +
                    " @PreviousBlogPostUrlName, @NextBlogPostUrlName, @NotesInternal, " +
                    " @PublishedOnDT, @SocialImageFilename, @BlogPostDescriptionPublic, " +
                    " @PreviousBlogPostTitle, @NextBlogPostTitle, @PublishedUpdatedDT, @ThumbnailImageFilename)");



                command.Parameters.AddWithValue("@BlogPostTitle", entity.BlogPostTitle.Trim());
                command.Parameters.AddWithValue("@UrlName", entity.UrlName);
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@ContentMain", SafeGetStringValue(entity.ContentMain));

                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopicId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopicId ?? (object)DBNull.Value);

                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.CreatedDt));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedDt.HasValue ? GetDateWithoutMilliseconds(entity.UpdatedDt.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PreviousBlogPostUrlName", SafeGetStringValue(entity.PreviousBlogPostUrlName));
                command.Parameters.AddWithValue("@NextBlogPostUrlName", SafeGetStringValue(entity.NextBlogPostUrlName));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(!string.IsNullOrEmpty(entity.NotesInternal) ? entity.NotesInternal.Trim() : entity.NotesInternal));
                command.Parameters.AddWithValue("@PublishedOnDT", GetDateWithoutMilliseconds(entity.PublishedOnDT));
                command.Parameters.AddWithValue("@SocialImageFilename", SafeGetStringValue(entity.SocialImageFilename));
                command.Parameters.AddWithValue("@BlogPostDescriptionPublic", SafeGetStringValue(!string.IsNullOrEmpty(entity.BlogPostDescriptionPublic) ? entity.BlogPostDescriptionPublic.Trim() : entity.BlogPostDescriptionPublic));

                command.Parameters.AddWithValue("@PreviousBlogPostTitle", SafeGetStringValue(entity.PreviousBlogPostTitle));
                command.Parameters.AddWithValue("@NextBlogPostTitle", SafeGetStringValue(entity.NextBlogPostTitle));

                command.Parameters.AddWithValue("@PublishedUpdatedDT", entity.PublishedUpdatedDT.HasValue ?
                    GetDateWithoutMilliseconds(entity.PublishedUpdatedDT.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ThumbnailImageFilename", SafeGetStringValue(entity.ThumbnailImageFilename));
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Blog post to the database");
            }
        }

        public override void Update(BlogPost entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                string sql =
                    $"UPDATE {TableNameBlogPost} SET BlogPostTitle = @BlogPostTitle, UrlName = @UrlName, IsDeleted = @IsDeleted, ContentMain = @ContentMain," +
                    $" PrimaryTopicID = @PrimaryTopicID, SecondaryTopicID = @SecondaryTopicID, IsEnabled = @IsEnabled, " +
                    $" PreviousBlogPostUrlName = @PreviousBlogPostUrlName, NextBlogPostUrlName = @NextBlogPostUrlName," +
                    $" NotesInternal = @NotesInternal, PublishedOnDT = @PublishedOnDT, " +
                    " PublishedUpdatedDT = @PublishedUpdatedDT, " +
                    " CreatedDT = @CreatedDT, UpdatedDT = @UpdatedDT, SocialImageFilename = @SocialImageFilename," +
                    " BlogPostDescriptionPublic = @BlogPostDescriptionPublic, " +
                    " PreviousBlogPostTitle = @PreviousBlogPostTitle," +
                    " NextBlogPostTitle = @NextBlogPostTitle, " +
                    " ThumbnailImageFilename = @ThumbnailImageFilename " +

                    $" WHERE BlogPostID = {entity.Id}";

                var command = MsAccessDbManager.CreateCommand(sql);

                command.Parameters.AddWithValue("@BlogPostTitle", entity.BlogPostTitle.Trim());
                command.Parameters.AddWithValue("@UrlName", entity.UrlName);
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@ContentMain", SafeGetStringValue(entity.ContentMain));

                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopicId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopicId ?? (object)DBNull.Value);

                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@PreviousBlogPostUrlName", SafeGetStringValue(entity.PreviousBlogPostUrlName));
                command.Parameters.AddWithValue("@NextBlogPostUrlName", SafeGetStringValue(entity.NextBlogPostUrlName));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(!string.IsNullOrEmpty(entity.NotesInternal) ? entity.NotesInternal.Trim() : entity.NotesInternal));
                command.Parameters.AddWithValue("@PublishedOnDT", GetDateWithoutMilliseconds(entity.PublishedOnDT));
                command.Parameters.AddWithValue("@PublishedUpdatedDT",
                    entity.PublishedUpdatedDT.HasValue ?
                    GetDateWithoutMilliseconds(entity.PublishedUpdatedDT.Value) :
                    (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.CreatedDt));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedDt.HasValue ? GetDateWithoutMilliseconds(entity.UpdatedDt.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SocialImageFilename", SafeGetStringValue(entity.SocialImageFilename));
                command.Parameters.AddWithValue("@BlogPostDescriptionPublic", SafeGetStringValue(!string.IsNullOrEmpty(entity.BlogPostDescriptionPublic) ? entity.BlogPostDescriptionPublic.Trim() : entity.BlogPostDescriptionPublic));

                command.Parameters.AddWithValue("@PreviousBlogPostTitle", SafeGetStringValue(entity.PreviousBlogPostTitle));
                command.Parameters.AddWithValue("@NextBlogPostTitle", SafeGetStringValue(entity.NextBlogPostTitle));
                command.Parameters.AddWithValue("@ThumbnailImageFilename", SafeGetStringValue(entity.ThumbnailImageFilename));
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Blog post to the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameBlogPost} SET IsDeleted = 1 WHERE BlogPostID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop product in the database");
            }
        }

        public override IEnumerable<BlogPost> GetAll()
        {
            var sql =
                "SELECT bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsDeleted = FALSE" +
                $" ORDER BY bp.CreatedDT DESC";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            });
        }

        public IEnumerable<BlogPost> GetN(int count, AdminBlogPostsSortOrder[] sortFields, SortDirection[] sortDirections)
        {
            var sql =
                $"SELECT TOP {count} bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18,  bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22," +
                " t2.TopicID as 23, t2.TopicName as 24, t2.DescriptionInternal as 25" +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsDeleted = FALSE";

            var tableNames = Enumerable.Repeat("bp", sortFields.Length);
            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()), sortDirections, tableNames);

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, list =>
            {
                var blogPost = MapBlogPost(0);
                // TODO: add topics to blogpost.

                list.Add(blogPost);
            });
        }

        //public void UpdatePublishedUpdatedDate(int id, DateTime date)
        //{
        //    //separate update on these fields
        //    using (MsAccessDbManager = new MsAccessDbManager())
        //    {
        //        var command = MsAccessDbManager.CreateCommand(
        //            $"UPDATE {TableNameBlogPost} SET PublishedUpdatedDT = @pud WHERE BlogPostID={id}");
        //        command.Parameters.AddWithValue("@pud",
        //            GetDateWithoutMilliseconds(date));

        //        var rowsAffected = command.ExecuteNonQuery();

        //        if (rowsAffected != 1)
        //            throw new InvalidOperationException("Error during updating Blog post to the database");
        //    }
        //}

        public IEnumerable<BlogPost> GetTopicRelatedBlogPosts(int webPageId, int? maxCount)
        {
            var sql =
                $"SELECT DISTINCT {(maxCount.HasValue ? $"TOP {maxCount.Value}" : "")} bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19" +
                $" FROM ((({TableNameWebPages} wp" +
                $" INNER JOIN {TableNameWebPageTopic} wpt ON wpt.WebPageID = wp.WebPageID)" +
                $" INNER JOIN {TableNameTopics} t ON wpt.TopicID = t.TopicID)" +
                $" INNER JOIN {TableNameBlogPost} bp ON bp.PrimaryTopicID = t.TopicID OR bp.SecondaryTopicID = t.TopicID)" +
                " WHERE wp.WebPageID = @WebPageId AND bp.IsDeleted = FALSE AND bp.IsEnabled = TRUE " +
                $" ORDER BY bp.PublishedUpdatedDT DESC, bp.BlogPostID ASC";

            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);
                list.Add(blogPost);
            }, new Dictionary<string, object>() { { "@WebPageId", webPageId } });
        }

        public IEnumerable<BlogPost> SearchSQL(List<string> terms, bool all = false)
        {
            var parameters = new Dictionary<string, object>();
            var sql =
                $"SELECT {(all ? "" : "TOP 10")} bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18,  bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39 " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsEnabled = TRUE AND bp.IsDeleted = FALSE ";

            if (terms.Count() > 0)
            {
                string whereClause = "";
                string regexPunctuation = "% [().,;:-_!/?" + "\"" + "]";
                for (int i = 0; i < terms.Count; i++)
                {
                    string term = terms[i], alias = $"term{i}";
                    string condition = SqlHelper.ConditionBlogPostContains(alias);
                    whereClause += " AND " + condition;

                    parameters.Add($"@{alias}", $"% {term}%");
                    parameters.Add($"@start{alias}", $"{term}%");
                    parameters.Add($"@punctuation{alias}", regexPunctuation + term + " %");

                }

                sql += whereClause;
            }

            sql += " ORDER BY bp.PublishedUpdatedDT DESC";

            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);

            }, parameters);
        }

        public void DeleteMentionedResources(int blogPostId, int resourceType)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"DELETE FROM {TableNameBlogPostResources} WHERE BlogPostID = @BlogPostID AND DnResourceTypeID  = @ResourceTypeID");
                command.Parameters.AddWithValue("@BlogPostID", blogPostId);
                command.Parameters.AddWithValue("@ResourceTypeID", resourceType);

                var rowsAffected = command.ExecuteNonQuery();
            }
        }

        public void InsertMentionedResources(int blogPostId, List<int> resourceIds, int resourceType)
        {
            OleDbCommand command;
            string sqlBase = $"INSERT INTO {TableNameBlogPostResources} (BlogPostID, ResourceID, DnResourceTypeID) VALUES (@BlogPostID, @ResourceID, @ResourceTypeID)";

            for (int i = 0; i < resourceIds.Count; i++)
            {
                using (MsAccessDbManager = new MsAccessDbManager())
                {
                    command = MsAccessDbManager.CreateCommand(sqlBase);
                    command.Parameters.AddWithValue($"@BlogPostID", blogPostId);
                    command.Parameters.AddWithValue($"@ResourceID", resourceIds[i]);
                    command.Parameters.AddWithValue($"@ResourceTypeID", resourceType);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected != 1)
                        throw new InvalidOperationException("Error during saving Article related Organisations to the database");
                }
            }
        }

        public IEnumerable<BlogPost> GetBlogPostsMentioningResource(int resourceId)
        {
            var sql =
                "SELECT bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39" +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" LEFT JOIN {TableNameBlogPostResources} bpr ON bp.BlogPostID = bpr.BlogPostID" +
                " WHERE bpr.ResourceId = @ResourceID AND " +
                " bp.IsEnabled = TRUE AND bp.IsDeleted = FALSE";

            return QueryAllData(sql, list =>
            {
                var blogPost = MapBlogPost(0);
                blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6).HasValue)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);

            }, new Dictionary<string, object> { { "ResourceID", resourceId } });
        }

        public BlogPost GetBlogPostByUrlName(string urlName)
        {
            var sql =
                "SELECT bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE LCase(bp.UrlName) = @UrlName AND bp.IsDeleted = FALSE AND bp.IsEnabled = TRUE";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            }, new Dictionary<string, object>() { { "UrlName", urlName.ToLower() } }).FirstOrDefault();
        }

        public BlogPost GetBlogPostById(int id)
        {
            var sql =
                "SELECT bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.BlogPostID = {id}";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            }).FirstOrDefault();
        }

        public IEnumerable<BlogPost> GetBlogPostsRelatedByTopic(int blogPostId, int limit)
        {
            var current = GetBlogPostById(blogPostId);
            var sql =
                $"SELECT TOP {limit} bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                $" bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                $" bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                $" bp.NotesInternal as 12," +
                $" bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                $" bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                $" t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28,  t1.ThumbnailImageFilename as 29," +
                $" t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39 " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.BlogPostID <> {blogPostId} AND bp.IsEnabled = TRUE AND bp.IsDeleted = FALSE AND ";

            sql += $" (bp.PrimaryTopicID = {current.PrimaryTopic.Id} OR bp.SecondaryTopicID = {current.PrimaryTopic.Id}";
            if (current.SecondaryTopic != null && current.SecondaryTopic.Id.GetValueOrDefault() != 0)
            {
                sql += $" OR bp.SecondaryTopicID = {current.SecondaryTopic.Id.Value} OR bp.PrimaryTopicID = {current.SecondaryTopic.Id.Value}";
            }
            sql += ")";

            sql += "ORDER BY bp.PublishedUpdatedDT DESC";

            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            });
        }

        public IEnumerable<BlogPost> GetLatest(int limit)
        {
            var sql =
                $"SELECT TOP {limit} bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                $" bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                $" bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                $" bp.NotesInternal as 12," +
                $" bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                $" bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                $" t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                $" t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsDeleted = FALSE AND bp.IsEnabled = TRUE" +
                $" ORDER BY bp.PublishedUpdatedDT DESC";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            });
        }

        public IEnumerable<BlogPost> GetLatestBlogsInLastXMonths(int limit, DateTime dateXMonthsAgo)
        {
            var sql =
                $"SELECT TOP {limit} bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                $" bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                $" bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                $" bp.NotesInternal as 12," +
                $" bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                $" bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                $" t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                $" t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsDeleted = FALSE AND bp.IsEnabled = TRUE AND (bp.PublishedUpdatedDT >= @dateXMonthsAgo OR bp.PublishedOnDT >= @dateXMonthsAgo)" +
                $" ORDER BY bp.PublishedUpdatedDT DESC";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            }, new Dictionary<string, object>() { { "dateXMonthsAgo", GetDateWithoutMilliseconds(dateXMonthsAgo) } });
        }

        public IEnumerable<BlogPost> GetEnabledBlogPosts()
        {
            var sql =
                "SELECT bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsDeleted = FALSE AND bp.IsEnabled = TRUE" +
                $" ORDER BY bp.CreatedDT DESC";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            });
        }

        public BlogPost FindNextBlogPost(DateTime moment, int? postBaseId)
        {
            var postBaseCondition = postBaseId.HasValue ? $" AND bp.BlogPostID <> {postBaseId.Value}" : "";
            var sql =
                "SELECT bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsDeleted = FALSE AND bp.IsEnabled = TRUE AND bp.CreatedDT > @moment {postBaseCondition}" +
                $" ORDER BY bp.CreatedDT DESC";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            }, new Dictionary<string, object>() { { "moment", GetDateWithoutMilliseconds(moment) } }).LastOrDefault();
        }

        public BlogPost FindPrevBlogPost(DateTime moment, int? postBaseId)
        {
            var postBaseCondition = postBaseId.HasValue ? $" AND bp.BlogPostID <> {postBaseId.Value}" : "";
            var sql =
                "SELECT bp.BlogPostID as 0, bp.BlogPostTitle as 1, bp.UrlName as 2, bp.IsDeleted as 3, bp.ContentMain as 4," +
                " bp.PrimaryTopicID as 5, bp.SecondaryTopicID as 6, bp.IsEnabled as 7, bp.CreatedDT as 8, bp.UpdatedDT as 9," +
                " bp.PreviousBlogPostUrlName as 10, bp.NextBlogPostUrlName as 11," +
                " bp.NotesInternal as 12," +
                " bp.PublishedOnDT as 13, bp.PublishedUpdatedDT as 14, bp.SocialImageFilename as 15, bp.BlogPostDescriptionPublic as 16," +
                " bp.PreviousBlogPostTitle as 17, bp.NextBlogPostTitle as 18, bp.ThumbnailImageFilename as 19," +
                " t1.TopicID as 20, t1.TopicName as 21, t1.DescriptionInternal as 22, t1.PrimaryWebPageId as 23, t1.NotesInternal as 24, t1.SocialImageFilename as 25, t1.SocialImageFilenameNews as 26, t1.BannerImageFileName as 27, t1.Hashtags as 28, t1.ThumbnailImageFilename as 29," +
                " t2.TopicID as 30, t2.TopicName as 31, t2.DescriptionInternal as 32, t2.PrimaryWebPageId as 33, t2.NotesInternal as 34, t2.SocialImageFilename as 35, t2.SocialImageFilenameNews as 36, t2.BannerImageFileName as 37, t2.Hashtags as 38, t2.ThumbnailImageFilename as 39  " +
                $" FROM (({TableNameBlogPost} bp" +
                $" LEFT JOIN {TableNameTopics} t1 ON bp.PrimaryTopicID = t1.TopicID)" +
                $" LEFT JOIN {TableNameTopics} t2 ON bp.SecondaryTopicID = t2.TopicID)" +
                $" WHERE bp.IsDeleted = FALSE AND bp.IsEnabled = TRUE AND bp.CreatedDT <= @moment {postBaseCondition}" +
                $" ORDER BY bp.CreatedDT DESC";
            return QueryAllData(sql, (list) =>
            {
                var blogPost = MapBlogPost(0);

                if (GetTableValue<int?>(5) != null)
                    blogPost.PrimaryTopic = MapTopic(20);

                if (GetTableValue<int?>(6) != null)
                    blogPost.SecondaryTopic = MapTopic(30);

                list.Add(blogPost);
            }, new Dictionary<string, object>() { { "moment", GetDateWithoutMilliseconds(moment) } }).FirstOrDefault();
        }

        public bool IsNonDeletedBlogPostExists(string urlName, int? excludeBlogPostId)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = excludeBlogPostId.HasValue
                     ? $"SELECT COUNT(*) FROM {TableNameBlogPost} bp WHERE bp.BlogPostID <> @ExcludeId AND bp.IsDeleted = 0 AND bp.UrlName = @UrlName"
                     : $"SELECT COUNT(*) FROM {TableNameBlogPost} bp WHERE bp.IsDeleted = 0 AND bp.UrlName = @UrlName";

                var command = MsAccessDbManager.CreateCommand(sql);

                if (excludeBlogPostId.HasValue)
                    command.Parameters.AddWithValue("@ExcludeId", excludeBlogPostId.Value);
                command.Parameters.AddWithValue("@UrlName", urlName);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
