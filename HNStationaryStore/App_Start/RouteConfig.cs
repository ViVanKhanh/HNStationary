using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HNStationaryStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                      name: "Home",
                      url: "trang-chu",
                      defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                      namespaces: new[] { "HNStationaryStore.Controllers" }
           );
            routes.MapRoute(
            name: "TimKiem",
            url: "tim-kiem",
            defaults: new { controller = "Product", action = "TimKiem", query = UrlParameter.Optional }
        );

            routes.MapRoute(
                      name: "UserInfo",
                      url: "thong-tin-ca-nhan",
                      defaults: new { controller = "UserInfo", action = "Index", id = UrlParameter.Optional },
                      namespaces: new[] { "HNStationaryStore.Controllers" }
           );
            routes.MapRoute(
                      name: "Introduce",
                      url: "gioi-thieu",
                      defaults: new { controller = "Introduce", action = "Index", id = UrlParameter.Optional },
                      namespaces: new[] { "HNStationaryStore.Controllers" }
           );
            routes.MapRoute(
                      name: "CheckOut",
                      url: "thanh-toan",
                      defaults: new { controller = "ShoppingCard", action = "CheckOut", id = UrlParameter.Optional },
                      namespaces: new[] { "HNStationaryStore.Controllers" }
           );
            routes.MapRoute(
                      name: "ShoppingCard",
                      url: "gio-hang",
                      defaults: new { controller = "ShoppingCard", action = "Index", id = UrlParameter.Optional },
                      namespaces: new[] { "HNStationaryStore.Controllers" }
           );
            routes.MapRoute(
                      name: "Contact",
                      url: "lien-he",
                      defaults: new { controller = "Contact", action = "Index", id = UrlParameter.Optional },
                      namespaces: new[] { "HNStationaryStore.Controllers" }
           );
            routes.MapRoute(
                       name: "News",
                       url: "tin-tuc",
                       defaults: new { controller = "News", action = "Index", id = UrlParameter.Optional },
                       namespaces: new[] { "HNStationaryStore.Controllers" }
            );
            routes.MapRoute(
                name: "NewsDetail",
                url: "{alias}-n{id}",
                defaults: new { controller = "News", action = "Detail", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                        name: "Product",
                        url: "san-pham",
                        defaults: new { controller = "Product", action = "Index", id = UrlParameter.Optional },
                        namespaces: new[] { "HNStationaryStore.Controllers" }
            );
            routes.MapRoute(
                        name: "DetailProduct",
                        url: "chi-tiet/{alias}-p{id}",
                        defaults: new { controller = "Product", action = "Detail", id = UrlParameter.Optional },
                        namespaces: new[] { "HNStationaryStore.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "HNStationaryStore.Controllers" }
            );
        }
    }
}
