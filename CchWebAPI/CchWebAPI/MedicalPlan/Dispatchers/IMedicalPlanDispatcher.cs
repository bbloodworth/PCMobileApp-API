using CchWebAPI.MedicalPlan.Data;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CchWebAPI.MedicalPlan.Dispatchers {
    public interface IMedicalPlanDispatcher {
        Task<Models.MedicalPlan> GetMedicalPlanAsync(Employer employer, int benefitPlanOptionKey);
        Task<Models.MedicalPlanAccumulation> GetMedicalPlanAccumulationAsync(Employer employer,
            int memberId, int medicalPlanId, int planYear);
    }

    public class MedicalPlanDispatcher : IMedicalPlanDispatcher {
        private IMedicalPlanRepository _repository;
        public MedicalPlanDispatcher(IMedicalPlanRepository repository) {
            _repository = repository;
        }

        public async Task<Models.MedicalPlan> GetMedicalPlanAsync(Employer employer, int medicalPlanId) {
            if (medicalPlanId < 1)
                throw new InvalidOperationException("Invalid medical plan.");

            if (employer == null || string.IsNullOrWhiteSpace(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            _repository.Initialize(employer.ConnectionString);

            var medicalPlanOption = await _repository.GetMedicalPlanAsync(medicalPlanId);

            var result = new Models.MedicalPlan(medicalPlanOption);

            return result;
        }

        public async Task<Models.MedicalPlanAccumulation> GetMedicalPlanAccumulationAsync(
            Employer employer, 
            int memberId,
            int medicalPlanId,
            int planYear) {

            if (employer == null || string.IsNullOrWhiteSpace(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            if (memberId < 1)
                throw new InvalidOperationException("Invalid member.");

            if (medicalPlanId < 1)
                throw new InvalidOperationException("Invalid medical plan.");

            if (planYear < 1)
                throw new InvalidOperationException("Invalid year.");

            _repository.Initialize(employer.ConnectionString);

            var medicalPlanAccumulation = await _repository.GetMedicalPlanAccumulationAsync(
                memberId, medicalPlanId, planYear);

            var result = new Models.MedicalPlanAccumulation(medicalPlanAccumulation);

            return result;

        }
    }
}