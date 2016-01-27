using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using ClearCost.IO.Log;
using DynamicAnimation.Controllers;

namespace DynamicAnimation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e) {
            var httpContext = ((MvcApplication)sender).Context;
            var controller = string.Empty;
            var action = string.Empty;
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (routeData != null) {
                if (routeData.Values["controller"] != null
                    && !string.IsNullOrEmpty(routeData.Values["controller"].ToString())) {

                    controller = routeData.Values["controller"].ToString();
                }

                if (routeData.Values["action"] != null 
                    && !string.IsNullOrEmpty(routeData.Values["action"].ToString())) {

                    action = routeData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();
            LogUtil.Log(string.Format("Exception in {0}.  Controller {1} is failing in action {2}.", httpContext.Request.UserHostName, controller, action), ex);
          

            var errorController = new ErrorController();
            var errorRouteData = new RouteData();

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            errorRouteData.Values["controller"] = "Error";
            errorRouteData.Values["action"] = "Index";
            
            ((IController)errorController).Execute(new RequestContext(new HttpContextWrapper(httpContext), errorRouteData));
        }
    }
}
