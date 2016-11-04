using CchWebAPI.Payrolls.Models;
using CchWebAPI.Payrolls.Data;
using CchWebAPI.Payrolls.Dispatchers;
using ClearCost.Net;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers {
    public class PayrollController : ApiController {
        IPayrollDispatcher _dispatcher;

        public PayrollController(IPayrollDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public PayrollController() { }

        private void InitDispatcher() {
            _dispatcher = new PayrollDispatcher(new PayrollRepository());
        }
        [HttpGet]
        public async Task<ApiResult<List<DatePaid>>> GetDatesPaidAsync(int employerId, int cchId) {
            if (String.IsNullOrWhiteSpace(DataWarehouse.GetEmployerConnectionString(employerId))) {
                return ApiResult<List<DatePaid>>.InvalidResult(string.Empty,
                    "This feature is not configured for the specified employerId.");
            }

            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);

            var result = await _dispatcher.GetDatePaidAsync(employer, cchId);

            return ApiResult<List<DatePaid>>.ValidResult(result, string.Empty);
        }

        public async Task<ApiResult<Paycheck>> GetPaycheck(int employerId, string documentId) {
            if (String.IsNullOrWhiteSpace(DataWarehouse.GetEmployerConnectionString(employerId))) {
                return ApiResult<Paycheck>.InvalidResult(string.Empty,
                    "This feature is not configured for the specified employerId.");
            }

            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == employerId);

            var result = await _dispatcher.GetPaycheckAsync(employer, documentId);

            return ApiResult<Paycheck>.ValidResult(result, string.Empty);
        }
    }
}
