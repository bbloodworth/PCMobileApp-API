using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Payroll.Models {
    public class Paycheck {
        public List<PayrollMetric> Earnings { get; set; }
        public List<PayrollMetric> Deductions { get; set; }
        public List<PayrollMetric> Taxes { get; set; }
        public List<PayrollMetric> PaidTimeOff { get; set; }

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

        public Paycheck(List<PaycheckDetails> list) {
            InitProperties();
            Merge(list);
        }

        private void InitProperties() {
            Earnings = new List<PayrollMetric>();
            Deductions = new List<PayrollMetric>();
            Taxes = new List<PayrollMetric>();
            PaidTimeOff = new List<PayrollMetric>();
        }

        private void Merge(List<PaycheckDetails> list) {
            if (list == null || list.Count == 0) {
                return;
            }

            CchId = list[0].CchId;
            FirstName = list[0].FirstName;
            LastName = list[0].LastName;
            PrimaryWorkLocationCode = list[0].PrimaryWorkLocationCode;
            FederalTaxElectionCode = list[0].FederalTaxElectionCode;
            StateOfWorkElectionCode = list[0].StateOfWorkElectionCode;
            StateOfResidenceElectionCode = list[0].StateOfResidenceElectionCode;
            PayDate = list[0].PayDate;
            DocumentId = list[0].DocumentId;
            DeliveryMethodCode = list[0].DeliveryMethodCode;

            Earnings = MapPayrollMetricByCategory(list, "Earning");
            Deductions = MapPayrollMetricByCategory(list, "Deduction");
            Taxes = MapPayrollMetricByCategory(list, "Tax");
            PaidTimeOff = MapPayrollMetricByCategory(list, "PTO Balance");
        }

        private List<Models.PayrollMetric> MapPayrollMetricByCategory(List<PaycheckDetails> list, string category) {
            var payrollMetrics = new List<Models.PayrollMetric>();

            list.Where(p => p.PayrollCategoryName == category).ToList().ForEach(p =>
                payrollMetrics.Add(new Models.PayrollMetric {
                    Category = p.PayrollCategoryName,
                    Name = p.PayrollMetricName,
                    DisplayName = p.PayrollMetricDisplayName,
                    PreTaxIndicator = p.PreTaxInd,
                    ContributionTypeCode = p.ContributionTypeCode,
                    Rate = p.PayrollMetricRate,
                    PeriodQuantity = p.PerPeriodQty,
                    PeriodAmount = p.PerPeriodAmt,
                    YtdQuantity = p.YearToDateQuantity,
                    YtdAmount = p.YearToDateAmount
                })
            );

            return payrollMetrics;
        }
    }
}