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
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);
            var result = await _dispatcher.GetMedicalPlanAsync(employer, medicalPlanId);

            return ApiResult<MedicalPlan.Models.MedicalPlan>.ValidResult(result, string.Empty);
        }
        [HttpGet]
        public async Task<ApiResult<MedicalPlan.Models.MedicalPlanAccumulation>>
            GetMedicalPlanAccumulationAsync(int employerId, int cchId, int medicalPlanId, int planYear) {
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);
            var result = await _dispatcher.GetMedicalPlanAccumulationAsync(employer, cchId,
                medicalPlanId, planYear);

            return ApiResult<MedicalPlan.Models.MedicalPlanAccumulation>.ValidResult(result, string.Empty);
        }
    }
}
