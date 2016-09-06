using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace CchWebAPI.Areas.v2.IdCards.Data {
    public enum CardType {

    }

    public class IdCard {
        public int CchId { get; set; }

        public int MemberId { get; set; }

        public string FileName { get; set; }

        public int TypeId { get; set; }

        public int ViewModeId { get; set; }

        public string Detail { get; set; }

        public string Url { get; set; }
        public class IdCardConfiguration : EntityTypeConfiguration<IdCard> {
            public IdCardConfiguration() {
                Ignore(p => p.Url);
            }
        }
    }
}