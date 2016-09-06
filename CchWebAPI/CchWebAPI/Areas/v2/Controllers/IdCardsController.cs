using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using CchWebAPI.Areas.v2.IdCards.Dispatchers;

namespace CchWebAPI.Areas.v2.Controllers
{
    public class IdCardsController : ApiController {

        public IdCardsController(IIdCardsDispatcher dispatcher) {

        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get() {
            return null;

        }
    }
}
