using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.IdCard.Models {
    public class MedicalPlanOption {
        public int BenefitPlanOptionKey { get; set; }
        public int RxPlanOptionKey { get; set; }
        public int DentalPlanOptionKey { get; set; }
        public int VisionPlanOptionKey { get; set; }

        public class MedicalPlanOptionConfiguration : EntityTypeConfiguration<MedicalPlanOption> {
            public MedicalPlanOptionConfiguration() {
                ToTable("MedicalPlanOption_d");
                HasKey(k => k.BenefitPlanOptionKey);
            }
        }
    }

    public class RxPlanOption {
        public int BenefitPlanOptionKey { get; set; }

        public class RxPlanOptionOptionConfiguration : EntityTypeConfiguration<RxPlanOption> {
            public RxPlanOptionOptionConfiguration() {
                ToTable("RxPlanOption_d");
                HasKey(k => k.BenefitPlanOptionKey);
            }
        }
    }

    public class DentalPlanOption {
        public int BenefitPlanOptionKey { get; set; }

        public class DentalPlanOptionConfiguration : EntityTypeConfiguration<DentalPlanOption> {
            public DentalPlanOptionConfiguration() {
                ToTable("DentalPlanOption_d ");
                HasKey(k => k.BenefitPlanOptionKey);
            }
        }
    }

    public class VisionPlanOption {
        public int BenefitPlanOptionKey { get; set; }

        public class VisionPlanOptionConfiguration : EntityTypeConfiguration<VisionPlanOption> {
            public VisionPlanOptionConfiguration() {
                ToTable("VisionPlanOption_d");
                HasKey(k => k.BenefitPlanOptionKey);
            }
        }
    }



}