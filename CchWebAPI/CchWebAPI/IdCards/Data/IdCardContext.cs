using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;
using ClearCost.Data;
using Newtonsoft.Json;

namespace CchWebAPI.IdCards.Data {
    public class IdCardsContext : ClearCostContext<IdCardsContext> {
        public IdCardsContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new IdCard.IdCardConfiguration());
            builder.Configurations.Add(new IdCardType.IdCardTypeConfiguration());
            builder.Configurations.Add(new IdCardTypeTranslation.IdCardTypeTranslationConfiguration());
        }

        public DbSet<IdCard> IdCards { get; set; }
        public DbSet<IdCardType> IdCardTypes { get; set; }
        public DbSet<IdCardTypeTranslation> IdCardTypeTranslations { get; set; }
    }

    public class IdCard {
        public int MemberId { get; set; }

        public int TypeId { get; set; }

        public int LocaleId { get; set; }

        public int ViewModeId { get; set; }

        public IdCardType CardType { get; set; }

        [JsonIgnore]
        public string DetailText { get; set; }

        public object Detail { get; set; }

        public string Url { get; set; }

        public string SecurityToken { get; set; }

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
                Property(p => p.DetailText).HasColumnName("CardMemberDataText");
                Ignore(p => p.RequestContextMemberId);
                Ignore(p => p.Url);
                Ignore(p => p.SecurityToken);
                Ignore(p => p.Detail);
                HasRequired(p => p.CardType).WithMany().HasForeignKey(k => k.TypeId);
            }
        }
    }

    public class IdCardType {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Translation { get; set; }

        public class IdCardTypeConfiguration : EntityTypeConfiguration<IdCardType> {
            public IdCardTypeConfiguration() {
                ToTable("CardType");
                HasKey(k => k.Id);
                Property(p => p.Id).HasColumnName("CardTypeID");
                Property(p => p.FileName).HasColumnName("CardTypeFileName");
                Ignore(p => p.Translation);
            }
        }
    }

    public class IdCardTypeTranslation {
        public int Id { get; set; }

        public int LocaleId { get; set; }
        public string CardTypeName { get; set; }

        public class IdCardTypeTranslationConfiguration : EntityTypeConfiguration<IdCardTypeTranslation> {
            public IdCardTypeTranslationConfiguration() {
                ToTable("CardTypeTranslation");
                HasKey(k => new { k.Id, k.LocaleId });
                Property(p => p.Id).HasColumnName("CardTypeId");
            }
        }
    }
}