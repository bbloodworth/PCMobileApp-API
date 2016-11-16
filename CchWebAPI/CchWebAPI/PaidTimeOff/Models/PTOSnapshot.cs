using System;
using System.Data.Entity.ModelConfiguration;


namespace CchWebAPI.PaidTimeOff.Models {
    public class PTOSnapshot {
        public int EmployerKey { get; set; }
        public int EmployeeKey { get; set; }
        public int PaycheckDateKey { get; set; }
        public int PayrollMetricKey { get; set; }
        public float AccrualRate { get; set; }
        public int PerPeriodHoursQty { get; set; }
        public int PerPeriodAccruedQty { get; set; }
        public int PerPeriodTakenQty { get; set; }
        public int YTDAccruedQty { get; set; }
        public int YTDTakenQty { get; set; }
        public int AvailableBalanceQty { get; set; }
        public bool CurrentRecordInd { get; set; }

        public class PTOSnapshotConfiguration : EntityTypeConfiguration<PTOSnapshot> {
            public PTOSnapshotConfiguration() {
                ToTable("PTOSnapshot_f");
                HasKey(k => k.EmployerKey);
            }
        }
    }
}