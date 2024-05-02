using Elixir.Models.Enums;

namespace Elixir.ViewModels.Base
{
    public abstract class BaseSortableListViewModel<TModel, TSortOrder> : BaseListViewModel<TModel>
    {
        public SortDirection SortDirection { get; set; }
        public TSortOrder SortOrder { get; set; }
        public UsersRecordLimit UsersRecordLimit { get; set; }

        public SortDirection CurrentSortDirection { get; set; }
        public TSortOrder CurrentSortOrder { get; set; }
        public UsersRecordLimit CurrentUsersRecordLimit { get; set; }
    }
}