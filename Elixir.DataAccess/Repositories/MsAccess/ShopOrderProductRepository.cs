using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class ShopOrderProductRepository : AbstractRepository<ShopOrderProduct>, IShopOrderProductRepository
    {
        public override void Insert(ShopOrderProduct entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameShopOrderProduct} (ShopOrderID, ShopProductID, DnShopProductName, ShopProductOptionID," +
                    $" DnShopProductOptionName, SKU, PricePaidPerUnit, Quantity)" +
                    $" VALUES (@ShopOrderID, @ShopProductID, @DnShopProductName, @ShopProductOptionID, @DnShopProductOptionName," +
                    $" @SKU, @PricePaidPerUnit, @Quantity)");

                command.Parameters.AddWithValue("@ShopOrderID", entity.ShopOrderId);
                command.Parameters.AddWithValue("@ShopProductID", entity.ShopProductId);
                command.Parameters.AddWithValue("@DnShopProductName", SafeGetStringValue(entity.DnShopProductName));
                command.Parameters.AddWithValue("@ShopProductOptionID", entity.ShopProductOptionId.HasValue ? entity.ShopProductOptionId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DnShopProductOptionName", SafeGetStringValue(entity.DnShopProductOptionName));
                command.Parameters.AddWithValue("@SKU", SafeGetStringValue(entity.SKU));
                command.Parameters.AddWithValue("@PricePaidPerUnit", entity.PricePaidPerUnit.HasValue ? entity.PricePaidPerUnit : (object)DBNull.Value);
                command.Parameters.AddWithValue("@Quantity", entity.Quantity.HasValue ? entity.Quantity : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving shop order product to the database");
            }
        }

        public override void Update(ShopOrderProduct entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopOrderProduct} SET" +
                    $" ShopOrderID = @ShopOrderID, ShopProductID = @ShopProductID, DnShopProductName = @DnShopProductName, ShopProductOptionID = @ShopProductOptionID," +
                    $" DnShopProductOptionName = @DnShopProductOptionName, SKU = @SKU, PricePaidPerUnit = @PricePaidPerUnit, Quantity = @Quantity" +
                    $" WHERE ShopOrderProductID = {entity.ShopOrderProductId}");

                command.Parameters.AddWithValue("@ShopOrderID", entity.ShopOrderId);
                command.Parameters.AddWithValue("@ShopProductID", entity.ShopProductId);
                command.Parameters.AddWithValue("@DnShopProductName", SafeGetStringValue(entity.DnShopProductName));
                command.Parameters.AddWithValue("@ShopProductOptionID", entity.ShopProductOptionId.HasValue ? entity.ShopProductOptionId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@DnShopProductOptionName", SafeGetStringValue(entity.DnShopProductOptionName));
                command.Parameters.AddWithValue("@SKU", SafeGetStringValue(entity.SKU));
                command.Parameters.AddWithValue("@PricePaidPerUnit", entity.PricePaidPerUnit.HasValue ? entity.PricePaidPerUnit : (object)DBNull.Value);
                command.Parameters.AddWithValue("@Quantity", entity.Quantity.HasValue ? entity.Quantity : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing shop order product in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"DELETE FROM {TableNameShopOrderProduct} WHERE ShopOrderProductId = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop order product in the database");
            }
        }

        public override IEnumerable<ShopOrderProduct> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ShopOrderProduct> GetShopOrderProducts(int orderId)
        {
            var sql = $"SELECT sop.ShopOrderProductID as 0, sop.ShopOrderID as 1, sop.ShopProductID as 2, sop.DnShopProductName as 3," +
                $" sop.ShopProductOptionID as 4, sop.DnShopProductOptionName as 5, sop.SKU as 6, sop.PricePaidPerUnit as 7, sop.Quantity as 8," +
                $" p.ShopProductID as 9, p.IsDeleted as 10, p.IsEnabled as 11, p.ShopProductName as 12, p.UrlName as 13," +
                $" p.SKU as 14, p.ShopProductDescription as 15, p.ContentMain as 16, p.ShopCategoryId as 17, p.PriceRRP as 18, p.PriceLongevist as 19," +
                $" p.ShippingPrice as 20, p.IsLongevistsOnly as 21, p.StockLevel as 22, p.DisplayOrder as 23, p.ImageThumb as 24, p.ImageMain as 25," +
                $" p.NotesInternal as 26, p.CreatedDT as 27, p.UpdatedDT as 28, p.CreatedBy as 29, p.UpdatedBy as 30, p.OptionsUnit as 31" +
                $" FROM {TableNameShopOrderProduct} sop" +
                $" LEFT JOIN {TableNameShopProduct} p on p.ShopProductID = sop.ShopProductID" +
                $" WHERE sop.ShopOrderID = {orderId}" +
                $" ORDER BY sop.DnShopProductName ASC";

            return QueryAllData(sql, list =>
            {
                var shopOrderProduct = MapShopOrderProduct(0);
                if (GetTableValue<int?>(2).HasValue)
                    shopOrderProduct.ShopProduct = MapShopProduct(9);
                list.Add(shopOrderProduct);
            });
        }

        public ShopOrderProduct GetProductByOrder(int orderId, int productId, int? optionId)
        {
            var sql = $"SELECT sop.ShopOrderProductID as 0, sop.ShopOrderID as 1, sop.ShopProductID as 2, sop.DnShopProductName as 3," +
                $" sop.ShopProductOptionID as 4, sop.DnShopProductOptionName as 5, sop.SKU as 6, sop.PricePaidPerUnit as 7, sop.Quantity as 8" +
                $" FROM {TableNameShopOrderProduct} sop" +
                $" WHERE sop.ShopOrderID = {orderId} AND sop.ShopProductID = {productId}" + (optionId.HasValue ? $" AND sop.ShopProductOptionID = {optionId.Value}" : "");

            return QueryAllData(sql, list =>
            {
                var shopOrderProduct = MapShopOrderProduct(0);
                list.Add(shopOrderProduct);
            }).FirstOrDefault();
        }
    }
}
