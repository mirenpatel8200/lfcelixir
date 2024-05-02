using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using Elixir.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.BusinessLogic.Processors
{
    public class ShopOrderProcessor : IShopOrderProcessor
    {
        private readonly IShopOrderRepository _shopOrderRepository;

        public ShopOrderProcessor(IShopOrderRepository shopOrderRepository)
        {
            _shopOrderRepository = shopOrderRepository;
        }

        public string CreateShopOrder(ShopOrder entity, bool isGuestCart = false)
        {
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;

            ShopOrder cart;
            if (isGuestCart)
            {
                cart = GetGuestCart(entity.IDHashCode);
                if (cart != null)
                    throw new InvalidOperationException("Guest has already cart.");
            }
            else
            {
                cart = GetUserCart((int)entity.UserId);
                if (cart != null)
                    throw new InvalidOperationException("User has already cart.");
            }

            _shopOrderRepository.Insert(entity);

            if (isGuestCart)
            {
                cart = GetGuestCart(entity.IDHashCode);
                if (cart == null)
                    throw new InvalidOperationException("Unable to find shop order that was added.");
            }
            else
            {
                cart = GetUserCart((int)entity.UserId);
                if (cart == null)
                    throw new InvalidOperationException("Unable to find shop order that was added.");
            }

            cart.IDHashCode = cart.CalculateIdHashCode();
            _shopOrderRepository.UpdateShopOrder(cart, true);
            return cart.IDHashCode;
        }

        public void UpdateShopOrder(ShopOrder entity)
        {
            entity.UpdatedOn = DateTime.Now;
            _shopOrderRepository.UpdateShopOrder(entity);
        }

        public void DeleteShopOrder(int id)
        {
            _shopOrderRepository.Delete(id);
        }

        public IEnumerable<ShopOrder> GetShopOrders(int limit, ShopOrdersSortOrder sortField, SortDirection sortDirection)
        {
            return _shopOrderRepository.GetShopOrders(limit, sortField, sortDirection);
        }

        public ShopOrder GetUserCart(int userId)
        {
            return _shopOrderRepository.GetUserCart(userId);
        }

        public ShopOrder GetGuestCart(string idHashCode)
        {
            return _shopOrderRepository.GetGuestCart(idHashCode);
        }

        public ShopOrder GetShopOrderByUser(string idHashCode, int userId)
        {
            return _shopOrderRepository.GetShopOrderByUser(idHashCode, userId);
        }

        public IEnumerable<ShopOrder> GetUserShopOrders(int userId)
        {
            return _shopOrderRepository.GetUserShopOrders(userId);
        }

        public ShopOrder GetShopOrderByIdHashCode(string idHashCode)
        {
            return _shopOrderRepository.GetShopOrderByIdHashCode(idHashCode);
        }
    }
}
