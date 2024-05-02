using System;
using System.Web;
using System.Web.Mvc;
using Elixir.Models.Identity;
using Elixir.ViewModels.Enums;
using Elixir.Views;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Elixir.Models.Filters
{
    /// <summary>
    /// Filter is used to select type of navbar which depends on the role of authenticated user.
    /// If the user is authorized to view Dashboard then navbar will have Admin Dashboard links.
    /// If the user is a member then the navbar will be populated with links to WebPages with ParentId = 1;
    /// </summary>
    public class NavbarDisplayActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var userManager = filterContext.HttpContext.Request.GetOwinContext().Get<AccessUserManager>();
            var viewData = filterContext.Controller.ViewData;
            var user = filterContext.HttpContext.User;

            // User is a member, he sees only WebPages in navbar - unless updated in authentication checks below - this means authenticated members still get MemberView navbar
            viewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.MemberView);
            if (request.IsAuthenticated)
            {
                var authenticatedUser = userManager.FindById(Int32.Parse(user.Identity.GetUserId()));
                // Add authenticated user so that it is accessible in all Views.
                viewData.AddOrUpdateValue(ViewDataKeys.AuthenticatedUser, authenticatedUser);

                //if (authenticatedUser.IsDashboardAuthorized)
                //{
                    // User is authorized to view admin dashboard.
                    viewData.AddOrUpdateValue(ViewDataKeys.HeadNavigationType, HeadNavigationType.AdminDashboard);
                //}
            }

        }
    }
}