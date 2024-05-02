using Elixir.Areas.Admin.Models;
using Elixir.Models.Enums;
using Elixir.ViewModels.Base;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Elixir.Areas.Admin.ViewModels
{
    public class UsersViewModel : BaseSortableListViewModel<BookUserModel, UsersSortOrder>
    {
        public IEnumerable<SelectListItem> SelectItems { get; set; }
    }
}