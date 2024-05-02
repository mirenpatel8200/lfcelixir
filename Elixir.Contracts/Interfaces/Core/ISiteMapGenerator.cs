using System.Collections.Generic;

namespace Elixir.Contracts.Interfaces.Core
{
    public interface ISiteMapGenerator
    {
        IEnumerable<SitemapInfo> GetSitemapInfo();
        string GenerateRootSiteMap();
        void SaveSiteMaps();
        string ServerRootPath { get; set; }
        string GetSiteMapUrl();
    }
}
