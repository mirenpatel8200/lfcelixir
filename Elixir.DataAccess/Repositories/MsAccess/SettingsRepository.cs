using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class SettingsRepository : AbstractRepository<SettingsEntry>, ISettingsRepository
    {
        public override void Insert(SettingsEntry entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(SettingsEntry entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var command = MsAccessDbManager.CreateCommand(
                    $"UPDATE {TableNameSettings} SET PairName = @PairName, PairValue = @PairValue, SettingsDescription = @SettingsDescription, PayPalTokenExpirationDT = @PayPalTokenExpirationDT" +
                    $" WHERE SettingsID = {entity.SettingsId}");

                command.Parameters.AddWithValue("@PairName", !string.IsNullOrEmpty(entity.PairName) ? entity.PairName.Trim() : entity.PairName);
                command.Parameters.AddWithValue("@PairValue", !string.IsNullOrEmpty(entity.PairValue) ? entity.PairValue.Trim() : entity.PairValue);
                command.Parameters.AddWithValue("@SettingsDescription", !string.IsNullOrEmpty(entity.SettingsDescription) ? entity.SettingsDescription.Trim() : SafeGetStringValue(entity.SettingsDescription));
                command.Parameters.AddWithValue("@PayPalTokenExpirationDT", entity.PayPalTokenExpirationDT.HasValue ? GetDateWithoutMilliseconds((DateTime)entity.PayPalTokenExpirationDT) : (object)DBNull.Value);

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during editing Settings in the database");
            }
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<SettingsEntry> GetAll()
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = $"SELECT s.SettingsID, s.PairName, s.PairValue, s.SettingsDescription, s.PayPalTokenExpirationDT" +
                          $" FROM {TableNameSettings} s ";
                return QueryAllData(sql, list =>
                {
                    var entry = MapSettingEntry(0);
                    list.Add(entry);
                });
            }
        }

        public SettingsEntry GetByPairName(string name)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                var sql = $"SELECT s.SettingsID, s.PairName, s.PairValue, s.SettingsDescription, s.PayPalTokenExpirationDT" +
                          $" FROM {TableNameSettings} s" +
                          $" WHERE s.PairName = @PairName";
                return QueryAllData(sql, list =>
                {
                    var entry = MapSettingEntry(0);
                    list.Add(entry);
                }, new Dictionary<string, object>()
                {
                    { "@Name", name }
                }).FirstOrDefault();
            }
        }
    }
}
