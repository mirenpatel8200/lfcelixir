using Elixir.Models;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface ISettingsRepository: IRepository<SettingsEntry>
    {
        SettingsEntry GetByPairName(string name);
    }
}
