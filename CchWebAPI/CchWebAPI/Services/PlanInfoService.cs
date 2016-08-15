using System.Threading.Tasks;
using CchWebAPI.PComm.Models;
using ClearCost.Data;

namespace CchWebAPI.Services {

    public class PlanInfoService {
        public async Task<HealthPlanSummary> GetHealthPlanSummaryAsync(int employerId, int cchId) {
            return await DataGateway.QueryScalarAsync<HealthPlanSummary>(
                "p_GetHealthPlanSummary",
                new {
                    CCHID = cchId
                },
                DataSource.Employer,
                employerId
            );
        }

        public void SubmitBenefitInquiry(int employerId, int cchId) {
            DataGateway.ExecuteCommand(
                "270_CallConsoleApp", new {
                EmployerId = employerId,
                CCHID = cchId
            },
            DataSource.Employer,
            employerId);
        }
    }
}
