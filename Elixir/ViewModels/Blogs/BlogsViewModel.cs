using System.Collections.Generic;
using Elixir.Models;

namespace Elixir.ViewModels
{
    public class BlogsViewModel
    {
        public IEnumerable<BlogPost> RecentBlogs { get; set; }
        public IEnumerable<int> ListOfYears { get; set; }
    }
}