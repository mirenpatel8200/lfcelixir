using Elixir.BusinessLogic.Exceptions;
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
    public class ShopProductProcessor : IShopProductProcessor
    {
        private readonly IShopProductRepository _shopProductRepository;

        public ShopProductProcessor(IShopProductRepository shopProductRepository)
        {
            _shopProductRepository = shopProductRepository;
        }

        public void CreateShopProduct(ShopProduct entity)
        {
            ValidateUrlName(entity.UrlName);
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;
            _shopProductRepository.Insert(entity);

            var addedShopProduct = _shopProductRepository.GetShopProductByUrlName(entity.UrlName);
            if (addedShopProduct == null)
                throw new InvalidOperationException("Unable to find shop product that was added.");
        }

        public void UpdateShopProduct(ShopProduct entity)
        {
            ValidateUrlName(entity.UrlName, entity.ShopProductId);
            entity.UpdatedOn = DateTime.Now;
            _shopProductRepository.Update(entity);
        }

        public void DeleteShopProduct(int id)
        {
            _shopProductRepository.Delete(id);
        }

        public IEnumerable<ShopProduct> GetAll()
        {
            return _shopProductRepository.GetAll();
        }

        public IEnumerable<ShopProduct> GetShopProducts(int limit, ShopProductsSortOrder sortField, SortDirection sortDirection)
        {
            return _shopProductRepository.GetShopProducts(limit, sortField, sortDirection);
        }

        public IEnumerable<ShopProduct> GetWebPageRelatedShopProducts(int webPageId)
        {
            return _shopProductRepository.GetWebPageRelatedShopProducts(webPageId);
        }

        public ShopProduct GetShopProductById(int id)
        {
            return _shopProductRepository.GetShopProductById(id);
        }

        public ShopProduct GetShopProductByUrlName(string urlName)
        {
            return _shopProductRepository.GetShopProductByUrlName(urlName);
        }

        public ShopProduct GetShopProductBySKU(string sku)
        {
            return _shopProductRepository.GetShopProductBySKU(sku);
        }

        private void ValidateUrlName(string urlName, int? excludeShopProductId = null)
        {
            var exists = _shopProductRepository.IsNonDeletedShopProductExists(urlName, excludeShopProductId);
            if (exists)
                throw new ModelValidationException("Shop product with specified UrlName already exists.");
        }
    }
}
