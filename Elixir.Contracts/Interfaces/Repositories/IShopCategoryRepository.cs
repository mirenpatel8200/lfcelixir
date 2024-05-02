using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IShopCategoryRepository : IRepository<ShopCategory>
    {
        IEnumerable<ShopCategory> GetShopCategories(int limit, ShopCategoriesSortOrder sortField, SortDirection sortDirections);
        ShopCategory GetShopCategoryByName(string name);
        ShopCategory GetShopCategoryById(int id);
        bool IsNonDeletedShopCategoryExists(string name, int? excludeShopCategoryId);
    }
}
