using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCost.Platform;
using CchWebAPI.BenefitContribution.Models;
using CchWebAPI.BenefitContribution.Data;

namespace CchWebAPI.BenefitContribution.Dispatchers
{
    public interface IContributionsDispatcher
    {
        Task<List<Models.BenefitContribution>> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode);
    }

    public class ContributionsDispatcher: IContributionsDispatcher
    {
        private IContributionsRepository _repository;

        public ContributionsDispatcher(IContributionsRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<List<Models.BenefitContribution>> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode)
        {
            if (cchId < 1)
            {
                throw new InvalidOperationException("Invalid Member Context");
            }
            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
            {
                throw new InvalidOperationException("Invalid Employer Context");
            }

            string employerConnectionString = 
                DataWarehouse.GetEmployerConnectionString(employer.Id).Equals(string.Empty) ? 
                employer.ConnectionString : 
                DataWarehouse.GetEmployerConnectionString(employer.Id);

            _repository.Initialize(employerConnectionString);

            var result = await _repository.GetContributionsByCchIdAsync(cchId, categoryCode);

            return result;
        }
    }
}
