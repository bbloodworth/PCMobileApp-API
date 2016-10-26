using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using ClearCost.Platform;
using ClearCost.Security;
using CchWebAPI.BenefitContributions.Models;
using CchWebAPI.BenefitContributions.Data;
using CchWebAPI.Support;

namespace CchWebAPI.BenefitContributions.Dispatchers
{
    public interface IContributionsDispatcher
    {
        //Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchid);
        Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode);

        List<BenefitContribution> Execute(int cchId, Employer employer);
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
            _repository.Initialize(employer.ConnectionString);

            var result = await _repository.GetContributionsByCchIdAsync(cchId, categoryCode);

            return result;
        }

        public List<BenefitContribution> Execute(int cchId, Employer employer)
        {
            if (cchId < 1)
            {
                throw new InvalidOperationException("Invalid Member Context");
            }
            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
            {
                throw new InvalidOperationException("Invalid Employer Context");
            }
            //string dwhConnectionString = "Data Source=KERMITDB\\MSPIGGY;Initial Catalog=CCH_DemoDWH;Trusted_Connection=true;Asynchronous Processing=True; MultipleActiveResultSets=true";

            _repository.Initialize(employer.ConnectionString);

            var result = _repository.GetContributionsByCchId(cchId);

            return result;
        }
    }
}
