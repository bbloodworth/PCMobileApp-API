using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCost.Platform;
using CchWebAPI.BenefitContributions.Models;
using CchWebAPI.BenefitContributions.Data;

namespace CchWebAPI.BenefitContributions.Dispatchers
{
    public interface IContributionsDispatcher
    {
        Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode);
    }

    public class ContributionsDispatcher: IContributionsDispatcher
    {
        private IContributionsRepository _repository;

        public ContributionsDispatcher(IContributionsRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode)
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

            var result = await _repository.GetContributionsByCchIdAsync(cchId, categoryCode);

            return result;
        }
    }
}
