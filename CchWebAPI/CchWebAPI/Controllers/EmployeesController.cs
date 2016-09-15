using CchWebAPI.Employees.Data;
using CchWebAPI.Employees.Dispatchers;
using ClearCost.Net;
using ClearCost.Platform;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers {
    [RoutePrefix("v2")]
    public class EmployeesController : ApiController {
        IEmployeesDispatcher _dispatcher;
        public EmployeesController(IEmployeesDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public EmployeesController() { }

        [HttpGet]
        [Route("employees/{employerId}/{cchId}")]
        public async Task<ApiResult<Employee>> GetEmployee(int employerId, int cchId) {
            var result = await _dispatcher.ExecuteAsync(cchId,
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(employerId)));

            return ApiResult<Employee>.ValidResult(result, string.Empty); ;
        }

    }
}