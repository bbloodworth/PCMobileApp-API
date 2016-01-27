
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

using ClearCost.IO.Log;
using System.Web.Routing;

namespace DynamicAnimation.Common {
    internal class ExceptionFilter : FilterAttribute, IExceptionFilter {

        public void OnException(ExceptionContext filterContext) {
            if (filterContext.ExceptionHandled)
                return;

            var statusCode = (int)HttpStatusCode.InternalServerError;

            if (filterContext.Exception is HttpException)
                statusCode = ((HttpException)filterContext.Exception).GetHttpCode();
            else if (filterContext.Exception is UnauthorizedAccessException) 
                statusCode = (int)HttpStatusCode.Forbidden;

            var controller = string.Empty;
            var action = string.Empty;
            var routeData = RouteTable.Routes.GetRouteData(filterContext.HttpContext);

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
            
            LogUtil.Log(string.Format("Exception in {0}.  Controller {1} is failing in action {2}.", filterContext.HttpContext.Request.UserHostName, controller, action), filterContext.Exception);

            var result = new ViewResult {
                ViewName = "~/Views/Card/Index.cshtml"
            };

            result.ViewBag.StatusCode = statusCode;

            filterContext.Result = result;
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = statusCode;
        }
    }
}
