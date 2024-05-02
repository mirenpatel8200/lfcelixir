using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces
{
    public interface IShopOrderProductProcessor
    {
        void CreateShopOrderProduct(ShopOrderProduct entity);
        void UpdateShopOrderProduct(ShopOrderProduct entity);
        void DeleteShopOrderProduct(int id);
        IEnumerable<ShopOrderProduct> GetShopOrderProducts(int orderId);
        ShopOrderProduct GetProductByOrder(int orderId, int productId, int? optionId);
    }
}
