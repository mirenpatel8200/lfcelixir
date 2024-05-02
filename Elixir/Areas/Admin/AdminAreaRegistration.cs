using System.Web.Mvc;
using Elixir.RouteConstraints;

namespace Elixir.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Admin";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();

            context.MapRoute(
               "shop_product_create",
               "admin/shop/product/create",
               new { controller = "Shop", action = "CreateProduct", id = UrlParameter.Optional },
               constraints: new { controller = new ExcludeController("manual") }
           );

            context.MapRoute(
              "shop_product_edit",
              "admin/shop/product/edit/{id}",
              new { controller = "Shop", action = "EditProduct", id = UrlParameter.Optional },
              constraints: new { controller = new ExcludeController("manual") }
          );

            context.MapRoute(
               "shop_category_create",
               "admin/shop/category/create",
               new { controller = "Shop", action = "CreateCategory", id = UrlParameter.Optional },
               constraints: new { controller = new ExcludeController("manual") }
           );

            context.MapRoute(
              "shop_category_edit",
              "admin/shop/category/edit/{id}",
              new { controller = "Shop", action = "EditCategory", id = UrlParameter.Optional },
              constraints: new { controller = new ExcludeController("manual") }
          );

            context.MapRoute(
              "shop_order_details",
              "admin/shop/order/{name}",
              new { controller = "Shop", action = "Order", name = UrlParameter.Optional },
              constraints: new { controller = new ExcludeController("manual") }
          );

            context.MapRoute(
             "shop_product_options_add_edit",
             "admin/shop/product/options/{shopProductId}",
             new { controller = "Shop", action = "Options", shopProductId = UrlParameter.Optional },
             constraints: new { controller = new ExcludeController("manual") }
         );

            context.MapRoute(
             "shop_options_delete",
             "admin/shop/deleteoption/{shopProductId}/{id}",
             new { controller = "Shop", action = "DeleteOption", shopProductId = UrlParameter.Optional, id = UrlParameter.Optional },
             constraints: new { controller = new ExcludeController("manual") }
         );

            context.MapRoute(
                "social_post_create",
                "admin/social/post/create",
                new { controller = "Social", action = "CreatePost", id = UrlParameter.Optional },
                constraints: new { controller = new ExcludeController("manual") }
            );

            context.MapRoute(
                "social_dashboard",
                "admin/social/{action}/{id}",
                new { controller = "Social", action = "Dashboard", id = UrlParameter.Optional },
                constraints: new { controller = new ExcludeController("manual") }
            );

            context.MapRoute(
                "admin_default",
                "admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                constraints: new { controller = new ExcludeController("manual") }
            );
        }
    }
}
