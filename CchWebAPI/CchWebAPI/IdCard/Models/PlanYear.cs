using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.IdCard.Models {
    public class PlanYear {
        public int PlanYearKey { get; set; }
        public int? PlanYearNum { get; set; }

        public class PlanYearConfiguration : EntityTypeConfiguration<PlanYear> {
            public PlanYearConfiguration() {
                ToTable("PlanYear_d");
                HasKey(k => k.PlanYearKey);
            }
        }
    }
}