using System.Data.Entity.ModelConfiguration;
using System;

namespace CchWebAPI.IdCard.Models {
    public class Date {
        public int DateKey { get; set; }
        public DateTime FullDate { get; set; }

        public class DateConfiguration : EntityTypeConfiguration<Date> {
            public DateConfiguration() {
                ToTable("Date_d");
                HasKey(k => k.DateKey);
            }
        }
    }
}