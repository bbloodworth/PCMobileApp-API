using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

using CchWebAPI.Services;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class HealthPlanSummaryController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> Get() {
            var service = new PlanInfoService();
            var summary = await service.GetHealthPlanSummaryAsync(Request.EmployerID(), Request.CCHID());

            if (!summary.AsOfDate.HasValue || !summary.YearToDateSpent.HasValue)
                service.SubmitBenefitInquiry(Request.EmployerID(), Request.CCHID());

            return Request.CreateResponse(HttpStatusCode.OK, (object)summary);
        }
    }
}
