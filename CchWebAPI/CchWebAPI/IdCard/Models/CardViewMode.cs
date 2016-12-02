using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.IdCard.Models {
    public class CardViewMode {
        public int CardViewModeId { get; set; }
        public string CardViewModeName { get; set; }

        public class CardViewModeConfiguration : EntityTypeConfiguration<CardViewMode> {
            public CardViewModeConfiguration() {
                ToTable("CardViewMode");
                HasKey(k => k.CardViewModeId);
            }
        }
    }
}