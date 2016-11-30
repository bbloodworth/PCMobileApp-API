using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Payroll.Models {
    public class PaycheckDetails {
        public int CchId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryWorkLocationCode { get; set; }
        public string FederalTaxElectionCode { get; set; }
        public string StateOfWorkElectionCode { get; set; }
        public string StateOfResidenceElectionCode { get; set; }
        public DateTime? PayDate { get; set; }
        public string DocumentId { get; set; }
        public string DeliveryMethodCode { get; set; }
        public string PayrollCategoryName { get; set; }
        public string PayrollMetricName { get; set; }
        public string PayrollMetricDisplayName { get; set; }
        public bool PreTaxInd { get; set; }
        public string ContributionTypeCode { get; set; }
        public decimal? PayrollMetricRate { get; set; }
        public decimal? PerPeriodQty { get; set; }
        public decimal? PerPeriodAmt { get; set; }
        public decimal? YearToDateQuantity { get; set; }
        public decimal? YearToDateAmount { get; set; }
    }
}