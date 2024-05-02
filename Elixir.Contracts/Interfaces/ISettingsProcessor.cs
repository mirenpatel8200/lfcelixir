using Elixir.Models;

namespace Elixir.Contracts.Interfaces
{
    public interface ISettingsProcessor
    {
        string GetLiveBufferToken();
        string GetDevBufferToken();
        string GetLiveServerDomainName();
        string GetMonitorPageResponse();
        SettingsEntry GetSettingsByName(string name);
        void UpdateSettigs(SettingsEntry settings);
    }
}
