using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using Elixir.Contracts.Interfaces;
using Elixir.Helpers;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using Elixir.Models.Json;
using Elixir.Models.Utils;
using Elixir.Utils;
using Elixir.ViewModels;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PostmarkDotNet;

namespace Elixir.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseAuthController
    {
        private readonly IUsersProcessor _usersProcessor;
        private readonly IAuditLogsProcessor _auditLogsProcessor;
        private readonly IUserRoleProcessor _userRoleProcessor;
        private readonly ICountryProcessor _countryProcessor;
        private readonly ISettingsProcessor _settingsProcessor;
        public AuthController(
            IUsersProcessor usersProcessor,
            IAuditLogsProcessor auditLogsProcessor,
            IUserRoleProcessor userRoleProcessor,
            ICountryProcessor countryProcessor,
            ISettingsProcessor settingsProcessor)
        {
            _usersProcessor = usersProcessor;
            _auditLogsProcessor = auditLogsProcessor;
            _userRoleProcessor = userRoleProcessor;
            _countryProcessor = countryProcessor;
            _settingsProcessor = settingsProcessor;
        }

        // Aug-20: changed login path the /page/login
        //[HttpGet]
        //public ActionResult Login(string returnUrl)
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    LoginViewModel loginViewModel = new LoginViewModel()
        //    {
        //        ReturlUrl = returnUrl
        //    };

        //    return View(loginViewModel);
        //}

        //[HttpPost]
        //public ActionResult Login(LoginViewModel loginViewModel)
        //{
        //    //if (Request.IsAuthenticated)
        //    //{
        //    //    return RedirectToAction("Index", "Home");
        //    //}

        //    var FailedLoginReachLimitInSec = ConfigurationManager.AppSettings["FailedLoginReachLimitInSec"] != null ? int.Parse(ConfigurationManager.AppSettings["FailedLoginReachLimitInSec"]) : 60;
        //    var FailedLoginAttemptAllowed = ConfigurationManager.AppSettings["FailedLoginAttemptAllowed"] != null ? int.Parse(ConfigurationManager.AppSettings["FailedLoginAttemptAllowed"]) : 60;
        //    var ipAddress = Request.UserHostAddress;
        //    if (!ModelState.IsValid)
        //    {
        //        return View(loginViewModel);
        //    }

        //    var user = _usersProcessor.GetUserByEmail(loginViewModel.Email);

        //    int NoOfFailedLogins;
        //    if (user == null || loginViewModel.Password.Equals(user.Password) == false)
        //    {
        //        //Login failure Log
        //        _auditLogsProcessor.Log(new AuditLog()
        //        {
        //            ActionTypeID = (byte)AuditLogActionType.LoginFailure,
        //            EntityID = user == null ? 0 : user.Id,
        //            EntityTypeID = (byte)AuditLogEntityType.User,
        //            IpAddressString = ipAddress,
        //            NotesLog = user == null ? loginViewModel.Email.ToLower() : "",
        //            UserID = 0
        //        });

        //        //Security reach Limit check

        //        if (user != null)
        //        {
        //            NoOfFailedLogins = _auditLogsProcessor.NoOfFailedLoginsForUserInSeconds(FailedLoginReachLimitInSec, string.Empty, user.Id);
        //            if (NoOfFailedLogins >= FailedLoginAttemptAllowed)
        //            {
        //                if (user != null)
        //                {
        //                    _auditLogsProcessor.Log(new AuditLog()
        //                    {
        //                        ActionTypeID = (byte)AuditLogActionType.LoginFailure,
        //                        EntityID = user.Id,
        //                        EntityTypeID = (byte)AuditLogEntityType.User,
        //                        IpAddressString = ipAddress,
        //                        NotesLog = "SECURITY RATE LIMIT REACHED",
        //                        UserID = 0
        //                    });
        //                }

        //            }
        //        }

        //        ModelState.AddModelError("", "Login failed. Please make sure both your email address and password are correct. If the problem persists, please contact your manager.");
        //        return View();
        //    }

        //    NoOfFailedLogins = _auditLogsProcessor.NoOfFailedLoginsForUserInSeconds(FailedLoginReachLimitInSec, string.Empty, user.Id);
        //    if (NoOfFailedLogins >= FailedLoginAttemptAllowed)
        //    {
        //        _auditLogsProcessor.Log(new AuditLog()
        //        {
        //            ActionTypeID = (byte)AuditLogActionType.LoginRestricted,
        //            EntityID = user.Id,
        //            EntityTypeID = (byte)AuditLogEntityType.User,
        //            IpAddressString = ipAddress,
        //            NotesLog = "SECURITY RATE LIMIT REACHED",
        //            UserID = 0
        //        });
        //        ModelState.AddModelError("", "Login failed. Please make sure both your email address and password are correct. If the problem persists, please contact your manager.");
        //        return View();
        //    }

        //    if (user.IsEnabled == false)
        //    {
        //        //Login failure Log
        //        _auditLogsProcessor.Log(new AuditLog()
        //        {
        //            ActionTypeID = (byte)AuditLogActionType.LoginFailure,
        //            EntityID = user == null ? 0 : user.Id,
        //            EntityTypeID = (byte)AuditLogEntityType.User,
        //            IpAddressString = ipAddress,
        //            NotesLog = user == null ? loginViewModel.Email.ToLower() : "",
        //            UserID = 0
        //        });
        //        ModelState.AddModelError("", "Account disabled. Please contact your manager.");
        //        return View();
        //    }

        //    AccessSignInManager.SignIn(user, true, true);
        //    _usersProcessor.UpdateUserLoginTime(user.Id);

        //    //Success Log           
        //    _auditLogsProcessor.Log(new AuditLog()
        //    {
        //        ActionTypeID = (byte)AuditLogActionType.LoginSuccess,
        //        EntityID = user.Id,
        //        EntityTypeID = (byte)AuditLogEntityType.User,
        //        IpAddressString = ipAddress,
        //        NotesLog = "",
        //        UserID = user.Id
        //    });

        //    //if (!string.IsNullOrEmpty(loginViewModel.ReturlUrl))
        //    //{
        //    //    return Redirect(loginViewModel.ReturlUrl);
        //    //}

        //    // Redirect based on user roles.
        //    var userRoles = _userRoleProcessor.GetUserRoles(user.Id).ToList();
        //    if (userRoles.Count > 1)
        //    {
        //        var lowestDisplayOrder = userRoles.Min(x => x.Role.DisplayOrder);
        //        var loginRedirectUrl = userRoles.Where(x => x.Role.DisplayOrder == lowestDisplayOrder).Select(x => x.Role.LoginRedirectUrl).FirstOrDefault();
        //        return Redirect(loginRedirectUrl);
        //    }
        //    else
        //        return Redirect(userRoles[0].Role.LoginRedirectUrl);
        //}


        //public ActionResult GenerateHashPasswordForOldAccounts(string Pass)
        //{
        //    var salt = PasswordUtil.GenerateSalt();
        //    var HashPassword = PasswordUtil.EncodePassword(Pass, salt);
        //    return Redirect("/?Salt=" + salt + "&Password=" + HashPassword);
        //}
        public ActionResult Logout()
        {
            var ipAddress = Request.UserHostAddress;
            var authedUserId = int.Parse(User.Identity.GetUserId());

            AuthenticationManager.SignOut("ApplicationCookie");

            //Logout Log
            _auditLogsProcessor.Log(new AuditLog()
            {
                ActionTypeID = (byte)AuditLogActionType.Logout,
                EntityID = authedUserId,
                EntityTypeID = (byte)AuditLogEntityType.User,
                IpAddressString = ipAddress,
                NotesLog = "",
                UserID = authedUserId
            });
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        // GET: /auth/confirm?e={e}&c={c}
        public async Task<ActionResult> Confirm(string e, string c)
        {
            if (string.IsNullOrEmpty(e))
                throw new ContentNotFoundException("Email has to be specified.");

            if (string.IsNullOrEmpty(c))
                throw new ContentNotFoundException("Security code has to be specified.");

            var user = _usersProcessor.GetUserByEmail(e.Trim());
            var isValid = true;
            var failedMsg = string.Empty;
            if (user == null)
            {
                // Add audit log (Email is not found)
                isValid = false;
                failedMsg = "Email is not found";
            }
            else if (user.IsEnabled)
            {
                // Add audit log (Already registered)
                isValid = false;
                failedMsg = "Already registered";
            }
            else if (DateTime.Compare(user.SecurityCodeExpiry.Value, DateTime.Now) < 0)
            {
                // Add audit log (Security code expired)
                isValid = false;
                failedMsg = "Security code expired";
            }
            else if (user.SecurityCode != c.Trim())
            {
                // Add audit log (Security code invalid)
                isValid = false;
                failedMsg = "Security code invalid";
            }

            if (!isValid)
            {
                _auditLogsProcessor.CreateAuditLog(new AuditLog
                {
                    UserID = 0,
                    IpAddressString = Request.UserHostAddress,
                    EntityTypeID = (byte)AuditLogEntityType.User,
                    EntityID = user != null ? user.Id : 0,
                    ActionTypeID = (byte)AuditLogActionType.UpdateFailure,
                    NotesLog = $"Registration confirmation: {failedMsg} e= {e} & c= {c}",
                    CreatedDT = DateTime.Now
                });
                // Always show Member navbar when looking at public WebPages.
                ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

                ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

                var bannerImageName = AppConstants.DefaultBannerName;
                ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerImageName);

                var ogImagePath = AppConstants.DefaultBannerNameSocial;
                ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImagePath);

                var pageTitle = "Account registration failed";

                // Meta-description.
                var metaDescription = "Account registration failed";
                ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, metaDescription);
                ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
                {
                    AltText = "Live Forever Club",
                    ImgTitle = pageTitle
                });

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
                return View();
            }

            // Update user record
            user.IsEnabled = true;
            user.LastLogin = DateTime.Now;
            user.UpdatedBy = user.Id;
            var users = _usersProcessor.GetAllUsers(null, UsersSortOrder.UserID, SortDirection.Descending).ToList();
            // Issue member number
            if (users != null && users.Count > 0)
            {
                var maxMemberNumber = users.Max(x => x.MemberNumber);
                if (maxMemberNumber.HasValue)
                    user.MemberNumber = maxMemberNumber.Value + 1;
                else
                    user.MemberNumber = 1;
            }
            _usersProcessor.UpdateUser(user);
            AccessSignInManager.SignIn(user, true, true);

            // Add audit log (Registration confirmed)
            _auditLogsProcessor.CreateAuditLog(new AuditLog
            {
                UserID = user.Id,
                IpAddressString = Request.UserHostAddress,
                EntityTypeID = (byte)AuditLogEntityType.User,
                EntityID = user.Id,
                ActionTypeID = (byte)AuditLogActionType.Update,
                NotesLog = "Registration confirmed",
                CreatedDT = DateTime.Now
            });
            TemplatedPostmarkMessage message;
            if (user.RegistrationMemberType == (int)Roles.Longevist)
            {
                message = new TemplatedPostmarkMessage
                {
                    From = "website@liveforever.club",
                    To = user.EmailAddress,
                    TemplateAlias = "welcome-member",
                    TrackOpens = true,
                    TrackLinks = LinkTrackingOptions.None,
                    InlineCss = true,
                    TemplateModel = new Dictionary<string, object> {
                                { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                                { "product_name", "Live Forever Club" },
                                { "name", user.UserName },
                                { "action_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/page/home" },
                                { "login_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/page/login" },
                                { "account_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/account" },
                                { "username", user.EmailAddress },
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
                return Redirect("/shop/product/membership-new");
            }
            else
            {
                message = new TemplatedPostmarkMessage
                {
                    From = "website@liveforever.club",
                    To = user.EmailAddress,
                    TemplateAlias = "welcome-supporter",
                    TrackOpens = true,
                    TrackLinks = LinkTrackingOptions.None,
                    InlineCss = true,
                    TemplateModel = new Dictionary<string, object> {
                                { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                                { "product_name", "Live Forever Club" },
                                { "name", user.UserName },
                                { "action_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/page/home" },
                                { "login_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/page/login" },
                                { "username", user.EmailAddress },
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
        }

        // GET: /auth/forgotten
        [HttpGet]
        public ActionResult Forgotten()
        {
            LoadForgotten();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Forgotten(ForgottenPassword model)
        {
            TempData.Clear();
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(model.EmailAddress))
                    model.EmailAddress = model.EmailAddress.Trim();

                var user = _usersProcessor.GetUserByEmail(model.EmailAddress);
                TemplatedPostmarkMessage message;
                if (user != null && user.IsEnabled)
                {
                    // Add Audit Log
                    var auditLog = new AuditLog
                    {
                        UserID = 0,
                        IpAddressString = Request.UserHostAddress,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        EntityID = user.Id,
                        ActionTypeID = (byte)AuditLogActionType.Update,
                        NotesLog = "Password reset request",
                        CreatedDT = DateTime.Now
                    };
                    _auditLogsProcessor.CreateAuditLog(auditLog);

                    // Update User
                    user.SecurityCode = Guid.NewGuid().ToString();
                    user.SecurityCodeExpiry = DateTime.Now.AddHours(4);
                    user.UpdatedBy = user.Id;
                    _usersProcessor.UpdateUser(user);

                    // Reset password email
                    message = new TemplatedPostmarkMessage
                    {
                        From = "website@liveforever.club",
                        To = model.EmailAddress,
                        TemplateAlias = "password-reset",
                        TrackOpens = true,
                        TrackLinks = LinkTrackingOptions.None,
                        InlineCss = true,
                        TemplateModel = new Dictionary<string, object> {
                                { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                                { "product_name", "Live Forever Club" },
                                { "name", user.UserName },
                                { "action_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/auth/passwordreset?e={model.EmailAddress}&c={user.SecurityCode}" },
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
                    // Add Audit Log
                    var auditLog = new AuditLog
                    {
                        UserID = 0,
                        IpAddressString = Request.UserHostAddress,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        ActionTypeID = (byte)AuditLogActionType.UpdateFailure,
                        NotesLog = "Password reset request – unknown email address",
                        CreatedDT = DateTime.Now
                    };
                    _auditLogsProcessor.CreateAuditLog(auditLog);

                    // Reset password unknown email
                    message = new TemplatedPostmarkMessage
                    {
                        From = "website@liveforever.club",
                        To = model.EmailAddress,
                        TemplateAlias = "password-reset-unknown",
                        TrackOpens = true,
                        TrackLinks = LinkTrackingOptions.None,
                        InlineCss = true,
                        TemplateModel = new Dictionary<string, object> {
                                { "product_url", Request.Url.GetLeftPart(UriPartial.Authority) },
                                { "product_name", "Live Forever Club" },
                                { "action_url", $"{Request.Url.GetLeftPart(UriPartial.Authority)}/auth/forgotten" },
                                { "operating_system", Request.Browser.Platform },
                                { "browser_name", $"{Request.Browser.Browser} {Request.Browser.Version}" },
                                { "support_url", "mailto:website@liveforever.club"},
                                { "company_name", "Live Forever Club CIC" },
                                { "company_address", "33, Station Road, Cholsey, Wallingford, OX10 9PT, UK" }
                        }
                    };
                }
                var result = await EmailService.SendEmail(message);
                if (!result)
                {
                    // Handle scenario when email is not sent
                }
                TempData["ForgottenPasswordSuccessMsg"] = "A password reset link will be sent to you if the email address is recognised.";
                return RedirectToAction("Forgotten");
            }
            LoadForgotten();
            return View(model);
        }

        // GET: /auth/passwordreset?e={e}&c={c}
        public ActionResult PasswordReset(string e, string c)
        {
            if (TempData["isRedirected"] == null)
            {
                if (string.IsNullOrEmpty(e))
                    throw new ContentNotFoundException("Email has to be specified.");

                if (string.IsNullOrEmpty(c))
                    throw new ContentNotFoundException("Security code has to be specified.");

                TempData.Clear();
            }

            LoadResetPassword();
            if (TempData["isRedirected"] != null && Convert.ToBoolean(TempData["isRedirected"].ToString()))
            {
                TempData.Remove("isRedirected");
                return View();
            }

            var user = _usersProcessor.GetUserByEmail(e.Trim());
            var isValid = true;
            var failedMsg = string.Empty;
            if (user == null)
            {
                // Add audit log (Email is not found)
                isValid = false;
                failedMsg = "Email is not found";
            }
            else if (!user.IsEnabled)
            {
                // Add audit log (Account is not enabled)
                isValid = false;
                failedMsg = "Account is not enabled";
            }
            else if (user.SecurityCodeExpiry != null && user.SecurityCodeExpiry.HasValue && DateTime.Compare(user.SecurityCodeExpiry.Value, DateTime.Now) < 0)
            {
                // Add audit log (Security code expired)
                isValid = false;
                failedMsg = "Security code expired";
            }
            else if (user.SecurityCode != c.Trim())
            {
                // Add audit log (Security code invalid)
                isValid = false;
                failedMsg = "Security code invalid";
            }

            if (!isValid)
            {
                // Verification failed, Add audit log
                _auditLogsProcessor.CreateAuditLog(new AuditLog
                {
                    UserID = 0,
                    IpAddressString = Request.UserHostAddress,
                    EntityTypeID = (byte)AuditLogEntityType.User,
                    EntityID = user != null ? user.Id : 0,
                    ActionTypeID = (byte)AuditLogActionType.UpdateFailure,
                    NotesLog = $"Password reset confirmation: {failedMsg} e= {e} & c= {c}",
                    CreatedDT = DateTime.Now
                });
                TempData["ResetPassVerificationFailedMsg"] = "Password reset details have not been recognised.";
                return View();
            }
            else
            {
                // Verification successful, View reset passoword form
                var model = new ResetPassword
                {
                    EmailAddress = user.EmailAddress,
                    UrlEmailAddress = user.EmailAddress,
                    UrlSecurityCode = user.SecurityCode
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult PasswordReset(ResetPassword model)
        {
            TempData.Clear();
            model = TrimmingResetPasswordForm(model);
            if (!string.IsNullOrEmpty(model.NewPassword) && !PasswordValidate(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "Password must contains 8-50 characters, one Uppercase, one Lowercase and one Digit!");
            }
            if (ModelState.IsValid)
            {
                var user = _usersProcessor.GetUserByEmail(model.UrlEmailAddress);
                var isValid = true;
                var failedMsg = string.Empty;
                if (user == null)
                {
                    // Add audit log (Email is not found)
                    isValid = false;
                    failedMsg = "Email is not found";
                }
                else if (!user.IsEnabled)
                {
                    // Add audit log (Account is not enabled)
                    isValid = false;
                    failedMsg = "Account is not enabled";
                }
                else if (DateTime.Compare(user.SecurityCodeExpiry.Value, DateTime.Now) < 0)
                {
                    // Add audit log (Security code expired)
                    isValid = false;
                    failedMsg = "Security code expired";
                }
                else if (user.SecurityCode != model.UrlSecurityCode.Trim())
                {
                    // Add audit log (Security code invalid)
                    isValid = false;
                    failedMsg = "Security code invalid";
                }

                if (!isValid)
                {
                    // Verification failed, Add audit log
                    _auditLogsProcessor.CreateAuditLog(new AuditLog
                    {
                        UserID = 0,
                        IpAddressString = Request.UserHostAddress,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        EntityID = user != null ? user.Id : 0,
                        ActionTypeID = (byte)AuditLogActionType.UpdateFailure,
                        NotesLog = $"Password reset confirmation: {failedMsg} e= {model.UrlEmailAddress} & c= {model.UrlSecurityCode}",
                        CreatedDT = DateTime.Now
                    });
                    TempData["ResetPassVerificationFailedMsg"] = "Password reset details have not been recognised.";
                }
                else
                {
                    // Verification successful, Update user record
                    var HashSalt = PasswordUtil.GenerateSalt();
                    var HashPassword = PasswordUtil.EncodePassword(model.NewPassword, HashSalt);
                    user.PasswordSalt = HashSalt;
                    user.PasswordHash = HashPassword;
                    user.SecurityCodeExpiry = DateTime.Now;
                    user.PasswordUpdatedOn = DateTime.Now;
                    user.UpdatedBy = user.Id;
                    user.LastLogin = DateTime.Now.ToLocalTime();
                    _usersProcessor.UpdateUser(user);
                    AccessSignInManager.SignIn(user, true, true);

                    // Add audit log (Password reset confirmed)
                    _auditLogsProcessor.CreateAuditLog(new AuditLog
                    {
                        UserID = user.Id,
                        IpAddressString = Request.UserHostAddress,
                        EntityTypeID = (byte)AuditLogEntityType.User,
                        EntityID = user.Id,
                        ActionTypeID = (byte)AuditLogActionType.Update,
                        NotesLog = "Password reset confirmed",
                        CreatedDT = DateTime.Now
                    });
                    TempData["ResetPassVerificationSuccessMsg"] = "Your password has been updated and you are logged in successfully.";
                }
                TempData["isRedirected"] = true;
                return RedirectToAction("PasswordReset", new { e = model.UrlEmailAddress, c = model.UrlSecurityCode });
            }
            LoadResetPassword();
            return View(model);
        }

        private ResetPassword TrimmingResetPasswordForm(ResetPassword model)
        {
            if (!string.IsNullOrEmpty(model.EmailAddress))
                model.EmailAddress = model.EmailAddress.Trim();
            if (!string.IsNullOrEmpty(model.NewPassword))
                model.NewPassword = model.NewPassword.Trim();
            if (!string.IsNullOrEmpty(model.UrlEmailAddress))
                model.UrlEmailAddress = model.UrlEmailAddress.Trim();
            if (!string.IsNullOrEmpty(model.UrlSecurityCode))
                model.UrlSecurityCode = model.UrlSecurityCode.Trim();
            return model;
        }

        private void LoadForgotten()
        {
            // Always show Member navbar when looking at public WebPages.
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

            var bannerImageName = AppConstants.DefaultBannerName;
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerImageName);

            var ogImagePath = AppConstants.DefaultBannerNameSocial;
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImagePath);

            var pageTitle = "Forgotten password";

            // Meta-description.
            var metaDescription = "Forgotten password";
            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, metaDescription);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
            {
                AltText = "Live Forever Club",
                ImgTitle = pageTitle
            });

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
        }

        private void LoadResetPassword()
        {
            // Always show Member navbar when looking at public WebPages.
            ViewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);

            ViewData.AddOrUpdateValue(ViewDataKeys.EnableImageBanner, true);

            var bannerImageName = AppConstants.DefaultBannerName;
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImagePath, AppConstants.FileSystem.WebPageBannerImageFolderServerPath + bannerImageName);

            var ogImagePath = AppConstants.DefaultBannerNameSocial;
            ViewData.AddOrUpdateValue(ViewDataKeys.OgImageName, "/" + AppConstants.FileSystem.PublicImagesFolderName + "/" + ogImagePath);

            var pageTitle = "Password Reset";

            // Meta-description.
            var metaDescription = "Password Reset";
            ViewData.AddOrUpdateValue(ViewDataKeys.MetaDescription, metaDescription);
            ViewData.AddOrUpdateValue(ViewDataKeys.BannerImageMeta, new BannerImageMetaVm()
            {
                AltText = "Live Forever Club",
                ImgTitle = pageTitle
            });

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
        }

        [HttpPost]
        public ActionResult CheckForPasswordValidity(string Password)
        {
            bool ifPasswordValid = false;
            try
            {
                ifPasswordValid = PasswordValidate(Password);
                return Json(!ifPasswordValid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public bool PasswordValidate(string Password)
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
    }
}