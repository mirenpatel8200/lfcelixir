using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces
{
    public interface IShopProductOptionProcessor
    {
        void CreateShopProductOption(ShopProductOption entity);
        void UpdateShopProductOption(ShopProductOption entity);
        void DeleteShopProductOption(int shopProductId, int id);
        IEnumerable<ShopProductOption> GetShopProductOptions(int shopProductId);
        IEnumerable<ShopProductOption> GetAll(int shopProductId);
        ShopProductOption GetShopProductOptionByName(int shopProductId, string name);
        ShopProductOption GetShopProductOptionById(int shopProductId, int id);
    }
}
