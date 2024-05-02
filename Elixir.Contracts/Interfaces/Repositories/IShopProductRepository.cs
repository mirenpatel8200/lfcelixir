using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IShopProductRepository : IRepository<ShopProduct>
    {
        IEnumerable<ShopProduct> GetShopProducts(int limit, ShopProductsSortOrder sortFields, SortDirection sortDirections);
        IEnumerable<ShopProduct> GetWebPageRelatedShopProducts(int webPageId);
        ShopProduct GetShopProductByUrlName(string urlName);
        ShopProduct GetShopProductBySKU(string sku);
        ShopProduct GetShopProductById(int id);
        bool IsNonDeletedShopProductExists(string urlName, int? excludeShopProductId);
    }
}
