using System;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using CchWebAPI.Formatters;

using ClearCost.IO.Log;
using System.Configuration;

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

            MvcHandler.DisableMvcResponseHeader = true;
            // Enables stronger encryption protocols.  Without this code, login fails on any server with TLS 1.0 disabled.
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            string logFile = string.Format("C:\\Inetpub\\Logs\\RequestLogs\\{0}-{1}.txt", 
                DateTime.Now.Ticks.ToString(), Guid.NewGuid().ToString());
            Request.SaveAs(logFile, true);
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            
            string appSetting = ConfigurationManager.AppSettings["ClearCost.LogRequestBody"];
            bool logRequestBody = string.IsNullOrWhiteSpace(appSetting) ? false : bool.Parse(appSetting);
            
            if (Request.HttpMethod.Equals("POST") && logRequestBody)
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
            Exception error = Server.GetLastError();
            var exceptionChainId = Guid.NewGuid();

            LogUtil.Log(string.Format("Unhandled exception in WAPI.  Exception chain id is {0}",
                exceptionChainId), error, exceptionChainId);
            Server.ClearError();

            Response.ContentType = "text/xml";
            Response.StatusCode = 500;

            Response.Write(string.Format("Exception chain Id {0}", exceptionChainId));

            var innerError = error.InnerException;

            while(innerError != null) {
                Response.Write(string.Format("Exception: {0} at {1}", innerError.Message, innerError.StackTrace));
                LogUtil.Log(string.Format("Unhandled exception in WAPI.  Exception chain id is {0}",
                    exceptionChainId), innerError);
                innerError = innerError.InnerException;
            }

            Response.End();
        }
    }
}
