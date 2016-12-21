using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CchWebAPI.Filters {
    public class V2EmployerFilterAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(HttpActionContext actionContext) {
            base.OnActionExecuting(actionContext);

            // Verify that a valid employerId is passed.
            int employerId = actionContext.Request.EmployerID();
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);

            if (employer == null) {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            actionContext.ActionArguments["employer"] = employer;
        }
    }
}