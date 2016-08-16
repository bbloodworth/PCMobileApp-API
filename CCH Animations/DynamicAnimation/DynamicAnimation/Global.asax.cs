using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using ClearCost.IO.Log;
using ClearCost.Web.Mvc;
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

            MvcHandler.DisableMvcResponseHeader = true;
            // Enables stronger encryption protocols.  Without this code, login fails on any server with TLS 1.0 disabled.
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
        }

        protected void Application_Error(object sender, EventArgs e) {
            ApplicationErrorHandler.OnError<ErrorController>(((MvcApplication)sender).Context, Server.GetLastError(),
                "Error", "Index");
        }
    }
}
