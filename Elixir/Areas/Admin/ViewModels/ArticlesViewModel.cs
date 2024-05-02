using Elixir.Areas.Admin.Models;
using Elixir.Models.Enums;
using Elixir.ViewModels.Base;

namespace Elixir.Areas.Admin.ViewModels
{
    public class ArticlesViewModel : BaseSortableListViewModel<ArticleModel, ArticlesSortField>
    {
        public string CurrentFilter { get; set; }
    }
}