using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TinTucMoiNhat
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "chi tiet",
                "{catname}/{name}-{id}",
                new { controller = "Home", action = "Detail", catname = UrlParameter.Optional, name = UrlParameter.Optional, id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "danh muc",
                "{catname}-{cat_id}/{pg}",
                new { controller = "Home", action = "Index", catname = UrlParameter.Optional, cat_id = UrlParameter.Optional, pg = UrlParameter.Optional }
            );
            routes.MapRoute(
                "fanpage",
                "fanpage/{catname}-{page_id}/{pg}",
                new { controller = "Home", action = "Fanpage", catname = UrlParameter.Optional, page_id = UrlParameter.Optional, pg = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
