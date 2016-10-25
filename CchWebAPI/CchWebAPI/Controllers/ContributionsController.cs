using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using CchWebAPI.BenefitContributions.Models;
using CchWebAPI.BenefitContributions.Dispatchers;
using ClearCost.Platform;
using ClearCost.Net;
using System.Threading.Tasks;

namespace CchWebAPI.Controllers
{
    public class ContributionsController : ApiController
    {
        IContributionsDispatcher _dispatcher;

        public ContributionsController(IContributionsDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        public ContributionsController() { }

        [HttpGet]
        //public async Task<ApiResult<List<BenefitContribution>>> GetContributions(int cchid)
        public async Task<ApiResult<List<BenefitContribution>>> GetContributions(int cchid)
        {
            int inCchId = cchid > 0 ? cchid : Request.CCHID();

            int employerId = Request.EmployerID();

            Employer employer = EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID()));

            var result = await _dispatcher.ExecuteAsync(
                inCchId,
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

            //var result = _testDispatcher.Execute(
            //    inCchId,
            //    EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

            //var result = _dispatcher.Execute(
            //    inCchId,
            //    EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

            return ApiResult<List<BenefitContribution>>.ValidResult(result, string.Empty);
        }
    }
}
