using Elixir.Contracts.Interfaces;
using Elixir.Helpers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Utils;
using Elixir.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PayPal.Api;
using PostmarkDotNet;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class PublicShopController : BaseController
    {
        private readonly IShopProductProcessor _shopProductProcessor;
        private readonly IShopOrderProcessor _shopOrderProcessor;
        private readonly IShopOrderProductProcessor _shopOrderProductProcessor;
        private readonly IShopProductOptionProcessor _shopProductOptionProcessor;
        private readonly ISettingsProcessor _settingsProcessor;
        private readonly IUsersProcessor _usersProcessor;
        private readonly IUserRoleProcessor _userRoleProcessor;
        private readonly IPaymentProcessor _paymentProcessor;
        private Payment payment;

        public PublicShopController(
            IShopProductProcessor shopProductProcessor,
            IShopOrderProcessor shopOrderProcessor,
            IShopOrderProductProcessor shopOrderProductProcessor,
            IShopProductOptionProcessor shopProductOptionProcessor,
            ISettingsProcessor settingsProcessor,
            IPaymentProcessor paymentProcessor,
            IUsersProcessor usersProcessor,
            IUserRoleProcessor userRoleProcessor
            )
        {
            _shopProductProcessor = shopProductProcessor;
            _shopOrderProcessor = shopOrderProcessor;
            _shopOrderProductProcessor = shopOrderProductProcessor;
            _shopProductOptionProcessor = shopProductOptionProcessor;
            _settingsProcessor = settingsProcessor;
            _usersProcessor = usersProcessor;
            _userRoleProcessor = userRoleProcessor;
            _paymentProcessor = paymentProcessor;
        }

        // GET: /shop/product/{name}
        public ActionResult Product(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ContentNotFoundException("Product's name has to be specified.");

            var product = _shopProductProcessor.GetShopProductByUrlName(name);

            if (product == null)
                throw new ContentNotFoundException("Product with the specified name does not exist.");
            if (product.IsDeleted)
                throw new ContentNotFoundException("Product with the specified name is deleted.");
            if (!product.IsEnabled)
                throw new ContentNotFoundException("Product with the specified name is disabled.");

            if (name.Equals("404", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 404;
            else if (name.Equals("500", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 500;

            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);
            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);
            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, $"Only £ {product.PriceRRP} in the Live Forever Club shop");
            var ogImageName = AppConstants.DefaultShopProductMain;
            if (!string.IsNullOrWhiteSpace(product.ImageMain))
            {
                ogImageName = product.ImageMain;
            }
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/shop/" + ogImageName);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + $"/shop/{AppConstants.DefaultBannerShop}");
            if (!Convert.ToBoolean(WebConfigurationManager.AppSettings["ShopMaintenanceModeOn"].ToString()))
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
                {
                    AltText = $"{product.ShopProductName} page banner",
                    ImgTitle = product.ShopProductName
                });
            }

            var isVisibleNewFlashText = _settingsProcessor.GetSettingsByName("IsVisibleNewFlashText");
            if (isVisibleNewFlashText != null)
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.IsVisibleNewFlashText, isVisibleNewFlashText);
                if (!string.IsNullOrEmpty(isVisibleNewFlashText.PairValue) && Convert.ToBoolean(isVisibleNewFlashText.PairValue))
                {
                    var newFlashText = _settingsProcessor.GetSettingsByName("NewFlashText");
                    if (newFlashText != null)
                        ViewData.AddOrUpdateValue(ViewDataKeys.NewFlashText, newFlashText);
                }
            }

            var viewModel = new ShopViewModel
            {
                ShopProduct = product,
                ShopProductOptions = new List<ShopProductOption>()
            };
            viewModel.ShopProductOptions = _shopProductOptionProcessor.GetAll(product.ShopProductId).OrderBy(x => x.DisplayOrder).ToList();

            if (viewModel.ShopProductOptions != null && viewModel.ShopProductOptions.Count > 0)
            {
                if (string.IsNullOrEmpty(viewModel.ShopProduct.OptionsUnit))
                    viewModel.ShopProduct.OptionsUnit = "Unit";
                var defaultOption = viewModel.ShopProductOptions.Where(x => x.IsDefaultOption).FirstOrDefault();

                var priceRRP = viewModel.ShopProduct.PriceRRP.Value;
                if (viewModel.ShopProduct.PriceLongevist.HasValue)
                {
                    if (Request.IsAuthenticated && User.IsInRole(Roles.Longevist.ToString()))
                        priceRRP = viewModel.ShopProduct.PriceLongevist.Value;
                }

                if (defaultOption != null)
                    viewModel.AddSelectList(nameof(viewModel.ShopProductOptions), GetShopProductOptionsSelectItems(viewModel.ShopProductOptions, priceRRP, defaultOption.ShopProductOptionId));
                else
                    viewModel.AddSelectList(nameof(viewModel.ShopProductOptions), GetShopProductOptionsSelectItems(viewModel.ShopProductOptions, priceRRP));
            }

            viewModel.ShopOrderProducts = new List<ShopOrderProduct>();
            ShopOrder cart = null;
            if (Request.IsAuthenticated)
                cart = _shopOrderProcessor.GetUserCart(Convert.ToInt32(User.Identity.GetUserId()));
            else
            {
                var guestCart = Request.Cookies.Get("cart");
                if (guestCart != null)
                    cart = _shopOrderProcessor.GetGuestCart(!string.IsNullOrEmpty(guestCart.Value) ? guestCart.Value.Trim() : guestCart.Value);
            }
            if (cart != null)
            {
                viewModel.ShopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

                if (viewModel.ShopOrderProducts != null && viewModel.ShopOrderProducts.Count > 0)
                {
                    viewModel.ShopNavigationBar = new ShopNavigationBar
                    {
                        TotalAmount = 0,
                        TotalQuantity = 0,
                        IsOnlyMembershipProducts = false
                    };
                    foreach (var orderProduct in viewModel.ShopOrderProducts)
                    {
                        viewModel.ShopNavigationBar.TotalAmount += (int)orderProduct.Quantity * (decimal)orderProduct.PricePaidPerUnit;
                        viewModel.ShopNavigationBar.TotalQuantity += (int)orderProduct.Quantity;
                    }
                    var membershipProducts = viewModel.ShopOrderProducts.Where(x => x.SKU.Substring(0, 2) == "SM").ToList();
                    if (membershipProducts != null && membershipProducts.Count > 0 && membershipProducts.Count == viewModel.ShopOrderProducts.Count)
                        viewModel.ShopNavigationBar.IsOnlyMembershipProducts = true;
                }
            }

            // Check isLongevistsOnly product for restricted products
            if (product.IsLongevistsOnly)
            {
                if (!Request.IsAuthenticated || !User.IsInRole(Roles.Longevist.ToString()))
                    viewModel.IsDisableAddToCart = true;
            }

            if (!string.IsNullOrEmpty(viewModel.ShopProduct.ContentMain))
            {
                var contentMain = viewModel.ShopProduct.ContentMain;
                var roles = ((ClaimsIdentity)User.Identity).Claims
                  .Where(c => c.Type == ClaimTypes.Role)
                  .Select(c => c.Value).ToList();
                viewModel.ShopProduct.ContentMain = CommonService.ElixcriptToSimpleText(contentMain, roles);
            }

            var currentHost = Request.Url.GetLeftPart(UriPartial.Authority);

            string productStructuredDataScript = ScriptUtils.FillFormatForProduct(ScriptUtils.ProductScriptFormat,
                product.ShopProductName, product.ShopProductDescription, product.SKU, product.ImageMain, product.PriceRRP.ToString(),
                "GBP", currentHost);

            productStructuredDataScript = HttpUtility.HtmlEncode(productStructuredDataScript);

            ViewData.AddOrUpdateValue(ViewDataKeys.ProductScript, productStructuredDataScript);

            return View(viewModel);
        }

        private IEnumerable<SelectListItem> GetShopProductOptionsSelectItems(List<ShopProductOption> options, decimal priceRRP, int selectedId = 0)
        {
            var result = new List<SelectListItem>();

            var isExtraPriceAvailable = options.Where(x => x.PriceExtra.HasValue && x.PriceExtra.Value != 0).ToList().Count > 0;

            foreach (var option in options)
            {
                var listItem = new SelectListItem
                {
                    Text = option.OptionName,
                    Value = option.ShopProductOptionId.ToString()
                };

                if (option.PriceExtra.HasValue && option.PriceExtra.Value != 0)
                    listItem.Text = $"{option.OptionName} (£{priceRRP + option.PriceExtra})";
                else
                {
                    if (isExtraPriceAvailable)
                        listItem.Text = $"{option.OptionName} (£{priceRRP})";
                }
                result.Add(listItem);
            }

            var itemToSelect = result.FirstOrDefault(x => x.Value.Equals(selectedId.ToString()));

            if (itemToSelect != null)
                itemToSelect.Selected = true;

            return result;
        }

        // GET: /shop/addtocart/{sku}
        public ActionResult AddToCart(string sku, int? option)
        {
            TempData.Clear();
            var userId = 0;
            var product = _shopProductProcessor.GetShopProductBySKU(sku);

            if (product == null)
                throw new ContentNotFoundException("Product with the specified name does not exist.");

            // If product is membership product then user must be logged in
            if (product.SKU.Substring(0, 2) == "SM" && !Request.IsAuthenticated)
                return Redirect("/page/login");

            if (product.IsLongevistsOnly)
            {
                // Product for Longevist Members
                if (!Request.IsAuthenticated || !User.IsInRole(Roles.Longevist.ToString()))
                {
                    TempData["PermissionDeniedErrMsg"] = "You have not sufficient permission to do this operation!";
                    return RedirectToAction("Product", "PublicShop", new { name = product.UrlName });
                }
            }

            ShopProductOption productOption = null;
            BookUser user = null;

            if (option != null && option > 0)
                productOption = _shopProductOptionProcessor.GetShopProductOptionById(product.ShopProductId, (int)option);

            if (Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(User.Identity.GetUserId());
                user = _usersProcessor.GetUserById(userId);
                if (user == null)
                    throw new ContentNotFoundException("Logged in user is not found.");
            }

            ShopOrder cart;
            if (userId > 0)
            {
                cart = _shopOrderProcessor.GetUserCart(userId);

                if (cart == null)
                {
                    // If cart is not available then create cart for user.
                    cart = new ShopOrder
                    {
                        StatusId = (int)OrderStatus.ShoppingCart,
                        UserId = userId,
                        CreatedByUserId = userId,
                        UpdatedByUserId = userId
                    };

                    _shopOrderProcessor.CreateShopOrder(cart);
                    cart = _shopOrderProcessor.GetUserCart(userId);
                    if (cart == null)
                        throw new ContentNotFoundException("Unable to find shop order that was added.");
                }
            }
            else
            {
                var guestCart = Request.Cookies["cart"];
                if (guestCart != null)
                {
                    cart = _shopOrderProcessor.GetGuestCart(!string.IsNullOrEmpty(guestCart.Value) ? guestCart.Value.Trim() : guestCart.Value);
                    if (cart == null)
                    {
                        Request.Cookies.Remove("cart");
                        cart = CreateGuestCart();
                    }
                }
                else
                    cart = CreateGuestCart();
            }

            var orderProduct = _shopOrderProductProcessor.GetProductByOrder(cart.ShopOrderId, product.ShopProductId, productOption?.ShopProductOptionId);

            if (orderProduct == null)
            {
                //If product is not available then add product for order.
                orderProduct = new ShopOrderProduct
                {
                    ShopOrderId = cart.ShopOrderId,
                    ShopProductId = product.ShopProductId,
                    DnShopProductName = product.ShopProductName,
                    ShopProductOptionId = productOption?.ShopProductOptionId,
                    DnShopProductOptionName = productOption?.OptionName,
                    SKU = productOption != null ? $"{product.SKU}-{productOption.SkuSuffix}" : product.SKU,
                    Quantity = 1
                };
                if (product.PriceLongevist.HasValue && Request.IsAuthenticated && User.IsInRole(Roles.Longevist.ToString()))
                    orderProduct.PricePaidPerUnit = productOption != null && productOption.PriceExtra != null && productOption.PriceExtra != 0 ? product.PriceLongevist + productOption.PriceExtra : product.PriceLongevist;
                else
                    orderProduct.PricePaidPerUnit = productOption != null && productOption.PriceExtra != null && productOption.PriceExtra != 0 ? product.PriceRRP + productOption.PriceExtra : product.PriceRRP;

                _shopOrderProductProcessor.CreateShopOrderProduct(orderProduct);
            }
            else
            {
                //If product is available then update the quantity of shop order product.
                orderProduct.Quantity += 1;
                orderProduct.DnShopProductName = product.ShopProductName;
                orderProduct.DnShopProductOptionName = productOption?.OptionName;
                orderProduct.SKU = productOption != null ? $"{product.SKU}-{productOption.SkuSuffix}" : product.SKU;
                if (product.PriceLongevist.HasValue && Request.IsAuthenticated && User.IsInRole(Roles.Longevist.ToString()))
                    orderProduct.PricePaidPerUnit = productOption != null && productOption.PriceExtra != null && productOption.PriceExtra != 0 ? product.PriceLongevist + productOption.PriceExtra : product.PriceLongevist;
                else
                    orderProduct.PricePaidPerUnit = productOption != null && productOption.PriceExtra != null && productOption.PriceExtra != 0 ? product.PriceRRP + productOption.PriceExtra : product.PriceRRP;

                _shopOrderProductProcessor.UpdateShopOrderProduct(orderProduct);
            }

            if (orderProduct.SKU.Substring(0, 2) == "SM" && Request.IsAuthenticated)
                cart.EmailAddress = user.EmailAddress;

            var shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

            var totalAmount = Convert.ToDecimal(0);
            var totalQuantity = 0;
            if (shopOrderProducts != null && shopOrderProducts.Count > 0)
            {
                foreach (var op in shopOrderProducts)
                {
                    totalAmount += (int)op.Quantity * (decimal)op.PricePaidPerUnit;
                    totalQuantity += (int)op.Quantity;
                }
            }
            cart.ItemsTotal = totalQuantity;
            cart.PricePaidTotal = totalAmount;
            _shopOrderProcessor.UpdateShopOrder(cart);
            var membershipProducts = shopOrderProducts.Where(x => x.SKU.Substring(0, 2) == "SM").ToList();
            if (membershipProducts != null && membershipProducts.Count > 0)
            {
                //Cart includes membership products
                if (shopOrderProducts.Count == membershipProducts.Count)
                {
                    //Cart has only membership products, then skip view cart page
                    return RedirectToAction("Shop", "WebPageVisual", new { name = "checkout" });
                }
            }
            return RedirectToAction("Shop", "WebPageVisual", new { name = "cart" });
        }

        public async Task<ActionResult> PaymentWithPaypal()
        {
            TempData.Clear();
            if (!Convert.ToBoolean(WebConfigurationManager.AppSettings["ShopMaintenanceModeOn"].ToString()))
            {
                ShopOrder cart = null;
                if (Request.IsAuthenticated)
                    cart = _shopOrderProcessor.GetUserCart(Convert.ToInt32(User.Identity.GetUserId()));
                else
                {
                    var guestCart = Request.Cookies.Get("cart");
                    if (guestCart != null)
                        cart = _shopOrderProcessor.GetGuestCart(!string.IsNullOrEmpty(guestCart.Value) ? guestCart.Value.Trim() : guestCart.Value);
                }
                if (cart == null)
                    throw new ContentNotFoundException("Unable to find shop order for logged in user.");

                BookUser user = null;
                if (Request.IsAuthenticated)
                {
                    user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                    if (user == null)
                        throw new ContentNotFoundException("Unable to find logged in user.");
                }

                var accessToken = GetPaypalAccessToken();

                //getting the apiContext as earlier
                APIContext apiContext = Configuration.GetAPIContext(accessToken);

                try
                {
                    var shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

                    // Get All membership products
                    var membershipProducts = shopOrderProducts.Where(x => x.SKU.Substring(0, 2) == "SM").ToList();

                    // Get memership product if it is available in cart
                    var membershipProduct = shopOrderProducts.Where(x => x.SKU.Substring(0, 2) == "SM").FirstOrDefault();
                    UserRole userRole = null;
                    if (membershipProduct != null)
                    {
                        userRole = _userRoleProcessor.GetUserRole(user.Id, (int)Roles.Supporter);
                        if (userRole == null)
                            throw new ContentNotFoundException("Unable to find user role for logged in user.");
                    }

                    //similar to credit card create itemlist and add item objects to it
                    var itemList = new ItemList() { items = new List<Item>() };

                    var totalAmount = Convert.ToDecimal(0);
                    var totalQuantity = 0;
                    var max = shopOrderProducts.Max(x => x.ShopProduct.ShippingPrice);
                    var shippingAmount = max != null ? max : Convert.ToDecimal(0);
                    if (shopOrderProducts != null && shopOrderProducts.Count > 0)
                    {
                        foreach (var orderProduct in shopOrderProducts)
                        {
                            totalAmount += (int)orderProduct.Quantity * (decimal)orderProduct.PricePaidPerUnit;
                            totalQuantity += (int)orderProduct.Quantity;

                            itemList.items.Add(new Item()
                            {
                                name = orderProduct.DnShopProductName,
                                currency = "GBP",
                                price = ((decimal)orderProduct.PricePaidPerUnit).ToString(),
                                quantity = ((int)orderProduct.Quantity).ToString(),
                                sku = orderProduct.SKU
                            });
                        }
                    }

                    // similar as we did for credit card, do here and create details object
                    var details = new Details()
                    {
                        tax = "0",
                        shipping = shippingAmount.ToString(),
                        subtotal = totalAmount.ToString()
                    };

                    // similar as we did for credit card, do here and create amount object
                    var amount = new Amount()
                    {
                        currency = "GBP",
                        total = (totalAmount + shippingAmount).ToString(), // Total must be equal to sum of shipping, tax and subtotal.
                        details = details
                    };

                    var quantityUnit = totalQuantity > 1 ? "items" : "item";
                    var transactionList = new List<Transaction>
                    {
                        new Transaction()
                        {
                            description = $"Total ({totalQuantity} {quantityUnit}): £ {amount.total}",
                            invoice_number = cart.IDHashCode,
                            amount = amount,
                            item_list = itemList
                        }
                    };

                    string payerId = Request.Params["PayerID"];
                    string paymentId = Request.Params["PaymentID"];

                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId, transactionList);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        TempData["CheckoutErrMsg"] = "Payment execution operation is failed.";
                        return RedirectToAction("Shop", "WebPageVisual", new { name = "checkout" });
                    }
                    cart = GetCurrentCart(cart);
                    _shopOrderProcessor.UpdateShopOrder(cart);
                    // Save Payment in database
                    var paymentObj = new Payments()
                    {
                        PaymentReference = executedPayment.transactions[0].related_resources[0].sale.id,
                        PaymentStatusId = 0,
                        Amount = Convert.ToDecimal(executedPayment.transactions[0].related_resources[0].sale.amount.total),
                        ProcessorName = "PayPal",
                        ShopOrderId = cart.ShopOrderId,
                        PaymentDate = Convert.ToDateTime(executedPayment.transactions[0].related_resources[0].sale.create_time),
                        CreatedBy = 0,
                        UpdatedBy = 0
                    };
                    var paymentResponse = JsonConvert.SerializeObject(executedPayment);
                    if (paymentResponse.Length > 8000)
                        paymentObj.PaymentResponse = paymentResponse.Substring(0, 8000); // Truncate response to 8000 characters
                    else
                        paymentObj.PaymentResponse = paymentResponse;
                    _paymentProcessor.CreatePayment(paymentObj);

                    if (!(membershipProducts != null && membershipProducts.Count > 0 && membershipProducts.Count == shopOrderProducts.Count))
                    {
                        var orderDetails = new List<dynamic>();

                        if (shopOrderProducts != null && shopOrderProducts.Count > 0)
                        {
                            foreach (var orderProduct in shopOrderProducts)
                            {
                                var option = !string.IsNullOrWhiteSpace(orderProduct.DnShopProductOptionName) ? $" - {orderProduct.DnShopProductOptionName}" : "";
                                dynamic orderDetail = new ExpandoObject();
                                orderDetail.description = $"{orderProduct.Quantity} x {orderProduct.DnShopProductName}{option}";
                                orderDetail.amount = (orderProduct.PricePaidPerUnit * orderProduct.Quantity).Value.ToString("f2");
                                orderDetails.Add(orderDetail);
                            }
                            dynamic shipping = new ExpandoObject();
                            shipping.description = "Shipping";
                            shipping.amount = cart.ShippingPricePaid.Value.ToString("f2");
                            orderDetails.Add(shipping);
                        }

                        var message = new TemplatedPostmarkMessage
                        {
                            From = "website@liveforever.club",
                            To = cart.EmailAddress,
                            TemplateAlias = "shop-acknowledgement",
                            TrackOpens = true,
                            TrackLinks = LinkTrackingOptions.None,
                            InlineCss = true,
                            TemplateModel = new Dictionary<string, object> {
                                { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                                { "product_name", "Live Forever Club" },
                                { "name", Request.IsAuthenticated? user.UserName: cart.AddressNameFirst },
                                { "order_id", cart.IDHashCode },
                                { "order_date", cart.OrderPlacedOn.Value.ToString("dd-MMM-yyyy") },
                                { "order_details", orderDetails },
                                { "total", cart.PricePaidTotal.Value.ToString("f2") },
                                { "operating_system", Request.Browser.Platform },
                                { "browser_name", $"{Request.Browser.Browser} {Request.Browser.Version}" },
                                { "support_email", "website@liveforever.club"},
                                { "company_name", "Live Forever Club CIC" },
                                { "company_address", "33, Station Road, Cholsey, Wallingford, OX10 9PT, UK" }
                            }
                        };
                        var result = await EmailService.SendEmail(message);
                        if (!result)
                        {
                            // Handle scenario when email is not sent
                        }
                    }
                    if (membershipProduct != null)
                    {
                        // Update user
                        user.UpdatedBy = Convert.ToInt32(User.Identity.GetUserId());
                        user.ExpiryDate = GetExpiryDate(membershipProduct.SKU);
                        // Assign IsFoundingMember
                        user.IsFoundingMember = user.MemberNumber.HasValue && user.MemberNumber <= 100;
                        _usersProcessor.UpdateUser(user);

                        // Update user role
                        userRole.RoleId = (int)Roles.Longevist;
                        _userRoleProcessor.UpdateUserRole(userRole);
                        // Sign in again to update the session cookie
                        // With this session gets updated roles for logged in user
                        await AccessSignInManager.SignInAsync(user, true, true);

                        var message = new TemplatedPostmarkMessage
                        {
                            From = "website@liveforever.club",
                            To = user.EmailAddress,
                            TemplateAlias = "upgrade-member",
                            TrackOpens = true,
                            TrackLinks = LinkTrackingOptions.None,
                            InlineCss = true,
                            TemplateModel = new Dictionary<string, object> {
                                { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                                { "product_name", "Live Forever Club" },
                                { "name", user.UserName },
                                { "expiry_date", user.ExpiryDate.Value.ToString("dd-MMM-yyyy") },
                                { "account_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/account" },
                                { "operating_system", Request.Browser.Platform },
                                { "browser_name", $"{Request.Browser.Browser} {Request.Browser.Version}" },
                                { "support_email", "website@liveforever.club"},
                                { "company_name", "Live Forever Club CIC" },
                                { "company_address", "33, Station Road, Cholsey, Wallingford, OX10 9PT, UK" }
                            }
                        };
                        var result = await EmailService.SendEmail(message);
                        if (!result)
                        {
                            // Handle scenario when email is not sent
                        }
                        return Redirect("/page/welcome");
                    }
                    return RedirectToAction("Shop", "WebPageVisual", new { name = "acknowledgement", order = cart.IDHashCode });
                }
                catch (Exception ex)
                {
                    TempData["CheckoutErrMsg"] = ex.Message;
                    return RedirectToAction("Shop", "WebPageVisual", new { name = "checkout" });
                }
            }
            return RedirectToAction("Shop", "WebPageVisual", new { name = "acknowledgement" });
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId, List<Transaction> transactionList)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId, transactions = transactionList };
            payment = new Payment() { id = paymentId };
            return payment.Execute(apiContext, paymentExecution);
        }

        private ShopOrder GetCurrentCart(ShopOrder cart)
        {
            cart.OrderPlacedOn = DateTime.Now;
            cart.StatusId = (int)OrderStatus.OrderBeingProcessed;
            cart.OrderPlacedIPAddressString = Request.UserHostAddress;

            var shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

            var totalAmount = Convert.ToDecimal(0);
            var totalQuantity = 0;
            var max = shopOrderProducts.Max(x => x.ShopProduct.ShippingPrice);
            var shippingAmount = max != null ? max : Convert.ToDecimal(0);
            if (shopOrderProducts != null && shopOrderProducts.Count > 0)
            {
                foreach (var orderProduct in shopOrderProducts)
                {
                    totalAmount += (int)orderProduct.Quantity * (decimal)orderProduct.PricePaidPerUnit;
                    totalQuantity += (int)orderProduct.Quantity;
                }
            }
            cart.ItemsTotal = totalQuantity;
            cart.ShippingPricePaid = shippingAmount;
            cart.PricePaidTotal = totalAmount + shippingAmount;
            return cart;
        }

        private string GetPaypalAccessToken()
        {
            var accessToken = string.Empty;

            var paypalTokenSettings = _settingsProcessor.GetSettingsByName("PayPalToken");
            if (paypalTokenSettings == null)
                accessToken = Configuration.GetAuthTokenCredential().GetAccessToken();
            else
            {
                if (!string.IsNullOrEmpty(paypalTokenSettings.PairValue) && paypalTokenSettings.PayPalTokenExpirationDT.HasValue)
                {
                    if (DateTime.Compare((DateTime)paypalTokenSettings.PayPalTokenExpirationDT, DateTime.Now) > 0)
                        accessToken = paypalTokenSettings.PairValue;
                }

                // Store new token
                if (string.IsNullOrEmpty(accessToken))
                {
                    var authTokenCredential = Configuration.GetAuthTokenCredential();
                    accessToken = authTokenCredential.GetAccessToken();
                    var tokenExpiry = authTokenCredential.AccessTokenExpirationInSeconds;

                    // Note:
                    // Here token expiry is 9 hours (540 minutes or 32400 seconds)
                    // But we set expiry 8 hours (480 minutes or 28800 seconds) for safety.

                    paypalTokenSettings.PairValue = accessToken;
                    if (tokenExpiry > 3600)
                        paypalTokenSettings.PayPalTokenExpirationDT = DateTime.Now.AddSeconds(tokenExpiry - 3600);
                    else
                        paypalTokenSettings.PayPalTokenExpirationDT = DateTime.Now.AddSeconds(tokenExpiry);
                    _settingsProcessor.UpdateSettigs(paypalTokenSettings);
                }
            }
            return accessToken;
        }

        private ShopOrder CreateGuestCart()
        {
            // If cart is not available then create cart for user.
            var cart = new ShopOrder
            {
                StatusId = (int)OrderStatus.ShoppingCart,
                UserId = 0,
                CreatedByUserId = 0,
                UpdatedByUserId = 0
            };
            cart.IDHashCode = cart.CalculateIdHashCode();

            var idHashCode = _shopOrderProcessor.CreateShopOrder(cart, true);
            cart = _shopOrderProcessor.GetGuestCart(idHashCode);
            if (cart == null)
                throw new ContentNotFoundException("Unable to find shop order that was added.");
            var guestCart = new HttpCookie("cart", idHashCode)
            {
                Expires = DateTime.Now.AddYears(1)
            };
            Response.Cookies.Add(guestCart);
            return cart;
        }

        private DateTime GetExpiryDate(string sku)
        {
            // SKU -> SM-MEMBNEW-1
            var parts = sku.Split('-'); // ["SM", "MEMBNEW", "1"]
            var value = parts[2]; // "1"
            switch (value)
            {
                case "1":
                case "1E":
                    return DateTime.Now.AddYears(1).AddDays(-1);
                case "2":
                    return DateTime.Now.AddYears(2).AddDays(-1);
                case "3":
                    return DateTime.Now.AddYears(3).AddDays(-1);
                default:
                    return DateTime.Now.AddYears(1).AddDays(-1);
            }
        }
    }
}