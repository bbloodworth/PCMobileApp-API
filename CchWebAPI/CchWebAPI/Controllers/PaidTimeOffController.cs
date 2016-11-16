using CchWebAPI.PaidTimeOff.Models;
using CchWebAPI.PaidTimeOff.Data;
using CchWebAPI.PaidTimeOff.Dispatchers;
using ClearCost.Net;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace CchWebAPI.Controllers
{
    public class PaidTimeOffController : ApiController 
    {
        IPaidTimeOffDispatcher _dispatcher;

        public PaidTimeOffController(IPaidTimeOffDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        public PaidTimeOffController() { }

        [HttpGet]
        public async Task<ApiResult<List<PaidTimeOffDetail>>> Get() {
            var employer = EmployerCache.Employers.FirstOrDefault(e => e.Id == Request.EmployerID());

            var result = await _dispatcher.GetPaidTimeOffTable(employer, Request.CCHID());

            return ApiResult<List<PaidTimeOffDetail>>.ValidResult(result, string.Empty);
        }



    }
}