using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class PaymentRepository : AbstractRepository<Payments>, IPaymentRepository
    {
        public override void Insert(Payments entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNamePayment} (PaymentReference, PaymentStatusID, Amount, ProcessorName, ShopOrderID," +
                    $" NotesUser, NotesAdmin, PaymentDT, PaymentResponse, UpdatedDT, UpdatedByUserId, CreatedDT, CreatedByUserId)" +
                    $" VALUES (@PaymentReference, @PaymentStatusID, @Amount, @ProcessorName, @ShopOrderID," +
                    $" @NotesUser, @NotesAdmin, @PaymentDT, @PaymentResponse, @UpdatedDT, @UpdatedByUserId, @CreatedDT, @CreatedByUserId)");

                command.Parameters.AddWithValue("@PaymentReference", SafeGetStringValue(entity.PaymentReference));
                command.Parameters.AddWithValue("@PaymentStatusID", entity.PaymentStatusId.HasValue ? entity.PaymentStatusId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@Amount", entity.Amount.HasValue ? entity.Amount : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProcessorName", SafeGetStringValue(entity.ProcessorName));
                command.Parameters.AddWithValue("@ShopOrderID", entity.ShopOrderId.HasValue ? entity.ShopOrderId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@NotesUser", SafeGetStringValue(entity.NotesUser));
                command.Parameters.AddWithValue("@NotesAdmin", SafeGetStringValue(entity.NotesAdmin));
                command.Parameters.AddWithValue("@PaymentDT", entity.PaymentDate.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.PaymentDate) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentResponse", SafeGetStringValue(entity.PaymentResponse));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedBy.HasValue ? entity.UpdatedBy : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDT", entity.CreatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.CreatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedByUserId", entity.CreatedBy.HasValue ? entity.CreatedBy : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving payment to the database");
            }
        }

        public override void Update(Payments entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNamePayment} SET" +
                    $" PaymentReference = @PaymentReference, PaymentStatusID = @PaymentStatusID, Amount = @Amount, ProcessorName = @ProcessorName, ShopOrderID = @ShopOrderID," +
                    $" NotesUser = @NotesUser, NotesAdmin = @NotesAdmin, PaymentDT = @PaymentDT, PaymentResponse = @PaymentResponse, UpdatedDT = @UpdatedDT, UpdatedByUserId = @UpdatedByUserId" +
                    $" WHERE PaymentID = {entity.PaymentId}");

                command.Parameters.AddWithValue("@PaymentReference", SafeGetStringValue(entity.PaymentReference));
                command.Parameters.AddWithValue("@PaymentStatusID", entity.PaymentStatusId.HasValue ? entity.PaymentStatusId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@Amount", entity.Amount.HasValue ? entity.Amount : (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProcessorName", SafeGetStringValue(entity.ProcessorName));
                command.Parameters.AddWithValue("@ShopOrderID", entity.ShopOrderId.HasValue ? entity.ShopOrderId : (object)DBNull.Value);
                command.Parameters.AddWithValue("@NotesUser", SafeGetStringValue(entity.NotesUser));
                command.Parameters.AddWithValue("@NotesAdmin", SafeGetStringValue(entity.NotesAdmin));
                command.Parameters.AddWithValue("@PaymentDT", entity.PaymentDate.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.PaymentDate) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@PaymentResponse", SafeGetStringValue(entity.PaymentResponse));
                command.Parameters.AddWithValue("@UpdatedDT", entity.UpdatedOn.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.UpdatedOn) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedByUserId", entity.UpdatedBy.HasValue ? entity.UpdatedBy : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing payment in the database");
            }
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Payments> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Payments> GetPaymentsByShopOrder(int shopOrderId)
        {
            var sql = $"SELECT p.PaymentID as 0, p.PaymentReference as 1, p.PaymentStatusID as 2, p.Amount as 3, p.ProcessorName as 4," +
                $" p.ShopOrderID as 5, p.NotesUser as 6, p.NotesAdmin as 7, p.PaymentDT as 8, p.PaymentResponse as 9," +
                $" p.UpdatedDT as 10, p.UpdatedByUserId as 11, p.CreatedDT as 12, p.CreatedByUserId as 13" +
                $" FROM {TableNamePayment} p" +
                $" WHERE p.ShopOrderID = {shopOrderId} ORDER BY p.UpdatedDT DESC";

            return QueryAllData(sql, list =>
            {
                var payment = MapPayment(0);
                list.Add(payment);
            }).ToList();
        }

        public Payments GetPaymentByPaymentReference(string paymentReference)
        {
            var sql = $"SELECT p.PaymentID as 0, p.PaymentReference as 1, p.PaymentStatusID as 2, p.Amount as 3, p.ProcessorName as 4," +
                $" p.ShopOrderID as 5, p.NotesUser as 6, p.NotesAdmin as 7, p.PaymentDT as 8, p.PaymentResponse as 9," +
                $" p.UpdatedDT as 10, p.UpdatedByUserId as 11, p.CreatedDT as 12, p.CreatedByUserId as 13" +
                $" FROM {TableNamePayment} p" +
                $" WHERE p.PaymentReference = @PaymentReference";

            return QueryAllData(sql, list =>
            {
                var payment = MapPayment(0);
                list.Add(payment);
            }, new Dictionary<string, object>() { { "PaymentReference", paymentReference } }).FirstOrDefault();
        }
    }
}
