using System.Collections.Generic;
using Elixir.Areas.AdminManual.Models;

namespace Elixir.Areas.AdminManual.ViewModels.ContentsPage
{
    public class ContentsViewModel
    {
        public ContentsViewModel()
        {
            
        }

        public int TotalPages { get; set; }
        public int TotalSections { get; set; }
        public int DraftedPages { get; set; }
        public int TotalLe40 { get; set; }
        public int CompletePagesCount { get; set; }

        public IEnumerable<SectionWithPagesModel> Sections { get; set; }
    }
}