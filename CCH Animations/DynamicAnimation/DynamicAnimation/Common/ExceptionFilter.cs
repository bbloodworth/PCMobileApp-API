
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

using ClearCost.Web.Mvc;

namespace DynamicAnimation.Common {
    internal class ExceptionFilter : FilterAttribute, IExceptionFilter {

        public void OnException(ExceptionContext filterContext) {
            ExceptionFilterHandler.OnException(filterContext, "~/Views/Card/Index.cshtml");
        }

    }
}
