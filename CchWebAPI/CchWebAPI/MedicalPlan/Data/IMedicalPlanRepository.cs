using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CchWebAPI.MedicalPlan.Data {
    public interface IMedicalPlanRepository {
        void Initialize(string connectionString);
        Task<MedicalPlanOption> GetMedicalPlanAsync(int benefitPlanOptionKey);
        Task<MedicalPlanAccumulation> GetMedicalPlanAccumulationAsync(int enrolledMemberKey,
            int benefitPlanOptionKey,
            int planYear);
    }

    public class MedicalPlanRepository : IMedicalPlanRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<MedicalPlanOption> GetMedicalPlanAsync(int benefitPlanOptionKey) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialize repository");

            var medicalPlanOption = new MedicalPlanOption();

            using (var ctx = new MedicalPlanContext(_connectionString)) {
                medicalPlanOption = await ctx.MedicalPlanOptions.FirstOrDefaultAsync(
                    p => p.BenefitPlanOptionKey.Equals(benefitPlanOptionKey));
            }

            return medicalPlanOption;
        }
        public async Task<MedicalPlanAccumulation> GetMedicalPlanAccumulationAsync(
            int enrolledMemberKey,
            int benefitPlanOptionKey,
            int planYear
            ) {

            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialize repository");

            var medicalPlanAccumulation = new MedicalPlanAccumulation();

            using (var ctx = new MedicalPlanContext(_connectionString)) {
                medicalPlanAccumulation = await ctx.MedicalPlanAccumulations
                    .FirstOrDefaultAsync(
                        p => p.EnrolledMemberKey.Equals(enrolledMemberKey)
                            && p.BenefitPlanOptionKey.Equals(benefitPlanOptionKey)
                            && p.PlanYear.PlanYearNum.Equals(planYear.ToString()));
            }

            return medicalPlanAccumulation;
        }
    }
}