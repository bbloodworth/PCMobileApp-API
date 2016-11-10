using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContribution.Models {
    public class Member {
        public int MemberKey { get; set; }
        public int CCHID { get; set; }

        public class MemberConfiguration : EntityTypeConfiguration<Member> {
            public MemberConfiguration() {
                ToTable("Member_d");
                HasKey(k => k.MemberKey);
            }
        }
    }
}