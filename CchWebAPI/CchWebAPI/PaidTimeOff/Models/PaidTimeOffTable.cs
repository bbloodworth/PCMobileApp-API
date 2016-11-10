using System;
using System.Data.Entity.ModelConfiguration;


namespace CchWebAPI.PaidTimeOff.Models {
    public class PaidTimeOffTable {
        public int CCHID { get; set; }
        public int EmployerKey { get; set; }
        public int PaycheckDateKey { get; set; }
        public float AccrualRate { get; set; }
	    public int PerPeriodHoursQty { get; set; }
        public int PerPeriodAccruedQty { get; set; }
        public int PerPeriodTakenQty { get; set; }
        public int YTDAccruedQty { get; set; }
        public int YTDTakenQty { get; set; }
        public int AvailableBalanceQty { get; set; }
        public int CurrentRecordInd { get; set; }
        public string PayrollCategoryName { get; set; }
        public string ReportingCategoryCode { get; set; }
    }


}