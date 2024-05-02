using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Elixir.BusinessLogic.Core.Utils;
using Elixir.Contracts.Interfaces;
using Elixir.Helpers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Identity;
using Elixir.Models.Json;
using Elixir.Models.Utils;
using Elixir.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.ViewModels.Navigation;
using Elixir.Views;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PostmarkDotNet;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class WebPageVisualController : BaseController
    {
        private readonly IWebPagesProcessor _webPagesProcessor;
        private readonly IArticlesProcessor _articlesProcessor;
        private readonly IBlogPostsProcessor _blogPostsProcessor;
        private readonly IResourcesProcessor _resourcesProcessor;
        private readonly IShopProductProcessor _shopProductProcessor;
        private readonly IShopOrderProcessor _shopOrderProcessor;
        private readonly IShopOrderProductProcessor _shopOrderProductProcessor;
        private readonly IUsersProcessor _usersProcessor;
        private readonly ICountryProcessor _countryProcessor;
        private ITopicsProcessor _topicsProcessor;
        private readonly IWebPageXTopicProcessor _webPageXTopicProcessor;
        private readonly IAuditLogsProcessor _auditLogsProcessor;
        private readonly IUserRoleProcessor _userRoleProcessor;
        private readonly ISettingsProcessor _settingsProcessor;

        public WebPageVisualController(
            IWebPagesProcessor webPagesProcessor,
            IArticlesProcessor articlesProcessor,
            ITopicsProcessor topicsProcessor,
            IWebPageXTopicProcessor webPageXTopicProcessor,
            IBlogPostsProcessor blogPostsProcessor,
            IResourcesProcessor resourcesProcessor,
            IShopProductProcessor shopProductProcessor,
            IShopOrderProcessor shopOrderProcessor,
            IShopOrderProductProcessor shopOrderProductProcessor,
            IUsersProcessor usersProcessor,
            ICountryProcessor countryProcessor,
            IAuditLogsProcessor auditLogsProcessor,
            IUserRoleProcessor userRoleProcessor,
            ISettingsProcessor settingsProcessor
            )
        {
            _webPagesProcessor = webPagesProcessor;
            _articlesProcessor = articlesProcessor;
            _topicsProcessor = topicsProcessor;
            _webPageXTopicProcessor = webPageXTopicProcessor;
            _blogPostsProcessor = blogPostsProcessor;
            _resourcesProcessor = resourcesProcessor;
            _shopProductProcessor = shopProductProcessor;
            _shopOrderProcessor = shopOrderProcessor;
            _shopOrderProductProcessor = shopOrderProductProcessor;
            _usersProcessor = usersProcessor;
            _countryProcessor = countryProcessor;
            _auditLogsProcessor = auditLogsProcessor;
            _userRoleProcessor = userRoleProcessor;
            _settingsProcessor = settingsProcessor;
        }

        //URL: /page/<name>
        public ActionResult Index(string name = "home")
        {
            var filteredName = name.GetAlphanumericValue();
            if (filteredName.Equals(""))
                filteredName = "home";

            var vm = new WebPageViewModel();

            var webPage = _webPagesProcessor.GetWebPageByUrlName(filteredName, (int)EnumWebPageType.Page);
            vm.WebPage = webPage ?? throw new ContentNotFoundException("The page with the specified name does not exist.");


            if (filteredName.Equals("404", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 404;
            else if (filteredName.Equals("500", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 500;

            if (vm.WebPage != null && !string.IsNullOrEmpty(vm.WebPage.ContentMain))
            {
                var contentMain = vm.WebPage.ContentMain;
                var roles = ((ClaimsIdentity)User.Identity).Claims
                  .Where(c => c.Type == ClaimTypes.Role)
                  .Select(c => c.Value).ToList();
                vm.WebPage.ContentMain = CommonService.ElixcriptToSimpleText(contentMain, roles);
            }

            if ((vm.IsLoginPage || vm.IsRegistrationPage) && Request.IsAuthenticated)
                return RedirectToAction("Index", new { name = "home" });
            vm = GetViewModel(vm, EnumWebPageType.Page);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Login(Login model)
        {
            TempData.Clear();
            model = TrimmingLoginForm(model);
            if (ModelState.IsValid)
            {
                var failedLoginReachLimitInSec = ConfigurationManager.AppSettings["FailedLoginReachLimitInSec"] != null ? int.Parse(ConfigurationManager.AppSettings["FailedLoginReachLimitInSec"]) : 60;
                var failedLoginAttemptAllowed = ConfigurationManager.AppSettings["FailedLoginAttemptAllowed"] != null ? int.Parse(ConfigurationManager.AppSettings["FailedLoginAttemptAllowed"]) : 60;
                var ipAddress = Request.UserHostAddress;
                var user = _usersProcessor.GetUserByEmail(model.EmailAddress);
                if (user == null)
                {
                    //Login failure Log
                    _auditLogsProcessor.Log(new AuditLog()
                    {
                        ActionTypeID = (byte)AuditLogActionType.LoginFailure,
                        EntityID = 0,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        IpAddressString = ipAddress,
                        NotesLog = model.EmailAddress,
                        UserID = 0
                    });
                    TempData["ErrorMsgLogin"] = "Login failed. Please make sure both your email address and password are correct. If the problem persists, please contact your manager.";
                    return RedirectToAction("Index", new { name = "login" });
                }
                var hashSalt = user.PasswordSalt;
                var hashPassword = PasswordUtil.EncodePassword(model.Password, hashSalt);
                if (user.PasswordHash.Equals(hashPassword) == false)
                {
                    //Login failure Log
                    _auditLogsProcessor.Log(new AuditLog()
                    {
                        ActionTypeID = (byte)AuditLogActionType.LoginFailure,
                        EntityID = user.Id,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        IpAddressString = ipAddress,
                        NotesLog = model.EmailAddress,
                        UserID = 0
                    });

                    //Security reach Limit check
                    var noOfFailedLogins = _auditLogsProcessor.NoOfFailedLoginsForUserInSeconds(failedLoginReachLimitInSec, string.Empty, user.Id);
                    if (noOfFailedLogins >= failedLoginAttemptAllowed)
                    {
                        _auditLogsProcessor.Log(new AuditLog()
                        {
                            ActionTypeID = (byte)AuditLogActionType.LoginFailure,
                            EntityID = user.Id,
                            EntityTypeID = (byte)AuditLogEntityType.User,
                            IpAddressString = ipAddress,
                            NotesLog = "SECURITY RATE LIMIT REACHED",
                            UserID = 0
                        });
                    }
                    TempData["ErrorMsgLogin"] = "Login failed. Please make sure both your email address and password are correct. If the problem persists, please contact your manager.";
                    return RedirectToAction("Index", new { name = "login" });
                }
                else
                {
                    //Security reach Limit check
                    var noOfFailedLogins = _auditLogsProcessor.NoOfFailedLoginsForUserInSeconds(failedLoginReachLimitInSec, string.Empty, user.Id);
                    if (noOfFailedLogins >= failedLoginAttemptAllowed)
                    {
                        _auditLogsProcessor.Log(new AuditLog()
                        {
                            ActionTypeID = (byte)AuditLogActionType.LoginRestricted,
                            EntityID = user.Id,
                            EntityTypeID = (byte)AuditLogEntityType.User,
                            IpAddressString = ipAddress,
                            NotesLog = "SECURITY RATE LIMIT REACHED",
                            UserID = 0
                        });
                        TempData["ErrorMsgLogin"] = "Login failed. Please make sure both your email address and password are correct. If the problem persists, please contact your manager.";
                        return RedirectToAction("Index", new { name = "login" });
                    }
                }
                if (user.IsEnabled == false)
                {
                    //Login failure Log
                    _auditLogsProcessor.Log(new AuditLog()
                    {
                        ActionTypeID = (byte)AuditLogActionType.LoginFailure,
                        EntityID = user.Id,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        IpAddressString = ipAddress,
                        NotesLog = model.EmailAddress,
                        UserID = 0
                    });
                    TempData["ErrorMsgLogin"] = "Account disabled. Please contact your manager.";
                    return RedirectToAction("Index", new { name = "login" });
                }
                AccessSignInManager.SignIn(user, true, true);
                _usersProcessor.UpdateUserLoginTime(user.Id);

                //Success Log           
                _auditLogsProcessor.Log(new AuditLog()
                {
                    ActionTypeID = (byte)AuditLogActionType.LoginSuccess,
                    EntityID = user.Id,
                    EntityTypeID = (byte)AuditLogEntityType.User,
                    IpAddressString = ipAddress,
                    NotesLog = "",
                    UserID = user.Id
                });

                // Redirect based on user roles.
                var userRoles = _userRoleProcessor.GetUserRoles(user.Id).ToList();
                if (userRoles.Count > 1)
                {
                    var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
                    var loginRedirectUrl = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role.LoginRedirectUrl).FirstOrDefault();
                    return Redirect(loginRedirectUrl);
                }
                else
                    return Redirect(userRoles[0].Role.LoginRedirectUrl);
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            TempData["ErrorMsgLogin"] = error.ErrorMessage;
                            break;
                        }
                    }
                    if (TempData["ErrorMsgLogin"] != null)
                        break;
                }
                return RedirectToAction("Index", new { name = "login" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Registration(Registration model)
        {
            TempData.Clear();
            model = TrimmingRegistrationForm(model);
            if (!string.IsNullOrEmpty(model.Password) && !PasswordValidate(model.Password))
                ModelState.AddModelError("Password", "Password must contains 8-50 characters, one Uppercase, one Lowercase and one Digit!");
            if (model.MembershipLevel != (int)Roles.Longevist && model.MembershipLevel != (int)Roles.Supporter)
                ModelState.AddModelError("MembershipLevel", "Select valid membership level!");
            int countryId = 0;
            if (!string.IsNullOrEmpty(model.CountryName))
            {
                var countryName = model.CountryName.Trim();
                var result = _countryProcessor.SearchCountryByName(countryName, 1);
                if (result != null && result.Count() > 0)
                    countryId = result.FirstOrDefault().CountryID;
                else
                    ModelState.AddModelError("CountryName", "Please select a country from the search results!");
            }

            if (ModelState.IsValid)
            {
                var user = _usersProcessor.GetUserByEmail(model.EmailAddress);
                TemplatedPostmarkMessage message;
                if (user == null)
                    message = CreateUser(model, countryId);
                else if (user.IsEnabled || (!user.IsEnabled && user.LastLogin.HasValue))
                {
                    // Add Audit Log
                    var auditLog = new AuditLog
                    {
                        UserID = 0,
                        IpAddressString = Request.UserHostAddress,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        EntityID = user.Id,
                        ActionTypeID = (byte)AuditLogActionType.CreateFailure,
                        NotesLog = "Already registered",
                        CreatedDT = DateTime.Now
                    };
                    _auditLogsProcessor.CreateAuditLog(auditLog);

                    message = new TemplatedPostmarkMessage
                    {
                        From = "website@liveforever.club",
                        To = model.EmailAddress,
                        TemplateAlias = "email-already-registered",
                        TrackOpens = true,
                        TrackLinks = LinkTrackingOptions.None,
                        InlineCss = true,
                        TemplateModel = new Dictionary<string, object> {
                                { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                                { "product_name", "Live Forever Club" },
                                { "name", model.FirstName },
                                { "action_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/auth/forgotten" },
                                { "operating_system", Request.Browser.Platform },
                                { "browser_name", $"{Request.Browser.Browser} {Request.Browser.Version}" },
                                { "support_url", "mailto:website@liveforever.club"},
                                { "company_name", "Live Forever Club CIC" },
                                { "company_address", "33, Station Road, Cholsey, Wallingford, OX10 9PT, UK" }
                        }
                    };
                }
                else
                {
                    //Previous registration incomplete
                    var log = new AuditLog
                    {
                        UserID = 0,
                        IpAddressString = Request.UserHostAddress,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        EntityID = user.Id,
                        ActionTypeID = (byte)AuditLogActionType.Delete,
                        NotesLog = "Previous registration incomplete",
                        CreatedDT = DateTime.Now
                    };
                    _auditLogsProcessor.CreateAuditLog(log);
                    _usersProcessor.DeleteUser(user.Id);
                    message = CreateUser(model, countryId);
                }
                var result = await EmailService.SendEmail(message);
                if (!result)
                {
                    // Handle scenario when email is not sent
                }
                TempData["RegistrationSuccessMsg"] = "We have sent you an email to confirm your email address - please check your Inbox (and spam!) and click on the link to confirm registration.";
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            TempData["ErrorMsgRegistration"] = error.ErrorMessage;
                            break;
                        }
                    }
                    if (TempData["ErrorMsgRegistration"] != null)
                        break;
                }
                TempData["model"] = model;
            }
            return RedirectToAction("Index", new { name = "registration" });
        }

        [HttpPost]
        public ActionResult SaveAccountDetails(AccountDetails model)
        {
            TempData.Clear();
            if (ModelState.IsValid)
            {
                if (!Request.IsAuthenticated)
                    throw new ContentNotFoundException("User is not authenticated.");

                var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                if (user == null)
                    throw new ContentNotFoundException("User is not found.");

                if (!string.IsNullOrEmpty(model.FirstName))
                    user.UserName = model.FirstName.Trim();

                if (!string.IsNullOrEmpty(model.LastName))
                    user.UserNameLast = model.LastName.Trim();
                else
                    user.UserNameLast = null;

                if (!string.IsNullOrEmpty(model.DescriptionPublic))
                    user.DescriptionPublic = model.DescriptionPublic.Trim();
                else
                    user.DescriptionPublic = null;

                //if (!string.IsNullOrEmpty(model.EmailAddress))
                //    user.EmailAddress = model.EmailAddress.Trim();
                //else
                //    user.EmailAddress = null;

                if (!string.IsNullOrEmpty(model.DisplayName))
                    user.UserNameDisplay = model.DisplayName.Trim();
                else
                    user.UserNameDisplay = null;

                if (!string.IsNullOrEmpty(model.CountryName) && !string.IsNullOrEmpty(model.CountryName.Trim()))
                {
                    var CountryName = model.CountryName.Trim();
                    var Result = _countryProcessor.SearchCountryByName(CountryName, 1);

                    if (Result != null && Result.Count() > 0)
                    {
                        user.CountryId = Result.FirstOrDefault().CountryID;
                    }
                    else
                    {
                        ModelState.AddModelError("CountryName", "Please select a country from the search results!");
                        TempData["ErrorMsgAccountDetails"] = "Please select a country from the search results!";
                        return RedirectToAction("Account", new { name = "details" });
                    }
                }
                user.UpdatedBy = user.Id;
                _usersProcessor.UpdateUser(user);
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            TempData["ErrorMsgAccountDetails"] = error.ErrorMessage;
                            break;
                        }
                    }
                    if (TempData["ErrorMsgAccountDetails"] != null)
                        break;
                }
                return RedirectToAction("Account", new { name = "details" });
            }
            return RedirectToAction("Account", new { name = "home" });
        }

        [HttpPost]
        public ActionResult SaveAccountProfileDetails(AccountProfileDetails model)
        {
            TempData.Clear();
            if (ModelState.IsValid)
            {
                if (!Request.IsAuthenticated)
                    throw new ContentNotFoundException("User is not authenticated.");

                var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                if (user == null)
                    throw new ContentNotFoundException("User is not found.");

                if (!string.IsNullOrEmpty(model.Visibilty))
                {
                    if (model.Visibilty == "Public")
                    {
                        user.ProfileIsPublic = true;
                        user.ProfileIsMembersOnly = false;
                    }
                    else if (model.Visibilty == "MemberOnly")
                    {
                        user.ProfileIsPublic = false;
                        user.ProfileIsMembersOnly = true;
                    }
                    else if (model.Visibilty == "Private")
                    {
                        user.ProfileIsPublic = false;
                        user.ProfileIsMembersOnly = false;
                    }
                }
                //Biography
                if (!string.IsNullOrEmpty(model.Biography))
                    user.Biography = model.Biography.Trim();
                else
                    user.Biography = null;

                //WebsiteUrl
                if (!string.IsNullOrEmpty(model.WebsiteUrl))
                    user.WebsiteUrl = model.WebsiteUrl.Trim();
                else
                    user.WebsiteUrl = null;

                //FacebookUrl
                if (!string.IsNullOrEmpty(model.FacebookUrl))
                    user.FacebookUrl = model.FacebookUrl.Trim();
                else
                    user.FacebookUrl = null;

                //InstagramUrl
                if (!string.IsNullOrEmpty(model.InstagramUrl))
                    user.InstagramUrl = model.InstagramUrl.Trim();
                else
                    user.InstagramUrl = null;

                //LinkedInUrl
                if (!string.IsNullOrEmpty(model.LinkedInUrl))
                    user.LinkedInUrl = model.LinkedInUrl.Trim();
                else
                    user.LinkedInUrl = null;

                //LinkedInUrl
                if (!string.IsNullOrEmpty(model.TwitterUrl))
                    user.TwitterUrl = model.TwitterUrl.Trim();
                else
                    user.TwitterUrl = null;

                //OtherUrl
                if (!string.IsNullOrEmpty(model.OtherUrl))
                    user.OtherUrl = model.OtherUrl.Trim();
                else
                    user.OtherUrl = null;

                //user.ProfileIsPublic = model.ProfileIsPublic;
                //if (!string.IsNullOrEmpty(model.DescriptionPublic))
                //    user.DescriptionPublic = model.DescriptionPublic.Trim();
                //else
                //    user.DescriptionPublic = null;
                user.UpdatedBy = user.Id;
                _usersProcessor.UpdateUser(user);
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            TempData["ErrorMsgAccountProfileDetails"] = error.ErrorMessage;
                            break;
                        }
                    }
                    if (TempData["ErrorMsgAccountProfileDetails"] != null)
                        break;
                }
                return RedirectToAction("Account", new { name = "profile" });
            }
            return RedirectToAction("Account", new { name = "home" });
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword model)
        {
            TempData.Clear();
            if (ModelState.IsValid)
            {
                if (!Request.IsAuthenticated)
                    throw new ContentNotFoundException("User is not authenticated.");

                var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                if (user == null)
                    throw new ContentNotFoundException("User is not found.");

                var HashSalt = PasswordUtil.GenerateSalt();
                var HashPassword = PasswordUtil.EncodePassword(model.NewPassword, HashSalt);

                var CurrentHashSalt = user.PasswordSalt;
                var CurrentHashPassword = PasswordUtil.EncodePassword(model.CurrentPassword, CurrentHashSalt);

                if (CurrentHashPassword == user.PasswordHash)
                {
                    user.PasswordHash = HashPassword;
                    user.PasswordSalt = HashSalt;
                    user.PasswordUpdatedOn = DateTime.Now;
                    user.UpdatedBy = user.Id;
                    _usersProcessor.UpdateUser(user);
                    TempData["SuccessMsgChangePassword"] = "Your password has been updated.";
                }
                else
                    TempData["ErrorMsgChangePassword"] = "Incorrect password! Please try again.";
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            TempData["ErrorMsgChangePassword"] = error.ErrorMessage;
                            break;
                        }
                    }
                    if (TempData["ErrorMsgChangePassword"] != null)
                        break;
                }
            }
            return RedirectToAction("Account", new { name = "password" });
        }

        //URL: /shop/{name}
        public ActionResult Shop(string name = "home", string order = null)
        {
            var filteredName = name.GetAlphanumericValue();
            if (filteredName.Equals(""))
                filteredName = "home";

            var vm = new WebPageViewModel();

            var webPage = _webPagesProcessor.GetWebPageByUrlName(filteredName, (int)EnumWebPageType.Shop);
            vm.WebPage = webPage ?? throw new ContentNotFoundException("The page with the specified name does not exist.");

            if (filteredName.Equals("404", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 404;
            else if (filteredName.Equals("500", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 500;

            if (vm.IsCartPage || vm.IsShippingDetailsPage || vm.IsCheckoutPage)
            {
                var shopOrderProducts = new List<ShopOrderProduct>();
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
                    shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();
                    if (shopOrderProducts != null && shopOrderProducts.Count > 0)
                    {
                        var membershipProducts = shopOrderProducts.Where(x => x.SKU.Substring(0, 2) == "SM").ToList();
                        if (vm.IsCheckoutPage)
                        {
                            if (membershipProducts != null && membershipProducts.Count > 0 && membershipProducts.Count == shopOrderProducts.Count)
                            {
                                if (string.IsNullOrEmpty(cart.EmailAddress))
                                    return RedirectToAction("Shop", "WebPageVisual", new { name = "cart" });
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(cart.EmailAddress) || string.IsNullOrEmpty(cart.AddressPostcode) || !cart.AddressCountryID.HasValue)
                                    return RedirectToAction("Shop", "WebPageVisual", new { name = "shipping" });
                            }
                        }
                        else
                        {
                            if (membershipProducts != null && membershipProducts.Count > 0 && membershipProducts.Count == shopOrderProducts.Count)
                                return RedirectToAction("Shop", "WebPageVisual", new { name = "checkout" });
                        }
                    }
                }
            }

            vm = GetViewModel(vm, EnumWebPageType.Shop, order);
            return View("Index", vm);
        }

        public ActionResult DeleteItem(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                throw new ContentNotFoundException("The product with the specified sku does not exist.");

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
                throw new ContentNotFoundException("Shop order is not found.");

            var shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

            if (shopOrderProducts != null && shopOrderProducts.Count > 0)
            {
                var product = shopOrderProducts.Where(x => x.SKU.ToLower() == sku.ToLower()).SingleOrDefault();
                if (product == null)
                    throw new ContentNotFoundException("Shop product is not found.");

                //Delete product
                _shopOrderProductProcessor.DeleteShopOrderProduct(product.ShopOrderProductId);
            }
            else
                throw new ContentNotFoundException("Shop products are not found.");

            shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();
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
            //If shop order has no products then delete shop order
            if (shopOrderProducts.Count <= 0)
            {
                _shopOrderProcessor.DeleteShopOrder(cart.ShopOrderId);
                //If user is not logged in then remove cart cookie
                if (!Request.IsAuthenticated)
                    Request.Cookies.Remove("cart");
            }
            return RedirectToAction("Shop", "WebPageVisual", new { name = "cart" });
        }

        public ActionResult IncrementItem(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                throw new ContentNotFoundException("The product with the specified sku does not exist.");

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
                throw new ContentNotFoundException("Shop order is not found.");

            var shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

            if (shopOrderProducts != null && shopOrderProducts.Count > 0)
            {
                var product = shopOrderProducts.Where(x => x.SKU.ToLower() == sku.ToLower()).SingleOrDefault();
                if (product == null)
                    throw new ContentNotFoundException("Shop product is not found.");

                //Increment product
                product.Quantity += 1;
                _shopOrderProductProcessor.UpdateShopOrderProduct(product);
            }
            else
                throw new ContentNotFoundException("Shop products are not found.");

            shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();
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
            return RedirectToAction("Shop", "WebPageVisual", new { name = "cart" });
        }

        public ActionResult DecrementItem(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                throw new ContentNotFoundException("The product with the specified sku does not exist.");

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
                throw new ContentNotFoundException("Shop order is not found.");

            var shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

            if (shopOrderProducts != null && shopOrderProducts.Count > 0)
            {
                var product = shopOrderProducts.Where(x => x.SKU.ToLower() == sku.ToLower()).SingleOrDefault();
                if (product == null)
                    throw new ContentNotFoundException("Shop product is not found.");

                //Decrement Item
                if (product.Quantity == 1)
                    _shopOrderProductProcessor.DeleteShopOrderProduct(product.ShopOrderProductId);
                else
                {
                    product.Quantity -= 1;
                    _shopOrderProductProcessor.UpdateShopOrderProduct(product);
                }
            }
            else
                throw new ContentNotFoundException("Shop products are not found.");

            shopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();
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
            //If shop order has no products then delete shop order
            if (shopOrderProducts.Count <= 0)
            {
                _shopOrderProcessor.DeleteShopOrder(cart.ShopOrderId);
                //If user is not logged in then remove cart cookie
                if (!Request.IsAuthenticated)
                    Request.Cookies.Remove("cart");
            }
            return RedirectToAction("Shop", "WebPageVisual", new { name = "cart" });
        }

        [HttpPost]
        public ActionResult SaveShippingDetails(ShippingDetails model)
        {
            TempData.Clear();
            if (ModelState.IsValid)
            {
                if (Request.IsAuthenticated)
                {
                    var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                    if (user == null)
                        throw new ContentNotFoundException("Logged in user is not found.");

                    if (model.EmailAddress.ToLower() != user.EmailAddress)
                    {
                        TempData["ErrorMsgShippingDetails"] = "The Email address field is not a valid e-mail address.";
                        return RedirectToAction("Shop", new { name = "shipping" });
                    }
                }

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
                    throw new ContentNotFoundException("Shop order is not found.");

                cart.EmailAddress = model.EmailAddress.Trim();
                cart.AddressNameFirst = model.FirstName.Trim();
                cart.AddressNameLast = model.LastName.Trim();
                cart.AddressLine1 = model.AddressLine1.Trim();
                if (!string.IsNullOrEmpty(model.AddressLine2))
                    cart.AddressLine2 = model.AddressLine2.Trim();
                if (!string.IsNullOrEmpty(model.AddressLine3))
                    cart.AddressLine3 = model.AddressLine3.Trim();
                cart.AddressTown = model.Town.Trim();
                cart.AddressPostcode = model.Postcode.Trim();
                if (!string.IsNullOrEmpty(model.ContactTelephoneNumber))
                    cart.TelephoneNumber = model.ContactTelephoneNumber.Trim();

                if (!string.IsNullOrEmpty(model.Country))
                {
                    var CountryName = model.Country.Trim();
                    var Result = _countryProcessor.SearchCountryByName(CountryName, 1);
                    if (Result != null && Result.Count() > 0)
                        cart.AddressCountryID = Result.FirstOrDefault().CountryID;
                    else
                    {
                        TempData["ErrorMsgShippingDetails"] = "The Country field is not a valid country.";
                        return RedirectToAction("Shop", new { name = "shipping" });
                    }
                }
                if (Request.IsAuthenticated)
                    cart.UpdatedByUserId = Convert.ToInt32(User.Identity.GetUserId());
                _shopOrderProcessor.UpdateShopOrder(cart);
            }
            else
            {
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        if (!string.IsNullOrEmpty(error.ErrorMessage))
                        {
                            TempData["ErrorMsgShippingDetails"] = error.ErrorMessage;
                            break;
                        }
                    }
                    if (TempData["ErrorMsgShippingDetails"] != null)
                        break;
                }
                return RedirectToAction("Shop", new { name = "shipping" });
            }
            return RedirectToAction("Shop", new { name = "checkout" });
        }

        //URL: /account/{name}
        public ActionResult Account(string name = "home", string order = null)
        {
            var filteredName = name.GetAlphanumericValue();
            if (filteredName.Equals(""))
                filteredName = "home";

            var vm = new WebPageViewModel();

            var webPage = _webPagesProcessor.GetWebPageByUrlName(filteredName, (int)EnumWebPageType.Account);
            vm.WebPage = webPage ?? throw new ContentNotFoundException("The page with the specified name does not exist.");

            if (filteredName.Equals("404", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 404;
            else if (filteredName.Equals("500", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 500;

            vm = GetViewModel(vm, EnumWebPageType.Account, order);
            return View("Index", vm);
        }

        //URL: /events/{name}
        public ActionResult Events(string name = "events-calendar")
        {
            var filteredName = name.GetAlphanumericValue();
            if (filteredName.Equals(""))
                filteredName = "events-calendar";

            var vm = new WebPageViewModel();

            var webPage = _webPagesProcessor.GetWebPageByUrlName(filteredName, (int)EnumWebPageType.Events);
            vm.WebPage = webPage ?? throw new ContentNotFoundException("The page with the specified name does not exist.");

            if (filteredName.Equals("404", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 404;
            else if (filteredName.Equals("500", StringComparison.OrdinalIgnoreCase))
                Response.StatusCode = 500;

            vm = GetViewModel(vm, EnumWebPageType.Events);
            return View("Index", vm);
        }

        [ChildActionOnly]
        public ActionResult RenderMainNavigationLinks()
        {
            var viewModel = new NavigationViewModel();
            //how many max?
            var rootWebPages = _webPagesProcessor.GetWebPagesChildren(1);
            foreach (var webPage in rootWebPages)
            {
                var childrenWebPages = _webPagesProcessor.GetWebPagesChildren(webPage.Id.Value).Select(x => new NavigationItem()
                {
                    Text = x.WebPageName,
                    Url = Url.Action("Index", new { name = x.UrlName })
                });

                viewModel.Items.Add(new NavigationItem
                {
                    Text = webPage.WebPageName,
                    Url = Url.Action("Index", new { name = webPage.UrlName }),
                    ChildItems = childrenWebPages
                });
            }
            var resourcesWebPage = viewModel.Items.Where(x => x.Text.ToLower() == "resources").FirstOrDefault();
            if(resourcesWebPage != null)
                resourcesWebPage.ChildItems = GetResourcesChildItems();
            return PartialView("_MainNavigation", viewModel);
        }

        [ChildActionOnly]
        public ActionResult RenderLatestBlogPosts()
        {
            var vm = new SidebarViewModel
            {
                LatestBlogPosts = _blogPostsProcessor.GetLatest().Select(x => new UrlLink(x.BlogPostTitle, x.UrlName))
            };

            return PartialView("_LatestBlogPosts", vm);
        }

        private WebPageViewModel GetViewModel(WebPageViewModel vm, EnumWebPageType webPageType, string order = null)
        {
            if (webPageType == EnumWebPageType.Page)
            {
                if (vm.IsMembersDirectoryPage)
                    vm.MembersDirectory = _usersProcessor.GetMembersDirectory().ToList();
                else if (vm.IsFoundingMembersPage)
                    vm.FoundingMembers = _usersProcessor.GetFoundingMembers().ToList();
                else
                {
                    // Get latest articles.
                    if (vm.WebPage.IsSubjectPage)
                    {
                        if (vm.IsHomePage)
                        {
                            vm.LatestArticles = _articlesProcessor.GetLatestArticles();
                            vm.RecentBlogPosts = _blogPostsProcessor.GetLatestBlogsInLastXMonths(count: 3, months: 3);
                            vm.LatestResources = _resourcesProcessor.GetLatestResources(4, ResourcesSortOrder.CreatedDT, SortDirection.Descending).OrderByDescending(x => x.IsPinnedPrimaryTopic).ThenByDescending(x => x.IsPinnedSecondaryTopic).ToList();
                        }
                        else
                        {
                            var mediaTypesBookOrFilm = new ResourceMediaTypes(
                                new List<ResourceMediaType>()
                                {
                            ResourceMediaType.IsBook,
                            ResourceMediaType.IsFilm
                                });
                            vm.LatestArticles = _articlesProcessor.GetWebPageRelatedArticles(vm.WebPage.Id.Value, maxCount: 3);
                            vm.RelatedBlogPosts = _blogPostsProcessor.GetRelatedBlogPosts(vm.WebPage.Id.Value, maxCount: 5);
                            vm.AllResources = _resourcesProcessor.GetWebPageReleatedResources(vm.WebPage.Id.Value, maxCount: 10).OrderByDescending(x => x.IsPinnedPrimaryTopic).ThenByDescending(x => x.IsPinnedSecondaryTopic).ToList();
                        }
                    }
                }
            }
            else if (webPageType == EnumWebPageType.Shop)
            {
                if (!Convert.ToBoolean(WebConfigurationManager.AppSettings["ShopMaintenanceModeOn"].ToString()))
                {
                    if (vm.IsShopHomePage || vm.IsCartPage)
                    {
                        if (vm.IsShopHomePage)
                            vm.ShopProducts = _shopProductProcessor.GetAll().ToList();

                        vm.ShopOrderProducts = new List<ShopOrderProduct>();
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
                            vm.ShopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

                            if (vm.ShopOrderProducts != null && vm.ShopOrderProducts.Count > 0)
                            {
                                var membershipProducts = vm.ShopOrderProducts.Where(x => x.SKU.Substring(0, 2) == "SM").ToList();
                                vm.TotalAmount = 0;
                                vm.TotalQuantity = 0;
                                foreach (var orderProduct in vm.ShopOrderProducts)
                                {
                                    vm.TotalAmount += (int)orderProduct.Quantity * (decimal)orderProduct.PricePaidPerUnit;
                                    vm.TotalQuantity += (int)orderProduct.Quantity;
                                }
                                vm.ShopNavigationBar = new ShopNavigationBar
                                {
                                    TotalAmount = vm.TotalAmount,
                                    TotalQuantity = vm.TotalQuantity,
                                    IsOnlyMembershipProducts = false
                                };
                                if (membershipProducts != null && membershipProducts.Count > 0 && membershipProducts.Count == vm.ShopOrderProducts.Count)
                                    vm.ShopNavigationBar.IsOnlyMembershipProducts = true;
                            }
                        }
                    }
                    else if (vm.IsCheckoutPage)
                    {
                        vm.ShopOrderProducts = new List<ShopOrderProduct>();
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
                            vm.ShopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();

                            if (vm.ShopOrderProducts != null && vm.ShopOrderProducts.Count > 0)
                            {
                                var membershipProducts = vm.ShopOrderProducts.Where(x => x.SKU.Substring(0, 2) == "SM").ToList();
                                vm.TotalAmount = 0;
                                vm.TotalQuantity = 0;
                                var max = vm.ShopOrderProducts.Max(x => x.ShopProduct.ShippingPrice);
                                vm.ShippingAmount = max != null ? max : Convert.ToDecimal(0);
                                foreach (var orderProduct in vm.ShopOrderProducts)
                                {
                                    vm.TotalAmount += (int)orderProduct.Quantity * (decimal)orderProduct.PricePaidPerUnit;
                                    vm.TotalQuantity += (int)orderProduct.Quantity;
                                }
                                vm.OrderTotalAmount = vm.TotalAmount + (decimal)vm.ShippingAmount;
                                if (membershipProducts != null && membershipProducts.Count > 0 && membershipProducts.Count == vm.ShopOrderProducts.Count)
                                {
                                    vm.ShippingDetails = new ShippingDetails
                                    {
                                        EmailAddress = cart.EmailAddress
                                    };
                                    vm.IsOnlyMembershipProducts = true;
                                }
                                else
                                {
                                    vm.ShippingDetails = new ShippingDetails
                                    {
                                        EmailAddress = cart.EmailAddress,
                                        FirstName = cart.AddressNameFirst,
                                        LastName = cart.AddressNameLast,
                                        AddressLine1 = cart.AddressLine1,
                                        AddressLine2 = cart.AddressLine2,
                                        AddressLine3 = cart.AddressLine3,
                                        Town = cart.AddressTown,
                                        Postcode = cart.AddressPostcode,
                                        ContactTelephoneNumber = cart.TelephoneNumber
                                    };
                                    var country = _countryProcessor.GetCountryById((int)cart.AddressCountryID);
                                    if (country != null)
                                        vm.ShippingDetails.Country = country.CountryName;
                                    vm.IsOnlyMembershipProducts = false;
                                }
                            }
                            var configuration = Helpers.Configuration.GetConfig();
                            vm.Environment = configuration["mode"].Trim().ToLower() == "live" ? "production" : configuration["mode"].Trim().ToLower();
                            vm.ClientId = configuration["clientId"];
                            if ((WebConfigurationManager.AppSettings["CurrentEnvironment"].ToString().ToLower() == "local" || WebConfigurationManager.AppSettings["CurrentEnvironment"].ToString().ToLower() == "dev") && vm.Environment == "production")
                                vm.WarningMessage = "WARNING: PayPal is in live mode";
                            else if (WebConfigurationManager.AppSettings["CurrentEnvironment"].ToString().ToLower() == "production" && vm.Environment == "sandbox")
                                vm.WarningMessage = "WARNING: PayPal is in sandbox mode";
                        }
                    }
                    else if (vm.IsAcknowledgementPage)
                    {
                        if (string.IsNullOrEmpty(order))
                            throw new ContentNotFoundException("Incorrect IdHashCode of Shop order.");

                        ShopOrder placedOrder = null;
                        if (Request.IsAuthenticated)
                            placedOrder = _shopOrderProcessor.GetShopOrderByUser(order, Convert.ToInt32(User.Identity.GetUserId()));
                        else
                            placedOrder = _shopOrderProcessor.GetShopOrderByIdHashCode(order);
                        if (placedOrder == null)
                            throw new ContentNotFoundException("Shop order with the specified IdHashCode does not exist.");

                        vm.IdHashCode = placedOrder.IDHashCode;
                        vm.OrderPlacedOn = placedOrder.OrderPlacedOn;
                        vm.OrderStatus = placedOrder.OrderStatusName;
                        vm.TotalQuantity = (int)placedOrder.ItemsTotal;
                        vm.ShippingAmount = placedOrder.ShippingPricePaid;
                        vm.OrderTotalAmount = (decimal)placedOrder.PricePaidTotal;
                    }
                    else if (vm.IsShippingDetailsPage)
                    {
                        vm.ShopOrderProducts = new List<ShopOrderProduct>();
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
                            vm.ShopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(cart.ShopOrderId).ToList();
                            if (vm.ShopOrderProducts != null && vm.ShopOrderProducts.Count > 0)
                            {
                                vm.ShippingDetails = new ShippingDetails();
                                if (Request.IsAuthenticated)
                                {
                                    var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                                    if (user == null)
                                        throw new ContentNotFoundException("Logged in user is not found.");
                                    if (!string.IsNullOrEmpty(cart.EmailAddress))
                                        vm.ShippingDetails.EmailAddress = cart.EmailAddress;
                                    else
                                        vm.ShippingDetails.EmailAddress = user.EmailAddress;

                                    if (!string.IsNullOrEmpty(cart.AddressNameFirst))
                                        vm.ShippingDetails.FirstName = cart.AddressNameFirst;
                                    else
                                        vm.ShippingDetails.FirstName = user.UserName;

                                    if (!string.IsNullOrEmpty(cart.AddressNameLast))
                                        vm.ShippingDetails.LastName = cart.AddressNameLast;
                                    else
                                        vm.ShippingDetails.LastName = user.UserNameLast;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(cart.EmailAddress))
                                        vm.ShippingDetails.EmailAddress = cart.EmailAddress;

                                    if (!string.IsNullOrEmpty(cart.AddressNameFirst))
                                        vm.ShippingDetails.FirstName = cart.AddressNameFirst;

                                    if (!string.IsNullOrEmpty(cart.AddressNameLast))
                                        vm.ShippingDetails.LastName = cart.AddressNameLast;
                                }

                                if (!string.IsNullOrEmpty(cart.AddressLine1))
                                    vm.ShippingDetails.AddressLine1 = cart.AddressLine1;

                                if (!string.IsNullOrEmpty(cart.AddressLine2))
                                    vm.ShippingDetails.AddressLine2 = cart.AddressLine2;

                                if (!string.IsNullOrEmpty(cart.AddressLine3))
                                    vm.ShippingDetails.AddressLine3 = cart.AddressLine3;

                                if (!string.IsNullOrEmpty(cart.AddressTown))
                                    vm.ShippingDetails.Town = cart.AddressTown;

                                if (!string.IsNullOrEmpty(cart.AddressPostcode))
                                    vm.ShippingDetails.Postcode = cart.AddressPostcode;

                                if (cart.AddressCountryID.HasValue)
                                {
                                    var country = _countryProcessor.GetCountryById((int)cart.AddressCountryID);
                                    if (country != null)
                                        vm.ShippingDetails.Country = country.CountryName;
                                    else
                                        vm.ShippingDetails.Country = "United Kingdom";
                                }
                                else
                                    vm.ShippingDetails.Country = "United Kingdom";

                                if (!string.IsNullOrEmpty(cart.TelephoneNumber))
                                    vm.ShippingDetails.ContactTelephoneNumber = cart.TelephoneNumber;
                            }
                        }
                    }
                    else
                        vm.ShopProducts = _shopProductProcessor.GetWebPageRelatedShopProducts((int)vm.WebPage.Id).ToList();
                }
            }
            else if (webPageType == EnumWebPageType.Account)
            {
                if (vm.IsAccountHomePage)
                {
                    if (!Request.IsAuthenticated)
                        throw new ContentNotFoundException("User is not authenticated.");

                    var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                    if (user == null)
                        throw new ContentNotFoundException("User is not found.");

                    var LoggedInUser = User.Identity.Name;
                    //Set MembershipLevel
                    vm.MembershipLevel = user.Role.RoleName;
                    //Set MemberNumber
                    vm.MembershipNumber = user.MemberNumber.HasValue ?
                        user.MemberNumber.ToString() : "N/A";
                    //Set IsFoundingMemeber
                    vm.IsFoundingMember = user.IsFoundingMember;
                    //Set Renewable date
                    vm.ExpiryDate = user.ExpiryDate.HasValue ?
                        Convert.ToDateTime(user.ExpiryDate).ToString("dd-MMM-yyyy") : "";

                }
                else if (vm.IsShopOrdersPage)
                {
                    if (!Request.IsAuthenticated)
                        throw new ContentNotFoundException("User is not authenticated.");

                    vm.TotalShopOrders = _shopOrderProcessor.GetUserShopOrders(Convert.ToInt32(User.Identity.GetUserId())).ToList();
                    vm.ShopOrders = vm.TotalShopOrders.Where(x => x.StatusId != (int)OrderStatus.ShoppingCart).ToList();
                    vm.UserShopCart = vm.TotalShopOrders.Where(x => x.StatusId == (int)OrderStatus.ShoppingCart).FirstOrDefault();
                }
                else if (vm.IsAccountDetailsPage)
                {
                    if (!Request.IsAuthenticated)
                        throw new ContentNotFoundException("User is not authenticated.");

                    var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                    if (user == null)
                        throw new ContentNotFoundException("User is not found.");

                    //string _CountryName = string.Empty;

                    vm.AccountDetails = new AccountDetails
                    {
                        FirstName = user.UserName,
                        LastName = user.UserNameLast,
                        DescriptionPublic = user.DescriptionPublic,
                        CountryName = user.DnCountryName,
                        DisplayName = user.UserNameDisplay,
                        EmailAddress = user.EmailAddress
                    };
                }
                else if (vm.IsAccountProfilePage)
                {
                    if (!Request.IsAuthenticated)
                        throw new ContentNotFoundException("User is not authenticated.");

                    var user = _usersProcessor.GetUserById(Convert.ToInt32(User.Identity.GetUserId()));
                    if (user == null)
                        throw new ContentNotFoundException("User is not found.");

                    //Visibility Condition
                    string _Visibilty = string.Empty;
                    if (!user.ProfileIsPublic && !user.ProfileIsMembersOnly)
                    {
                        _Visibilty = "Private";
                    }
                    else if (!user.ProfileIsPublic && user.ProfileIsMembersOnly)
                    {
                        _Visibilty = "MemberOnly";
                    }
                    else if (user.ProfileIsPublic && !user.ProfileIsMembersOnly)
                    {
                        _Visibilty = "Public";
                    }

                    vm.AccountProfileDetails = new AccountProfileDetails
                    {
                        ProfileIsPublic = user.ProfileIsPublic,
                        DescriptionPublic = user.DescriptionPublic,
                        Visibilty = _Visibilty,
                        IDHashCode = user.IdHashCode,
                        Biography = user.Biography,
                        FacebookUrl = user.FacebookUrl,
                        InstagramUrl = user.InstagramUrl,
                        OtherUrl = user.OtherUrl,
                        LinkedInUrl = user.LinkedInUrl,
                        TwitterUrl = user.TwitterUrl,
                        WebsiteUrl = user.WebsiteUrl
                    };
                }
                else if (vm.IsChangePasswordPage)
                {
                    if (!Request.IsAuthenticated)
                        throw new ContentNotFoundException("User is not authenticated.");
                }
                else if (vm.IsOrderDetailsPage)
                {
                    if (!Request.IsAuthenticated)
                        throw new ContentNotFoundException("User is not authenticated.");

                    if (string.IsNullOrEmpty(order))
                        throw new ContentNotFoundException("Incorrect IdHashCode of Shop order.");

                    var placedOrder = _shopOrderProcessor.GetShopOrderByUser(order, Convert.ToInt32(User.Identity.GetUserId()));
                    if (placedOrder == null)
                        throw new ContentNotFoundException("Shop order with the specified IdHashCode does not exist.");

                    vm.IdHashCode = placedOrder.IDHashCode;
                    vm.OrderPlacedOn = placedOrder.OrderPlacedOn;
                    vm.OrderStatus = placedOrder.OrderStatusName;
                    vm.TotalQuantity = (int)placedOrder.ItemsTotal;
                    vm.ShippingAmount = placedOrder.ShippingPricePaid;
                    vm.OrderTotalAmount = (decimal)placedOrder.PricePaidTotal;
                    vm.NotesPublic = placedOrder.NotesPublic;

                    vm.ShopOrderProducts = _shopOrderProductProcessor.GetShopOrderProducts(placedOrder.ShopOrderId).ToList();

                    if (vm.ShopOrderProducts == null || vm.ShopOrderProducts.Count <= 0)
                        throw new ContentNotFoundException("Shop order products are not found.");

                    vm.TotalAmount = 0;
                    foreach (var orderProduct in vm.ShopOrderProducts)
                    {
                        vm.TotalAmount += (int)orderProduct.Quantity * (decimal)orderProduct.PricePaidPerUnit;
                    }
                }
            }
            else if (webPageType == EnumWebPageType.Events)
            {
                if (vm.IsEventsHomePage)
                {
                    var eventResources = _resourcesProcessor.GetEventResources().ToList();
                    if (eventResources != null && eventResources.Count > 0)
                    {
                        foreach (var resource in eventResources)
                        {
                            resource.IsEventCalendar = true;
                        }
                    }
                    vm.CurrentEventResources = eventResources.Where(x => x.ProductionDate.HasValue && DateTime.Compare(x.ProductionDate.Value, DateTime.Now) >= 0).OrderBy(x => x.ProductionDate).ToList();
                    vm.PastEventResources = eventResources.Where(x => x.ProductionDate == null || (x.ProductionDate.HasValue && DateTime.Compare(x.ProductionDate.Value, DateTime.Now) < 0)).OrderByDescending(x => x.ProductionDate).ToList();
                }
            }

            if (!vm.IsHomePage && !vm.IsShopHomePage && !vm.IsAccountHomePage && !vm.IsEventsHomePage)
            {
                // No need to display subpages on home page because it already has navbar links to those.
                vm.Subpages = _webPagesProcessor.GetWebPageSiblings(vm.WebPage.Id.Value).Select(x =>
                    new SubpageViewModel() { Url = x.UrlName, Title = x.WebPageTitle });
            }

            if (vm.WebPage.ParentID.HasValue)
            {
                //TODO: lowercase to page
                var parentPage = _webPagesProcessor.GetWebPageById(vm.WebPage.ParentID.Value);

                if (parentPage != null)
                {
                    vm.LinkToParentText = parentPage.WebPageName;
                    vm.LinkToParentUrlName = parentPage.UrlName;
                    vm.HasParent = true;
                }
            }

            // Always show Member navbar when looking at public WebPages.
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

            var bannerImageName = vm.WebPage.GetBannerImageName(webPageType == EnumWebPageType.Events ? AppConstants.DefaultEventBannerName : AppConstants.DefaultBannerName);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerImageName);

            var ogImagePath = vm.WebPage.GetSocialImageName(webPageType == EnumWebPageType.Events ? AppConstants.DefaultEventBannerNameSocial : AppConstants.DefaultBannerNameSocial);
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImagePath);

            var pageTitle = vm.WebPage.WebPageTitle;

            // Meta-description.
            var metaDescription = vm.WebPage.IsSubjectPage
                ? $"{pageTitle} information, news and resources on the Live Forever Club website"
                : $"{pageTitle} - from the Live Forever Club website";
            if (!string.IsNullOrWhiteSpace(vm.WebPage.MetaDescription))
                metaDescription = vm.WebPage.MetaDescription;

            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, metaDescription);

            if (webPageType == EnumWebPageType.Page || webPageType == EnumWebPageType.Account || webPageType == EnumWebPageType.Events || (!Convert.ToBoolean(WebConfigurationManager.AppSettings["ShopMaintenanceModeOn"].ToString()) && webPageType == EnumWebPageType.Shop))
            {
                ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
                {
                    AltText = vm.WebPage.IsSubjectPage ? $"{pageTitle} web page banner" : "Live Forever Club",
                    ImgTitle = pageTitle
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
            return vm;
        }

        [HttpGet]
        public ActionResult FetchAutocompleteForCountry(string term)
        {
            var list = _countryProcessor.SearchCountry(term, 10).Select(x => new AutocompleteJson(x.CountryName, x.CountryName));
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        private Registration TrimmingRegistrationForm(Registration model)
        {
            if (!string.IsNullOrEmpty(model.FirstName))
                model.FirstName = model.FirstName.Trim();
            if (!string.IsNullOrEmpty(model.LastName))
                model.LastName = model.LastName.Trim();
            if (!string.IsNullOrEmpty(model.EmailAddress))
                model.EmailAddress = model.EmailAddress.Trim();
            if (!string.IsNullOrEmpty(model.Password))
                model.Password = model.Password.Trim();
            return model;
        }

        private Login TrimmingLoginForm(Login model)
        {
            if (!string.IsNullOrEmpty(model.EmailAddress))
                model.EmailAddress = model.EmailAddress.Trim();
            if (!string.IsNullOrEmpty(model.Password))
                model.Password = model.Password.Trim();
            return model;
        }

        private bool PasswordValidate(string Password)
        {
            if ((Password.Length >= 8 && Password.Length <= 50)
                 && Password.Any(char.IsUpper)
                 && Password.Any(char.IsLower)
                 && Password.Any(char.IsDigit))
            {
                return true;
            }
            return false;
        }

        private TemplatedPostmarkMessage CreateUser(Registration model, int countryId)
        {
            var PasswordSalt = PasswordUtil.GenerateSalt();
            var HashPassword = PasswordUtil.EncodePassword(model.Password, PasswordSalt);
            // Add New User
            var bookUser = new BookUser
            {
                IsEnabled = false,
                SecurityCode = Guid.NewGuid().ToString(),
                SecurityCodeExpiry = DateTime.Now.AddHours(4),
                RegistrationMemberType = model.MembershipLevel,
                UserName = model.FirstName,
                UserNameLast = model.LastName,
                EmailAddress = model.EmailAddress,
                ProfileIsPublic = true,
                CreatedBy = 0,
                UpdatedBy = 0,
                PasswordHash = HashPassword,
                PasswordSalt = PasswordSalt,
                NewsletterSubscriber = model.NewsletterSubscriber
            };
            if (countryId > 0)
            {
                bookUser.CountryId = countryId;
            }
            var newUserId = _usersProcessor.CreateUser(bookUser);

            // Add User Role
            var userRole = new UserRole
            {
                UserId = newUserId,
                RoleId = (int)Roles.Supporter
            };
            _userRoleProcessor.CreateUserRole(userRole);

            // Add Audit Log
            var registrationMemberType = bookUser.RegistrationMemberType == (int)Roles.Longevist ? "Full" : "Supporter";
            var auditLog = new AuditLog
            {
                UserID = 0,
                IpAddressString = Request.UserHostAddress,
                EntityTypeID = (byte)AuditLogEntityType.User,
                EntityID = newUserId,
                ActionTypeID = (byte)AuditLogActionType.Create,
                NotesLog = $"RegistrationMemberType={registrationMemberType}",
                CreatedDT = DateTime.Now
            };
            _auditLogsProcessor.CreateAuditLog(auditLog);

            return new TemplatedPostmarkMessage
            {
                From = "website@liveforever.club",
                To = bookUser.EmailAddress,
                TemplateAlias = "confirm-registration-2020",
                TrackOpens = true,
                TrackLinks = LinkTrackingOptions.None,
                InlineCss = true,
                TemplateModel = new Dictionary<string, object> {
                        { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                        { "product_name", "Live Forever Club" },
                        { "name", bookUser.UserName },
                        { "action_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/auth/confirm?e={bookUser.EmailAddress}&c={bookUser.SecurityCode}" },
                        { "operating_system", Request.Browser.Platform },
                        { "browser_name", $"{Request.Browser.Browser} {Request.Browser.Version}" },
                        { "support_url", "mailto:website@liveforever.club"},
                        { "company_name", "Live Forever Club CIC" },
                        { "company_address", "33, Station Road, Cholsey, Wallingford, OX10 9PT, UK" }
                }
            };
        }

        private IEnumerable<NavigationItem> GetResourcesChildItems()
        {
            var childItems = new List<NavigationItem>
            {
                new NavigationItem()
                {
                    Text = "Blog",
                    Url = "/blog"
                },
                new NavigationItem()
                {
                    Text = "Events",
                    Url = "/events"
                },
                new NavigationItem()
                {
                    Text = "Shop",
                    Url = "/shop"
                }
            };

            if (Request.IsAuthenticated)
            {
                childItems.Add(new NavigationItem()
                {
                    Text = "My Account",
                    Url = "/account"
                });
            }
            return childItems;
        }
    }
}
