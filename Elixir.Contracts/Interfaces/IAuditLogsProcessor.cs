using System.Collections.Generic;
using Elixir.Models;
using Elixir.Models.Enums;

namespace Elixir.Contracts.Interfaces
{
    public interface IAuditLogsProcessor
    {
        void Log(AuditLog model);
        void ArticleLinkTrackLog(int userId, string ipAddress, int articleId, string notesLog);
        void CreateAuditLog(AuditLog auditLog);
        int NoOfFailedLoginsForUserInSeconds(int seconds, string userEmail, int userId);
        int NoOfFailedLoginsByHours(int hours = 0);
        int NoOfFailedLoginsByDays(int days = 0);

        IEnumerable<AuditLog> Get100AuditLog(AuditLogSortField sortField, SortDirection sortDirection, int UserId, int Entity, int EntityId);
       
        IEnumerable<AuditLog> GetFilteredAuditLogs(string filter, AuditLogSortField sortField, SortDirection sortDirection,int UserId,int Entity,int EntityId);

    }
}
