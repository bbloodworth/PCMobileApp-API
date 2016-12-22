using CchWebAPI.Payroll.Models;
using CchWebAPI.Payroll.Data;
using CchWebAPI.Payroll.Dispatchers;
using ClearCost.Net;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using CchWebAPI.Filters;

namespace CchWebAPI.Controllers {
    [V2EmployerFilter]
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
        [V2EmployeeFilter]
        public async Task<ApiResult<List<DatePaid>>> GetDatesPaidAsync(Employer employer, int cchId) {
            var result = await _dispatcher.GetDatePaidAsync(employer, cchId);

            return ApiResult<List<DatePaid>>.ValidResult(result, string.Empty);
        }
        public async Task<ApiResult<Paycheck>> GetPaycheckAsync(Employer employer, string documentId) {
            var result = await _dispatcher.GetPaycheckAsync(employer, documentId, Request.CCHID());

            return ApiResult<Paycheck>.ValidResult(result, string.Empty);
        }
    }
}
