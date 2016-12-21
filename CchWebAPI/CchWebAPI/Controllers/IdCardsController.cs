using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

using CchWebAPI.IdCard.Dispatchers;
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
        public async Task<IHttpActionResult> Get() {
            var result = await _dispatcher.ExecuteAsync(Request.CCHID(),
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

            return Ok(result);
        }
    }
}
