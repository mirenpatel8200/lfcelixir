using System.Collections.Generic;
using Elixir.Contracts.Interfaces.Core;

namespace Elixir.Areas.Admin.ViewModels
{
    public class WebSiteViewModel
    {
        public IEnumerable<SitemapInfo> SitemapInfo { get; set; }
        public string SiteMapUrl { get; set; }
        public string ValidationHash { get; set; }
    }
}