using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces
{
    public interface IShopProductProcessor
    {
        void CreateShopProduct(ShopProduct entity);
        void UpdateShopProduct(ShopProduct entity);
        void DeleteShopProduct(int id);
        IEnumerable<ShopProduct> GetAll();
        IEnumerable<ShopProduct> GetShopProducts(int limit, ShopProductsSortOrder sortField, SortDirection sortDirection);
        IEnumerable<ShopProduct> GetWebPageRelatedShopProducts(int webPageId);
        ShopProduct GetShopProductById(int id);
        ShopProduct GetShopProductByUrlName(string urlName);
        ShopProduct GetShopProductBySKU(string sku);
    }
}
