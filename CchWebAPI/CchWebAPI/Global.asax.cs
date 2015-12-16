using System;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CchWebAPI.Formatters;

namespace CchWebAPI
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector),
                new AreaHttpControllerSelector(GlobalConfiguration.Configuration));

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            //For Debugging
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SupportedMediaTypes.Clear();

            //For Live - Force JSON
            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonpMediaTypeFormatter());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string uniqueId = DateTime.Now.Ticks.ToString();
            string logFile = string.Format("C:\\Inetpub\\Logs\\RequestLogs\\{0}.txt", uniqueId);
            Request.SaveAs(logFile, true);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Request.HttpMethod.Equals("POST"))
            {
                Request.InputStream.Position = 0;
                byte[] bytes = Request.BinaryRead(Request.TotalBytes);
                string s = Encoding.UTF8.GetString(bytes);

                if (!string.IsNullOrEmpty(s))
                {
                    int queryLength = 0;
                    if (Request.QueryString.Count > 0)
                    {
                        queryLength = Request.ServerVariables["QUERY_STRING"].Length;
                        Response.AppendToLog("&");
                    }
                    if ((queryLength + s.Length) < 4000)
                    {
                        Response.AppendToLog(s);
                    }
                    else
                    {
                        Response.AppendToLog(s.Substring(0, 4000 - queryLength));
                        Response.AppendToLog("|||...|||");
                    }
                }
            }
        }

        protected void Application_Error()
        {
            //Exception error = Server.GetLastError();
            Server.ClearError();

            Response.ContentType = "text/xml";
            Response.StatusCode = 404;

            Response.SuppressContent = true;

            Response.End();
        }
    }
}
