using Elixir.Areas.Admin.Models;
using Elixir.Models.Enums;
using Elixir.ViewModels.Base;

namespace Elixir.Areas.Admin.ViewModels
{
    public class TopicsViewModel : BaseSortableListViewModel<TopicModel, TopicSortOrder>
    {
    }
}