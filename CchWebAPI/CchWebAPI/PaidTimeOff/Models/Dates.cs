using System;
using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.PaidTimeOff.Models
{
    public class Dates
    {
        public int DateKey { get; set; }
        public DateTime FullDate { get; set; }

        public class DatesConfiguration: EntityTypeConfiguration<Dates>
        {
            public DatesConfiguration()
            {
                ToTable("Date_d");
                HasKey(k => k.DateKey);
            }
        }
    }
}
