using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class ShopOrderRepository : AbstractRepository<ShopOrder>, IShopOrderRepository
    {
        public override void Insert(ShopOrder entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameShopOrder} (IsDeleted, IDHashCode, StatusID, UserID, OrderPlacedDT, OrderPlacedIPAddressString, OrderDespatchedDT," +
                    $" NotesPublic, NotesInternal, UpdatedDT, UpdatedByUserId, CreatedDT, CreatedByUserId, ItemsTotal, ShippingPricePaid, PricePaidTotal," +
                    $" EmailAddress, AddressNameFirst, AddressNameLast, AddressLine1, AddressLine2, AddressLine3, AddressTown, AddressPostcode, AddressCountryID, TelephoneNumber)" +
                    $" VALUES (@IsDeleted, @IDHashCode, @StatusID, @UserID, @OrderPlacedDT, @OrderPlacedIPAddressString, OrderDespatchedDT," +
                    $" @NotesPublic, @NotesInternal, @UpdatedDT, @UpdatedByUserId, @CreatedDT, @CreatedByUserId, @ItemsTotal, @ShippingPricePaid, @PricePaidTotal," +
                    $" @EmailAddress, @AddressNameFirst, @AddressNameLast, @AddressLine1, @AddressLine2, @AddressLine3, @AddressTown, @AddressPostcode, @AddressCountryID, @TelephoneNumber)");

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                command.Parameters.AddWithValue("@IDHashCode", SafeGetStringValue(entity.IDHashCode));
                command.Parameters.AddWithValue("@StatusID", entity.StatusId.HasValue ? entity.StatusId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserID", entity.UserId.HasValue ? entity.UserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderPlacedDT", entity.OrderPlacedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.OrderPlacedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderPlacedIPAddressString", SafeGetStringValue(entity.OrderPlacedIPAddressString));
                command.Parameters.AddWithValue("@OrderDespatchedDT", entity.OrderDespatchedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.OrderDespatchedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@NotesPublic", SafeGetStringValue(entity.NotesPublic));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDT", entity.CreatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.CreatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedByUserId.HasValue ? entity.CreatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ItemsTotal", entity.ItemsTotal.HasValue ? entity.ItemsTotal : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShippingPricePaid", entity.ShippingPricePaid.HasValue ? entity.ShippingPricePaid : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PricePaidTotal", entity.PricePaidTotal.HasValue ? entity.PricePaidTotal : (object)DBNull.Value);
                command.Parameters.AddWithValue("@EmailAddress", SafeGetStringValue(entity.EmailAddress));
                command.Parameters.AddWithValue("@AddressNameFirst", SafeGetStringValue(entity.AddressNameFirst));
                command.Parameters.AddWithValue("@AddressNameLast", SafeGetStringValue(entity.AddressNameLast));
                command.Parameters.AddWithValue("@AddressLine1", SafeGetStringValue(entity.AddressLine1));
                command.Parameters.AddWithValue("@AddressLine2", SafeGetStringValue(entity.AddressLine2));
                command.Parameters.AddWithValue("@AddressLine3", SafeGetStringValue(entity.AddressLine3));
                command.Parameters.AddWithValue("@AddressTown", SafeGetStringValue(entity.AddressTown));
                command.Parameters.AddWithValue("@AddressPostcode", SafeGetStringValue(entity.AddressPostcode));
                command.Parameters.AddWithValue("@AddressCountryID", entity.AddressCountryID.HasValue ? entity.AddressCountryID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@TelephoneNumber", SafeGetStringValue(entity.TelephoneNumber));

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving shop order to the database");
            }
        }

        public override void Update(ShopOrder entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateShopOrder(ShopOrder entity, bool isNewShopOrder = false)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = null;
                if (isNewShopOrder)
                {
                    command = MsAccessDbManager.CreateCommand(
                       $"UPDATE {TableNameShopOrder} SET" +
                       $" IsDeleted = @IsDeleted, IDHashCode = @IDHashCode, StatusID = @StatusID, UserID = @UserID," +
                       $" OrderPlacedDT = @OrderPlacedDT, OrderPlacedIPAddressString = @OrderPlacedIPAddressString, OrderDespatchedDT = @OrderDespatchedDT," +
                       $" NotesPublic = @NotesPublic, NotesInternal = @NotesInternal, UpdatedDT = @UpdatedDT, UpdatedByUserId = @UpdatedByUserId," +
                       $" CreatedDT = @CreatedDT, CreatedByUserId = @CreatedByUserId, ItemsTotal = @ItemsTotal, ShippingPricePaid = @ShippingPricePaid, PricePaidTotal = @PricePaidTotal," +
                       $" EmailAddress = @EmailAddress, AddressNameFirst = @AddressNameFirst, AddressNameLast = @AddressNameLast, AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2," +
                       $" AddressLine3 = @AddressLine3, AddressTown = @AddressTown, AddressPostcode = @AddressPostcode, AddressCountryID = @AddressCountryID, TelephoneNumber = @TelephoneNumber" +
                       $" WHERE ShopOrderID = {entity.ShopOrderId}");
                }
                else
                {
                    command = MsAccessDbManager.CreateCommand(
                       $"UPDATE {TableNameShopOrder} SET" +
                       $" IsDeleted = @IsDeleted, StatusID = @StatusID, UserID = @UserID," +
                       $" OrderPlacedDT = @OrderPlacedDT, OrderPlacedIPAddressString = @OrderPlacedIPAddressString, OrderDespatchedDT = @OrderDespatchedDT," +
                       $" NotesPublic = @NotesPublic, NotesInternal = @NotesInternal, UpdatedDT = @UpdatedDT, UpdatedByUserId = @UpdatedByUserId," +
                       $" CreatedDT = @CreatedDT, CreatedByUserId = @CreatedByUserId, ItemsTotal = @ItemsTotal, ShippingPricePaid = @ShippingPricePaid, PricePaidTotal = @PricePaidTotal," +
                       $" EmailAddress = @EmailAddress, AddressNameFirst = @AddressNameFirst, AddressNameLast = @AddressNameLast, AddressLine1 = @AddressLine1, AddressLine2 = @AddressLine2," +
                       $" AddressLine3 = @AddressLine3, AddressTown = @AddressTown, AddressPostcode = @AddressPostcode, AddressCountryID = @AddressCountryID, TelephoneNumber = @TelephoneNumber" +
                       $" WHERE ShopOrderID = {entity.ShopOrderId}");
                }

                command.Parameters.AddWithValue("@IsDeleted", entity.IsDeleted);
                if (isNewShopOrder)
                    command.Parameters.AddWithValue("@IDHashCode", SafeGetStringValue(entity.IDHashCode));
                command.Parameters.AddWithValue("@StatusID", entity.StatusId.HasValue ? entity.StatusId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserID", entity.UserId.HasValue ? entity.UserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderPlacedDT", entity.OrderPlacedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.OrderPlacedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderPlacedIPAddressString", SafeGetStringValue(entity.OrderPlacedIPAddressString));
                command.Parameters.AddWithValue("@OrderDespatchedDT", entity.OrderDespatchedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.OrderDespatchedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@NotesPublic", SafeGetStringValue(entity.NotesPublic));
                command.Parameters.AddWithValue("@NotesInternal", SafeGetStringValue(entity.NotesInternal));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedByUserId.HasValue ? entity.UpdatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDT", entity.CreatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.CreatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedByUserId.HasValue ? entity.CreatedByUserId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ItemsTotal", entity.ItemsTotal.HasValue ? entity.ItemsTotal : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShippingPricePaid", entity.ShippingPricePaid.HasValue ? entity.ShippingPricePaid : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PricePaidTotal", entity.PricePaidTotal.HasValue ? entity.PricePaidTotal : (object)DBNull.Value);
                command.Parameters.AddWithValue("@EmailAddress", SafeGetStringValue(entity.EmailAddress));
                command.Parameters.AddWithValue("@AddressNameFirst", SafeGetStringValue(entity.AddressNameFirst));
                command.Parameters.AddWithValue("@AddressNameLast", SafeGetStringValue(entity.AddressNameLast));
                command.Parameters.AddWithValue("@AddressLine1", SafeGetStringValue(entity.AddressLine1));
                command.Parameters.AddWithValue("@AddressLine2", SafeGetStringValue(entity.AddressLine2));
                command.Parameters.AddWithValue("@AddressLine3", SafeGetStringValue(entity.AddressLine3));
                command.Parameters.AddWithValue("@AddressTown", SafeGetStringValue(entity.AddressTown));
                command.Parameters.AddWithValue("@AddressPostcode", SafeGetStringValue(entity.AddressPostcode));
                command.Parameters.AddWithValue("@AddressCountryID", entity.AddressCountryID.HasValue ? entity.AddressCountryID : (object)DBNull.Value);
                command.Parameters.AddWithValue("@TelephoneNumber", SafeGetStringValue(entity.TelephoneNumber));

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing shop order in the database");
            }
        }

        public override void Delete(int id)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameShopOrder} SET IsDeleted = 1 WHERE ShopOrderID = {id}");

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during deleting shop order in the database");
            }
        }

        public override IEnumerable<ShopOrder> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ShopOrder> GetShopOrders(int limit, ShopOrdersSortOrder sortField, SortDirection sortDirections)
        {
            var sql = $"SELECT TOP {limit} so.ShopOrderID as 0, so.IsDeleted as 1, so.IDHashCode as 2, so.StatusID as 3, so.UserID as 4, so.OrderPlacedDT as 5," +
                $" so.OrderPlacedIPAddressString as 6, so.OrderDespatchedDT as 7, so.NotesPublic as 8, so.NotesInternal as 9," +
                $" so.CreatedDT as 10, so.CreatedByUserId as 11, so.UpdatedDT as 12, so.UpdatedByUserId as 13," +
                $" so.ItemsTotal as 14, so.ShippingPricePaid as 15, so.PricePaidTotal as 16, so.EmailAddress as 17, so.AddressNameFirst as 18, so.AddressNameLast as 19," +
                $" so.AddressLine1 as 20, so.AddressLine2 as 21, so.AddressLine3 as 22, so.AddressTown as 23, so.AddressPostcode as 24, so.AddressCountryID as 25, so.TelephoneNumber as 26" +
                $" FROM {TableNameShopOrder} so" +
                $" WHERE so.IsDeleted = FALSE" +
                $" ORDER BY so.{sortField.ToString()} {(sortDirections == SortDirection.Ascending ? "ASC" : "DESC")}";

            return QueryAllData(sql, list =>
            {
                var shopOrder = MapShopOrder(0);
                list.Add(shopOrder);
            });
        }

        public ShopOrder GetUserCart(int userId)
        {
            var sql = $"SELECT so.ShopOrderID as 0, so.IsDeleted as 1, so.IDHashCode as 2, so.StatusID as 3, so.UserID as 4, so.OrderPlacedDT as 5," +
                $" so.OrderPlacedIPAddressString as 6, so.OrderDespatchedDT as 7, so.NotesPublic as 8, so.NotesInternal as 9," +
                $" so.CreatedDT as 10, so.CreatedByUserId as 11, so.UpdatedDT as 12, so.UpdatedByUserId as 13," +
                $" so.ItemsTotal as 14, so.ShippingPricePaid as 15, so.PricePaidTotal as 16, so.EmailAddress as 17, so.AddressNameFirst as 18, so.AddressNameLast as 19," +
                $" so.AddressLine1 as 20, so.AddressLine2 as 21, so.AddressLine3 as 22, so.AddressTown as 23, so.AddressPostcode as 24, so.AddressCountryID as 25, so.TelephoneNumber as 26" +
                $" FROM {TableNameShopOrder} so" +
                $" WHERE so.UserID = {userId} AND so.StatusID = {(int)OrderStatus.ShoppingCart} AND so.IsDeleted = FALSE";

            return QueryAllData(sql, list =>
            {
                var shopOrder = MapShopOrder(0);
                list.Add(shopOrder);
            }).FirstOrDefault();
        }

        public ShopOrder GetGuestCart(string idHashCode, int userId = 0)
        {
            var sql = $"SELECT so.ShopOrderID as 0, so.IsDeleted as 1, so.IDHashCode as 2, so.StatusID as 3, so.UserID as 4, so.OrderPlacedDT as 5," +
               $" so.OrderPlacedIPAddressString as 6, so.OrderDespatchedDT as 7, so.NotesPublic as 8, so.NotesInternal as 9," +
               $" so.CreatedDT as 10, so.CreatedByUserId as 11, so.UpdatedDT as 12, so.UpdatedByUserId as 13," +
               $" so.ItemsTotal as 14, so.ShippingPricePaid as 15, so.PricePaidTotal as 16, so.EmailAddress as 17, so.AddressNameFirst as 18, so.AddressNameLast as 19," +
               $" so.AddressLine1 as 20, so.AddressLine2 as 21, so.AddressLine3 as 22, so.AddressTown as 23, so.AddressPostcode as 24, so.AddressCountryID as 25, so.TelephoneNumber as 26" +
               $" FROM {TableNameShopOrder} so" +
               $" WHERE so.IDHashCode = @IDHashCode AND so.UserID = {userId} AND so.StatusID = {(int)OrderStatus.ShoppingCart} AND so.IsDeleted = FALSE";

            return QueryAllData(sql, list =>
            {
                var shopOrder = MapShopOrder(0);
                list.Add(shopOrder);
            }, new Dictionary<string, object>() { { "IDHashCode", idHashCode } }).FirstOrDefault();
        }

        public ShopOrder GetShopOrderByUser(string idHashCode, int userId)
        {
            var sql = $"SELECT so.ShopOrderID as 0, so.IsDeleted as 1, so.IDHashCode as 2, so.StatusID as 3, so.UserID as 4, so.OrderPlacedDT as 5," +
               $" so.OrderPlacedIPAddressString as 6, so.OrderDespatchedDT as 7, so.NotesPublic as 8, so.NotesInternal as 9," +
               $" so.CreatedDT as 10, so.CreatedByUserId as 11, so.UpdatedDT as 12, so.UpdatedByUserId as 13," +
               $" so.ItemsTotal as 14, so.ShippingPricePaid as 15, so.PricePaidTotal as 16, so.EmailAddress as 17, so.AddressNameFirst as 18, so.AddressNameLast as 19," +
                $" so.AddressLine1 as 20, so.AddressLine2 as 21, so.AddressLine3 as 22, so.AddressTown as 23, so.AddressPostcode as 24, so.AddressCountryID as 25, so.TelephoneNumber as 26" +
               $" FROM {TableNameShopOrder} so" +
               $" WHERE so.IDHashCode = @IDHashCode AND so.UserID = {userId} AND so.IsDeleted = FALSE";

            return QueryAllData(sql, list =>
            {
                var shopOrder = MapShopOrder(0);
                list.Add(shopOrder);
            }, new Dictionary<string, object>() { { "IDHashCode", idHashCode } }).FirstOrDefault();
        }

        public IEnumerable<ShopOrder> GetUserShopOrders(int userId)
        {
            var sql = $"SELECT so.ShopOrderID as 0, so.IsDeleted as 1, so.IDHashCode as 2, so.StatusID as 3, so.UserID as 4, so.OrderPlacedDT as 5," +
                $" so.OrderPlacedIPAddressString as 6, so.OrderDespatchedDT as 7, so.NotesPublic as 8, so.NotesInternal as 9," +
                $" so.CreatedDT as 10, so.CreatedByUserId as 11, so.UpdatedDT as 12, so.UpdatedByUserId as 13," +
                $" so.ItemsTotal as 14, so.ShippingPricePaid as 15, so.PricePaidTotal as 16, so.EmailAddress as 17, so.AddressNameFirst as 18, so.AddressNameLast as 19," +
                $" so.AddressLine1 as 20, so.AddressLine2 as 21, so.AddressLine3 as 22, so.AddressTown as 23, so.AddressPostcode as 24, so.AddressCountryID as 25, so.TelephoneNumber as 26" +
                $" FROM {TableNameShopOrder} so" +
                $" WHERE so.UserID = {userId} AND so.IsDeleted = FALSE" +
                $" ORDER BY so.UpdatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var shopOrder = MapShopOrder(0);
                list.Add(shopOrder);
            });
        }

        public ShopOrder GetShopOrderByIdHashCode(string idHashCode)
        {
            var sql = $"SELECT so.ShopOrderID as 0, so.IsDeleted as 1, so.IDHashCode as 2, so.StatusID as 3, so.UserID as 4, so.OrderPlacedDT as 5," +
               $" so.OrderPlacedIPAddressString as 6, so.OrderDespatchedDT as 7, so.NotesPublic as 8, so.NotesInternal as 9," +
               $" so.CreatedDT as 10, so.CreatedByUserId as 11, so.UpdatedDT as 12, so.UpdatedByUserId as 13," +
               $" so.ItemsTotal as 14, so.ShippingPricePaid as 15, so.PricePaidTotal as 16, so.EmailAddress as 17, so.AddressNameFirst as 18, so.AddressNameLast as 19," +
                $" so.AddressLine1 as 20, so.AddressLine2 as 21, so.AddressLine3 as 22, so.AddressTown as 23, so.AddressPostcode as 24, so.AddressCountryID as 25, so.TelephoneNumber as 26" +
               $" FROM {TableNameShopOrder} so" +
               $" WHERE so.IDHashCode = @IDHashCode AND so.IsDeleted = FALSE";

            return QueryAllData(sql, list =>
            {
                var shopOrder = MapShopOrder(0);
                list.Add(shopOrder);
            }, new Dictionary<string, object>() { { "IDHashCode", idHashCode } }).FirstOrDefault();
        }
    }
}
