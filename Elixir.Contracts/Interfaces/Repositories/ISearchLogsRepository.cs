using Elixir.Models;
using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface ISearchLogsRepository : IRepository<SearchLog>
    {
        IEnumerable<SearchLog> GetLogs(int limit);
    }
}
