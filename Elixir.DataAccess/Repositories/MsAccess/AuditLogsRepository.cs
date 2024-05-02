using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.DataAccess.Repositories.MsAccess
{
    public class AuditLogsRepository : AbstractRepository<AuditLog>, IAuditLogsRepository
    {
        public override void Insert(AuditLog entity)
        {
            using (MsAccessDbManager = new MsAccessDbManager())
            {
                OleDbCommand command = MsAccessDbManager.CreateCommand(
                    $"INSERT INTO {TableNameAuditLogs} (CreatedDT, IpAddressString, UserID,EntityTypeID,EntityID,ActionTypeID,NotesLog) " +
                    $"VALUES (@CreatedDT, @IpAddressString, @UserID, @EntityTypeID, @EntityID, @ActionTypeID, @NotesLog)");

                    command.Parameters.AddWithValue("@CreatedDT", GetDateWithoutMilliseconds(entity.CreatedDT));
                    command.Parameters.AddWithValue("@IpAddressString", SafeGetStringValue( entity.IpAddressString));
                    command.Parameters.AddWithValue("@UserID", entity.UserID);
                    command.Parameters.AddWithValue("@EntityTypeID", entity.EntityTypeID);
                    command.Parameters.AddWithValue("@EntityID", entity.EntityID);
                    command.Parameters.AddWithValue("@ActionTypeID", entity.ActionTypeID);
                    command.Parameters.AddWithValue("@NotesLog", SafeGetStringValue( entity.NotesLog));

                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected != 1)
                    throw new InvalidOperationException("Error during saving AuditLog to the database");
            }
        }
        public override void Update(AuditLog entity)
        {
            throw new NotImplementedException();
        }

        public override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<AuditLog> GetAll()        
        {
            var sql =
                 $"SELECT al.AuditLogID,al.CreatedDT,al.IpAddressString,al.UserID,al.EntityTypeID,al.EntityID, al.ActionTypeID, al.NotesLog FROM {TableNameAuditLogs} al";

            return QueryAllData(sql, list =>
            {
                var auditLog = MapAuditLog(0);

                list.Add(auditLog);
            });
        }

        public int NoOfFailedLoginsForUserInSeconds(int seconds,string userEmail,int userId)
        {
            string sql = string.Empty;
            if (userId>0)
            {
                sql = $"SELECT COUNT(*) FROM {TableNameAuditLogs} al WHERE (al.EntityID  = {userId}) AND (al.ActionTypeID = 11) AND (CreatedDT BETWEEN NOW() AND DateAdd('s', -{seconds}, NOW()))";
            }
            else if(!string.IsNullOrEmpty(userEmail))
            {
                sql = $"SELECT COUNT(*) FROM {TableNameAuditLogs} al WHERE  (al.NotesLog = '{userEmail.ToLower()}') AND (al.ActionTypeID = 11) AND (CreatedDT BETWEEN NOW() AND DateAdd('s', -{seconds}, NOW()))";
            }
            var count = QueryScalar(sql);
            return count;
        }

        public int NoOfFailedLoginsByHours(int hours=0)
        {
            var sql =$"SELECT COUNT(*) FROM {TableNameAuditLogs} al WHERE (al.ActionTypeID = 11) AND (CreatedDT BETWEEN NOW() AND DateAdd('h', -{hours}, NOW()))";
           
            var count = QueryScalar(sql);
            return count;
        }
        public int NoOfFailedLoginsByDays(int days = 0)
        {
            var sql = $"SELECT COUNT(*) FROM {TableNameAuditLogs} al WHERE (al.ActionTypeID = 11) AND (CreatedDT BETWEEN NOW() AND DateAdd('d', -{days}, NOW()))";

            var count = QueryScalar(sql);
            return count;
        }

        public IEnumerable<AuditLog> GetN(int count, AuditLogSortField[] sortFields, SortDirection[] sortDirections, int UserId, int Entity, int EntityId)
        {
            var sql =
                $"SELECT TOP {count} al.AuditLogID,al.CreatedDT,al.IpAddressString,al.UserID,al.EntityTypeID,al.EntityID, al.ActionTypeID, al.NotesLog FROM {TableNameAuditLogs} al";
          
            const string sortingTableName = "al";
            var tableNames = Enumerable.Repeat(sortingTableName, sortFields.Length);

            if (UserId >= 0 || Entity > 0 || EntityId >= 0)
            {
                sql = sql + " where ";
                if (UserId >= 0)
                {
                    sql = sql + " al.UserID=" + UserId;
                }
                if (Entity > 0)
                {
                    if (UserId >= 0)
                    {
                        sql = sql + " AND al.EntityTypeID=" + Entity;
                    }
                    else
                    {
                        sql = sql + " al.EntityTypeID=" + Entity;
                    }
                    
                }
                if (EntityId >= 0)
                {
                    if (UserId >= 0 || Entity>0)
                    {
                        sql = sql + " AND al.EntityID=" + EntityId;
                    }
                    else
                    {
                        sql = sql + " al.EntityID=" + EntityId;
                    }
                    
                }
            }

            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()).ToArray(), sortDirections, tableNames.ToArray());

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, list =>
            {
                var auditLog = MapAuditLog(0);
                list.Add(auditLog);
            });
        }

        public IEnumerable<AuditLog> GetFiltered(string filter, AuditLogSortField[] sortFields, SortDirection[] sortDirections)
        {
            bool filterExists = !string.IsNullOrEmpty(filter);
            var parameters = new Dictionary<string, object>();

            var sql =
               $"SELECT al.AuditLogID,al.CreatedDT,al.IpAddressString,al.UserID,al.EntityTypeID,al.EntityID, al.ActionTypeID, al.NotesLog FROM {TableNameAuditLogs} al"; ;
            if (filterExists)
            {
                sql += " WHERE al.AuditLogId LIKE @filter";
                parameters.Add("@filter", $"%{filter}%");
            }

            const string sortingTableName = "al";
            var tableNames = Enumerable.Repeat(sortingTableName, sortFields.Length);

            var ctx = new OrmSortingContext(sortFields.Select(x => x.ToString()).ToArray(), sortDirections, tableNames.ToArray());

            var tools = new OrmTools(sql);
            sql = tools.AppendSorting(ctx);

            return QueryAllData(sql, (list =>
            {
                var auditLog = MapAuditLog(0);
                list.Add(auditLog);
            }), parameters);
        }

    }
}
