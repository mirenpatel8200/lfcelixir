using System;
using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;

namespace Elixir.BusinessLogic.Processors
{
    public class SettingsProcessor : ISettingsProcessor
    {
        private readonly ISettingsRepository _settingsRepository;

        public SettingsProcessor(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        private SettingsEntry GetRequiredEntry(string name)
        {
            var entry = _settingsRepository.GetByPairName(name);
            if (entry == null)
                throw new InvalidOperationException($"Settings entry '{name}' is not found.");

            return entry;
        }

        public string GetLiveBufferToken()
        {
            return GetRequiredEntry("BufferTokenLive").PairValue;
        }

        public string GetDevBufferToken()
        {
            return GetRequiredEntry("BufferTokenDev").PairValue;
        }

        public string GetLiveServerDomainName()
        {
            return GetRequiredEntry("LiveServerDomainName").PairValue;
        }

        public string GetMonitorPageResponse()
        {
            return GetRequiredEntry("MonitorPageResponse").PairValue;
        }

        public SettingsEntry GetSettingsByName(string name)
        {
            return GetRequiredEntry(name);
        }

        public void UpdateSettigs(SettingsEntry settings)
        {
            _settingsRepository.Update(settings);
        }
    }
}
