using System.Collections.Generic;
using Elixir.Models;

namespace Elixir.Areas.AdminManual.Models
{
    public class SectionWithPagesModel
    {
        public BookSection BookSection { get; set; }
        public int PagesCount { get; set; }
        public IEnumerable<ContentsPageModel> Pages { get; set; }
    }
}