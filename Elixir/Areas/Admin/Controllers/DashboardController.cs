using Elixir.Controllers;
using Elixir.Models;
using Elixir.Models.Identity;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elixir.Contracts.Interfaces;
using Elixir.Models.Exceptions;
using Microsoft.AspNet.Identity.Owin;
using Elixir.Models.Enums;
using System.Net;
using Elixir.Areas.Admin.ViewModels;
using Elixir.Attributes;

namespace Elixir.Areas.Admin.Controllers
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator)]
    public class DashboardController : BaseController
    {
        private readonly IAuditLogsProcessor _auditLogsProcessor;
        private readonly IShopOrderProcessor _shopOrderProcessor;
        private readonly IUsersProcessor _usersProcessor;

        public DashboardController(
            IAuditLogsProcessor auditLogsProcessor,
            IShopOrderProcessor shopOrderProcessor,
            IUsersProcessor usersProcessor)
        {
            _auditLogsProcessor = auditLogsProcessor;
            _shopOrderProcessor = shopOrderProcessor;
            _usersProcessor = usersProcessor;
        }
        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            // TODO: this is a stub, should be reviewed.
            DashboardViewModel model = new DashboardViewModel
            {
                ShopOrders = _shopOrderProcessor.GetShopOrders(3, ShopOrdersSortOrder.UpdatedDT, SortDirection.Descending).ToList(),
                Users = _usersProcessor.GetUsers(3, UsersSortOrder.UpdatedDT, SortDirection.Descending).ToList()
            };

            AccessUserManager userManager = Request.GetOwinContext().Get<AccessUserManager>();
            BookUser authedUser = userManager.FindById(int.Parse(User.Identity.GetUserId()));
            var ipAddress = Request.UserHostAddress;
            _auditLogsProcessor.Log(new AuditLog()
            {
                ActionTypeID = (byte)AuditLogActionType.View,
                EntityID = 0,
                EntityTypeID = (byte)AuditLogEntityType.ReportDashboard,
                IpAddressString = ipAddress,
                NotesLog = "Admin dashboard",
                UserID = authedUser.Id
            });

            model.FailedLogin1Hour = _auditLogsProcessor.NoOfFailedLoginsByHours(1);
            model.FailedLogin24Hours = _auditLogsProcessor.NoOfFailedLoginsByHours(24);
            model.FailedLogin28Days = _auditLogsProcessor.NoOfFailedLoginsByDays(28);

            if (model.FailedLogin24Hours < 10)
            {
                model.FailedLoginRateText = "OK";
                model.FailedLoginRateColor = "green";
            }
            if (model.FailedLogin24Hours >= 10)
            {
                model.FailedLoginRateText = "FAIL";
                model.FailedLoginRateColor = "red";
            }
            return View(model);
        }
    }
}