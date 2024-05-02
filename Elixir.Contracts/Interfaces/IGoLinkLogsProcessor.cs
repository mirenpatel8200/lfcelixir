using System.Collections.Generic;
using Elixir.Models;

namespace Elixir.Contracts.Interfaces
{
    public interface IGoLinkLogsProcessor
    {
        void Log(int goLinkId, string ipAddress);
        IEnumerable<GoLinkLog> GetLogs(int maxNumber = 100);
    }
}
