using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

using ClearCost.Data;

namespace CchWebAPI.IdCards.Data {
    public class IdCardsContext : ClearCostContext<IdCardsContext> {
        public IdCardsContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new IdCard.IdCardConfiguration());
            builder.Configurations.Add(new IdCardType.IdCardTypeConfiguration());
        }

        public DbSet<IdCard> IdCards { get; set; }
        public DbSet<IdCardType> IdCardTypes { get; set; }
    }

    public class IdCard {
        public int MemberId { get; set; }

        public int TypeId { get; set; }

        public int LocaleId { get; set; }

        public int ViewModeId { get; set; }

        public IdCardType CardType { get; set; }

        public string Detail { get; set; }

        public string Url { get; set; }

        //CCHID of person who made the request so the client can compare to the card and see if the 
        //card is for this member or for a dependent.
        public int RequestContextMemberId { get; set; }

        public class IdCardConfiguration : EntityTypeConfiguration<IdCard> {
            public IdCardConfiguration() {
                ToTable("MemberIDCard");
                HasKey(k => new { k.MemberId, k.TypeId, k.LocaleId, k.ViewModeId });
                Property(p => p.MemberId).HasColumnName("CCHID");
                Property(p => p.TypeId).HasColumnName("CardTypeId");
                Property(p => p.ViewModeId).HasColumnName("CardViewModeId");
                Property(p => p.Detail).HasColumnName("CardMemberDataText");
                Ignore(p => p.RequestContextMemberId);
                Ignore(p => p.Url);
                HasRequired(p => p.CardType).WithMany().HasForeignKey(k => k.TypeId);
            }
        }
    }

    public class IdCardType {
        public int Id { get; set; }
        public string FileName { get; set; }

        public class IdCardTypeConfiguration : EntityTypeConfiguration<IdCardType> {
            public IdCardTypeConfiguration() {
                ToTable("CardType");
                HasKey(k => k.Id);
                Property(p => p.Id).HasColumnName("CardTypeID");
                Property(p => p.FileName).HasColumnName("CardTypeFileName");
            }
        }
    }
}