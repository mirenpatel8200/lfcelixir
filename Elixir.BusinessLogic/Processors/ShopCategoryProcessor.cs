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
    public class ShopCategoryProcessor : IShopCategoryProcessor
    {
        private readonly IShopCategoryRepository _shopCategoryRepository;

        public ShopCategoryProcessor(IShopCategoryRepository shopCategoryRepository)
        {
            _shopCategoryRepository = shopCategoryRepository;
        }

        public void CreateShopCategory(ShopCategory entity)
        {
            ValidateName(entity.ShopCategoryName);
            entity.CreatedOn = DateTime.Now;
            entity.UpdatedOn = DateTime.Now;
            _shopCategoryRepository.Insert(entity);

            var addedShopCategory = _shopCategoryRepository.GetShopCategoryByName(entity.ShopCategoryName);
            if (addedShopCategory == null)
                throw new InvalidOperationException("Unable to find shop category that was added.");
        }

        public void UpdateShopCategory(ShopCategory entity)
        {
            ValidateName(entity.ShopCategoryName, entity.ShopCategoryId);
            entity.UpdatedOn = DateTime.Now;
            _shopCategoryRepository.Update(entity);
        }

        public void DeleteShopCategory(int id)
        {
            _shopCategoryRepository.Delete(id);
        }

        public IEnumerable<ShopCategory> GetAll()
        {
            return _shopCategoryRepository.GetAll();
        }

        public IEnumerable<ShopCategory> GetShopCategories(int limit, ShopCategoriesSortOrder sortField, SortDirection sortDirection)
        {
            return _shopCategoryRepository.GetShopCategories(limit, sortField, sortDirection);
        }

        public ShopCategory GetShopCategoryByName(string name)
        {
            return _shopCategoryRepository.GetShopCategoryByName(name);
        }

        public ShopCategory GetShopCategoryById(int id)
        {
            return _shopCategoryRepository.GetShopCategoryById(id);
        }

        private void ValidateName(string name, int? excludeShopCategoryId = null)
        {
            var exists = _shopCategoryRepository.IsNonDeletedShopCategoryExists(name, excludeShopCategoryId);
            if (exists)
                throw new ModelValidationException("Shop category with specified Name already exists.");
        }
    }
}
