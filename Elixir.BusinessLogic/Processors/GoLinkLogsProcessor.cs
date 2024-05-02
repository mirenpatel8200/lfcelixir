using System;
using System.Collections.Generic;
using System.Linq;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.BusinessLogic.Processors
{
    public class GoLinkLogsProcessor : IGoLinkLogsProcessor
    {
        private readonly IGoLinkLogsRepository _goLinkLogsRepository;
        private readonly ISettingsRepository _settingsRepository;

        public GoLinkLogsProcessor(IGoLinkLogsRepository goLinkLogsRepository, ISettingsRepository settingsRepository)
        {
            _goLinkLogsRepository = goLinkLogsRepository;
            _settingsRepository = settingsRepository;
        }

        public void Log(int goLinkId, string ipAddress)
        {
            if (!IsIpIgnored(ipAddress))
            {
                var logRecord = new GoLinkLog()
                {
                    Created = DateTime.Now,
                    GoLinkId = goLinkId,
                    IPAddress = ipAddress
                };

                _goLinkLogsRepository.Insert(logRecord);
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

        public IEnumerable<GoLinkLog> GetLogs(int maxNumber)
        {
            return _goLinkLogsRepository.GetLogs(maxNumber);
        }
    }
}
