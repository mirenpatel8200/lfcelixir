using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IShopOrderProductRepository : IRepository<ShopOrderProduct>
    {
        IEnumerable<ShopOrderProduct> GetShopOrderProducts(int orderId);
        ShopOrderProduct GetProductByOrder(int orderId, int productId, int? optionId);
    }
}
