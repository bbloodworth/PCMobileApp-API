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
        public async Task<ApiResult<List<BenefitContribution>>> GetContributions(int cchid, string categoryCode)
        {
            var result = await _dispatcher.GetContributionsByCchIdAsync(
                cchid,
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())),
                categoryCode);

            return ApiResult<List<BenefitContribution>>.ValidResult(result, string.Empty);
        }

        //[HttpGet]
        //public ApiResult<List<BenefitContribution>> GetContribution(int cchid)
        //{
        //    var result = _dispatcher.Execute(
        //        cchid,
        //        EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(Request.EmployerID())));

        //    return ApiResult<List<BenefitContribution>>.ValidResult(result, string.Empty);
        //}
    }
}
