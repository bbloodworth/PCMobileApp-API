using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.IdCard.Models {
    public class Relationship {
        public int RelationshipKey { get; set; }
        
        public class RelationshipConfiguration : EntityTypeConfiguration<Relationship> {
            public RelationshipConfiguration() {
                ToTable("Relationship_d");
                HasKey(k => k.RelationshipKey);
            }
        }
    }
}