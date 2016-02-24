using System;
using ClearCost.Data;

namespace CchWebAPI.PComm.Models {
    public class HealthPlanSummary {
        public string PlanName { get; set; }
        public int Deductible { get; set; }
        [Column(Name = "YTDSpent")]
        public int? YearToDateSpent { get; set; }
        public int? Copay { get; set; }
        public int? Coinsurance { get; set; }
        public int CoinsuranceComplement { get; set; }
        [Column(Name ="OOPMax")]
        public int OutOfPocketMax { get; set; }
        public DateTime? AsOfDate { get; set; }
    }
}
