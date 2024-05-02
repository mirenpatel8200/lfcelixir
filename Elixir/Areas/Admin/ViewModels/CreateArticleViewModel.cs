using Elixir.Areas.Admin.Models;
using Elixir.ViewModels.Base;

namespace Elixir.Areas.Admin.ViewModels
{
    public class CreateArticleViewModel : BaseCUWithMultipleSelectViewModel<ArticleModel>
    {
        public string SubmitName_Create = "Create";
        public string SubmitName_CreateAndSocial = "Create & Social";
    }
}