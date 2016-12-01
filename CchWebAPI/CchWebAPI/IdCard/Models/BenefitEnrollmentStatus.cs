using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.IdCard.Models {
    public class BenefitEnrollmentStatus {
        public int BenefitEnrollmentStatusKey { get; set; }
        
        public class BenefitEnrollmentStatusConfiguration : EntityTypeConfiguration<BenefitEnrollmentStatus> {
            public BenefitEnrollmentStatusConfiguration() {
                ToTable("BenefitEnrollmentStatus_d");
                HasKey(k => k.BenefitEnrollmentStatusKey);
            }
        }
    }
}