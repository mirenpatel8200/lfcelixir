using System.Collections.Generic;
using Elixir.Models;

namespace Elixir.ViewModels
{
    public class SidebarViewModel
    {
        public IEnumerable<UrlLink> LatestBlogPosts { get; set; }
    }
}