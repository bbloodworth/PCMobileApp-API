using System;
using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.IdCard.Models
{
    public class BenefitEnrollment
    {
        public int EmployerKey { get; set; }
        public int BenefitPlanOptionKey { get; set; }
        public int EnrolledMemberKey { get; set; }
        public int SubscriberMemberKey { get; set; }
        public int PlanYearKey { get; set; }
        public int? EmployeeAnnualContributionPct { get; set; }
        public int CoverageTierKey { get; set; }
        public int BenefitEnrollmentStatusKey { get; set; }
        public int RelationshipKey { get; set; }
        public int EffectiveFromDateKey { get; set; }
        public int EffectiveToDateKey { get; set; }
        public int EnrollmentChangeDateKey { get; set; }

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
