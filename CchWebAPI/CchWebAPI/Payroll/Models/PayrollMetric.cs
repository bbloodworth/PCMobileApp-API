using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Payroll.Models {
    public class PayrollMetric {
        public string Category { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool PreTaxIndicator { get; set; }
        public string ContributionTypeCode { get; set; }
        public decimal? Rate { get; set; }
        public decimal? PeriodQuantity { get; set; }
        public decimal? PeriodAmount { get; set; }
        public decimal? YtdQuantity { get; set; }
        public decimal? YtdAmount { get; set; }
    }
}