using CchWebAPI.Employee.Dispatchers;
using CchWebAPI.Employee.Models;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CchWebAPI.Filters {
    public class V2EmployeeFilterAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(HttpActionContext actionContext) {
            base.OnActionExecuting(actionContext);

            // The cchId in the url needs to match the cchId in the Request or one of their dependents.
            // Retrieve employee dependents
            var employer = EmployerCache.Employers.FirstOrDefault(e =>
                        e.Id == actionContext.Request.EmployerID());

            var cchId = actionContext.Request.CCHID();
            var members = new List<PlanMember>();

            var repository = new Employee.Data.V2.EmployeeRepository();
            repository.Initialize(employer.ConnectionString);


            // Is this a datawarehouse database?
            bool isDataWarehouseEnabled = false;
            Task.Run(async () => isDataWarehouseEnabled =
                await repository.IsExistingTableAsync("dbo", "BenefitEnrollment_f")).Wait();

            // Always add a record from Request.CCHID()
            members.Add(new PlanMember {
                CchId = cchId
            });

            if (isDataWarehouseEnabled) {
                // Grab list of members.
                var dispatcher = new EmployeeDispatcher(repository);
                Task.Run(async () => members.AddRange(await dispatcher.GetEmployeeDependentsAsync(
                                employer,
                                actionContext.Request.CCHID()))).Wait();
            }

            // Verify that the requested cchId is viewable by the logged in cchId.
            int requestedCchId = 0;
            bool validCchIdRequest = false;

            Int32.TryParse(actionContext.ActionArguments["cchId"].ToString(), out requestedCchId);

            foreach(var member in members) {
                if (member.CchId == requestedCchId) {
                    validCchIdRequest = true;
                    break;
                }
            }

            if (!validCchIdRequest) {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }
    }
}