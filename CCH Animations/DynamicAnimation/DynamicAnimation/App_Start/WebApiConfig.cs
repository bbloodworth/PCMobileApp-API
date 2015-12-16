using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using DynamicAnimation.Handlers;

namespace DynamicAnimation
{
    public static class WebApiConfig
    {
        private static IEnumerable<DelegatingHandler> ProxyHandlers
        {
            get
            {
                return new DelegatingHandler[] {
                    new CorsHandler(), 
                    new ProxyHandler()
                };
            }
        }

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "VideoAuthenticateMemberData",
                routeTemplate: "{controller}/VideoAuth/MemberData",
                defaults: new { action = "GetAuthMemberData" },
                constraints: new { controller = "Proxy" },
                handler: HttpClientFactory.CreatePipeline(
                    new HttpControllerDispatcher(config),
                    ProxyHandlers)
                );
        }
    }
}
