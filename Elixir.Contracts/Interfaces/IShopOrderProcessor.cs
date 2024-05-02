using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Contracts.Interfaces
{
    public interface IShopOrderProcessor
    {
        string CreateShopOrder(ShopOrder entity, bool isGuestCart = false);
        void UpdateShopOrder(ShopOrder entity);
        void DeleteShopOrder(int id);
        IEnumerable<ShopOrder> GetShopOrders(int limit, ShopOrdersSortOrder sortField, SortDirection sortDirection);
        ShopOrder GetUserCart(int userId);
        ShopOrder GetGuestCart(string idHashCode);
        ShopOrder GetShopOrderByUser(string idHashCode, int userId);
        IEnumerable<ShopOrder> GetUserShopOrders(int userId);
        ShopOrder GetShopOrderByIdHashCode(string idHashCode);
    }
}

