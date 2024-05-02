using Elixir.Models;
using System;
using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces
{
    public interface ISearchLogsProcessor
    {
        void Log(DateTime created, string ipAddress, string search, int wordCount);
        //IEnumerable<SearchLog> GetLogs(int maxNumber = 100);
    }
}
