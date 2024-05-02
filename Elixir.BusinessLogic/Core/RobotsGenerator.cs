using System;
using System.IO;
using Elixir.Contracts.Interfaces.Core;

namespace Elixir.BusinessLogic.Core
{
    public class RobotsGenerator : IRobotsGenerator
    {
        public string GenerateRobots(string serverUrl, Func<string, string> mapServerPath)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentNullException(nameof(serverUrl), "URL host name is empty.");
            if (mapServerPath == null)
                throw new ArgumentNullException(nameof(mapServerPath));

            var robotsDev = "robots.dev.txt";
            var robotsLive = "robots.live.txt";
            var robotsPath = "~/App_Data/robots";


            var resultPath = Path.Combine(robotsPath, robotsDev);

            if (serverUrl.StartsWith("liveforever.club", StringComparison.OrdinalIgnoreCase))
                resultPath = Path.Combine(robotsPath, robotsLive);

            var path = mapServerPath(resultPath);
            var robotsTxt = File.ReadAllText(path);

            return robotsTxt;
        }
    }
}
