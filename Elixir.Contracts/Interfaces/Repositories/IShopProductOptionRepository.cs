using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IShopProductOptionRepository : IRepository<ShopProductOption>
    {
        void Delete(int shopProductId, int id);
        IEnumerable<ShopProductOption> GetShopProductOptions(int shopProductId);
        IEnumerable<ShopProductOption> GetAll(int shopProductId);
        ShopProductOption GetShopProductOptionByName(int shopProductId, string name);
        ShopProductOption GetShopProductOptionById(int shopProductId, int id);
        bool IsNonDeletedShopProductOptionExists(int shopProductId, string name, int? excludeShopProductOptionId);
    }
}
