using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Payrolls.Models {
    public class Paycheck {
        public List<PayrollMetric> Earnings { get; set; }
        public List<PayrollMetric> Deductions { get; set; }
        public List<PayrollMetric> Taxes { get; set; }

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

        public Paycheck() {
            InitProperties();
        }

        public Paycheck(List<PaycheckDetailsQueryResult> query) {
            InitProperties();
            Merge(query);
        }

        private void InitProperties() {
            Earnings = new List<PayrollMetric>();
            Deductions = new List<PayrollMetric>();
            Taxes = new List<PayrollMetric>();
        }

        private void Merge(List<PaycheckDetailsQueryResult> query) {
            if (query.Count > 0) {
                CchId = query[0].CchId;
                FirstName = query[0].FirstName;
                LastName = query[0].LastName;
                PrimaryWorkLocationCode = query[0].PrimaryWorkLocationCode;
                FederalTaxElectionCode = query[0].FederalTaxElectionCode;
                StateOfWorkElectionCode = query[0].StateOfWorkElectionCode;
                StateOfResidenceElectionCode = query[0].StateOfResidenceElectionCode;
                PayDate = query[0].PayDate;
                DocumentId = query[0].DocumentId;
                DeliveryMethodCode = query[0].DeliveryMethodCode;
            }
            
            Earnings = MapPayrollMetricByCategory(query, "Earning");
            Deductions = MapPayrollMetricByCategory(query, "Deduction");
            Taxes = MapPayrollMetricByCategory(query, "Tax");
        }

        private List<Models.PayrollMetric> MapPayrollMetricByCategory(List<PaycheckDetailsQueryResult> query, string category) {
            List<Models.PayrollMetric> payrollMetrics = new List<Models.PayrollMetric>();

            foreach (var payrollMetric in query.Where(p => p.PayrollCategoryName == category)) {
                payrollMetrics.Add(new Models.PayrollMetric {
                    Category = payrollMetric.PayrollCategoryName,
                    Name = payrollMetric.PayrollMetricName,
                    PreTaxIndicator = payrollMetric.PreTaxInd,
                    ContributionTypeCode = payrollMetric.ContributionTypeCode,
                    Rate = payrollMetric.PayrollMetricRate,
                    PeriodQuantity = payrollMetric.PerPeriodQty,
                    PeriodAmount = payrollMetric.PerPeriodAmt,
                    YtdQuantity = payrollMetric.YTDQty,
                    YtdAmount = payrollMetric.YTDAmt
                });
            }

            return payrollMetrics;
        }
    }
}