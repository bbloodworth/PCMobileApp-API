using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CchWebAPI.PaidTimeOff.Models {
    public class PaidTimeOffDetail {
        [JsonProperty("MemberId")]
        public int CchId { get; set; }
        [JsonIgnore]
        public int EmployerKey { get; set; }
        public int PaycheckDateKey { get; set; }
        public double? AccrualRate { get; set; }
	    public decimal? PerPeriodHoursQty { get; set; }
        public double? PerPeriodAccruedQty { get; set; }
        public decimal? PerPeriodTakenQty { get; set; }
        public double? YTDAccruedQty { get; set; }
        public float? YTDTakenQty { get; set; }
        public double? AvailableBalanceQty { get; set; }
        public bool CurrentRecordInd { get; set; }
        public string PayrollCategoryName { get; set; }
        public string ReportingCategoryCode { get; set; }
    }

    public class PaidTimeOffSubTable {
        public string PaidTimeOffType { get; set; }
        public double? AvailableHours { get; set; }
        public List<PaidTimeOffValues> PaidTimeOffStats { get; set; }
    }

    public class PaidTimeOffValues {
        public string PaidTimeOffType { get; set; }
        public double? YearToDate { get; set; }
        public decimal? Current { get; set; }
    }

}