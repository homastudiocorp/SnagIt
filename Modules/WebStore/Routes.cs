using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Mvc.Routes;
using System.Web.Routing;
using System.Web.Mvc;

namespace WebStore
{
    public class Routes : IRouteProvider
    {
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                             new RouteDescriptor {
                                                     Route = new Route(
                                                         "Basket/Add",
                                                         new RouteValueDictionary {
                                                                                      {"area", "WebStore"},
                                                                                      {"controller", "Basket"},
                                                                                      {"action", "Add"}
                                                                                  },
                                                         new RouteValueDictionary(),
                                                         new RouteValueDictionary {
                                                                                      {"area", "WebStore"}
                                                                                  },
                                                         new MvcRouteHandler())
                                                 },
                                new RouteDescriptor {
                                                        Route = new Route(
                                                            "Basket/Remove",
                                                            new RouteValueDictionary {
                                                                                        {"area", "WebStore"},
                                                                                        {"controller", "Basket"},
                                                                                        {"action", "Remove"}
                                                                                    },
                                                            new RouteValueDictionary(),
                                                            new RouteValueDictionary {
                                                                                        {"area", "WebStore"}
                                                                                    },
                                                            new MvcRouteHandler())
                                                    },
                                new RouteDescriptor {
                                                        Route = new Route(
                                                            "Basket/Update",
                                                            new RouteValueDictionary {
                                                                                        {"area", "WebStore"},
                                                                                        {"controller", "Basket"},
                                                                                        {"action", "Update"}
                                                                                    },
                                                            new RouteValueDictionary(),
                                                            new RouteValueDictionary {
                                                                                        {"area", "WebStore"}
                                                                                    },
                                                            new MvcRouteHandler())
                                                    },
                                new RouteDescriptor {
                                                        Route = new Route(
                                                            "Basket/ProceedToChackout",
                                                            new RouteValueDictionary {
                                                                                        {"area", "WebStore"},
                                                                                        {"controller", "Basket"},
                                                                                        {"action", "ProceedToChackout"}
                                                                                    },
                                                            new RouteValueDictionary(),
                                                            new RouteValueDictionary {
                                                                                        {"area", "WebStore"}
                                                                                    },
                                                            new MvcRouteHandler())
                                                    }
            };
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
    }
}