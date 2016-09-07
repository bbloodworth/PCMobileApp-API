using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using CchWebAPI.Areas.v2.IdCards.Dispatchers;
using CchWebAPI.Services;

namespace CchWebAPI.Areas.v2.Controllers
{
    public class IdCardsController : ApiController {
        IIdCardsDispatcher _dispatcher;
        public IdCardsController(IIdCardsDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get() {
            var result = await _dispatcher.ExecuteAsync(Request.CCHID(),
                PlatformDataCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
