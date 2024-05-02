using Elixir.Areas.Admin.Models;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Attributes;
using Elixir.BusinessLogic.Exceptions;
using Elixir.Contracts.Interfaces;
using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class ShopController : BaseController
    {
        private readonly IShopProductProcessor _shopProductProcessor;
        private readonly IShopCategoryProcessor _shopCategoryProcessor;
        private readonly IWebPagesProcessor _webPagesProcessor;
        private readonly IShopProductOptionProcessor _shopProductOptionProcessor;
        private readonly IShopOrderProcessor _shopOrderProcessor;
        private readonly IShopOrderProductProcessor _shopOrderProductProcessor;
        private readonly IPaymentProcessor _paymentProcessor;

        public ShopController(
            IShopProductProcessor shopProductProcessor,
            IShopCategoryProcessor shopCategoryProcessor,
            IWebPagesProcessor webPagesProcessor,
            IShopProductOptionProcessor shopProductOptionProcessor,
            IShopOrderProcessor shopOrderProcessor,
            IShopOrderProductProcessor shopOrderProductProcessor,
            IPaymentProcessor paymentProcessor)
        {
            _shopProductProcessor = shopProductProcessor;
            _shopCategoryProcessor = shopCategoryProcessor;
            _webPagesProcessor = webPagesProcessor;
            _shopProductOptionProcessor = shopProductOptionProcessor;
            _shopOrderProcessor = shopOrderProcessor;
            _shopOrderProductProcessor = shopOrderProductProcessor;
            _paymentProcessor = paymentProcessor;
        }

        // GET: admin/shop
        public ActionResult Index()
        {
            return View();
        }

        // GET: admin/shop/product
        public ActionResult Product(ShopProductsSortOrder sortBy = ShopProductsSortOrder.ShopProductID, SortDirection direction = SortDirection.Descending)
        {
            if (!Enum.IsDefined(typeof(ShopProductsSortOrder), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");

            var shopProducts = _shopProductProcessor.GetShopProducts(10, sortBy, direction).Select(p => new ShopProductModel(p));
            var viewModel = new ShopProductsViewModel();
            return SortableListView(shopProducts, sortBy, direction, viewModel);
        }

        // GET: admin/shop/product/create
        [HttpGet]
        public ActionResult CreateProduct()
        {
            var viewModel = new CreateShopProductViewModel
            {
                Model = new ShopProductModel()
            };
            viewModel.AddSelectList(nameof(viewModel.Model.ShopCategory), GetShopCategorySelectItems());
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateProduct(CreateShopProductViewModel viewModel)
        {
            try
            {
                viewModel.Model = UpdateShopCategoryObject(viewModel.Model);
                if (ModelState.IsValid)
                {
                    viewModel = ProductTrimmingData(viewModel);
                    SetCreatedByForProduct(viewModel.Model);
                    SetUpdatedByForProduct(viewModel.Model);

                    var duplicateShopProduct = _shopProductProcessor.GetShopProductByUrlName(viewModel.Model.UrlName);
                    if (duplicateShopProduct != null)
                    {
                        throw new ModelValidationException("Shop product with specified Url Name already exists.");
                    }
                    var duplicateShopProductBySKU = _shopProductProcessor.GetShopProductBySKU(viewModel.Model.SKU);
                    if (duplicateShopProductBySKU != null)
                    {
                        throw new ModelValidationException("Shop product with specified SKU already exists.");
                    }
                    _shopProductProcessor.CreateShopProduct(viewModel.Model);
                    return RedirectToAction("Product");
                }
            }
            catch (ModelValidationException mve)
            {
                ModelState.AddModelError("", mve.Message);
            }
            if (viewModel.IsSelectsListEmpty)
            {
                IEnumerable<SelectListItem> shopCategoriesItems;
                if (viewModel.Model.ShopCategory != null && viewModel.Model.ShopCategory.ShopCategoryId > 0)
                    shopCategoriesItems = GetShopCategorySelectItems(viewModel.Model.ShopCategory.ShopCategoryId);
                else
                    shopCategoriesItems = GetShopCategorySelectItems();
                viewModel.AddSelectList(nameof(viewModel.Model.ShopCategory), shopCategoriesItems);
            }
            return View(viewModel);
        }

        // GET: admin/shop/product/edit/{id}
        [HttpGet]
        public ActionResult EditProduct(int? id)
        {
            if (id == null)
                throw new ContentNotFoundException("Incorrect id of Shop product.");

            var viewModel = new EditShopProductViewModel();

            var shopProduct = _shopProductProcessor.GetShopProductById(id.Value);
            if (shopProduct == null)
                throw new ContentNotFoundException("The Shop product with specified id does not exist.");

            viewModel.Model = new ShopProductModel(shopProduct);
            IEnumerable<SelectListItem> shopCategoriesItems;
            if (viewModel.Model.ShopCategory != null && viewModel.Model.ShopCategory.ShopCategoryId > 0)
                shopCategoriesItems = GetShopCategorySelectItems(viewModel.Model.ShopCategory.ShopCategoryId);
            else
                shopCategoriesItems = GetShopCategorySelectItems();
            viewModel.AddSelectList(nameof(viewModel.Model.ShopCategory), shopCategoriesItems);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProduct(EditShopProductViewModel viewModel)
        {
            try
            {
                viewModel.Model = UpdateShopCategoryObject(viewModel.Model);
                if (ModelState.IsValid)
                {
                    viewModel = ProductTrimmingData(viewModel);
                    SetUpdatedByForProduct(viewModel.Model);

                    var duplicateShopProduct = _shopProductProcessor.GetShopProductByUrlName(viewModel.Model.UrlName);
                    if (duplicateShopProduct != null && viewModel.Model.ShopProductId == duplicateShopProduct.ShopProductId)
                    {
                        var duplicateShopProductBySKU = _shopProductProcessor.GetShopProductBySKU(viewModel.Model.SKU);
                        if (duplicateShopProductBySKU != null && viewModel.Model.ShopProductId != duplicateShopProductBySKU.ShopProductId)
                            throw new ModelValidationException("Shop product with specified SKU already exists.");
                        _shopProductProcessor.UpdateShopProduct(viewModel.Model);
                        return RedirectToAction("Product");
                    }
                    else
                        throw new ModelValidationException("Shop product with specified Url Name already exists.");
                }
            }
            catch (ModelValidationException mve)
            {
                ModelState.AddModelError("", mve.Message);
            }
            if (viewModel.IsSelectsListEmpty)
            {
                IEnumerable<SelectListItem> shopCategoriesItems;
                if (viewModel.Model.ShopCategory != null && viewModel.Model.ShopCategory.ShopCategoryId > 0)
                    shopCategoriesItems = GetShopCategorySelectItems(viewModel.Model.ShopCategory.ShopCategoryId);
                else
                    shopCategoriesItems = GetShopCategorySelectItems();
                viewModel.AddSelectList(nameof(viewModel.Model.ShopCategory), shopCategoriesItems);
            }
            return View(viewModel);
        }

        public ActionResult DeleteProduct(int id)
        {
            if (id <= 0)
                throw new ContentNotFoundException("Incorrect id of Shop product");

            _shopProductProcessor.DeleteShopProduct(id);

            return RedirectToAction("Product");
        }

        // GET: admin/shop/category
        public ActionResult Category(ShopCategoriesSortOrder sortBy = ShopCategoriesSortOrder.UpdatedDT, SortDirection direction = SortDirection.Descending)
        {
            if (!Enum.IsDefined(typeof(ShopCategoriesSortOrder), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");

            var shopCategories = _shopCategoryProcessor.GetShopCategories(100, sortBy, direction).Select(sc => new ShopCategoryModel(sc));
            var viewModel = new ShopCategoriesViewModel();
            return SortableListView(shopCategories, sortBy, direction, viewModel);
        }

        // GET: admin/shop/category/create
        [HttpGet]
        public ActionResult CreateCategory()
        {
            var viewModel = new CreateShopCategoryViewModel
            {
                Model = new ShopCategoryModel()
            };
            viewModel.AddSelectList(nameof(viewModel.Model.PrimaryWebPage), GetWebPageSelectItems());
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateCategory(CreateShopCategoryViewModel viewModel)
        {
            try
            {
                viewModel.Model = UpdateWebPageObject(viewModel.Model);
                if (ModelState.IsValid)
                {
                    viewModel = CategoryTrimmingData(viewModel);
                    SetCreatedByForCategory(viewModel.Model);
                    SetUpdatedByForCategory(viewModel.Model);

                    var duplicateShopCategory = _shopCategoryProcessor.GetShopCategoryByName(viewModel.Model.ShopCategoryName);
                    if (duplicateShopCategory != null)
                    {
                        throw new ModelValidationException("Shop category with specified Name already exists.");
                    }
                    var webPage = _webPagesProcessor.GetWebPageById(viewModel.Model.PrimaryWebPageId.Value);
                    if (webPage == null)
                    {
                        throw new ModelValidationException("Selected web page is not found.");
                    }
                    viewModel.Model.DnPrimaryWebPageName = webPage.WebPageName;
                    viewModel.Model.DnPrimaryWebPageUrlName = webPage.UrlName;
                    _shopCategoryProcessor.CreateShopCategory(viewModel.Model);
                    return RedirectToAction("Category");
                }
            }
            catch (ModelValidationException mve)
            {
                ModelState.AddModelError("", mve.Message);
            }

            if (viewModel.IsSelectsListEmpty)
            {
                IEnumerable<SelectListItem> webPageItems;
                if (viewModel.Model.PrimaryWebPage?.Id != null)
                    webPageItems = GetWebPageSelectItems(viewModel.Model.PrimaryWebPage.Id.Value);
                else
                    webPageItems = GetWebPageSelectItems();
                viewModel.AddSelectList(nameof(viewModel.Model.PrimaryWebPage), webPageItems);
            }

            return View(viewModel);
        }

        // GET: admin/shop/category/edit/{id}
        [HttpGet]
        public ActionResult EditCategory(int? id)
        {
            if (id == null)
                throw new ContentNotFoundException("Incorrect id of Shop category.");

            var viewModel = new EditShopCategoryViewModel();

            var shopCategory = _shopCategoryProcessor.GetShopCategoryById(id.Value);
            if (shopCategory == null)
                throw new ContentNotFoundException("The Shop category with specified id does not exist.");

            viewModel.Model = new ShopCategoryModel(shopCategory);
            IEnumerable<SelectListItem> webPageItems;
            if (viewModel.Model.PrimaryWebPage?.Id != null)
                webPageItems = GetWebPageSelectItems(viewModel.Model.PrimaryWebPage.Id.Value);
            else
                webPageItems = GetWebPageSelectItems();
            viewModel.AddSelectList(nameof(viewModel.Model.PrimaryWebPage), webPageItems);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditCategory(EditShopCategoryViewModel viewModel)
        {
            try
            {
                viewModel.Model = UpdateWebPageObject(viewModel.Model);
                if (ModelState.IsValid)
                {
                    viewModel = CategoryTrimmingData(viewModel);
                    SetUpdatedByForCategory(viewModel.Model);

                    var duplicateShopCategory = _shopCategoryProcessor.GetShopCategoryByName(viewModel.Model.ShopCategoryName);
                    if (duplicateShopCategory != null && viewModel.Model.ShopCategoryId != duplicateShopCategory.ShopCategoryId)
                    {
                        throw new ModelValidationException("Shop category with specified Name already exists.");
                    }
                    var webPage = _webPagesProcessor.GetWebPageById(viewModel.Model.PrimaryWebPageId.Value);
                    if (webPage == null)
                    {
                        throw new ModelValidationException("Selected web page is not found.");
                    }
                    viewModel.Model.DnPrimaryWebPageName = webPage.WebPageName;
                    viewModel.Model.DnPrimaryWebPageUrlName = webPage.UrlName;
                    _shopCategoryProcessor.UpdateShopCategory(viewModel.Model);
                    return RedirectToAction("Category");
                }
            }
            catch (ModelValidationException mve)
            {
                ModelState.AddModelError("", mve.Message);
            }
            if (viewModel.IsSelectsListEmpty)
            {
                IEnumerable<SelectListItem> webPageItems;
                if (viewModel.Model.PrimaryWebPage?.Id != null)
                    webPageItems = GetWebPageSelectItems(viewModel.Model.PrimaryWebPage.Id.Value);
                else
                    webPageItems = GetWebPageSelectItems();
                viewModel.AddSelectList(nameof(viewModel.Model.PrimaryWebPage), webPageItems);
            }
            return View(viewModel);
        }

        public ActionResult DeleteCategory(int id)
        {
            if (id <= 0)
                throw new ContentNotFoundException("Incorrect id of Shop category");

            _shopCategoryProcessor.DeleteShopCategory(id);

            return RedirectToAction("Category");
        }

        // GET: admin/shop/orders
        public ActionResult Orders(ShopOrdersSortOrder sortBy = ShopOrdersSortOrder.IDHashCode, SortDirection direction = SortDirection.Descending)
        {
            if (!Enum.IsDefined(typeof(ShopOrdersSortOrder), sortBy))
                throw new ContentNotFoundException("Incorrect sorting parameter.");
            if (!Enum.IsDefined(typeof(SortDirection), direction))
                throw new ContentNotFoundException("Incorrect sort direction parameter.");

            var shopOrders = _shopOrderProcessor.GetShopOrders(100, sortBy, direction).Select(so => new ShopOrderModel(so));
            var viewModel = new ShopOrdersViewModel();
            return SortableListView(shopOrders, sortBy, direction, viewModel);
        }

        // GET: admin/shop/order/{name}
        public ActionResult Order(string name,bool modelStateInvalid=false)
        {
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ContentNotFoundException("Shop order's name has to be specified.");

            var shopOrder = _shopOrderProcessor.GetShopOrderByIdHashCode(name);
            if (shopOrder == null)
                throw new ContentNotFoundException("Shop order is not found.");

           
            var viewModel = new ShopOrderDetailsViewModel
            {
                ShopOrder = shopOrder
            };
            if (modelStateInvalid)
            {
                if (TempData["NotesPublic"] != null)
                {
                    viewModel.NotesPublic = TempData["NotesPublic"].ToString();
                    if (viewModel.NotesPublic.Length>100)
                    {
                        ModelState.AddModelError("NotesPublic", "Notes Public: Length should be less than 100.");
                    }
                }
                if (TempData["NotesInternal"] != null)
                {
                    viewModel.NotesInternal = TempData["NotesInternal"].ToString();
                   if (viewModel.NotesInternal.Length > 100)
                    {
                        ModelState.AddModelError("NotesInternal", "Notes Internal: Length should be less than 100.");
                    }
                }
            }
            else
            {
                viewModel.NotesPublic = shopOrder.NotesPublic;
                viewModel.NotesInternal = shopOrder.NotesInternal;
            }
            viewModel.ShopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(shopOrder.ShopOrderId).ToList();

            if (viewModel.ShopOrderProducts == null || viewModel.ShopOrderProducts.Count <= 0)
                throw new ContentNotFoundException("Shop order products are not found.");

            viewModel.TotalAmount = 0;
            foreach (var orderProduct in viewModel.ShopOrderProducts)
            {
                viewModel.TotalAmount += (int)orderProduct.Quantity * (decimal)orderProduct.PricePaidPerUnit;
            }
            viewModel.Payments = _paymentProcessor.GetPaymentsByShopOrder(shopOrder.ShopOrderId).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Order(ShopOrderDetailsViewModel model)
        {

            if (string.IsNullOrEmpty(model.NotesInternal))
            {
                model.NotesInternal = "";
            }
            if (string.IsNullOrEmpty(model.NotesPublic))
            {
                model.NotesPublic = "";
            }
            if ( model.NotesInternal!=null && model.NotesPublic != null && model.NotesInternal.Length<=100 && model.NotesPublic.Length <= 100)
            {
                var shopOrder = _shopOrderProcessor.GetShopOrderByIdHashCode(model.IDHashCode);
                if (shopOrder == null)
                    throw new ContentNotFoundException("Shop order is not found.");
                
                shopOrder.NotesInternal = model.NotesInternal;
                shopOrder.NotesPublic = model.NotesPublic;
                shopOrder.UpdatedOn = DateTime.Now;
                var accessUserManager = Request.GetOwinContext().Get<AccessUserManager>();
                var authedUserId = int.Parse(User.Identity.GetUserId());
                shopOrder.UpdatedByUserId= authedUserId;
                _shopOrderProcessor.UpdateShopOrder(shopOrder);
                return Redirect("/admin/shop/orders");
            }
            else
            { 
                TempData["NotesPublic"] = model.NotesPublic;
                TempData["NotesInternal"] = model.NotesInternal;
                return Redirect("/admin/shop/order?name=" + model.IDHashCode+ "&modelStateInvalid=true");
            }
        }

        // GET: admin/shop/product/options/{shopProductId}
        public ActionResult Options(int shopProductId)
        {
            var shopProduct = _shopProductProcessor.GetShopProductById(shopProductId);
            if (shopProduct == null)
                throw new ContentNotFoundException("The Shop product with specified id does not exist.");

            var viewModel = new CreateEditShopProductOptionViewModel
            {
                ShopProductId = shopProduct.ShopProductId,
                ShopProductName = shopProduct.ShopProductName,
                OptionsUnit = shopProduct.OptionsUnit,
                ShopProductOptionModels = new List<ShopProductOptionModel>()
            };

            var maxDisplayOrder = 0;
            var options = _shopProductOptionProcessor.GetShopProductOptions(shopProductId).ToList();
            if (options != null && options.Count > 0)
            {
                foreach (var option in options)
                {
                    viewModel.ShopProductOptionModels.Add(new ShopProductOptionModel(option));
                }
                maxDisplayOrder = (int)viewModel.ShopProductOptionModels.Max(x => x.DisplayOrder == null ? 0 : x.DisplayOrder);
            }

            // default shop product option
            viewModel.ShopProductOptionModels.Add(new ShopProductOptionModel()
            {
                ShopProductId = shopProduct.ShopProductId,
                DisplayOrder = maxDisplayOrder + 1
            });
            viewModel.ShopProductOptionModels = viewModel.ShopProductOptionModels.OrderBy(x => x.DisplayOrder).ToList();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Options(ShopProductOptionModel model)
        {
            TempData.Clear();
            if (ModelState.IsValid)
            {
                if (model.ShopProductOptionId > 0)
                {
                    // Edit shop product option
                    model = ShopProductOptionTrimmingData(model);
                    SetUpdatedByForOption(model);

                    var duplicateShopProductOption = _shopProductOptionProcessor.GetShopProductOptionByName(model.ShopProductId, model.OptionName);
                    if (duplicateShopProductOption != null && model.ShopProductOptionId != duplicateShopProductOption.ShopProductOptionId)
                    {
                        TempData[$"ErrDuplicate_{model.ShopProductOptionId}"] = $"Shop product option with {model.OptionName} Name already exists.";
                        return RedirectToAction("Options", new { shopProductId = model.ShopProductId });
                    }
                    _shopProductOptionProcessor.UpdateShopProductOption(model);
                }
                else
                {
                    // Create shop product option
                    model = ShopProductOptionTrimmingData(model);
                    SetCreatedByForOption(model);
                    SetUpdatedByForOption(model);

                    var duplicateShopProductOption = _shopProductOptionProcessor.GetShopProductOptionByName(model.ShopProductId, model.OptionName);
                    if (duplicateShopProductOption != null)
                    {
                        TempData[$"ErrDuplicate_{model.ShopProductOptionId}"] = $"Shop product option with {model.OptionName} Name already exists.";
                        return RedirectToAction("Options", new { shopProductId = model.ShopProductId });
                    }
                    _shopProductOptionProcessor.CreateShopProductOption(model);
                }
            }
            return RedirectToAction("Options", new { shopProductId = model.ShopProductId });
        }

        public ActionResult DeleteOption(int shopProductId, int id)
        {
            if (shopProductId <= 0)
                throw new ContentNotFoundException("Incorrect id of Shop product");

            if (id <= 0)
                throw new ContentNotFoundException("Incorrect id of Shop product option");

            var shopProduct = _shopProductProcessor.GetShopProductById(shopProductId);
            if (shopProduct == null)
                throw new ContentNotFoundException("The Shop product with specified id does not exist.");

            _shopProductOptionProcessor.DeleteShopProductOption(shopProductId, id);
            return RedirectToAction("Options", new { shopProductId = shopProduct.ShopProductId });
        }

        private dynamic ProductTrimmingData(dynamic viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Model.ShopProductName))
                viewModel.Model.ShopProductName = viewModel.Model.ShopProductName.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.UrlName))
                viewModel.Model.UrlName = viewModel.Model.UrlName.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.SKU))
                viewModel.Model.SKU = viewModel.Model.SKU.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.ShopProductDescription))
                viewModel.Model.ShopProductDescription = viewModel.Model.ShopProductDescription.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.ImageMain))
                viewModel.Model.ImageMain = viewModel.Model.ImageMain.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.ImageThumb))
                viewModel.Model.ImageThumb = viewModel.Model.ImageThumb.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.NotesInternal))
                viewModel.Model.NotesInternal = viewModel.Model.NotesInternal.Trim();
            return viewModel;
        }

        private dynamic CategoryTrimmingData(dynamic viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Model.ShopCategoryName))
                viewModel.Model.ShopCategoryName = viewModel.Model.ShopCategoryName.Trim();
            if (!string.IsNullOrEmpty(viewModel.Model.NotesInternal))
                viewModel.Model.NotesInternal = viewModel.Model.NotesInternal.Trim();
            return viewModel;
        }

        private dynamic ShopProductOptionTrimmingData(dynamic model)
        {
            if (!string.IsNullOrEmpty(model.OptionName))
                model.OptionName = model.OptionName.Trim();
            if (!string.IsNullOrEmpty(model.SkuSuffix))
                model.SkuSuffix = model.SkuSuffix.Trim();
            return model;
        }

        private IEnumerable<SelectListItem> GetWebPageSelectItems(int selectedId = 0)
        {
            var webPages = _webPagesProcessor.GetAllShopWebPages().OrderBy(x => x.WebPageTitle);
            var result = new List<SelectListItem>();

            foreach (var webPage in webPages)
            {
                var listItem = new SelectListItem
                {
                    Text = webPage.WebPageTitle,
                    Value = webPage.Id.Value.ToString()
                };
                result.Add(listItem);
            }

            var itemToSelect = result.FirstOrDefault(x => x.Value.Equals(selectedId.ToString()));

            if (itemToSelect != null)
                itemToSelect.Selected = true;

            return result;
        }

        private ShopCategoryModel UpdateWebPageObject(ShopCategoryModel category)
        {
            category.PrimaryWebPage = new WebPage()
            {
                Id = category.PrimaryWebPageId
            };
            return category;
        }

        private IEnumerable<SelectListItem> GetShopCategorySelectItems(int selectedId = 0)
        {
            var categories = _shopCategoryProcessor.GetAll().OrderBy(x => x.ShopCategoryName);
            var result = new List<SelectListItem>();

            foreach (var category in categories)
            {
                var listItem = new SelectListItem
                {
                    Text = category.ShopCategoryName,
                    Value = category.ShopCategoryId.ToString()
                };
                result.Add(listItem);
            }

            var itemToSelect = result.FirstOrDefault(x => x.Value.Equals(selectedId.ToString()));

            if (itemToSelect != null)
                itemToSelect.Selected = true;

            return result;
        }

        private ShopProductModel UpdateShopCategoryObject(ShopProductModel product)
        {
            product.ShopCategory = new ShopCategory()
            {
                ShopCategoryId = product.ShopCategoryId.HasValue ? product.ShopCategoryId.Value : 0
            };
            return product;
        }

        private List<string> GetErrorsFromModelState()
        {
            var errors = new List<string>();

            foreach (var modelState in ModelState.Values)
                foreach (var err in modelState.Errors)
                    errors.Add(err.ErrorMessage);

            return errors;
        }

        #region Created By / Updated By Fields
        private void SetCreatedByForProduct(ShopProductModel model)
        {
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.CreatedBy = authedUserId;
        }
        private void SetUpdatedByForProduct(ShopProductModel model)
        {
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.UpdatedBy = authedUserId;
        }
        private void SetCreatedByForCategory(ShopCategoryModel model)
        {
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.CreatedByUserId = authedUserId;
        }
        private void SetUpdatedByForCategory(ShopCategoryModel model)
        {
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.UpdatedByUserId = authedUserId;
        }
        private void SetCreatedByForOption(ShopProductOptionModel model)
        {
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.CreatedByUserId = authedUserId;
        }
        private void SetUpdatedByForOption(ShopProductOptionModel model)
        {
            var authedUserId = int.Parse(User.Identity.GetUserId());
            model.UpdatedByUserId = authedUserId;
        }
        #endregion
    }
}