using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;
using ClearCost.Data;
using Newtonsoft.Json;
using CchWebAPI.IdCard.Models;

namespace CchWebAPI.IdCard.Data {
    public class IdCardsContext : ClearCostContext<IdCardsContext> {
        public IdCardsContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new IdCard.IdCardConfiguration());
            builder.Configurations.Add(new IdCardType.IdCardTypeConfiguration());
            builder.Configurations.Add(new IdCardTypeTranslation.IdCardTypeTranslationConfiguration());
            builder.Configurations.Add(new Member.MemberConfiguration());
            builder.Configurations.Add(new BenefitEnrollment.BenefitEnrollmentConfiguration());
            builder.Configurations.Add(new PlanYear.PlanYearConfiguration());
            builder.Configurations.Add(new MedicalPlanOption.MedicalPlanOptionConfiguration());
            builder.Configurations.Add(new RxPlanOption.RxPlanOptionOptionConfiguration());
            builder.Configurations.Add(new DentalPlanOption.DentalPlanOptionConfiguration());
            builder.Configurations.Add(new VisionPlanOption.VisionPlanOptionConfiguration());
            builder.Configurations.Add(new CoverageTier.CoverageTierConfiguration());
            builder.Configurations.Add(new BenefitEnrollmentStatus.BenefitEnrollmentStatusConfiguration());
            builder.Configurations.Add(new Relationship.RelationshipConfiguration());
            builder.Configurations.Add(new Date.DateConfiguration());
            builder.Configurations.Add(new CardViewMode.CardViewModeConfiguration());
        }

        public DbSet<IdCard> IdCards { get; set; }
        public DbSet<IdCardType> IdCardTypes { get; set; }
        public DbSet<IdCardTypeTranslation> IdCardTypeTranslations { get; set; }
        public DbSet<BenefitEnrollment> BenefitEnrollments { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<PlanYear> PlanYears { get; set; }
        public DbSet<MedicalPlanOption> MedicalPlanOptions { get; set; }
        public DbSet<RxPlanOption> RxPlanOptions { get; set; }
        public DbSet<DentalPlanOption> DentalPlanOptions { get; set; }
        public DbSet<VisionPlanOption> VisionPlanOptions { get; set; }
        public DbSet<CoverageTier> CoverageTiers { get; set; }
        public DbSet<BenefitEnrollmentStatus> BenefitEnrollmentsStatuses { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Date> Dates { get; set; }
        public DbSet<CardViewMode> CardViewModes { get; set; }

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