using Elixir.Models;

namespace Elixir.Areas.AdminManual.Models
{
    public class ContentsPageModel
    {
        public BookPage BookPage { get; set; }
        public int PageNumber { get; set; }
    }
}