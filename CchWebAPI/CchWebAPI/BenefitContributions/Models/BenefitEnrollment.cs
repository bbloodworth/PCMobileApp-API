using System;
using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContributions.Models
{
    public class BenefitEnrollment
    {
        public int EmployerKey { get; set; }
        public int BenefitPlanOptionKey { get; set; }
        public int EnrolledMemberKey { get; set; }

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
