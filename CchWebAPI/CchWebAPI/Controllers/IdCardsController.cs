using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using CchWebAPI.IdCards.Dispatchers;
using CchWebAPI.IdCards.Data;
using ClearCost.Platform;
using ClearCost.Net;

namespace CchWebAPI.Controllers
{
    //[RoutePrefix("v2/IdCards")]
    public class IdCardsController : ApiController {
        IIdCardsDispatcher _dispatcher;
        public IdCardsController(IIdCardsDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }

        public IdCardsController() { }

        [HttpGet]
        //[Route("")]
        public async Task<ApiResult<List<IdCard>>> Get() {
            var result = await _dispatcher.ExecuteAsync(Request.CCHID(),
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

            return ApiResult<List<IdCard>>.ValidResult(result, string.Empty);
        }
    }
}
