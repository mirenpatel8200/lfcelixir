using Elixir.Areas.Admin.Models;
using Elixir.Models.Enums;
using Elixir.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elixir.Areas.Admin.ViewModels
{
    public class ShopCategoriesViewModel : BaseSortableListViewModel<ShopCategoryModel, ShopCategoriesSortOrder>
    {
    }
}