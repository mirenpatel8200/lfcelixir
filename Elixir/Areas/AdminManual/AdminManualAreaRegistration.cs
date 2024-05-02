using System.Web.Mvc;

namespace Elixir.Areas.AdminManual
{
    public class AdminManualAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "AdminManual";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "admin_manual_default",
                "admin/manual/{controller}/{action}/{id}",
                new { controller = "Content", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}