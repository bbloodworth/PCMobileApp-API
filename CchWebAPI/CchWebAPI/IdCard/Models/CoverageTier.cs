using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.IdCard.Models {
    public class CoverageTier {
        public int CoverageTierKey { get; set; }
        
        public class CoverageTierConfiguration : EntityTypeConfiguration<CoverageTier> {
            public CoverageTierConfiguration() {
                ToTable("CoverageTier_d");
                HasKey(k => k.CoverageTierKey);
            }
        }
    }
}