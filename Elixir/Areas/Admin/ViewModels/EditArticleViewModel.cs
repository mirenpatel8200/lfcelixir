using Elixir.Areas.Admin.Models;
using Elixir.ViewModels.Base;

namespace Elixir.Areas.Admin.ViewModels
{
    public class EditArticleViewModel : BaseCUWithMultipleSelectViewModel<ArticleModel>
    {
        public string SubmitName_Save = "Save";
        public string SubmitName_SaveAndSocial = "Save & Social";
    }
}