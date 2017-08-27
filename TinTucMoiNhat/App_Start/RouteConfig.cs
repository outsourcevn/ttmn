﻿using System;
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
                "thoi tiet",
                "du-bao-thoi-tiet",
                new { controller = "Home", action = "Weather"}
            );
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
                "fanpage post",
                "fanpost/bai-viet/{title}-{id}",
                new { controller = "Home", action = "FanpageDetail", title = UrlParameter.Optional, id = UrlParameter.Optional}
            );
            routes.MapRoute(
                "youtube",
                "youtube/{catname}-{page_id}/{pg}",
                new { controller = "Home", action = "Youtube", catname = UrlParameter.Optional, page_id = UrlParameter.Optional, pg = UrlParameter.Optional }
            );
            routes.MapRoute(
                "youtube post",
                "youtube/video/{title}-{id}",
                new { controller = "Home", action = "YoutubeDetail", title = UrlParameter.Optional, id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "thong tin doanh nghiep",
                "thong-tin-doanh-nghiep/{catname}-{page_id}/{cat_id}/{pg}",
                new { controller = "Home", action = "Company", catname = UrlParameter.Optional, page_id = UrlParameter.Optional, cat_id = UrlParameter.Optional, pg = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
