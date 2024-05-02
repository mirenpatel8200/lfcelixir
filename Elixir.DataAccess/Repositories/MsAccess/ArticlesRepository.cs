using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text.RegularExpressions;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class ArticlesRepository : AbstractRepository<Article>, IArticlesRepository
    {
        public override void Insert(Article entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameArticles} (ArticleTitle, OriginalTitle, DnPublisherName," +
                    $" PublisherURL, ArticleDate, PrimaryTopicID, SecondaryTopicID, BulletPoints, IsEnabled," +
                    $" NotesInternal, IsDeleted, CreatedDT, UpdatedDT, Summary, UrlName, IDHashCode, " +
                    $" SocialImageFilename, PublisherResourceId, ReporterResourceID, DnReporterName, DisplaySocialImage, " +
                    $" IsHumour, CreatedByUserId, UpdatedByUserId) " +
                    $" VALUES (@ArticleTitle, @OriginalTitle, @DnPublisherName, @PublisherURL," +
                    $" @ArticleDate, @PrimaryTopicID, @SecondaryTopicID, @BulletPoints, @IsEnabled," +
                    $" @NotesInternal, @IsDeleted, @CreatedDT, @UpdatedDT, @Summary, @UrlName," +
                    $" @IDHashCode, @SocialImageFilename, @PublisherResourceId, @ReporterResourceID, @DnReporterName, @DisplaySocialImage," +
                    $" @IsHumour, @CreatedByUserId, @UpdatedByUserId)");

                command.Parameters.AddWithValue("@ArticleTitle", !string.IsNullOrEmpty(entity.Title) ? entity.Title.Trim() : entity.Title);
                command.Parameters.AddWithValue("@OriginalTitle", SafeGetStringValue(!string.IsNullOrEmpty(entity.OriginalTitle) ? entity.OriginalTitle.Trim() : entity.OriginalTitle));
                command.Parameters.AddWithValue("@DnPublisherName", SafeGetStringValue(entity.DnPublisherName));
                command.Parameters.AddWithValue("@PublisherURL", !string.IsNullOrEmpty(entity.PublisherUrl) ? entity.PublisherUrl.Trim() : entity.PublisherUrl);
                command.Parameters.AddWithValue("@ArticleDate", GetDateWithoutMilliseconds(entity.ArticleDate));
                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopic.Id);

                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopic != null && entity.SecondaryTopic.Id.HasValue ? entity.SecondaryTopic.Id.Value : (object)DBNull.Value);

                command.Parameters.AddWithValue("@BulletPoints", SafeGetStringValue(entity.BulletPoints));
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(!string.IsNullOrEmpty(entity.Notes) ? entity.Notes.Trim() : entity.Notes));

                // There's no need to add a new article already with "IsDeleted" set to true.
                // Deleting can be made separately after adding.
                command.Parameters.AddWithValue("@IsDeleted", false);

                command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.Created));
                command.Parameters.AddWithValue("@UpdatedDT", entity.Updated.HasValue ? GetDateWithoutMilliseconds(entity.Updated.Value) : (object)DBNull.Value);

                command.Parameters.AddWithValue("@Summary", SafeGetStringValue(!string.IsNullOrEmpty(entity.Summary) ? entity.Summary.Trim() : entity.Summary));
                command.Parameters.AddWithValue("@UrlName", SafeGetStringValue(entity.UrlName));
                command.Parameters.AddWithValue("@IDHashCode", SafeGetStringValue(entity.IdHashCode));
                command.Parameters.AddWithValue("@SocialImageFilename", SafeGetStringValue(entity.SocialImageFilename));
                command.Parameters.AddWithValue("@PublisherResourceId", entity.PublisherResourceId ?? (object)DBNull.Value);

                command.Parameters.AddWithValue("@ReporterResourceID", entity.ReporterResourceId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DnReporterName", SafeGetStringValue(entity.DnReporterName));
                command.Parameters.AddWithValue("@DisplaySocialImage", entity.DisplaySocialImage);
                command.Parameters.AddWithValue("@IsHumour", entity.IsHumour);

                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedByUserId.HasValue ? entity.CreatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Article to the database");
            }
        }

        /// <summary>
        /// Updates specified entity. Note that Id is not updated using this method.
        /// </summary>
        /// <param name="entity"></param>
        public override void Update(Article entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameArticles} SET ArticleTitle = @ArticleTitle, OriginalTitle = @OriginalTitle, DnPublisherName = @DnPublisherName, " +
                        $"PublisherURL = @PublisherURL, ArticleDate = @ArticleDate, PrimaryTopicID = @PrimaryTopicID, " +
                        $"SecondaryTopicID = @SecondaryTopicID, BulletPoints = @BulletPoints, IsEnabled = @IsEnabled, NotesInternal = @NotesInternal, UpdatedDT = @UpdatedDT," +
                    $" Summary = @Summary, UrlName = @UrlName " + (entity.IdHashCode != null ? ", IDHashCode = @IDHashCode" : "") + ", SocialImageFilename = @SocialImageFilename," +
                    " PublisherResourceId = @PublisherResourceId, ReporterResourceID = @ReporterResourceID, DnReporterName = @DnReporterName, DisplaySocialImage = @DisplaySocialImage, " +
                    " IsHumour = @IsHumour, UpdatedByUserId = @UpdatedByUserId " +
                    $" WHERE ArticleID = {entity.Id}");

                command.Parameters.AddWithValue("@ArticleTitle", !string.IsNullOrEmpty(entity.Title) ? entity.Title.Trim() : entity.Title);
                command.Parameters.AddWithValue("@OriginalTitle", SafeGetStringValue(!string.IsNullOrEmpty(entity.OriginalTitle) ? entity.OriginalTitle.Trim() : entity.OriginalTitle));
                command.Parameters.AddWithValue("@DnPublisherName", SafeGetStringValue(entity.DnPublisherName));
                command.Parameters.AddWithValue("@PublisherURL", SafeGetStringValue(!string.IsNullOrEmpty(entity.PublisherUrl) ? entity.PublisherUrl.Trim() : entity.PublisherUrl));
                command.Parameters.AddWithValue("@ArticleDate", entity.ArticleDate);
                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopic.Id);
                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopic != null && entity.SecondaryTopic.Id.HasValue ? entity.SecondaryTopic.Id : (object)DBNull.Value);
                command.Parameters.AddWithValue("@BulletPoints", SafeGetStringValue(entity.BulletPoints));
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(!string.IsNullOrEmpty(entity.Notes) ? entity.Notes.Trim() : entity.Notes));
                command.Parameters.AddWithValue("@UpdatedDT", entity.Updated.HasValue ? GetDateWithoutMilliseconds(entity.Updated.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@Summary", SafeGetStringValue(!string.IsNullOrEmpty(entity.Summary) ? entity.Summary.Trim() : entity.Summary));
                command.Parameters.AddWithValue("@UrlName", SafeGetStringValue(entity.UrlName));

                if (entity.IdHashCode != null)
                    command.Parameters.AddWithValue("@IDHashCode", SafeGetStringValue(entity.IdHashCode));

                command.Parameters.AddWithValue("@SocialImageFilename", SafeGetStringValue(entity.SocialImageFilename));
                command.Parameters.AddWithValue("@PublisherResourceId", entity.PublisherResourceId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ReporterResourceID", entity.ReporterResourceId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DnReporterName", SafeGetStringValue(entity.DnReporterName));
                command.Parameters.AddWithValue("@DisplaySocialImage", entity.DisplaySocialImage);
                command.Parameters.AddWithValue("@IsHumour", entity.IsHumour);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing Article in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameArticles} SET IsDeleted = 1 WHERE ArticleID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting Article in the database");
            }
        }

        public override IEnumerable<Article> GetAll()
        {
            var sql =
                $"SELECT a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15," +
                $" a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28, t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37, t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40, t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42, t2.Hashtags as 43,  t2.ThumbnailImageFilename as 44 " +
                $" FROM (({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicID) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicID";

            return QueryAllData(sql, list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);

                if (GetTableValue<int?>(7).HasValue)
                    article.SecondaryTopic = MapTopic(35);

                list.Add(article);
            });
        }

        public IEnumerable<Article> SearchSQL(List<string> terms, bool all = false)
        {
            var parameters = new Dictionary<string, object>();
            var sql =
                $"SELECT {(all ? "" : "TOP 20")} a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15," +
                $" a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28, t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37, t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40, t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42, t2.Hashtags as 43, t2.ThumbnailImageFilename as 44 " +
                $" FROM (({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicID) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicID";

            if (terms.Count() > 0)
            {
                string whereClause = " WHERE ";
                string regexPunctuation = "% [().,;:-_!/?" + "\"" + "]";
                for (int i = 0; i < terms.Count; i++)
                {
                    string term = terms[i], alias = $"term{i}";
                    string condition = SqlHelper.ConditionArticleContains(alias);
                    whereClause += condition;

                    parameters.Add($"@{alias}", $"% {term}%");
                    parameters.Add($"@start{alias}", $"{term}%");
                    parameters.Add($"@punctuation{alias}", regexPunctuation + term + " %");

                    if (i < terms.Count - 1)
                        whereClause += " AND ";
                }

                if (all == false)
                {
                    var date2YearsAgo = GetDateWithoutMilliseconds(DateTime.Now.AddYears(-2));
                    whereClause += " AND a.ArticleDate >= @date2YearsAgo";
                    parameters.Add("@date2YearsAgo", date2YearsAgo);
                }
                sql += whereClause;
            }

            sql += " ORDER BY a.ArticleDate DESC";

            return QueryAllData(sql, list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);

                if (GetTableValue<int?>(7).HasValue)
                    article.SecondaryTopic = MapTopic(35);

                list.Add(article);

            }, parameters);
        }

        public Article GetArticleById(int id)
        {
            var sql =
                $"SELECT a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15, a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24, " +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28, t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34, " +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37, t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40, t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42, t2.Hashtags as 43, t2.ThumbnailImageFilename as 44, " +
                $" u.UserNameFirst as 45, u.UserNameLast as 46" +
                $" FROM ((({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicId) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicId)" +
                $" LEFT JOIN {TableNameUsers} u ON a.UpdatedByUserId = u.UserID" +
                $" WHERE a.ArticleID = @Id";

            return QueryAllData(sql, (list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);
                article.SecondaryTopic = MapTopic(35);

                if (GetTableValue<int?>(24).HasValue)
                {
                    var username = MapUser(45);
                    article.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }

                list.Add(article);
            }), new Dictionary<string, object>() { { "Id", id } }).FirstOrDefault();
        }

        //public bool IsArticleExists(string urlName)
        //{
        //    using (MsAccessDbManager = new MsAccessDbManager())
        //    {
        //        var sql = $"SELECT COUNT(*) FROM {TableNameArticles} a WHERE a.ArticleID = @UrlName";

        //        var command = MsAccessDbManager.CreateCommand(sql);
        //        command.Parameters.AddWithValue("@UrlName", urlName);

        //        var result = (int)command.ExecuteScalar();

        //        return result > 0;
        //    }
        //}

        public bool IsNonDeletedArticleExists(string urlName, int? excludeArticleId = null)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = excludeArticleId.HasValue
                    ? $"SELECT COUNT(*) FROM {TableNameArticles} a WHERE a.ArticleID <> @ExcludeId AND a.IsDeleted = FALSE AND a.UrlName = @UrlName"
                    : $"SELECT COUNT(*) FROM {TableNameArticles} a WHERE a.IsDeleted = FALSE AND a.UrlName = @UrlName";

                var command = MsAccessDbManager.CreateCommand(sql);

                if (excludeArticleId.HasValue)
                    command.Parameters.AddWithValue("@ExcludeId", excludeArticleId.Value);
                command.Parameters.AddWithValue("@UrlName", urlName);

                var result = (int)command.ExecuteScalar();

                return result > 0;
            }
        }

        public Article GetArticleByUrlName(string urlName)
        {
            var sql =
                $"SELECT a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15, a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28," +
                $" t1.NotesInternal as 29, t1.SocialImageFilename as 30," +
                $" t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32," +
                $" t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37," +
                $" t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40," +
                $" t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42," +
                $" t2.Hashtags as 43 , t2.ThumbnailImageFilename as 44" +
                $" FROM (({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicId) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicId" +
                $" WHERE a.UrlName = @UrlName";

            return QueryAllData(sql, (list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);
                article.SecondaryTopic = MapTopic(35);

                list.Add(article);
            }), new Dictionary<string, object>() { { "UrlName", urlName } }).FirstOrDefault();
        }

        public Article GetArticleByPublisherUrl(string publisherUrl, int? idToExclude = null)
        {
            var publisherUrlWithTrailingSlash = $"/{publisherUrl}/";
            var publisherUrlWithLeadingSlash = $"/{publisherUrl}";
            var sql =
                $"SELECT a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15, a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28," +
                $" t1.NotesInternal as 29, t1.SocialImageFilename as 30," +
                $" t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32," +
                $" t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37," +
                $" t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40," +
                $" t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42," +
                $" t2.Hashtags as 43, t2.ThumbnailImageFilename as 44" +
                $" FROM (({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicId) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicId" +
                $" WHERE a.IsDeleted = FALSE AND (a.PublisherURL LIKE @PublisherUrlWithLeadingSlash OR a.PublisherURL LIKE @PublisherUrlWithTrailingSlash)";

            if (idToExclude.HasValue)
            {
                sql += $" AND a.ArticleID <> {idToExclude.Value}";
            }

            return QueryAllData(sql, list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);
                article.SecondaryTopic = MapTopic(35);

                list.Add(article);
            }, new Dictionary<string, object>() { { "PublisherUrlWithLeadingSlash", $"%{publisherUrlWithLeadingSlash}" }, { "PublisherUrlWithTrailingSlash", $"%{publisherUrlWithTrailingSlash}" } }).FirstOrDefault();
        }

        public IEnumerable<Article> GetWebPageRelatedArticles(int webPageId, int? maxCount, DateTime? startingArticleDate)
        {
            var sql =
                $"SELECT DISTINCT {(maxCount.HasValue ? $"TOP {maxCount.Value}" : "")} a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                " a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                " a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15, a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                " a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24" +
                $" FROM ((({TableNameWebPages} wp" +
                $" INNER JOIN {TableNameWebPageTopic} wpt ON wpt.WebPageID = wp.WebPageID)" +
                $" INNER JOIN {TableNameTopics} t ON wpt.TopicID = t.TopicID)" +
                $" INNER JOIN {TableNameArticles} a ON a.PrimaryTopicID = t.TopicID OR a.SecondaryTopicID = t.TopicID)" +
                " WHERE wp.WebPageID = @WebPageId AND a.IsDeleted = FALSE AND a.IsEnabled = TRUE " + (startingArticleDate.HasValue ? " AND a.ArticleDate > @StartDate" : "") +
                " ORDER BY a.ArticleDate DESC, a.ArticleID ASC";

            var args = new Dictionary<string, object>()
            {
                { "WebPageId", webPageId.ToString() }
            };

            if (startingArticleDate.HasValue)
                args.Add("StartDate", GetDateWithoutMilliseconds(startingArticleDate.Value));

            return QueryAllData(sql, (list =>
            {
                var article = MapArticle(0);
                //article.PrimaryTopic = MapTopic(16);

                //if (GetTableValue<int?>(7).HasValue)
                //    article.SecondaryTopic = MapTopic(19);

                list.Add(article);
            }), args);
        }

        public int CountWebPageRelatedArticles(int webpageId, DateTime? startArticleDate)
        {
            var sql = $"SELECT COUNT(a.ArticleID) " +
              $" FROM ((({TableNameWebPages} wp" +
              $" INNER JOIN {TableNameWebPageTopic} wpt ON wpt.WebPageID = wp.WebPageID)" +
              $" INNER JOIN {TableNameTopics} t ON wpt.TopicID = t.TopicID)" +
              $" INNER JOIN {TableNameArticles} a ON a.PrimaryTopicID = t.TopicID OR a.SecondaryTopicID = t.TopicID)" +
              " WHERE wp.WebPageID = @WebPageId AND a.IsDeleted = FALSE AND a.IsEnabled = TRUE " +
              (startArticleDate.HasValue ? " AND a.ArticleDate > @StartDate" : "");
            var args = new Dictionary<string, object>()
            {
                { "WebPageId", webpageId.ToString() }
            };

            if (startArticleDate.HasValue)
                args.Add("StartDate", GetDateWithoutMilliseconds(startArticleDate.Value));

            var count = QueryScalar(sql, args);
            return count;
        }

        public Article GetArticleByHash(string hash)
        {
            var sql =
                $"SELECT a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15, a.UrlName as 16, a.IDHashCode as 17," +
                $" a.SocialImageFilename as 18, a.ReporterResourceID as 19, a.DnReporterName as 20," +
                $" a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24, " +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28," +
                $" t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31," +
                $" t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37," +
                $" t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40," +
                $" t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42, t2.Hashtags as 43,  t2.ThumbnailImageFilename as 44" +
                $" FROM (({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicID) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicID" +
                $" WHERE a.IDHashCode = @IDHashCode";

            return QueryAllData(sql, (list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);
                article.SecondaryTopic = MapTopic(35);

                list.Add(article);
            }), new Dictionary<string, object>() { { "IDHashCode", hash } }).FirstOrDefault();
        }

        public IEnumerable<Article> GetN(int count, ArticlesSortField[] sortFields, SortDirection[] sortDirections, int TopicFilter)
        {
            var sql =
                $"SELECT TOP {count} a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15, a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28," +
                $" t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37, t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40, t2.SocialImageFilenameNews as 41," +
                $" t2.BannerImageFileName as 42, t2.Hashtags as 43,   t2.ThumbnailImageFilename as 44," +
                $" u.UserNameFirst as 45, u.UserNameLast as 46" +
                $" FROM ((({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicId) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicId)" +
                $" LEFT JOIN {TableNameUsers} u ON a.UpdatedByUserId = u.UserID" +
                $" WHERE a.IsDeleted = FALSE";

            if (TopicFilter > 0)
            {
                sql = sql + " AND a.PrimaryTopicID=" + TopicFilter + " or  a.SecondaryTopicID=" + TopicFilter;
            }
            const string sortingTableName = "a";
            var tableNames = Enumerable.Repeat(sortingTableName, sortFields.Length);

            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()).ToArray(), sortDirections, tableNames.ToArray());
            ctx.ReplaceField(ArticlesSortField.PrimaryTopic.ToString(), "TopicName", "t1");

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);

                if (GetTableValue<int?>(7).HasValue)
                    article.SecondaryTopic = MapTopic(35);
                if (GetTableValue<int?>(24).HasValue)
                {
                    var username = MapUser(45);
                    article.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }

                list.Add(article);
            });
        }

        public IEnumerable<Article> GetFiltered(string filter, ArticlesSortField[] sortFields, SortDirection[] sortDirections, bool IncludeAllFields, int TopicFilter)
        {
            bool filterExists = !string.IsNullOrEmpty(filter);
            var parameters = new Dictionary<string, object>();

            var sql =
                $"SELECT a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15, a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28," +
                $" t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37, t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40, t2.SocialImageFilenameNews as 41," +
                $" t2.BannerImageFileName as 42, t2.Hashtags as 43, t2.ThumbnailImageFilename as 44," +
                $" u.UserNameFirst as 45, u.UserNameLast as 46" +
                $" FROM ((({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicId) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicId)" +
                $" LEFT JOIN {TableNameUsers} u ON a.UpdatedByUserId = u.UserID";
            if (filterExists)
            {
                if (IncludeAllFields)
                {
                    sql += " WHERE (a.ArticleTitle LIKE '%{{filter}}%' or a.BulletPoints LIKE '%{{filter}}%' or a.Summary LIKE '%{{filter}}%') ";
                    sql = sql.Replace("{{filter}}", filter);
                }
                else
                {
                    sql += " WHERE (a.ArticleTitle LIKE '%{{filter}}%') ";
                    sql = sql.Replace("{{filter}}", filter);
                }
                if (TopicFilter > 0)
                {
                    sql = sql + " and (a.PrimaryTopicID=" + TopicFilter + " or  a.SecondaryTopicID=" + TopicFilter + ")";
                }
            }
            else
            {
                if (TopicFilter > 0)
                {
                    sql = sql + " where a.PrimaryTopicID=" + TopicFilter + " or  a.SecondaryTopicID=" + TopicFilter;
                }
            }

            const string sortingTableName = "a";
            var tableNames = Enumerable.Repeat(sortingTableName, sortFields.Length);

            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()).ToArray(), sortDirections, tableNames.ToArray());
            ctx.ReplaceField(ArticlesSortField.PrimaryTopic.ToString(), "TopicName", "t1");

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, (list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);

                if (GetTableValue<int?>(7).HasValue)
                    article.SecondaryTopic = MapTopic(35);
                if (GetTableValue<int?>(24).HasValue)
                {
                    var username = MapUser(45);
                    article.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }
                list.Add(article);
            }), parameters);
        }

        public void InsertMentionedResources(int articleId, List<int> resourceIds, int resourceType)
        {
            OleDbCommand command;
            string sqlBase = $"INSERT INTO {TableNameArticleResources} (ArticleID, ResourceID, DnResourceTypeID) VALUES (@ArticleID, @ResourceID, @ResourceTypeID)";

            for (int i = 0; i < resourceIds.Count; i++)
            {
                using (MsAccessDbManager = new MsAccessDbManager())
                {
                    command = MsAccessDbManager.CreateCommand(sqlBase);
                    command.Parameters.AddWithValue($"@ArticleID", articleId);
                    command.Parameters.AddWithValue($"@ResourceID", resourceIds[i]);
                    command.Parameters.AddWithValue($"@ResourceTypeID", resourceType);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected != 1)
                        throw new InvalidOperationException("Error during saving Article related Organisations to the database");
                }
            }
        }

        public void DeleteMentionedResources(int articleId, int resourceType)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"DELETE FROM {TableNameArticleResources} WHERE ArticleID = @ArticleID AND DnResourceTypeID = @ResourceTypeID");
                command.Parameters.AddWithValue("@ArticleID", articleId);
                command.Parameters.AddWithValue("@ResourceTypeID", resourceType);

                var rowsAffected = command.ExecuteNonQuery();
            }
        }

        public IEnumerable<Article> GetArticlesMentioningResource(int resourceId)
        {
            var date2YearsAgo = GetDateWithoutMilliseconds(DateTime.Now.AddYears(-2));

            var sql =
                $"SELECT TOP 20 a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15," +
                $" a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28, t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37, t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40, t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42, t2.Hashtags as 43 , t2.ThumbnailImageFilename as 44 " +
                $" FROM ((({TableNameArticles} a) " +
                $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicID) " +
                $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicID)" +
                $" LEFT JOIN {TableNameArticleResources} ar ON a.ArticleID = ar.ArticleID" +
                $" WHERE ar.ResourceID = @ResourceID AND a.ArticleDate >= @Date2YearsAgo AND a.IsDeleted = FALSE" +
                $" ORDER BY a.ArticleDate DESC";

            return QueryAllData(sql, list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);

                if (GetTableValue<int?>(7).HasValue)
                    article.SecondaryTopic = MapTopic(35);

                list.Add(article);

            }, new Dictionary<string, object>
            {
                { "ResourceID", resourceId },
                { "Date2YearsAgo", date2YearsAgo}
            });
        }

        public IEnumerable<Article> GetLatestArticles(int limit)
        {
            var sql = $"SELECT TOP {limit} a.ArticleID as 0, a.ArticleTitle as 1, a.OriginalTitle as 2, a.PublisherResourceId as 3, a.DnPublisherName as 4, a.PublisherURL as 5," +
                      $" a.ArticleDate as 6, a.PrimaryTopicID as 7, a.SecondaryTopicID as 8, a.BulletPoints as 9, a.IsEnabled as 10," +
                      $" a.NotesInternal as 11, a.IsDeleted as 12, a.CreatedDT as 13, a.UpdatedDT as 14, a.Summary as 15," +
                      $" a.UrlName as 16, a.IDHashCode as 17, a.SocialImageFilename as 18," +
                      $" a.ReporterResourceID as 19, a.DnReporterName as 20, a.DisplaySocialImage as 21, a.IsHumour as 22, a.CreatedByUserId as 23, a.UpdatedByUserId as 24," +
                      $" t1.TopicID as 25, t1.TopicName as 26, t1.DescriptionInternal as 27, t1.PrimaryWebPageId as 28, t1.NotesInternal as 29, t1.SocialImageFilename as 30, t1.SocialImageFilenameNews as 31, t1.BannerImageFileName as 32, t1.Hashtags as 33, t1.ThumbnailImageFilename as 34," +
                      $" t2.TopicID as 35, t2.TopicName as 36, t2.DescriptionInternal as 37, t2.PrimaryWebPageId as 38, t2.NotesInternal as 39, t2.SocialImageFilename as 40, t2.SocialImageFilenameNews as 41, t2.BannerImageFileName as 42, t2.Hashtags as 43,  t2.ThumbnailImageFilename as 44 " +
                      $" FROM (({TableNameArticles} a) " +
                      $" LEFT JOIN {TableNameTopics} t1 ON a.PrimaryTopicID = t1.TopicID) " +
                      $" LEFT JOIN {TableNameTopics} t2 ON a.SecondaryTopicID = t2.TopicID" +
                      $" WHERE a.IsDeleted = FALSE AND a.IsEnabled = TRUE" +
                      $" ORDER BY a.ArticleDate DESC";

            return QueryAllData(sql, list =>
            {
                var article = MapArticle(0);
                article.PrimaryTopic = MapTopic(25);

                if (GetTableValue<int?>(7).HasValue)
                    article.SecondaryTopic = MapTopic(35);

                list.Add(article);
            });
        }
    }
}
