using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Elixir.BusinessLogic.Processors
{
    public class SearchLogsProcessor : ISearchLogsProcessor
    {
        private readonly ISearchLogsRepository _searchLogsRepository;
        private readonly ISettingsRepository _settingsRepository;

        public SearchLogsProcessor(ISearchLogsRepository searchLogsRepository,
            ISettingsRepository settingsRepository)
        {
            _searchLogsRepository = searchLogsRepository;
            _settingsRepository = settingsRepository;
        }

        //public IEnumerable<SearchLog> GetLogs(int maxNumber = 100)
        //{
        //    return _searchLogsRepository.GetLogs(100);
        //}

        public void Log(DateTime created, string ipAddress, string searched, int wordCount)
        {
            if (!IsIpIgnored(ipAddress))
            {
                var logRecord = new SearchLog()
                {
                    Created = created,
                    IPAddress = ipAddress,
                    Search = searched,
                    WordCount = wordCount
                };

                _searchLogsRepository.Insert(logRecord);
            }
        }

        private bool IsIpIgnored(string ipAddress)
        {
            var settingsEntry = _settingsRepository.GetByPairName("LogIgnoreIP");
            if (settingsEntry == null)
                return false;
            var ips = settingsEntry.PairValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return ips.Any(x => x.Equals(ipAddress, StringComparison.OrdinalIgnoreCase));
        }
    }
}
