using System.Web.Mvc;
using Elixir.Models.Filters;

namespace Elixir
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new NavbarDisplayActionAttribute());
        }
    }
}
