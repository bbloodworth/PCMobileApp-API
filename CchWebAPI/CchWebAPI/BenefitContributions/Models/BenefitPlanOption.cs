using System;
using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContributions.Models {
    public class BenefitPlanOption {
        public int BenefitPlanOptionKey { get; set; }
        public string BenefitPlanOptionName { get; set; }
        public string PayerName { get; set; }
        public string BenefitPlanTypeCode { get; set; }
        public string BenefitTypeName { get; set;  }

        public class BenefitPlanOptionConfiguration : EntityTypeConfiguration<BenefitPlanOption> {
            public BenefitPlanOptionConfiguration() {
                ToTable("BenefitPlanOption_d");
                HasKey(k => k.BenefitPlanOptionKey);
            }
        }
    }
}