﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClearCost.Platform;
using CchWebAPI.BenefitContributions.Models;
using CchWebAPI.BenefitContributions.Data;

namespace CchWebAPI.BenefitContributions.Dispatchers
{
    public interface IBenefitContributionsDispatcher
    {
        Task<BenefitContribution> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode);
    }

    public class ContributionsDispatcher: IBenefitContributionsDispatcher
    {
        private IBenefitContributionsRepository _repository;

        public ContributionsDispatcher(IBenefitContributionsRepository repository)
        {
            _repository = repository;
        }
        
        public async Task<BenefitContribution> GetContributionsByCchIdAsync(int cchId, Employer employer, string categoryCode)
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

            var benefitContributions = await _repository.GetContributionsByCchIdAsync(cchId, categoryCode);

            var result = new BenefitContribution();

            result.BenefitContributions = benefitContributions;

            // TODO actually get these values
            result.PayrollFileReceivedDate = await _repository.GetMaxPayrollDate();            
            result.BenefitContributionType = await _repository.GetBenefitName(cchId, categoryCode);
            result.PercentageElected = await _repository.GetPercentageElected(cchId, categoryCode);

            // Business Logic for PayrollMetricName/PayrollFileReceived
            return result;
        }
    }
}
