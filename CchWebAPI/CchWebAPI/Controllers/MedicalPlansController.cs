using CchWebAPI.MedicalPlan.Models;
using CchWebAPI.MedicalPlan.Data;
using CchWebAPI.MedicalPlan.Dispatchers;
using ClearCost.Net;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers {
    public class MedicalPlansController : ApiController {
        IMedicalPlanDispatcher _dispatcher;

        public MedicalPlansController(IMedicalPlanDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public MedicalPlansController() { }

        private void InitDispatcher() {
            _dispatcher = new MedicalPlanDispatcher(new MedicalPlanRepository());
        }
        [HttpGet]
        public async Task<ApiResult<MedicalPlan.Models.MedicalPlan>> GetMedicalPlanAsync(int employerId, int medicalPlanId) {
            if (string.IsNullOrWhiteSpace(EmployerConnectionString.GetConnectionString(employerId).DataWarehouse)) {
                return ApiResult<MedicalPlan.Models.MedicalPlan>.InvalidResult(string.Empty,
                    "This feature is not configured for the specified employerId.");
            }

            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);

            var result = await _dispatcher.GetMedicalPlanAsync(employer, medicalPlanId);

            return ApiResult<MedicalPlan.Models.MedicalPlan>.ValidResult(result, string.Empty);
        }
        [HttpGet]
        public async Task<ApiResult<MedicalPlan.Models.MedicalPlanAccumulation>>
            GetMedicalPlanAccumulationAsync(int employerId, int cchId, int medicalPlanId, int planYear) {

            if (string.IsNullOrWhiteSpace(EmployerConnectionString.GetConnectionString(employerId).DataWarehouse)) {
                return ApiResult<MedicalPlan.Models.MedicalPlanAccumulation>.InvalidResult(string.Empty,
                    "This feature is not configured for the specified employerId.");
            }

            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);

            var result = await _dispatcher.GetMedicalPlanAccumulationAsync(employer, cchId,
                medicalPlanId, planYear);

            return ApiResult<MedicalPlan.Models.MedicalPlanAccumulation>.ValidResult(result, string.Empty);
        }
    }
}
