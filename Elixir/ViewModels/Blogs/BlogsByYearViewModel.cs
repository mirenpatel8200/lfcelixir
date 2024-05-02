using System.Collections.Generic;
using Elixir.Models;

namespace Elixir.ViewModels
{
    public class BlogsByYearViewModel
    {
        public int Year { get; set; }
        public IEnumerable<BlogPost> BlogPosts { get; set; }
    }
}