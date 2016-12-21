using CchWebAPI.Employee.Dispatchers;
using CchWebAPI.Employee.Models;
using CchWebAPI.Filters;
using ClearCost.Net;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers {
    //[RoutePrefix("v2")]
    [V2EmployerFilter]
    [V2EmployeeFilter]
    public class EmployeesController : ApiController {
        IEmployeeDispatcher _dispatcher;
        public EmployeesController(IEmployeeDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public EmployeesController() { }

        [HttpGet]
        //[Route("employees/{employerId}/{cchId}")]
        public async Task<IHttpActionResult> GetEmployeeAsync(Employer employer, int cchId) {
            var employee = await _dispatcher.GetEmployeeAsync(employer, cchId);
            return Ok(employee);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetEmployeeBenefitPlanMembersAsync(Employer employer, int cchId, int planId) {
            var planMembers = await _dispatcher.GetEmployeeBenefitPlanMembersAsync(employer, cchId, planId);

            return Ok(planMembers);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetEmployeeBenefitsEnrolled(Employer employer, int cchId, int year) {
            var benefits = await _dispatcher.GetEmployeeBenefitsEnrolled(employer, cchId, year);
            return Ok(benefits);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetEmployeeBenefitsEligible(Employer employer, int cchId) {
            var benefits = await _dispatcher.GetEmployeeBenefitsEligible(employer, cchId);

            return Ok(benefits);
        }
        [HttpGet]
        public async Task<IHttpActionResult> LoadEmployeeAndDependentsAccumulations(Employer employer, int cchId) {
            using (var ctx = new PlatformContext()) {
                var pEmployerId = new SqlParameter("@employerId", employer.Id);
                var pCchId = new SqlParameter("@cchId", cchId);

                await ctx.Database.ExecuteSqlCommandAsync("exec CCH_FrontEnd2.dbo.[270_CallConsoleApp] @EmployerID, @CCHID", pEmployerId, pCchId);
            }
            return Ok();
        }
    }
}