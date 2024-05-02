using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class ShopCategoryRepository : AbstractRepository<ShopCategory>, IShopCategoryRepository
    {
        public override void Insert(ShopCategory entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameShopCategory} (IsDeleted, IsEnabled, ShopCategoryName, PrimaryWebPageId, DnPrimaryWebPageName," +
                    $" DnPrimaryWebPageUrlName, ImageThumb, ImageMain, NotesInternal, CreatedDT, UpdatedDT, CreatedByUserId, UpdatedByUserId)" +
                    $" VALUES (@IsDeleted, @IsEnabled, @ShopCategoryName, @PrimaryWebPageId, @DnPrimaryWebPageName, @DnPrimaryWebPageUrlName," +
                    $" @ImageThumb, @ImageMain, @NotesInternal, @CreatedDT, @UpdatedDT, @CreatedByUserId, @UpdatedByUserId)");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@ShopCategoryName", SafeGetStringValue(entity.ShopCategoryName));
                command.Parameters.AddWithValue("@PrimaryWebPageId", entity.PrimaryWebPageId.HasValue ? entity.PrimaryWebPageId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DnPrimaryWebPageName", SafeGetStringValue(entity.DnPrimaryWebPageName));
                command.Parameters.AddWithValue("@DnPrimaryWebPageUrlName", SafeGetStringValue(entity.DnPrimaryWebPageUrlName));
                command.Parameters.AddWithValue("@ImageThumb", SafeGetStringValue(entity.ImageThumb));
                command.Parameters.AddWithValue("@ImageMain", SafeGetStringValue(entity.ImageMain));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@CreatedDT", entity.CreatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.CreatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedByUserId.HasValue ? entity.CreatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving shop category to the database");
            }
        }

        public override void Update(ShopCategory entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopCategory} SET" +
                    $" IsDeleted = @IsDeleted, IsEnabled = @IsEnabled, ShopCategoryName = @ShopCategoryName, PrimaryWebPageId = @PrimaryWebPageId," +
                    $" DnPrimaryWebPageName = @DnPrimaryWebPageName, DnPrimaryWebPageUrlName = @DnPrimaryWebPageUrlName, ImageThumb = @ImageThumb," +
                    $" ImageMain = @ImageMain, NotesInternal = @NotesInternal, UpdatedDT = @UpdatedDT, UpdatedByUserId = @UpdatedByUserId" +
                    $" WHERE ShopCategoryID = {entity.ShopCategoryId}");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@ShopCategoryName", SafeGetStringValue(entity.ShopCategoryName));
                command.Parameters.AddWithValue("@PrimaryWebPageId", entity.PrimaryWebPageId.HasValue ? entity.PrimaryWebPageId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DnPrimaryWebPageName", SafeGetStringValue(entity.DnPrimaryWebPageName));
                command.Parameters.AddWithValue("@DnPrimaryWebPageUrlName", SafeGetStringValue(entity.DnPrimaryWebPageUrlName));
                command.Parameters.AddWithValue("@ImageThumb", SafeGetStringValue(entity.ImageThumb));
                command.Parameters.AddWithValue("@ImageMain", SafeGetStringValue(entity.ImageMain));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing shop category in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopCategory} SET IsDeleted = 1 WHERE ShopCategoryID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop category in the database");
            }
        }

        public override IEnumerable<ShopCategory> GetAll()
        {
            var sql = $"SELECT sc.ShopCategoryID as 0, sc.IsDeleted as 1, sc.IsEnabled as 2, sc.ShopCategoryName as 3," +
                $" sc.PrimaryWebPageId as 4, sc.DnPrimaryWebPageName as 5, sc.DnPrimaryWebPageUrlName as 6, sc.ImageThumb as 7, sc.ImageMain as 8," +
                $" sc.NotesInternal as 9, sc.CreatedDT as 10, sc.UpdatedDT as 11, sc.CreatedByUserId as 12, sc.UpdatedByUserId as 13" +
                $" FROM {TableNameShopCategory} sc" +
                $" WHERE sc.IsDeleted = FALSE AND sc.IsEnabled = TRUE" +
                $" ORDER BY sc.UpdatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var shopCategory = MapShopCategory(0);
                list.Add(shopCategory);
            });
        }

        public IEnumerable<ShopCategory> GetShopCategories(int limit, ShopCategoriesSortOrder sortField, SortDirection sortDirections)
        {
            var sql = $"SELECT TOP {limit} sc.ShopCategoryID as 0, sc.IsDeleted as 1, sc.IsEnabled as 2, sc.ShopCategoryName as 3," +
                $" sc.PrimaryWebPageId as 4, sc.DnPrimaryWebPageName as 5, sc.DnPrimaryWebPageUrlName as 6, sc.ImageThumb as 7, sc.ImageMain as 8," +
                $" sc.NotesInternal as 9, sc.CreatedDT as 10, sc.UpdatedDT as 11, sc.CreatedByUserId as 12, sc.UpdatedByUserId as 13" +
                $" FROM {TableNameShopCategory} sc" +
                $" WHERE sc.IsDeleted = 0" +
                $" ORDER BY sc.{sortField.ToString()} {(sortDirections == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var shopCategory = MapShopCategory(0);
                list.Add(shopCategory);
            });
        }

        public ShopCategory GetShopCategoryByName(string name)
        {
            var sql = $"SELECT sc.ShopCategoryID as 0, sc.IsDeleted as 1, sc.IsEnabled as 2, sc.ShopCategoryName as 3," +
                $" sc.PrimaryWebPageId as 4, sc.DnPrimaryWebPageName as 5, sc.DnPrimaryWebPageUrlName as 6, sc.ImageThumb as 7, sc.ImageMain as 8," +
                $" sc.NotesInternal as 9, sc.CreatedDT as 10, sc.UpdatedDT as 11, sc.CreatedByUserId as 12, sc.UpdatedByUserId as 13" +
                $" FROM {TableNameShopCategory} sc" +
                $" WHERE LCase(sc.ShopCategoryName) = @Name AND sc.IsDeleted = 0";

            return QueryAllData(sql, list =>
            {
                var shopCategory = MapShopCategory(0);
                list.Add(shopCategory);
            }, new Dictionary<string, object>() { { "Name", name.ToLower() } }).FirstOrDefault();
        }

        public ShopCategory GetShopCategoryById(int id)
        {
            var sql = $"SELECT sc.ShopCategoryID as 0, sc.IsDeleted as 1, sc.IsEnabled as 2, sc.ShopCategoryName as 3," +
               $" sc.PrimaryWebPageId as 4, sc.DnPrimaryWebPageName as 5, sc.DnPrimaryWebPageUrlName as 6, sc.ImageThumb as 7, sc.ImageMain as 8," +
               $" sc.NotesInternal as 9, sc.CreatedDT as 10, sc.UpdatedDT as 11, sc.CreatedByUserId as 12, sc.UpdatedByUserId as 13," +
               $" u.UserNameFirst as 14, u.UserNameLast as 15," +
               $" wp.WebPageID as 16, wp.UrlName as 17, wp.WebPageName as 18, wp.IsDeleted as 19, " +
               $" wp.WebPageTitle as 20, wp.ContentMain as 21, wp.ParentID as 22, wp.IsSubjectPage as 23, " +
               $" wp.DisplayOrder as 24, wp.IsEnabled as 25, wp.NotesInternal as 26, " +
               $" wp.CreatedDT as 27, wp.UpdatedDT as 28, wp.BannerImageFileName as 29, " +
               $" wp.SocialImageFileName as 30, wp.MetaDescription as 31, " +
               $" wp.PrimaryTopicID as 32, wp.SecondaryTopicID as 33, " +
               $" wp.PublishedOnDT as 34, wp.PublishedUpdatedDT as 35, " +
               $" wp.TypeID as 36 " +
               $" FROM (({TableNameShopCategory} sc)" +
               $" LEFT JOIN {TableNameWebPages} wp ON sc.PrimaryWebPageId = wp.WebPageID)" +
               $" LEFT JOIN {TableNameUsers} u on sc.UpdatedByUserId = u.UserID" +
               $" WHERE sc.ShopCategoryID = {id} AND sc.IsDeleted = 0";

            return QueryAllData(sql, list =>
            {
                var shopCategory = MapShopCategory(0);
                if (GetTableValue<int?>(4).HasValue)
                    shopCategory.PrimaryWebPage = MapWebPage(16);
                if (GetTableValue<int?>(13).HasValue)
                {
                    var username = MapUser(14);
                    shopCategory.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }
                list.Add(shopCategory);
            }).FirstOrDefault();
        }

        public bool IsNonDeletedShopCategoryExists(string name, int? excludeShopCategoryId)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = excludeShopCategoryId.HasValue
                     ? $"SELECT COUNT(*) FROM {TableNameShopCategory} sc WHERE sc.ShopCategoryID <> @ExcludeId AND sc.IsDeleted = 0 AND LCase(sc.ShopCategoryName) = @Name"
                     : $"SELECT COUNT(*) FROM {TableNameShopCategory} sc WHERE sc.IsDeleted = 0 AND LCase(sc.ShopCategoryName) = @Name";

                var command = MsAccessDbManager.CreateCommand(sql);

                if (excludeShopCategoryId.HasValue)
                    command.Parameters.AddWithValue("@ExcludeId", excludeShopCategoryId.Value);
                command.Parameters.AddWithValue("@Name", name.ToLower());

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
