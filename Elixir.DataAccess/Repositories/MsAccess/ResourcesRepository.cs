using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elixir.Models.Enums;
using Elixir.Utils;
using System.Data.OleDb;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class ResourcesRepository : AbstractRepository<Resource>, IResourcesRepository
    {
        public ResourcesRepository()
        {
        }
        //TODO: check if soft delete is wanted
        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameResources} SET IsDeleted = 1 WHERE ResourceID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting Resource in the database");
            }
        }

        public override IEnumerable<Resource> GetAll()
        {
            var sql = $"SELECT r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6, " +
                    " r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                    " r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                    " r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                    " r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                    " r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                    " r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                    $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                    $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                    $" r.IsHealthOrg as 49, r.IsHealthPro as 50," +
                    $" r.IsHiddenPublic as 51, r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                    $" u.UserNameFirst as 60, u.UserNameLast as 61, c.CountryAutoID as 62, c.CountryName as 63, c.ContinentCode as 64, c.CountryID as 65" +
                    $" FROM ((({TableNameResources} r)" +
                    $" LEFT JOIN {TableNameUsers} u ON r.UpdatedByUserId = u.UserID)" +
                    $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)";

            return QueryAllData(sql, (list =>
            {
                var resource = MapResource(0);
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(62);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);
            }));
        }

        public IEnumerable<Resource> GetN(int count, ResourcesSortOrder[] sortFields, SortDirection[] sortDirections,
            string filter)
        {
            var filterExists = !string.IsNullOrWhiteSpace(filter);
            var parameters = new Dictionary<string, object>();

            var sql = $"SELECT TOP {count} r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6, " +
                    " r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                    " r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                    " r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                    " r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                    " r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                    " r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                    $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                    $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                    $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51, " +
                    $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                    $" u.UserNameFirst as 60, u.UserNameLast as 61, c.CountryAutoID as 62, c.CountryName as 63, c.ContinentCode as 64, c.CountryID as 65" +
                    $" FROM ((({TableNameResources} r)" +
                    $" LEFT JOIN {TableNameUsers} u ON r.UpdatedByUserId = u.UserID)" +
                    $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
                    $" WHERE r.IsDeleted = FALSE";

            if (filterExists)
            {
                sql += " AND r.ResourceName LIKE @filter";
                parameters.Add("@filter", $"%{filter}%");
            }
            else
            {
                parameters = null;
            }

            var ctx = new OrmSortingContext(sortFields.Select(x => "r." + x.ToString()), sortDirections);

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, (list =>
            {
                var resource = MapResource(0);
                if (GetTableValue<int?>(43).HasValue)
                {
                    var username = MapUser(60);
                    resource.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(62);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);
            }), parameters);
        }

        public IEnumerable<Resource> SearchResource(string term, ResourceTypes resourceTypes, int? maxCount = null, bool shortMatch = false)
        {
            term = StringUtils.FixSqlLikeClause(term);

            var resourceTypesClause = new StringBuilder("(");

            var dbValues = resourceTypes.ToDatabaseValues();
            for (var i = 0; i < dbValues.Length; i++)
            {
                var databaseValue = dbValues[i];
                resourceTypesClause.Append("ResourceTypeId = ");
                resourceTypesClause.Append(databaseValue);

                if (i != dbValues.Length - 1)
                {
                    resourceTypesClause.Append(" OR ");
                }
            }

            resourceTypesClause.Append(")");

            var sql = $"SELECT {(maxCount.HasValue ? "TOP " + maxCount.Value : "")} ResourceID, ResourceName, IsDeleted, ExternalUrl, FacebookHandle, TwitterHandle, ResourceTypeId," +
               $" IsAcademia, IsCompany, IsPublisher, IsAuthor, IsJournalist, IsCompanyRep, IsAcademic, IsBook, IsFilm, IsVideo," +
               $" IsEnabled, NotesInternal, CreatedDT, UpdatedDT," +
               $" ResourceDescriptionInternal, ContentMain, TwitterRetweets," +
               $" LinkedInUrl, YouTubeUrl, UrlName, PrimaryTopicID, SecondaryTopicID, IsHumour, " +
               $" IsInstitute, IsApplication, IsCompetition, IsInformation, IsProduct," +
               $" IsResearch, AmazonProductCode, ResourceDescriptionPublic, IsAudio, " +
               $" ParentResourceID, IsJournal, DnParentResourceName, CreatedByUserId, UpdatedByUserId, ProductionDate, " +
               $" IsAdvocate, IsArtist, IsPolitician, IsEducation, IsHealthOrg, IsHealthPro, IsHiddenPublic, IsEvent, EndDate, CountryId," +
               $" IsPinnedPrimaryTopic, IsPinnedSecondaryTopic, PinnedPrimaryTopicOrder, PinnedSecondaryTopicOrder, IsClubDiscount" +
               $" FROM {TableNameResources} WHERE IsDeleted = FALSE AND {resourceTypesClause} AND ResourceName LIKE @Term ";

            if (shortMatch)
            {
                sql += $"AND LEN(ResourceName) <= {term.Length + 3} ";
            }

            sql += " ORDER BY ResourceName";

            return QueryAllData(sql, list =>
            {
                var resource = MapResource(0);
                list.Add(resource);
            }, new Dictionary<string, object>()
            {
                {"@Term", $"%{term}%" }
            });
        }

        public IEnumerable<Resource> Search(List<string> terms, bool all = false)
        {
            var sqlParams = new Dictionary<string, object>() { };
            string likeClause = "(";
            for (int i = 0; i < terms.Count; i++)
            {
                var termfixed = StringUtils.FixSqlLikeClause(terms[i]);

                likeClause += $"(ResourceName LIKE @Term{i} OR ResourceDescriptionPublic LIKE @Term{i})";
                if (i < terms.Count - 1)
                    likeClause += " AND ";

                sqlParams.Add($"@Term{i}", $"%{termfixed}%");
            }
            likeClause += ")";

            var sql = $"SELECT {(all ? "" : "TOP 10")} ResourceID, ResourceName, IsDeleted, ExternalUrl, FacebookHandle, TwitterHandle, ResourceTypeId," +
               $" IsAcademia, IsCompany, IsPublisher, IsAuthor, IsJournalist, IsCompanyRep, IsAcademic, IsBook, IsFilm, IsVideo," +
               $" IsEnabled, NotesInternal, CreatedDT, UpdatedDT," +
               $" ResourceDescriptionInternal, ContentMain, TwitterRetweets," +
               $" LinkedInUrl, YouTubeUrl, UrlName, PrimaryTopicID, SecondaryTopicID, IsHumour, " +
               $" IsInstitute, IsApplication, IsCompetition, IsInformation, IsProduct," +
               $" IsResearch, AmazonProductCode, ResourceDescriptionPublic, IsAudio, " +
               $" ParentResourceID, IsJournal, DnParentResourceName, CreatedByUserId, UpdatedByUserId, ProductionDate, " +
               $" IsAdvocate, IsArtist, IsPolitician, IsEducation, IsHealthOrg, IsHealthPro, IsHiddenPublic, IsEvent, EndDate, CountryId," +
               $" IsPinnedPrimaryTopic, IsPinnedSecondaryTopic, PinnedPrimaryTopicOrder, PinnedSecondaryTopicOrder, IsClubDiscount" +
               $" FROM {TableNameResources} WHERE IsDeleted = FALSE AND IsEnabled = TRUE AND " +
               $" [LIKECLAUSE] [UpdatedDtFilter] " +
               $" ORDER BY UpdatedDT DESC";

            if (all)
            {
                sql = sql.Replace("[UpdatedDtFilter]", "");
            }
            else
            {
                sql = sql.Replace("[UpdatedDtFilter]", " AND UpdatedDT >= @date2YearsAgo");
                var date2YearsAgo = DateTime.Now.AddYears(-2);
                sqlParams.Add("@date2YearsAgo", GetDateWithoutMilliseconds(date2YearsAgo));
            }

            sql = sql.Replace("[LIKECLAUSE]", likeClause);

            return QueryAllData(sql, list =>
            {
                var resource = MapResource(0);
                list.Add(resource);
            }, sqlParams);
        }

        public Resource GetResourceById(int id)
        {
            var sql = $"SELECT r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6," +
                         $" r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                         $" r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                         $" r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                         $" r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                         $" r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                         $" r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                         $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                         " r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                         $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51, " +
                         $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                         $" u.UserNameFirst as 60, u.UserNameLast as 61," +
                         $" t1.TopicID as 62, t1.TopicName as 63, t1.DescriptionInternal as 64, t1.PrimaryWebPageId as 65," +
                         $" t1.NotesInternal as 66, t1.SocialImageFilename as 67," +
                         $" t1.SocialImageFilenameNews as 68, t1.BannerImageFileName as 69," +
                         $" t1.Hashtags as 70, t1.ThumbnailImageFilename as 71," +
                         $" t2.TopicID as 72, t2.TopicName as 73, t2.DescriptionInternal as 74," +
                         $" t2.PrimaryWebPageId as 75, t2.NotesInternal as 76, t2.SocialImageFilename as 77," +
                         $" t2.SocialImageFilenameNews as 78, t2.BannerImageFileName as 79," +
                         $" t2.Hashtags as 80, t2.ThumbnailImageFilename as 81, c.CountryAutoID as 82, c.CountryName as 83, c.ContinentCode as 84, c.CountryID as 85" +
                         $" FROM ((((({TableNameResources} r) " +
                         $" LEFT JOIN {TableNameUsers} u on r.UpdatedByUserId = u.UserID)" +
                         $" LEFT JOIN {TableNameTopics} t1 on r.PrimaryTopicID = t1.TopicId)" +
                         $" LEFT JOIN {TableNameTopics} t2 on r.SecondaryTopicID = t2.TopicId)" +
                         $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
                         $" WHERE ResourceID = @Id";

            return QueryAllData(sql, (list =>
            {
                var resource = MapResource(0);
                if (GetTableValue<int?>(27).HasValue)
                    resource.PrimaryTopic = MapTopic(62);
                if (GetTableValue<int?>(28).HasValue)
                    resource.SecondaryTopic = MapTopic(72);
                if (GetTableValue<int?>(43).HasValue)
                {
                    var username = MapUser(60);
                    resource.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(82);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);
            }), new Dictionary<string, object>() { { "Id", id } }).FirstOrDefault();
        }

        public override void Insert(Resource entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameResources} (ResourceName, IsDeleted, ExternalUrl, FacebookHandle, TwitterHandle, ResourceTypeId," +
                    $" IsAcademia, IsCompany, IsPublisher, IsAuthor, IsJournalist, IsCompanyRep, IsAcademic," +
                    $" IsBook, IsFilm, IsVideo, IsEnabled, NotesInternal, CreatedDT, UpdatedDT, " +
                    $" ResourceDescriptionInternal, ContentMain, TwitterRetweets, LinkedInUrl, YouTubeUrl, UrlName, PrimaryTopicID, SecondaryTopicID, IsHumour," +
                    $" IsInstitute, IsApplication, IsCompetition, IsInformation, IsProduct, IsResearch, AmazonProductCode, ResourceDescriptionPublic, IsAudio, " +
                    $" ParentResourceID, IsJournal, DnParentResourceName, CreatedByUserId, UpdatedByUserId, ProductionDate, IsAdvocate, IsArtist, IsPolitician, " +
                    $"IsEducation, IsHealthOrg, IsHealthPro, IsEvent, EndDate, CountryId, IsPinnedPrimaryTopic, IsPinnedSecondaryTopic, PinnedPrimaryTopicOrder, PinnedSecondaryTopicOrder, IsClubDiscount)" +
                    $" VALUES (@ResourceName, @IsDeleted, @ExternalUrl, @FacebookHandle, @TwitterHandle, @ResourceTypeId," +
                    $" @IsAcademia, @IsCompany, @IsPublisher, @IsAuthor, @IsJournalist, @IsCompanyRep, @IsAcademic," +
                    $" @IsBook, @IsFilm, @IsVideo, @IsEnabled, @NotesInternal, @CreatedDT, @UpdatedDT, " +
                    $" @ResourceDescriptionInternal, @ContentMain, @TwitterRetweets, @LinkedInUrl, @YouTubeUrl, @UrlName, @PrimaryTopicID, @SecondaryTopicID, @IsHumour," +
                    $"@IsInstitute, @IsApplication, @IsCompetition, @IsInformation, @IsProduct, @IsResearch, @AmazonProductCode, @ResourceDescriptionPublic, @IsAudio, " +
                    $"@ParentResourceID, @IsJournal, @DnParentResourceName, @CreatedByUserId, @UpdatedByUserId, @ProductionDate, @IsAdvocate, @IsArtist, @IsPolitician, " +
                    $"@IsEducation, @IsHealthOrg, @IsHealthPro, @IsEvent, @EndDate, @CountryId, @IsPinnedPrimaryTopic, @IsPinnedSecondaryTopic, @PinnedPrimaryTopicOrder, @PinnedSecondaryTopicOrder, @IsClubDiscount)");

                command.Parameters.AddWithValue("@ResourceName", SafeGetStringValue(entity.ResourceName));
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@ExternalUrl", SafeGetStringValue(entity.ExternalUrl));
                command.Parameters.AddWithValue("@FacebookHandle", SafeGetStringValue(entity.FacebookHandle));
                command.Parameters.AddWithValue("@TwitterHandle", SafeGetStringValue(entity.TwitterHandle));
                command.Parameters.AddWithValue("@ResourceTypeId", entity.ResourceTypeId);
                command.Parameters.AddWithValue("@IsAcademia", entity.IsAcademia);
                command.Parameters.AddWithValue("@IsCompany", entity.IsCompany);
                command.Parameters.AddWithValue("@IsPublisher", entity.IsPublisher);
                command.Parameters.AddWithValue("@IsAuthor", entity.IsAuthor);
                command.Parameters.AddWithValue("@IsJournalist", entity.IsJournalist);
                command.Parameters.AddWithValue("@IsCompanyRep", entity.IsCompanyRep);
                command.Parameters.AddWithValue("@IsAcademic", entity.IsAcademic);
                command.Parameters.AddWithValue("@IsBook", entity.IsBook);
                command.Parameters.AddWithValue("@IsFilm", entity.IsFilm);
                command.Parameters.AddWithValue("@IsVideo", entity.IsVideo);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.CreatedDT));
                command.Parameters.AddWithValue("@UpdatedDT", GetDateWithoutMilliseconds(entity.UpdatedDT));
                command.Parameters.AddWithValue("@ResourceDescriptionInternal", SafeGetStringValue(entity.ResourceDescriptionInternal));
                command.Parameters.AddWithValue("@ContentMain", SafeGetStringValue(entity.ContentMain));
                command.Parameters.AddWithValue("@TwitterRetweets", entity.TwitterRetweets);
                command.Parameters.AddWithValue("@LinkedInUrl", SafeGetStringValue(entity.LinkedInUrl));
                command.Parameters.AddWithValue("@YouTubeUrl", SafeGetStringValue(entity.YouTubeUrl));
                command.Parameters.AddWithValue("@UrlName", SafeGetStringValue(entity.UrlName));
                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopicID.HasValue ? entity.PrimaryTopicID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopicID.HasValue ? entity.SecondaryTopicID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsHumour", entity.IsHumour);
                command.Parameters.AddWithValue("@IsInstitute", entity.IsInstitute);
                command.Parameters.AddWithValue("@IsApplication", entity.IsApplication);
                command.Parameters.AddWithValue("@IsCompetition", entity.IsCompetition);
                command.Parameters.AddWithValue("@IsInformation", entity.IsInformation);
                command.Parameters.AddWithValue("@IsProduct", entity.IsProduct);
                command.Parameters.AddWithValue("@IsResearch", entity.IsResearch);
                command.Parameters.AddWithValue("@AmazonProductCode", SafeGetStringValue(entity.AmazonProductCode));
                command.Parameters.AddWithValue("@ResourceDescriptionPublic", SafeGetStringValue(entity.ResourceDescriptionPublic));
                command.Parameters.AddWithValue("@IsAudio", entity.IsAudio);
                command.Parameters.AddWithValue("@ParentResourceID", entity.ParentResourceID.HasValue ? entity.ParentResourceID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsJournal", entity.IsJournal);
                command.Parameters.AddWithValue("@DnParentResourceName", SafeGetStringValue(entity.DnParentResourceName));
                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedByUserId.HasValue ? entity.CreatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProductionDate", entity.ProductionDate.HasValue ?
                    GetDateWithoutMilliseconds(entity.ProductionDate.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsAdvocate", entity.IsAdvocate);
                command.Parameters.AddWithValue("@IsArtist", entity.IsArtist);
                command.Parameters.AddWithValue("@IsPolitician", entity.IsPolitician);
                command.Parameters.AddWithValue("@IsEducation", entity.IsEducation);
                command.Parameters.AddWithValue("@IsHealthOrg", entity.IsHealthOrg);
                command.Parameters.AddWithValue("@IsHealthPro", entity.IsHealthPro);
                command.Parameters.AddWithValue("@IsEvent", entity.IsEvent);
                command.Parameters.AddWithValue("@EndDate", entity.EndDate.HasValue ?
                    GetDateWithoutMilliseconds(entity.EndDate.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CountryId", entity.CountryId);
                command.Parameters.AddWithValue("@IsPinnedPrimaryTopic", entity.IsPinnedPrimaryTopic);
                command.Parameters.AddWithValue("@IsPinnedSecondaryTopic", entity.IsPinnedSecondaryTopic);
                command.Parameters.AddWithValue("@PinnedPrimaryTopicOrder", entity.PinnedPrimaryTopicOrder ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PinnedSecondaryTopicOrder", entity.PinnedSecondaryTopicOrder ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsClubDiscount", entity.IsClubDiscount);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving Resource to the database");
            }
        }

        public override void Update(Resource entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameResources} SET" +
                    " ResourceName = @ResourceName, IsDeleted = @IsDeleted, ExternalUrl = @ExternalUrl, FacebookHandle = @FacebookHandle," +
                    " TwitterHandle = @TwitterHandle, ResourceTypeId = @ResourceTypeId," +
                    " IsAcademia = @IsAcademia, IsCompany = @IsCompany, IsPublisher = @IsPublisher, IsAuthor = @IsAuthor," +
                    " IsJournalist = @IsJournalist, IsCompanyRep = @IsCompanyRep, IsAcademic = @IsAcademic," +
                    " IsBook = @IsBook, IsFilm = @IsFilm, IsVideo = @IsVideo, IsEnabled = @IsEnabled, NotesInternal = @NotesInternal, UpdatedDT = @UpdatedDT, " +
                    " ResourceDescriptionInternal = @ResourceDescriptionInternal, ContentMain = @ContentMain, " +
                    " TwitterRetweets = @TwitterRetweets, LinkedInUrl = @LinkedInUrl, YouTubeUrl = @YouTubeUrl, " +
                    " UrlName = @UrlName, PrimaryTopicID = @PrimaryTopicID, SecondaryTopicID = @SecondaryTopicID, IsHumour = @IsHumour," +
                    " IsInstitute = @IsInstitute, IsApplication = @IsApplication, IsCompetition = @IsCompetition, IsInformation = @IsInformation," +
                    " IsProduct = @IsProduct, IsResearch = @IsResearch, AmazonProductCode = @AmazonProductCode, ResourceDescriptionPublic = @ResourceDescriptionPublic," +
                    " IsAudio = @IsAudio, ParentResourceID = @ParentResourceID, IsJournal = @IsJournal, DnParentResourceName = @DnParentResourceName," +
                    " UpdatedByUserId = @UpdatedByUserId, ProductionDate = @ProductionDate, " +
                    " IsAdvocate = @IsAdvocate, IsArtist = @IsArtist, IsPolitician = @IsPolitician, IsEducation = @IsEducation, " +
                    " IsHealthOrg = @IsHealthOrg, IsHealthPro = @IsHealthPro, " +
                    " IsEvent = @IsEvent, EndDate = @EndDate, CountryId = @CountryId, IsPinnedPrimaryTopic = @IsPinnedPrimaryTopic," +
                    " IsPinnedSecondaryTopic = @IsPinnedSecondaryTopic, PinnedPrimaryTopicOrder = @PinnedPrimaryTopicOrder, PinnedSecondaryTopicOrder = @PinnedSecondaryTopicOrder, IsClubDiscount = @IsClubDiscount" +
                    $" WHERE ResourceID = {entity.Id}");

                command.Parameters.AddWithValue("@ResourceName", entity.ResourceName);
                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@ExternalUrl", SafeGetStringValue(entity.ExternalUrl));
                command.Parameters.AddWithValue("@FacebookHandle", SafeGetStringValue(entity.FacebookHandle));
                command.Parameters.AddWithValue("@TwitterHandle", SafeGetStringValue(entity.TwitterHandle));
                command.Parameters.AddWithValue("@ResourceTypeId", entity.ResourceTypeId);
                command.Parameters.AddWithValue("@IsAcademia", entity.IsAcademia);
                command.Parameters.AddWithValue("@IsCompany", entity.IsCompany);
                command.Parameters.AddWithValue("@IsPublisher", entity.IsPublisher);
                command.Parameters.AddWithValue("@IsAuthor", entity.IsAuthor);
                command.Parameters.AddWithValue("@IsJournalist", entity.IsJournalist);
                command.Parameters.AddWithValue("@IsCompanyRep", entity.IsCompanyRep);
                command.Parameters.AddWithValue("@IsAcademic", entity.IsAcademic);
                command.Parameters.AddWithValue("@IsBook", entity.IsBook);
                command.Parameters.AddWithValue("@IsFilm", entity.IsFilm);
                command.Parameters.AddWithValue("@IsVideo", entity.IsVideo);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@UpdatedDT", GetDateWithoutMilliseconds(entity.UpdatedDT));

                command.Parameters.AddWithValue("@ResourceDescriptionInternal", SafeGetStringValue(entity.ResourceDescriptionInternal));
                command.Parameters.AddWithValue("@ContentMain", SafeGetStringValue(entity.ContentMain));
                command.Parameters.AddWithValue("@TwitterRetweets", entity.TwitterRetweets);
                command.Parameters.AddWithValue("@LinkedInUrl", SafeGetStringValue(entity.LinkedInUrl));
                command.Parameters.AddWithValue("@YouTubeUrl", SafeGetStringValue(entity.YouTubeUrl));
                command.Parameters.AddWithValue("@UrlName", SafeGetStringValue(entity.UrlName));
                command.Parameters.AddWithValue("@PrimaryTopicID", entity.PrimaryTopicID.HasValue ? entity.PrimaryTopicID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@SecondaryTopicID", entity.SecondaryTopicID.HasValue ? entity.SecondaryTopicID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsHumour", entity.IsHumour);
                command.Parameters.AddWithValue("@IsInstitute", entity.IsInstitute);
                command.Parameters.AddWithValue("@IsApplication", entity.IsApplication);
                command.Parameters.AddWithValue("@IsCompetition", entity.IsCompetition);
                command.Parameters.AddWithValue("@IsInformation", entity.IsInformation);
                command.Parameters.AddWithValue("@IsProduct", entity.IsProduct);
                command.Parameters.AddWithValue("@IsResearch", entity.IsResearch);
                command.Parameters.AddWithValue("@AmazonProductCode", SafeGetStringValue(entity.AmazonProductCode));
                command.Parameters.AddWithValue("@ResourceDescriptionPublic", SafeGetStringValue(entity.ResourceDescriptionPublic));
                command.Parameters.AddWithValue("@IsAudio", entity.IsAudio);
                command.Parameters.AddWithValue("@ParentResourceID", entity.ParentResourceID.HasValue ? entity.ParentResourceID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsJournal", entity.IsJournal);
                command.Parameters.AddWithValue("@DnParentResourceName", SafeGetStringValue(entity.DnParentResourceName));
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProductionDate", entity.ProductionDate.HasValue ?
                    GetDateWithoutMilliseconds(entity.ProductionDate.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsAdvocate", entity.IsAdvocate);
                command.Parameters.AddWithValue("@IsArtist", entity.IsArtist);
                command.Parameters.AddWithValue("@IsPolitician", entity.IsPolitician);
                command.Parameters.AddWithValue("@IsEducation", entity.IsEducation);
                command.Parameters.AddWithValue("@IsHealthOrg", entity.IsHealthOrg);
                command.Parameters.AddWithValue("@IsHealthPro", entity.IsHealthPro);
                command.Parameters.AddWithValue("@IsEvent", entity.IsEvent);
                command.Parameters.AddWithValue("@EndDate", entity.EndDate.HasValue ?
                    GetDateWithoutMilliseconds(entity.EndDate.Value) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CountryId", entity.CountryId);
                command.Parameters.AddWithValue("@IsPinnedPrimaryTopic", entity.IsPinnedPrimaryTopic);
                command.Parameters.AddWithValue("@IsPinnedSecondaryTopic", entity.IsPinnedSecondaryTopic);
                command.Parameters.AddWithValue("@PinnedPrimaryTopicOrder", entity.PinnedPrimaryTopicOrder ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PinnedSecondaryTopicOrder", entity.PinnedSecondaryTopicOrder ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsClubDiscount", entity.IsClubDiscount);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing Resource in the database");
            }
        }

        public IEnumerable<Resource> GetMentionedResources(int entityId, int? resourceType = null, EntityType rootEntityType = EntityType.Article)
        {
            string associationTable = "", FK = "", PK = "", joinOn = "ResourceID";
            if (rootEntityType == EntityType.Article)
            {
                associationTable = TableNameArticleResources;
                FK = "ArticleID";
                PK = "ArticleResourceID";
            }
            else if (rootEntityType == EntityType.BlogPost)
            {
                associationTable = TableNameBlogPostResources;
                FK = "BlogPostID";
                PK = "BlogPostResourceID";
            }
            else if (rootEntityType == EntityType.Resource)
            {
                associationTable = TableNameResourceResources;
                FK = "ResourceID";
                PK = "ResourceResourceID";
                joinOn = "ResourceMentionedID";
            }

            var getResourcesSQL =
                $"SELECT r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6, " +
                " r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                " r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                " r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                " r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                " r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                " r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51, " +
                $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                $" u.UserNameFirst as 60, u.UserNameLast as 61, assoc.{PK} as 62, c.CountryAutoID as 63, c.CountryName as 64, c.ContinentCode as 65, c.CountryID as 66" +
                $" FROM (((({TableNameResources} r) " +
                $" LEFT JOIN {TableNameUsers} u ON r.UpdatedByUserId = u.UserID)" +
                $" LEFT JOIN {associationTable} assoc ON r.ResourceID = assoc.{joinOn})" +
                $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
                $" WHERE assoc.{FK} = {entityId} ";

            if (resourceType != null)
            {
                string dnField = rootEntityType == EntityType.Resource ?
                    "DnResourceMentionedTypeID" : "DnResourceTypeID";
                getResourcesSQL += $" AND assoc.{dnField}= {resourceType}";
            }

            return QueryAllData(getResourcesSQL, (list =>
            {
                var resource = MapResource(0);
                resource.MentionedOrder = GetTableValue<int>(62); //order by this
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(63);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);
            }));

        }

        public Resource GetResourceByUrlName(string urlName)
        {
            var sql = $"SELECT r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6," +
                    $" r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                    $" r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                    $" r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                    $" r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                    $" r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                    $" r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                    $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                    $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                    $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51," +
                    $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                    $" u.UserNameFirst as 60, u.UserNameLast as 61," +
                    $" t1.TopicID as 62, t1.TopicName as 63, t1.DescriptionInternal as 64, t1.PrimaryWebPageId as 65," +
                    $" t1.NotesInternal as 66, t1.SocialImageFilename as 67," +
                    $" t1.SocialImageFilenameNews as 68, t1.BannerImageFileName as 69," +
                    $" t1.Hashtags as 70, t1.ThumbnailImageFilename as 71," +
                    $" t2.TopicID as 72, t2.TopicName as 73, t2.DescriptionInternal as 74," +
                    $" t2.PrimaryWebPageId as 75, t2.NotesInternal as 76, t2.SocialImageFilename as 77," +
                    $" t2.SocialImageFilenameNews as 78, t2.BannerImageFileName as 79," +
                    $" t2.Hashtags as 80, t2.ThumbnailImageFilename as 81, c.CountryAutoID as 82, c.CountryName as 83, c.ContinentCode as 84, c.CountryID as 85," +
                    $" wp1.WebPageID as 86, wp1.UrlName as 87, wp1.WebPageName as 88, wp1.IsDeleted as 89," +
                    $" wp1.WebPageTitle as 90, wp1.ContentMain as 91, wp1.ParentID as 92, wp1.IsSubjectPage as 93," +
                    $" wp1.DisplayOrder as 94, wp1.IsEnabled as 95, wp1.NotesInternal as 96," +
                    $" wp1.CreatedDT as 97, wp1.UpdatedDT as 98, wp1.BannerImageFileName as 99," +
                    $" wp1.SocialImageFileName as 100, wp1.MetaDescription as 101, " +
                    $" wp1.PrimaryTopicID as 102, wp1.SecondaryTopicID as 103," +
                    $" wp1.PublishedOnDT as 104, wp1.PublishedUpdatedDT as 105, wp1.TypeID as 106," +
                    $" wp2.WebPageID as 107, wp2.UrlName as 108, wp2.WebPageName as 109, wp2.IsDeleted as 110," +
                    $" wp2.WebPageTitle as 111, wp2.ContentMain as 112, wp2.ParentID as 113, wp2.IsSubjectPage as 114," +
                    $" wp2.DisplayOrder as 115, wp2.IsEnabled as 116, wp2.NotesInternal as 117," +
                    $" wp2.CreatedDT as 118, wp2.UpdatedDT as 119, wp2.BannerImageFileName as 120," +
                    $" wp2.SocialImageFileName as 121, wp2.MetaDescription as 122, " +
                    $" wp2.PrimaryTopicID as 123, wp2.SecondaryTopicID as 124," +
                    $" wp2.PublishedOnDT as 125, wp2.PublishedUpdatedDT as 126, wp2.TypeID as 127" +
                    $" FROM ((((((({TableNameResources} r) " +
                    $" LEFT JOIN {TableNameUsers} u on r.UpdatedByUserId = u.UserID)" +
                    $" LEFT JOIN {TableNameTopics} t1 on r.PrimaryTopicID = t1.TopicId)" +
                    $" LEFT JOIN {TableNameTopics} t2 on r.SecondaryTopicID = t2.TopicId)" +
                    $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
                    $" LEFT JOIN {TableNameWebPages} wp1 ON t1.PrimaryWebPageId = wp1.WebPageID)" +
                    $" LEFT JOIN {TableNameWebPages} wp2 ON t2.PrimaryWebPageId = wp2.WebPageID)" +
                    $" WHERE r.IsDeleted = FALSE AND r.IsEnabled = TRUE AND r.IsHiddenPublic = FALSE AND r.UrlName = @UrlName";

            return QueryAllData(sql, (list =>
            {
                var resource = MapResource(0);

                resource.PrimaryTopic = MapTopic(62);
                if (GetTableValue<int?>(65).HasValue)
                {
                    resource.PrimaryTopic.PrimaryWebPage = MapWebPage(86);
                }
                if (GetTableValue<int?>(28).HasValue)
                {
                    resource.SecondaryTopic = MapTopic(72);
                    if (GetTableValue<int?>(75).HasValue)
                    {
                        resource.SecondaryTopic.PrimaryWebPage = MapWebPage(107);
                    }
                }
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(82);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);

            }), new Dictionary<string, object>() { { "UrlName", urlName } }).FirstOrDefault();
        }

        public Resource GetResourceByExternalUrl(string url, int? idToExclude = null)
        {
            var externalUrlWithTrailingSlash = $"/{url}/";
            var externalUrlWithLeadingSlash = $"/{url}";
            var sql =
                $"SELECT r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6," +
                $" r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                $" r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                $" r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                $" r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                $" r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                $" r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51," +
                $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                $" u.UserNameFirst as 60, u.UserNameLast as 61," +
                $" t1.TopicID as 62, t1.TopicName as 63, t1.DescriptionInternal as 64, t1.PrimaryWebPageId as 65," +
                $" t1.NotesInternal as 66, t1.SocialImageFilename as 67," +
                $" t1.SocialImageFilenameNews as 68, t1.BannerImageFileName as 69," +
                $" t1.Hashtags as 70, t1.ThumbnailImageFilename as 71," +
                $" t2.TopicID as 72, t2.TopicName as 73, t2.DescriptionInternal as 74," +
                $" t2.PrimaryWebPageId as 75, t2.NotesInternal as 76, t2.SocialImageFilename as 77," +
                $" t2.SocialImageFilenameNews as 78, t2.BannerImageFileName as 79," +
                $" t2.Hashtags as 80, t2.ThumbnailImageFilename as 81, c.CountryAutoID as 82, c.CountryName as 83, c.ContinentCode as 84, c.CountryID as 85" +
                $" FROM ((((({TableNameResources} r) " +
                $" LEFT JOIN {TableNameUsers} u on r.UpdatedByUserId = u.UserID)" +
                $" LEFT JOIN {TableNameTopics} t1 on r.PrimaryTopicID = t1.TopicId)" +
                $" LEFT JOIN {TableNameTopics} t2 on r.SecondaryTopicID = t2.TopicId)" +
                $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
                $" WHERE r.IsDeleted = FALSE AND (r.ExternalUrl LIKE @ExternalUrlWithLeadingSlash OR r.ExternalUrl LIKE @ExternalUrlWithTrailingSlash)";

            if (idToExclude.HasValue)
            {
                sql += $" AND r.ResourceID <> {idToExclude.Value}";
            }

            return QueryAllData(sql, list =>
            {
                var resource = MapResource(0);

                resource.PrimaryTopic = MapTopic(62);
                if (GetTableValue<int?>(28).HasValue)
                    resource.SecondaryTopic = MapTopic(72);
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(82);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);

            }, new Dictionary<string, object>() { { "ExternalUrlWithLeadingSlash", $"%{url}" }, { "ExternalUrlWithTrailingSlash", $"%{externalUrlWithTrailingSlash}" } }).FirstOrDefault();
        }

        public IEnumerable<Resource> GetChildResources(int parentId, int resourceType)
        {
            var parameters = new Dictionary<string, object>();
            var SQL =
                $"SELECT TOP 20 r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6, " +
                " r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                " r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                " r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                " r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                " r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                " r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                " r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                " r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                " r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51," +
                " r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                " u.UserNameFirst as 60, u.UserNameLast as 61, c.CountryAutoID as 62, c.CountryName as 63, c.ContinentCode as 64, c.CountryID as 65" +
                $" FROM ((({TableNameResources} r) " +
                $" LEFT JOIN {TableNameUsers} u ON r.UpdatedByUserId = u.UserID)" +
                $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
                $" WHERE r.ParentResourceID = @ParentID AND r.ResourceTypeId = @ResourceType AND " +
                $" r.UpdatedDT >= @Date2YearsAgo" +
                $" ORDER BY r.UpdatedDT";

            parameters.Add("@ParentID", parentId);
            parameters.Add("@ResourceType", resourceType);
            parameters.Add("@Date2YearsAgo", GetDateWithoutMilliseconds(DateTime.Now.AddYears(-2)));

            return QueryAllData(SQL, (list =>
            {
                var resource = MapResource(0);
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(62);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);
            }), parameters);

        }

        //public IEnumerable<Resource> GetResourcesByTopicsAndMediaType(int primaryTopicId, int? secondaryTopicId, ResourceMediaTypes mediaTypes = null, int count = 10)
        //{
        //    var parameters = new Dictionary<string, object>();
        //    var hasSecondaryTopic = secondaryTopicId.HasValue;

        //    var sql =
        //        $"SELECT TOP {count} r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6, " +
        //        " r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
        //        " r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
        //        " r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
        //        " r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
        //        " r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
        //        " r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
        //        $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
        //        $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
        //        $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51, " +
        //        $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54," +
        //        $" u.UserNameFirst as 55, u.UserNameLast as 56, c.CountryAutoID as 57, c.CountryName as 58, c.ContinentCode as 59, c.CountryID as 60" +
        //        $" FROM ((({TableNameResources} r)" +
        //        $" LEFT JOIN {TableNameUsers} u ON r.UpdatedByUserId = u.UserID)" +
        //        $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
        //        $" WHERE ";
        //    if (hasSecondaryTopic)
        //    {
        //        sql += $"(r.PrimaryTopicID = @PrimaryTopicID OR r.SecondaryTopicID = @SecondaryTopicID) ";
        //        parameters.Add("@PrimaryTopicID", primaryTopicId);
        //        parameters.Add("@SecondaryTopicID", secondaryTopicId.Value);
        //    }
        //    else
        //    {
        //        sql += $"r.PrimaryTopicID = @PrimaryTopicID ";
        //        parameters.Add("@PrimaryTopicID", primaryTopicId);
        //    }

        //    if (mediaTypes != null)
        //    {
        //        var mediaTypesCondition = mediaTypes.ToSqlFilter();
        //        sql += $" AND ({mediaTypesCondition})";
        //    }

        //    sql += $" AND r.UpdatedDT >= @Date2YearsAgo ";
        //    sql += "ORDER BY r.UpdatedDT DESC";

        //    parameters.Add("@Date2YearsAgo", GetDateWithoutMilliseconds(DateTime.Now.AddYears(-2)));

        //    return QueryAllData(sql, (list =>
        //    {
        //        var resource = MapResource(0);
        //        if (GetTableValue<int?>(43).HasValue)
        //        {
        //            var username = MapUser(55);
        //            resource.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
        //        }
        //        if (GetTableValue<int?>(54).HasValue)
        //        {
        //            resource.Country = MapCountry(57);
        //            resource.DnCountryName = resource.Country.CountryName;
        //        }
        //        //if (GetTableValue<int?>(56).HasValue)
        //        //{
        //        //    var CountryId = GetTableValue<int?>(56).Value;
        //        //    resource.CountryId = CountryId;
        //        //    if (resource.CountryId > 0)
        //        //    {
        //        //        var country = _countryRepository.GetCountryById(resource.CountryId);
        //        //        if (country != null)
        //        //        {
        //        //            resource.DnCountryName = country.CountryName;
        //        //        }
        //        //    }
        //        //}
        //        list.Add(resource);
        //    }), parameters);
        //}

        public IEnumerable<Resource> GetWebPageReleatedResources(int webPageId, int? maxCount, DateTime? resourceUpdatedDate)
        {
            var sql =
                $"SELECT DISTINCT {(maxCount.HasValue ? $"TOP {maxCount.Value}" : "")} r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6," +
                " r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                " r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                " r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                " r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29," +
                " r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                " r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38," +
                $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43," +
                $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48," +
                $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51," +
                $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                $" u.UserNameFirst as 60, u.UserNameLast as 61, c.CountryAutoID as 62, c.CountryName as 63, c.ContinentCode as 64, c.CountryID as 65" +
                $" FROM (((((({TableNameWebPages} wp)" +
                $" INNER JOIN {TableNameWebPageTopic} wpt ON wpt.WebPageID = wp.WebPageID)" +
                $" INNER JOIN {TableNameTopics} t ON wpt.TopicID = t.TopicID)" +
                $" INNER JOIN {TableNameResources} r ON r.PrimaryTopicID = t.TopicID OR r.SecondaryTopicID = t.TopicID)" +
                $" LEFT JOIN {TableNameUsers} u ON r.UpdatedByUserId = u.UserID)" +
                $" LEFT JOIN {TableNameCountry} c ON r.CountryID = c.CountryAutoID)" +
                " WHERE wp.WebPageID = @WebPageId AND r.IsDeleted = FALSE AND r.IsEnabled = TRUE" + (resourceUpdatedDate.HasValue ? " AND r.UpdatedDT >= @ResourceUpdatedDate" : "") +
                $" ORDER BY r.UpdatedDT DESC, r.ResourceID ASC";

            var args = new Dictionary<string, object>()
            {
                { "WebPageId", webPageId.ToString() }
            };

            if (resourceUpdatedDate.HasValue)
                args.Add("ResourceUpdatedDate", GetDateWithoutMilliseconds(resourceUpdatedDate.Value));

            return QueryAllData(sql, list =>
            {
                var resource = MapResource(0);
                if (GetTableValue<int?>(43).HasValue)
                {
                    var username = MapUser(60);
                    resource.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }
                if (GetTableValue<int?>(54).HasValue)
                {
                    resource.Country = MapCountry(62);
                    resource.DnCountryName = resource.Country.CountryName;
                }
                list.Add(resource);
            }, args);
        }

        public void DeleteMentionedResources(int resourceId, int resourceType)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"DELETE FROM {TableNameResourceResources} " +
                    $"WHERE ResourceID = @ResourceID AND DnResourceMentionedTypeID = @ResourceTypeID");
                command.Parameters.AddWithValue("@ResourceID", resourceId);
                command.Parameters.AddWithValue("@ResourceTypeID", resourceType);

                var rowsAffected = command.ExecuteNonQuery();
            }
        }

        public void InsertMentionedResources(int resourceId, List<int> resourceIds, int resourceType)
        {
            OleDbCommand command;
            string sqlBase = $"INSERT INTO {TableNameResourceResources}" +
                $" (ResourceID, ResourceMentionedID, DnResourceMentionedTypeID) " +
                $"VALUES (@ResourceID, @ResourceMentionedID, @DnResourceMentionedTypeID)";

            for (int i = 0; i < resourceIds.Count; i++)
            {
                using (MsAccessDbManager = new MsAccessDbManager())
                {
                    command = MsAccessDbManager.CreateCommand(sqlBase);
                    command.Parameters.AddWithValue($"@ResourceID", resourceId);
                    command.Parameters.AddWithValue($"@ResourceMentionedID", resourceIds[i]);
                    command.Parameters.AddWithValue($"@DnResourceMentionedTypeID", resourceType);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected != 1)
                        throw new InvalidOperationException("Error during saving Resource related Resource to the database");
                }
            }

        }

        public IEnumerable<Resource> GetResourcesMentioningResource(int resourceId)
        {
            var getResourcesSQL =
                $"SELECT r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6, " +
                " r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                " r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                " r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                " r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                " r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                " r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51, " +
                $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59," +
                $" u.UserNameFirst as 60, u.UserNameLast as 61, rr.ResourceResourceID as 62" +
                $" FROM (({TableNameResources} r) " +
                $" LEFT JOIN {TableNameUsers} u ON r.UpdatedByUserId = u.UserID)" +
                $" LEFT JOIN {TableNameResourceResources} rr ON r.ResourceID = rr.ResourceID " +
                $" WHERE rr.ResourceMentionedID = {resourceId} ";

            return QueryAllData(getResourcesSQL, (list =>
            {
                var resource = MapResource(0);
                resource.MentionedOrder = GetTableValue<int>(62);
                list.Add(resource);
            }));
        }

        public IEnumerable<Resource> GetEventResources()
        {
            var sql = $"SELECT r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6," +
                      $" r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                      $" r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                      $" r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                      $" r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                      $" r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                      $" r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                      $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                      " r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                      $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51, " +
                      $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59" +
                      $" FROM {TableNameResources} r" +
                      $" WHERE r.IsEvent = TRUE AND r.IsDeleted = FALSE AND r.IsEnabled = TRUE";

            return QueryAllData(sql, list =>
            {
                var resource = MapResource(0);
                list.Add(resource);
            }).ToList();
        }

        public IEnumerable<Resource> GetLatestResources(int limit, ResourcesSortOrder sortField, SortDirection sortDirections)
        {
            var sql = $"SELECT TOP {limit} r.ResourceID as 0, r.ResourceName as 1, r.IsDeleted as 2, r.ExternalUrl as 3, r.FacebookHandle as 4, r.TwitterHandle as 5, r.ResourceTypeId as 6," +
                      $" r.IsAcademia as 7, r.IsCompany as 8, r.IsPublisher as 9, r.IsAuthor as 10, r.IsJournalist as 11, r.IsCompanyRep as 12, r.IsAcademic as 13, r.IsBook as 14, r.IsFilm as 15, r.IsVideo as 16," +
                      $" r.IsEnabled as 17, r.NotesInternal as 18, r.CreatedDT as 19, r.UpdatedDT as 20," +
                      $" r.ResourceDescriptionInternal as 21, r.ContentMain as 22, r.TwitterRetweets as 23," +
                      $" r.LinkedInUrl as 24, r.YouTubeUrl as 25, r.UrlName as 26, r.PrimaryTopicID as 27, r.SecondaryTopicID as 28, r.IsHumour as 29, " +
                      $" r.IsInstitute as 30, r.IsApplication as 31, r.IsCompetition as 32, r.IsInformation as 33, r.IsProduct as 34," +
                      $" r.IsResearch as 35, r.AmazonProductCode as 36, r.ResourceDescriptionPublic as 37, r.IsAudio as 38, " +
                      $" r.ParentResourceID as 39, r.IsJournal as 40, r.DnParentResourceName as 41, r.CreatedByUserId as 42, r.UpdatedByUserId as 43, " +
                      $" r.ProductionDate as 44, r.IsAdvocate as 45, r.IsArtist as 46, r.IsPolitician as 47, r.IsEducation as 48, " +
                      $" r.IsHealthOrg as 49, r.IsHealthPro as 50, r.IsHiddenPublic as 51, " +
                      $" r.IsEvent as 52, r.EndDate as 53, r.CountryId as 54, r.IsPinnedPrimaryTopic as 55, r.IsPinnedSecondaryTopic as 56, r.PinnedPrimaryTopicOrder as 57, r.PinnedSecondaryTopicOrder as 58, r.IsClubDiscount as 59" +
                      $" FROM {TableNameResources} r" +
                      $" WHERE r.IsDeleted = FALSE AND r.IsEnabled = TRUE" +
                      $" ORDER BY r.{sortField.ToString()} {(sortDirections == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var resource = MapResource(0);
                list.Add(resource);
            }).ToList();
        }
    }
}
