using Elixir.Contracts.Interfaces;
using Elixir.Contracts.Interfaces.Repositories;
using Elixir.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.BusinessLogic.Processors
{
    public class ShopOrderProductProcessor : IShopOrderProductProcessor
    {
        private readonly IShopOrderProductRepository _shopOrderProductRepository;

        public ShopOrderProductProcessor(IShopOrderProductRepository shopOrderProductRepository)
        {
            _shopOrderProductRepository = shopOrderProductRepository;
        }

        public void CreateShopOrderProduct(ShopOrderProduct entity)
        {
            _shopOrderProductRepository.Insert(entity);

            var orderProduct = GetProductByOrder(entity.ShopOrderId, entity.ShopProductId, entity.ShopProductOptionId);
            if (orderProduct == null)
                throw new InvalidOperationException("Unable to find shop order product that was added.");
        }

        public void UpdateShopOrderProduct(ShopOrderProduct entity)
        {
            _shopOrderProductRepository.Update(entity);
        }

        public void DeleteShopOrderProduct(int id)
        {
            _shopOrderProductRepository.Delete(id);
        }

        public IEnumerable<ShopOrderProduct> GetShopOrderProducts(int orderId)
        {
            return _shopOrderProductRepository.GetShopOrderProducts(orderId);
        }

        public ShopOrderProduct GetProductByOrder(int orderId, int productId, int? optionId)
        {
            return _shopOrderProductRepository.GetProductByOrder(orderId, productId, optionId);
        }
    }
}
