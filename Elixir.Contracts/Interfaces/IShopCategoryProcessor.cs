using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces
{
    public interface IShopCategoryProcessor
    {
        void CreateShopCategory(ShopCategory entity);
        void UpdateShopCategory(ShopCategory entity);
        void DeleteShopCategory(int id);
        IEnumerable<ShopCategory> GetAll();
        IEnumerable<ShopCategory> GetShopCategories(int limit, ShopCategoriesSortOrder sortField, SortDirection sortDirections);
        ShopCategory GetShopCategoryByName(string name);
        ShopCategory GetShopCategoryById(int id);
    }
}
