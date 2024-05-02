using System;
using System.Collections.Generic;
using Elixir.Areas.AdminManual.Models;
using Elixir.ViewModels.Base;

namespace Elixir.Areas.AdminManual.ViewModels
{
    public class CreateChapterViewModel : BaseCUViewModel<ChapterModel>
    {
        public CreateChapterViewModel()
        {
            
        }

        public CreateChapterViewModel(ChapterModel model, Dictionary<int, string> chapterTypes)
        {
            if(chapterTypes == null || model == null)
                throw new ArgumentNullException(nameof(chapterTypes));

            Model = model;
            Model.ChapterTypes = chapterTypes;
        }
    }
}