using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using CchWebAPI.IdCards.Dispatchers;
using CchWebAPI.Services;

namespace CchWebAPI.Controllers
{
    public class IdCardsController : ApiController {
        IIdCardsDispatcher _dispatcher;
        public IdCardsController(IIdCardsDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }

        [HttpGet]
        [Route("v2/IdCards")]
        public async Task<HttpResponseMessage> Get() {
            var result = await _dispatcher.ExecuteAsync(Request.CCHID(),
                PlatformDataCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
