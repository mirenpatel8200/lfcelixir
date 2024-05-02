
using Elixir.Models.Enums;
using Elixir.Models.Exceptions;
using System;
using System.Web;
using System.Web.Mvc;

namespace Elixir.Attributes
{
    //[Obsolete("Change to ExecutorAssist's version")]
    public class CustomAuthorize : AuthorizeAttribute
    {
        private readonly Roles[] allowedRoles;

        public CustomAuthorize(params Roles[] roles)
        {
            allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;

            if (httpContext.Request.IsAuthenticated)
            {
                foreach (var role in allowedRoles)
                {
                    if (httpContext.User.IsInRole(role.ToString()))
                        authorize = true;
                }
            }
            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
                throw new ContentNotFoundException("User has not access to access requested content.");
            else
                filterContext.Result = new RedirectResult("/page/Login");
        }

        //protected override void HandleUnauthorizedRequest(AuthorizationContext context)
        //{
        //    if (context.HttpContext.User.Identity.IsAuthenticated)
        //    {
        //        //custom error page
        //        context.Result = new RedirectResult("/Error/Unauthorized");
        //    }
        //    else
        //    {
        //        context.Result = new RedirectResult("/Auth/Login");
        //    }
        //}
    }
}