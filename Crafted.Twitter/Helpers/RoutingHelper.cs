using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;

namespace Crafted.Twitter.Helpers
{
    public static class RoutingHelper
    {

        public static void RegisterRoutes()
        {
            var routes = RouteTable.Routes;
            var handler = new Twitter.Handlers.TwitterServiceHandler();
            var prefix = ConfigHelper.GetAppSettingString(ConfigKey.HandlerPath);
            if (prefix.Contains("~/")) prefix = prefix.Replace("~/", "");

            if ((from Route r in routes where r.Url == prefix + "{filename}" select r).Count<Route>() == 0)
            {
                using (routes.GetWriteLock())
                {

                    var route = new Route(prefix + "{filename}", handler)
                    {
                        // we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
                        Defaults = new RouteValueDictionary(new { controller = "TwitterHandler", action = "ProcessRequest" }),
                        Constraints = new RouteValueDictionary(new { controller = "TwitterHandler", action = "ProcessRequest" })
                    };

                    // put our routes at the beginning
                    routes.Insert(0, route);
                }
            }
        }
    }
}
