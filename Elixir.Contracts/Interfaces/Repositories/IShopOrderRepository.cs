using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces.Repositories
{
    public interface IShopOrderRepository : IRepository<ShopOrder>
    {
        void UpdateShopOrder(ShopOrder entity, bool isNewShopOrder = false);
        IEnumerable<ShopOrder> GetShopOrders(int limit, ShopOrdersSortOrder sortField, SortDirection sortDirections);
        ShopOrder GetUserCart(int userId);
        ShopOrder GetGuestCart(string idHashCode, int userId = 0);
        ShopOrder GetShopOrderByUser(string idHashCode, int userId);
        IEnumerable<ShopOrder> GetUserShopOrders(int userId);
        ShopOrder GetShopOrderByIdHashCode(string idHashCode);
    }
}
