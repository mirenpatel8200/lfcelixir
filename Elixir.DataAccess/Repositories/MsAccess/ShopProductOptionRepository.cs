using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class ShopProductOptionRepository : AbstractRepository<ShopProductOption>, IShopProductOptionRepository
    {
        public override void Insert(ShopProductOption entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameShopProductOption} (IsDeleted, IsEnabled, ShopProductID, OptionName, SkuSuffix," +
                    $" PriceExtra, StockLevel, DisplayOrder, IsDefaultOption, UpdatedDT, UpdatedByUserId, CreatedDT, CreatedByUserId)" +
                    $" VALUES (@IsDeleted, @IsEnabled, @ShopProductID, @OptionName, @SkuSuffix, @PriceExtra," +
                    $" @StockLevel, @DisplayOrder, @IsDefaultOption, @UpdatedDT, @UpdatedByUserId, @CreatedDT, @CreatedByUserId)");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@ShopProductID", entity.ShopProductId);
                command.Parameters.AddWithValue("@OptionName", SafeGetStringValue(entity.OptionName));
                command.Parameters.AddWithValue("@SkuSuffix", SafeGetStringValue(entity.SkuSuffix));
                command.Parameters.AddWithValue("@PriceExtra", entity.PriceExtra.HasValue ? entity.PriceExtra : (object)DBNull.Value);
                command.Parameters.AddWithValue("@StockLevel", entity.StockLevel.HasValue ? entity.StockLevel : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder.HasValue ? entity.DisplayOrder : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsDefaultOption", entity.IsDefaultOption);
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDT", entity.CreatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.CreatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedByUserId.HasValue ? entity.CreatedByUserId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving shop product option to the database");
            }
        }

        public override void Update(ShopProductOption entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopProductOption} SET" +
                    $" IsDeleted = @IsDeleted, IsEnabled = @IsEnabled, ShopProductID = @ShopProductID, OptionName = @OptionName," +
                    $" SkuSuffix = @SkuSuffix, PriceExtra = @PriceExtra, StockLevel = @StockLevel, DisplayOrder = @DisplayOrder," +
                    $" IsDefaultOption = @IsDefaultOption, UpdatedDT = @UpdatedDT, UpdatedByUserId = @UpdatedByUserId" +
                    $" WHERE ShopProductOptionID = {entity.ShopProductOptionId}");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IsEnabled", entity.IsEnabled);
                command.Parameters.AddWithValue("@ShopProductID", entity.ShopProductId);
                command.Parameters.AddWithValue("@OptionName", SafeGetStringValue(entity.OptionName));
                command.Parameters.AddWithValue("@SkuSuffix", SafeGetStringValue(entity.SkuSuffix));
                command.Parameters.AddWithValue("@PriceExtra", entity.PriceExtra.HasValue ? entity.PriceExtra : (object)DBNull.Value);
                command.Parameters.AddWithValue("@StockLevel", entity.StockLevel.HasValue ? entity.StockLevel : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DisplayOrder", entity.DisplayOrder.HasValue ? entity.DisplayOrder : (object)DBNull.Value);
                command.Parameters.AddWithValue("@IsDefaultOption", entity.IsDefaultOption);
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing shop product option in the database");
            }
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ShopProductOption> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Delete(int shopProductId, int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopProductOption} SET IsDeleted = 1 WHERE ShopProductOptionID = {id} AND ShopProductID = {shopProductId}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop product option in the database");
            }
        }

        public IEnumerable<ShopProductOption> GetShopProductOptions(int shopProductId)
        {
            var sql = $"SELECT spo.ShopProductOptionID as 0, spo.IsDeleted as 1, spo.IsEnabled as 2, spo.ShopProductID as 3," +
                $" spo.OptionName as 4, spo.SkuSuffix as 5, spo.PriceExtra as 6, spo.StockLevel as 7, spo.DisplayOrder as 8," +
                $" spo.IsDefaultOption as 9, spo.CreatedDT as 10, spo.UpdatedDT as 11, spo.CreatedByUserId as 12, spo.UpdatedByUserId as 13" +
                $" FROM {TableNameShopProductOption} spo" +
                $" WHERE spo.ShopProductID = {shopProductId} AND spo.IsDeleted = 0" +
                $" ORDER BY spo.UpdatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var shopProductOption = MapShopProductOption(0);
                list.Add(shopProductOption);
            });
        }

        public IEnumerable<ShopProductOption> GetAll(int shopProductId)
        {
            var sql = $"SELECT spo.ShopProductOptionID as 0, spo.IsDeleted as 1, spo.IsEnabled as 2, spo.ShopProductID as 3," +
                $" spo.OptionName as 4, spo.SkuSuffix as 5, spo.PriceExtra as 6, spo.StockLevel as 7, spo.DisplayOrder as 8," +
                $" spo.IsDefaultOption as 9, spo.CreatedDT as 10, spo.UpdatedDT as 11, spo.CreatedByUserId as 12, spo.UpdatedByUserId as 13" +
                $" FROM {TableNameShopProductOption} spo" +
                $" WHERE spo.ShopProductID = {shopProductId} AND spo.IsDeleted = FALSE AND spo.IsEnabled = TRUE" +
                $" ORDER BY spo.UpdatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var shopProductOption = MapShopProductOption(0);
                list.Add(shopProductOption);
            });
        }

        public ShopProductOption GetShopProductOptionByName(int shopProductId, string name)
        {
            var sql = $"SELECT spo.ShopProductOptionID as 0, spo.IsDeleted as 1, spo.IsEnabled as 2, spo.ShopProductID as 3," +
                $" spo.OptionName as 4, spo.SkuSuffix as 5, spo.PriceExtra as 6, spo.StockLevel as 7, spo.DisplayOrder as 8," +
                $" spo.IsDefaultOption as 9, spo.CreatedDT as 10, spo.UpdatedDT as 11, spo.CreatedByUserId as 12, spo.UpdatedByUserId as 13" +
                $" FROM {TableNameShopProductOption} spo" +
                $" WHERE spo.ShopProductID = {shopProductId} AND LCase(spo.OptionName) = @Name AND spo.IsDeleted = 0";

            return QueryAllData(sql, list =>
            {
                var shopProductOption = MapShopProductOption(0);
                list.Add(shopProductOption);
            }, new Dictionary<string, object>() { { "Name", name.ToLower() } }).FirstOrDefault();
        }

        public ShopProductOption GetShopProductOptionById(int shopProductId, int id)
        {
            var sql = $"SELECT spo.ShopProductOptionID as 0, spo.IsDeleted as 1, spo.IsEnabled as 2, spo.ShopProductID as 3," +
                $" spo.OptionName as 4, spo.SkuSuffix as 5, spo.PriceExtra as 6, spo.StockLevel as 7, spo.DisplayOrder as 8," +
                $" spo.IsDefaultOption as 9, spo.CreatedDT as 10, spo.UpdatedDT as 11, spo.CreatedByUserId as 12, spo.UpdatedByUserId as 13" +
                $" FROM {TableNameShopProductOption} spo" +
                $" WHERE spo.ShopProductID = {shopProductId} AND spo.ShopProductOptionID = @ShopProductOptionID AND spo.IsDeleted = 0";

            return QueryAllData(sql, list =>
            {
                var shopProductOption = MapShopProductOption(0);
                list.Add(shopProductOption);
            }, new Dictionary<string, object>() { { "ShopProductOptionID", id } }).FirstOrDefault();
        }

        public bool IsNonDeletedShopProductOptionExists(int shopProductId, string name, int? excludeShopProductOptionId)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = excludeShopProductOptionId.HasValue
                     ? $"SELECT COUNT(*) FROM {TableNameShopProductOption} spo WHERE spo.ShopProductOptionID <> @ExcludeId AND spo.ShopProductID = @ShopProductID AND spo.IsDeleted = 0 AND LCase(spo.OptionName) = @Name"
                     : $"SELECT COUNT(*) FROM {TableNameShopProductOption} spo WHERE spo.ShopProductID = @ShopProductID AND spo.IsDeleted = 0 AND LCase(spo.OptionName) = @Name";

                var command = MsAccessDbManager.CreateCommand(sql);

                if (excludeShopProductOptionId.HasValue)
                    command.Parameters.AddWithValue("@ExcludeId", excludeShopProductOptionId.Value);
                command.Parameters.AddWithValue("@ShopProductID", shopProductId);
                command.Parameters.AddWithValue("@Name", name.ToLower());

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
