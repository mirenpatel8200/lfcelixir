using Elixir.Models;
using Elixir.Models.Enums;
using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IAuditLogsRepository : IRepository<AuditLog>
    {
        int NoOfFailedLoginsForUserInSeconds(int seconds, string userEmail, int userId);
        int NoOfFailedLoginsByHours(int hours = 0);
        int NoOfFailedLoginsByDays(int days = 0);

        IEnumerable<AuditLog> GetN(int count, AuditLogSortField[] sortFields, SortDirection[] sortDirections, int UserId, int Entity, int EntityId);

        IEnumerable<AuditLog> GetFiltered(string filter, AuditLogSortField[] sortFields, SortDirection[] sortDirections);
    }
}
