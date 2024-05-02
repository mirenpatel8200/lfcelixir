using Elixir.BusinessLogic.Exceptions;
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
    public class ShopProductOptionProcessor : IShopProductOptionProcessor
    {
        private readonly IShopProductOptionRepository _shopProductOptionRepository;

        public ShopProductOptionProcessor(IShopProductOptionRepository shopProductOptionRepository)
        {
            _shopProductOptionRepository = shopProductOptionRepository;
        }

        public void CreateShopProductOption(ShopProductOption entity)
        {
            ValidateName(entity.ShopProductId, entity.OptionName);
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;
            _shopProductOptionRepository.Insert(entity);

            var addedShopProductOption = _shopProductOptionRepository.GetShopProductOptionByName(entity.ShopProductId, entity.OptionName);
            if (addedShopProductOption == null)
                throw new InvalidOperationException("Unable to find shop product option that was added.");
        }

        public void UpdateShopProductOption(ShopProductOption entity)
        {
            ValidateName(entity.ShopProductId, entity.OptionName, entity.ShopProductOptionId);
            entity.UpdatedOn = DateTime.Now;
            _shopProductOptionRepository.Update(entity);
        }

        public void DeleteShopProductOption(int shopProductId, int id)
        {
            _shopProductOptionRepository.Delete(shopProductId, id);
        }

        public IEnumerable<ShopProductOption> GetShopProductOptions(int shopProductId)
        {
            return _shopProductOptionRepository.GetShopProductOptions(shopProductId);
        }

        public IEnumerable<ShopProductOption> GetAll(int shopProductId)
        {
            return _shopProductOptionRepository.GetAll(shopProductId);
        }

        public ShopProductOption GetShopProductOptionByName(int shopProductId, string name)
        {
            return _shopProductOptionRepository.GetShopProductOptionByName(shopProductId, name);
        }

        public ShopProductOption GetShopProductOptionById(int shopProductId, int id)
        {
            return _shopProductOptionRepository.GetShopProductOptionById(shopProductId, id);
        }

        private void ValidateName(int shopProductId, string name, int? excludeShopProductOptionId = null)
        {
            var exists = _shopProductOptionRepository.IsNonDeletedShopProductOptionExists(shopProductId, name, excludeShopProductOptionId);
            if (exists)
                throw new ModelValidationException("Shop product option with specified Name already exists.");
        }
    }
}
