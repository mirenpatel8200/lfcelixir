using System;
using System.Collections.Generic;
using Elixir.Areas.AdminManual.Models;
using Elixir.ViewModels.Base;

namespace Elixir.Areas.AdminManual.ViewModels
{
    public class EditChapterViewModel : BaseCUViewModel<ChapterModel>
    {
        public EditChapterViewModel()
        {
            
        }

        public EditChapterViewModel(ChapterModel model, Dictionary<int, string> chapterTypes)
        {
            if (chapterTypes == null || model == null)
                throw new ArgumentNullException();

            Model = model;
            Model.ChapterTypes = chapterTypes;
        }
    }
}