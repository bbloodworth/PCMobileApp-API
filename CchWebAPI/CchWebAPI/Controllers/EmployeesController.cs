using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers {
    [RoutePrefix("v2")]
    public class EmployeesController : ApiController {
        IEmployeesDispatcher _dispatcher;
        public IEmployeesDispatcher(IEmployeesDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }
        public EmployeesController() { }

        [HttpGet]
        [Route("employees/{employeeid}")]
        public async Task<ApiResult<Employee>> Get() {
            var result = await _dispatcher.ExecuteAsync()
        }

    }
}
