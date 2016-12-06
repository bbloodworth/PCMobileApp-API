using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCost.Platform;
using CchWebAPI.BenefitContribution.Models;
using CchWebAPI.BenefitContribution.Data;

namespace CchWebAPI.BenefitContribution.Dispatchers
{
    public interface IBenefitContributionsDispatcher
    {
        Task<Models.BenefitContribution> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode);
    }

    public class ContributionsDispatcher: IBenefitContributionsDispatcher
    {
        private IBenefitContributionsRepository _repository;

        public ContributionsDispatcher(IBenefitContributionsRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<Models.BenefitContribution> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode)
        {
            if (cchId < 1)
            {
                throw new InvalidOperationException("Invalid Member Context");
            }
            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
            {
                throw new InvalidOperationException("Invalid Employer Context");
            }

            _repository.Initialize(employer.ConnectionString);

            var benefitContributions = await _repository.GetContributionsByCchIdAsync(cchId, categoryCode);

            var result = new Models.BenefitContribution();

            result.BenefitContributions = benefitContributions;

            // TODO actually get these values
            result.PayrollFileReceivedDate = await _repository.GetMaxPayrollDateAsync();            
            List<String> benefitNames = await _repository.GetBenefitNameAsync(cchId, categoryCode);
            result.PayerName = benefitNames[0];
            result.BenefitTypeName = benefitNames[1];
            result.PercentageElected = await _repository.GetPercentageElectedAsync(cchId, categoryCode);

            // Business Logic for PayrollMetricName/PayrollFileReceived
            return result;
        }
    }
}
