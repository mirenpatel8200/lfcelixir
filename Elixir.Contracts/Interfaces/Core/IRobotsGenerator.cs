using System;

namespace Elixir.Contracts.Interfaces.Core
{
    public interface IRobotsGenerator
    {
        string GenerateRobots(string serverUrl, Func<string, string> mapServerPath);
    }
}
