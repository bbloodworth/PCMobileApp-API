using CchWebAPI.Employee.Dispatchers;
using CchWebAPI.Employee.Models;
using ClearCost.Net;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers {
    //[RoutePrefix("v2")]
    public class EmployeesController : ApiController {
        IEmployeeDispatcher _dispatcher;
        public EmployeesController(IEmployeeDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public EmployeesController() { }

        [HttpGet]
        //[Route("employees/{employerId}/{cchId}")]
        public async Task<ApiResult<Employee.Models.Employee>> GetEmployeeAsync(int employerId, int cchId) {
            try
            {
                var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);
                var employee = await _dispatcher.GetEmployeeAsync(employer, cchId);

                return ApiResult<Employee.Models.Employee>.ValidResult(employee, string.Empty);
            }
            catch (Exception ex)
            {
                return ApiResult<Employee.Models.Employee>.InvalidResult(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ApiResult<List<PlanMember>>> GetEmployeeBenefitPlanMembersAsync(int employerId, int cchId, int planId) {
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(employerId));
            var planMembers = await _dispatcher.GetEmployeeBenefitPlanMembersAsync(employer, cchId, planId);

            return ApiResult<List<PlanMember>>.ValidResult(planMembers, string.Empty);
        }
        [HttpGet]
        public async Task<ApiResult<List<BenefitPlan>>> GetEmployeeBenefitsEnrolled(int employerId, int cchId, int year) {
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(employerId));
            var benefits = await _dispatcher.GetEmployeeBenefitsEnrolled(employer, cchId, year);

            return ApiResult<List<BenefitPlan>>.ValidResult(benefits, string.Empty);
        }
        [HttpGet]
        public async Task<ApiResult<List<BenefitPlan>>> GetEmployeeBenefitsEligible(int employerId, int cchId) {
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(employerId));
            var benefits = await _dispatcher.GetEmployeeBenefitsEligible(employer, cchId);

            return ApiResult<List<BenefitPlan>>.ValidResult(benefits, string.Empty);
        }
    }
}