using Elixir.Handlers;
using System.Security.Policy;
using System.Web.Mvc;
using System.Web.Routing;

namespace Elixir
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapMvcAttributeRoutes();

            //routes.MapRoute(
            //    name: "Admin",
            //    url: "admin/{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action="Index", id = UrlParameter.Optional });

            //root was here

            // TODO: AY - use areas for Admin/Page, remove hierarchical routing.

            //  var customImagesRoute = new Route("members-only/{imageName}", new CustomRouteHandler("LFM"));
            // routes.Add(customImagesRoute);


            routes.MapRoute(
                name: "GoToLink",
                url: "go/{shortCode}",
                defaults: new { controller = "Go", action = "Index" });

            routes.MapRoute(
                name: "WebsiteWebPageView",
                url: "page/{name}",
                defaults: new { controller = "WebPageVisual", action = "Index", name = UrlParameter.Optional });
            routes.MapRoute(
                name: "MembersOnly",
                url: "membersonly/{name}",
                defaults: new { controller = "MembersOnly", action = "Index", name = UrlParameter.Optional });
            routes.MapRoute(
               name: "WebsiteShopAddToCart",
               url: "shop/addtocart/{sku}/{option}",
               defaults: new { controller = "PublicShop", action = "AddToCart", sku = UrlParameter.Optional, option = UrlParameter.Optional });
            routes.MapRoute(
               name: "WebsiteShopPlaceOrder",
               url: "shop/placeorder",
               defaults: new { controller = "PublicShop", action = "PlaceOrder" });
            routes.MapRoute(
               name: "WebsiteShopPaymentWithPayPal",
               url: "shop/paymentwithpaypal",
               defaults: new { controller = "PublicShop", action = "PaymentWithPayPal" });
            routes.MapRoute(
              name: "WebsiteWebShopProductDelete",
              url: "shop/product/delete/{sku}",
              defaults: new { controller = "WebPageVisual", action = "DeleteItem", sku = UrlParameter.Optional });
            routes.MapRoute(
              name: "WebsiteWebShopProductIncrement",
              url: "shop/product/increment/{sku}",
              defaults: new { controller = "WebPageVisual", action = "IncrementItem", sku = UrlParameter.Optional });
            routes.MapRoute(
             name: "WebsiteWebShopProductDecrement",
             url: "shop/product/decrement/{sku}",
             defaults: new { controller = "WebPageVisual", action = "DecrementItem", sku = UrlParameter.Optional });
            routes.MapRoute(
              name: "WebsiteShopProduct",
              url: "shop/product/{name}",
              defaults: new { controller = "PublicShop", action = "Product", name = UrlParameter.Optional });
            routes.MapRoute(
                name: "WebsiteWebShopView",
                url: "shop/{name}/{order}",
                defaults: new { controller = "WebPageVisual", action = "Shop", name = UrlParameter.Optional, order = UrlParameter.Optional });
            routes.MapRoute(
               name: "WebsiteWebAccountView",
               url: "account/{name}/{order}",
               defaults: new { controller = "WebPageVisual", action = "Account", name = UrlParameter.Optional, order = UrlParameter.Optional });
            routes.MapRoute(
               name: "WebsiteWebEventView",
               url: "events/{name}",
               defaults: new { controller = "WebPageVisual", action = "Events", name = UrlParameter.Optional });

            routes.MapRoute(
                name: "WebsiteBlogView",
                url: "blog/{name}",
                defaults: new { controller = "PublicBlog", action = "ViewBlog" });
            routes.MapRoute(
                name: "WebsiteBlogsByYear",
                url: "blog/year/{year}",
                defaults: new { controller = "PublicBlog", action = "ViewByYear" });
            routes.MapRoute(
                name: "WebsiteBlogsHome",
                url: "blog",
                defaults: new { controller = "PublicBlog", action = "Index" });
            routes.MapRoute(
                name: "WebsiteResources",
                url: "resources/{name}",
                defaults: new { controller = "Resources", action = "Index", name = UrlParameter.Optional });
            routes.MapRoute(
               name: "WebsiteMembers",
               url: "members/{name}",
               defaults: new { controller = "Members", action = "Index", name = UrlParameter.Optional });
            //routes.MapRoute(
            //   name: "File",
            //   url: "{controller}/{action}/{name}",
            //   defaults: new { controller = "File", action = "Index", name = UrlParameter.Optional });

            //routes.MapRoute(
            //    name: "Admin_Manual_Area",
            //    url: "admin/manual/{controller}/{action}/{id}",
            //    defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional });

            //routes.MapRoute(
            //    name: "Admin",
            //    url: "admin/{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

#if !DEBUG
            routes.MapRoute(
                name: "CatchAllUrls",
                url: "{*url}",
                defaults: new { controller = "Error", action = "CatchAllUrls" });
#endif

            //routes.MapRoute(
            //   name: "Root",
            //   url: "",
            //   defaults: new { controller = "Home", action = "Index" });
        }
    }
}
