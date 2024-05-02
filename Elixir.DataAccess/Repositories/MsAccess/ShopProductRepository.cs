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
    public class ShopProductRepository : AbstractRepository<ShopProduct>, IShopProductRepository
    {
        public override void Insert(ShopProduct entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameShopProduct} (IsDeleted, IsEnabled, ShopProductName, UrlName, SKU, ShopProductDescription, ContentMain, ShopCategoryId," +
                    $" PriceRRP, PriceLongevist, ShippingPrice, IsLongevistsOnly, StockLevel, DisplayOrder, OptionsUnit, ImageThumb, ImageMain, NotesInternal, UpdatedDT, UpdatedBy, CreatedDT, CreatedBy)" +
                    $" VALUES (@IsDeleted, @IsEnabled, @ShopProductName, @UrlName, @SKU, @ShopProductDescription, @ContentMain, @ShopCategoryId," +
                    $" @PriceRRP, @PriceLongevist, @ShippingPrice, @IsLongevistsOnly, @StockLevel, @DisplayOrder, @OptionsUnit, @ImageThumb, @ImageMain, @NotesInternal, @UpdatedDT, @UpdatedBy, @CreatedDT, @CreatedBy)");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@ShopProductName", SafeGetStringValue(entity.ShopProductName));
                command.Parameters.AddWithValue("@UrlName", SafeGetStringValue(entity.UrlName));
                command.Parameters.AddWithValue("@SKU", SafeGetStringValue(entity.SKU));
                command.Parameters.AddWithValue("@ShopProductDescription", SafeGetStringValue(entity.ShopProductDescription));
                command.Parameters.AddWithValue("@ContentMain", SafeGetStringValue(entity.ContentMain));
                command.Parameters.AddWithValue("@ShopCategoryId", entity.ShopCategoryId.HasValue ? entity.ShopCategoryId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PriceRRP", entity.PriceRRP.HasValue ? entity.PriceRRP : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PriceLongevist", entity.PriceLongevist.HasValue ? entity.PriceLongevist : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShippingPrice", entity.ShippingPrice.HasValue ? entity.ShippingPrice : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsLongevistsOnly", entity.IsLongevistsOnly);
                command.Parameters.AddWithValue("@StockLevel", entity.StockLevel.HasValue ? entity.StockLevel : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder.HasValue ? entity.DisplayOrder : (object)DBNull.Value);
                command.Parameters.AddWithValue("@OptionsUnit", SafeGetStringValue(entity.OptionsUnit));
                command.Parameters.AddWithValue("@ImageThumb", SafeGetStringValue(entity.ImageThumb));
                command.Parameters.AddWithValue("@ImageMain", SafeGetStringValue(entity.ImageMain));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@CreatedDT", entity.CreatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.CreatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy.HasValue ? entity.CreatedBy : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.HasValue ? entity.UpdatedBy : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving shop product to the database");
            }
        }

        public override void Update(ShopProduct entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopProduct} SET" +
                    $" IsDeleted = @IsDeleted, IsEnabled = @IsEnabled, ShopProductName = @ShopProductName, SKU = @SKU," +
                    $" ShopProductDescription = @ShopProductDescription, ContentMain = @ContentMain, ShopCategoryId = @ShopCategoryId, PriceRRP = @PriceRRP," +
                    $" PriceLongevist = @PriceLongevist, ShippingPrice = @ShippingPrice, IsLongevistsOnly = @IsLongevistsOnly, StockLevel = @StockLevel, DisplayOrder = @DisplayOrder, OptionsUnit = @OptionsUnit," +
                    $" ImageThumb = @ImageThumb, ImageMain = @ImageMain, NotesInternal = @NotesInternal, UpdatedDT = @UpdatedDT, UpdatedBy = @UpdatedBy" +
                    $" WHERE ShopProductID = {entity.ShopProductId}");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@ShopProductName", SafeGetStringValue(entity.ShopProductName));
                command.Parameters.AddWithValue("@SKU", SafeGetStringValue(entity.SKU));
                command.Parameters.AddWithValue("@ShopProductDescription", SafeGetStringValue(entity.ShopProductDescription));
                command.Parameters.AddWithValue("@ContentMain", SafeGetStringValue(entity.ContentMain));
                command.Parameters.AddWithValue("@ShopCategoryId", entity.ShopCategoryId.HasValue ? entity.ShopCategoryId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PriceRRP", entity.PriceRRP.HasValue ? entity.PriceRRP : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PriceLongevist", entity.PriceLongevist.HasValue ? entity.PriceLongevist : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShippingPrice", entity.ShippingPrice.HasValue ? entity.ShippingPrice : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsLongevistsOnly", entity.IsLongevistsOnly);
                command.Parameters.AddWithValue("@StockLevel", entity.StockLevel.HasValue ? entity.StockLevel : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder.HasValue ? entity.DisplayOrder : (object)DBNull.Value);
                command.Parameters.AddWithValue("@OptionsUnit", SafeGetStringValue(entity.OptionsUnit));
                command.Parameters.AddWithValue("@ImageThumb", SafeGetStringValue(entity.ImageThumb));
                command.Parameters.AddWithValue("@ImageMain", SafeGetStringValue(entity.ImageMain));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.HasValue ? entity.UpdatedBy : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing shop product in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopProduct} SET IsDeleted = 1 WHERE ShopProductID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop product in the database");
            }
        }

        public override IEnumerable<ShopProduct> GetAll()
        {
            var sql = $"SELECT p.ShopProductID as 0, p.IsDeleted as 1, p.IsEnabled as 2, p.ShopProductName as 3, p.UrlName as 4," +
                $" p.SKU as 5, p.ShopProductDescription as 6, p.ContentMain as 7, p.ShopCategoryId as 8, p.PriceRRP as 9, p.PriceLongevist as 10," +
                $" p.ShippingPrice as 11, p.IsLongevistsOnly as 12, p.StockLevel as 13, p.DisplayOrder as 14, p.ImageThumb as 15, p.ImageMain as 16," +
                $" p.NotesInternal as 17, p.CreatedDT as 18, p.UpdatedDT as 19, p.CreatedBy as 20, p.UpdatedBy as 21, p.OptionsUnit as 22" +
                $" FROM {TableNameShopProduct} p" +
                $" WHERE p.IsDeleted = FALSE AND p.IsEnabled = TRUE AND p.SKU not like @SKU" +
                $" ORDER BY p.DisplayOrder ASC";

            return QueryAllData(sql, list =>
            {
                var shopProduct = MapShopProduct(0);
                list.Add(shopProduct);
            }, new Dictionary<string, object>() { { "SKU", "SM%" } });
        }

        public IEnumerable<ShopProduct> GetShopProducts(int limit, ShopProductsSortOrder sortField, SortDirection sortDirections)
        {
            var sql = $"SELECT TOP {limit} p.ShopProductID as 0, p.IsDeleted as 1, p.IsEnabled as 2, p.ShopProductName as 3, p.UrlName as 4," +
                $" p.SKU as 5, p.ShopProductDescription as 6, p.ContentMain as 7, p.ShopCategoryId as 8, p.PriceRRP as 9, p.PriceLongevist as 10," +
                $" p.ShippingPrice as 11, p.IsLongevistsOnly as 12, p.StockLevel as 13, p.DisplayOrder as 14, p.ImageThumb as 15, p.ImageMain as 16," +
                $" p.NotesInternal as 17, p.CreatedDT as 18, p.UpdatedDT as 19, p.CreatedBy as 20, p.UpdatedBy as 21, p.OptionsUnit as 22" +
                $" FROM {TableNameShopProduct} p" +
                $" WHERE p.IsDeleted = 0" +
                $" ORDER BY p.{sortField.ToString()} {(sortDirections == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var shopProduct = MapShopProduct(0);
                list.Add(shopProduct);
            });
        }

        public IEnumerable<ShopProduct> GetWebPageRelatedShopProducts(int webPageId)
        {
            var sql = $"SELECT p.ShopProductID as 0, p.IsDeleted as 1, p.IsEnabled as 2, p.ShopProductName as 3, p.UrlName as 4," +
                $" p.SKU as 5, p.ShopProductDescription as 6, p.ContentMain as 7, p.ShopCategoryId as 8, p.PriceRRP as 9, p.PriceLongevist as 10," +
                $" p.ShippingPrice as 11, p.IsLongevistsOnly as 12, p.StockLevel as 13, p.DisplayOrder as 14, p.ImageThumb as 15, p.ImageMain as 16," +
                $" p.NotesInternal as 17, p.CreatedDT as 18, p.UpdatedDT as 19, p.CreatedBy as 20, p.UpdatedBy as 21, p.OptionsUnit as 22" +
                $" FROM ({TableNameWebPages} wp" +
                $" LEFT JOIN {TableNameShopCategory} sc ON sc.PrimaryWebPageId = wp.WebPageID)" +
                $" LEFT JOIN {TableNameShopProduct} p ON p.ShopCategoryId = sc.ShopCategoryID" +
                $" WHERE  wp.WebPageID = {webPageId} AND p.IsDeleted = FALSE AND p.IsEnabled = TRUE" +
                $" ORDER BY p.UpdatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var shopProduct = MapShopProduct(0);
                list.Add(shopProduct);
            });
        }

        public ShopProduct GetShopProductByUrlName(string urlName)
        {
            var sql = $"SELECT p.ShopProductID as 0, p.IsDeleted as 1, p.IsEnabled as 2, p.ShopProductName as 3, p.UrlName as 4," +
                $" p.SKU as 5, p.ShopProductDescription as 6, p.ContentMain as 7, p.ShopCategoryId as 8, p.PriceRRP as 9, p.PriceLongevist as 10," +
                $" p.ShippingPrice as 11, p.IsLongevistsOnly as 12, p.StockLevel as 13, p.DisplayOrder as 14, p.ImageThumb as 15, p.ImageMain as 16," +
                $" p.NotesInternal as 17, p.CreatedDT as 18, p.UpdatedDT as 19, p.CreatedBy as 20, p.UpdatedBy as 21, p.OptionsUnit as 22" +
                $" FROM {TableNameShopProduct} p" +
                $" WHERE p.UrlName = @UrlName AND p.IsDeleted = 0";

            return QueryAllData(sql, list =>
            {
                var shopProduct = MapShopProduct(0);
                list.Add(shopProduct);
            }, new Dictionary<string, object>() { { "UrlName", urlName } }).FirstOrDefault();
        }

        public ShopProduct GetShopProductBySKU(string sku)
        {
            var sql = $"SELECT p.ShopProductID as 0, p.IsDeleted as 1, p.IsEnabled as 2, p.ShopProductName as 3, p.UrlName as 4," +
                $" p.SKU as 5, p.ShopProductDescription as 6, p.ContentMain as 7, p.ShopCategoryId as 8, p.PriceRRP as 9, p.PriceLongevist as 10," +
                $" p.ShippingPrice as 11, p.IsLongevistsOnly as 12, p.StockLevel as 13, p.DisplayOrder as 14, p.ImageThumb as 15, p.ImageMain as 16," +
                $" p.NotesInternal as 17, p.CreatedDT as 18, p.UpdatedDT as 19, p.CreatedBy as 20, p.UpdatedBy as 21, p.OptionsUnit as 22" +
                $" FROM {TableNameShopProduct} p" +
                $" WHERE p.SKU = @SKU AND p.IsDeleted = 0";

            return QueryAllData(sql, list =>
            {
                var shopProduct = MapShopProduct(0);
                list.Add(shopProduct);
            }, new Dictionary<string, object>() { { "SKU", sku } }).FirstOrDefault();
        }

        public ShopProduct GetShopProductById(int id)
        {
            var sql = $"SELECT p.ShopProductID as 0, p.IsDeleted as 1, p.IsEnabled as 2, p.ShopProductName as 3, p.UrlName as 4," +
                $" p.SKU as 5, p.ShopProductDescription as 6, p.ContentMain as 7, p.ShopCategoryId as 8, p.PriceRRP as 9, p.PriceLongevist as 10," +
                $" p.ShippingPrice as 11, p.IsLongevistsOnly as 12, p.StockLevel as 13, p.DisplayOrder as 14, p.ImageThumb as 15, p.ImageMain as 16," +
                $" p.NotesInternal as 17, p.CreatedDT as 18, p.UpdatedDT as 19, p.CreatedBy as 20, p.UpdatedBy as 21, p.OptionsUnit as 22," +
                $" u.UserNameFirst as 23, u.UserNameLast as 24," +
                $" sc.ShopCategoryID as 25, sc.IsDeleted as 26, sc.IsEnabled as 27, sc.ShopCategoryName as 28," +
                $" sc.PrimaryWebPageId as 29, sc.DnPrimaryWebPageName as 30, sc.DnPrimaryWebPageUrlName as 31, sc.ImageThumb as 32, sc.ImageMain as 33," +
                $" sc.NotesInternal as 34, sc.CreatedDT as 35, sc.UpdatedDT as 36, sc.CreatedByUserId as 37, sc.UpdatedByUserId as 38" +
                $" FROM (({TableNameShopProduct} p)" +
                $" LEFT JOIN {TableNameUsers} u on p.UpdatedBy = u.UserID)" +
                $" LEFT JOIN {TableNameShopCategory} sc on p.ShopCategoryId = sc.ShopCategoryID" +
                $" WHERE p.ShopProductID = {id} AND p.IsDeleted = 0";

            return QueryAllData(sql, list =>
            {
                var shopProduct = MapShopProduct(0);
                if (GetTableValue<int?>(21).HasValue)
                {
                    var username = MapUser(23);
                    shopProduct.LastUpdatedBy = $"{username.FirstName} {username.LastName}";
                }
                if (GetTableValue<int?>(8).HasValue)
                    shopProduct.ShopCategory = MapShopCategory(25);
                list.Add(shopProduct);
            }).FirstOrDefault();
        }

        public bool IsNonDeletedShopProductExists(string urlName, int? excludeShopProductId)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = excludeShopProductId.HasValue
                     ? $"SELECT COUNT(*) FROM {TableNameShopProduct} p WHERE p.ShopProductID <> @ExcludeId AND p.IsDeleted = 0 AND p.UrlName = @UrlName"
                     : $"SELECT COUNT(*) FROM {TableNameShopProduct} p WHERE p.IsDeleted = 0 AND p.UrlName = @UrlName";

                var command = MsAccessDbManager.CreateCommand(sql);

                if (excludeShopProductId.HasValue)
                    command.Parameters.AddWithValue("@ExcludeId", excludeShopProductId.Value);
                command.Parameters.AddWithValue("@UrlName", urlName);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
