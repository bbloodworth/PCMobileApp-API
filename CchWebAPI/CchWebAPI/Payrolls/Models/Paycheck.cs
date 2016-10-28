using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Payrolls.Models {
    public class Paycheck {
        public List<PayrollMetric> Earnings { get; set; }
        public List<PayrollMetric> Deductions { get; set; }
        public List<PayrollMetric> Taxes { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PrimaryWorkLocationCode { get; set; }
        public string FederalTaxElectionCode { get; set; }
        public string StateOfWorkElectionCode { get; set; }
        public string StateOfResidenceElectionCode { get; set; }
        public DateTime PayDate { get; set; }
        public int DocumentId { get; set; }
        public string DeliveryMethodCode { get; set; }

        public Paycheck() {
            Earnings = new List<PayrollMetric>();
            Deductions = new List<PayrollMetric>();
            Taxes = new List<PayrollMetric>();
        }
    }
}