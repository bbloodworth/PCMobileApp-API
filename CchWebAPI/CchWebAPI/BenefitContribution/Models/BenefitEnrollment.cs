using System;
using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContribution.Models
{
    public class BenefitEnrollment
    {
        public int EmployerKey { get; set; }
        public int BenefitPlanOptionKey { get; set; }
        public int EnrolledMemberKey { get; set; }
        public int EmployeeAnnualContributionPct { get; set; }
        public int PlanYearKey { get; set; }

        public class BenefitEnrollmentConfiguration : EntityTypeConfiguration<BenefitEnrollment>
        {
            public BenefitEnrollmentConfiguration()
            {
                ToTable("BenefitEnrollment_f");
                HasKey(k => k.BenefitPlanOptionKey);
                HasKey(k => k.EnrolledMemberKey);
            }
        }
    }
}
