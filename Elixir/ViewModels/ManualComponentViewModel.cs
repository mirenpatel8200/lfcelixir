using Elixir.ViewModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.ViewModels
{
    public class ManualComponentViewModel
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? PageNumber { get; set; }

        public int? TipNumber { get; set; }

        public string ImageSrc { get; set; }

        public ManualComponentType Type { get; set; }

        public string SectionName { get; set; }

        public int? Cost { get; set; }

        public int? Ease { get; set; }

        public int Impact { get; set; }

        public string Tips { get; set; }

        public string Resources { get; set; }

        public string TipImage { get; set; }

        public string Research { get; set; }

        public bool MemberMode { get; set; }

        public bool ShowChapter { get; set; }

        public bool IsChapterFirstPage { get; set; }

        public bool IsLongevist { get; set; }
        public bool IsSupporter { get; set; }
        public bool IsAuthenticated { get; set; }


        public ManualComponentViewModel()
        {
        }
    }
}