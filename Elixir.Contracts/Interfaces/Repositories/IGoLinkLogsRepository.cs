using Elixir.Models;
using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IGoLinkLogsRepository : IRepository<GoLinkLog>
    {
        IEnumerable<GoLinkLog> GetLogs(int limit);
    }
}
