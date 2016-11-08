using System;
using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContribution.Models
{
    public class Payroll
    {
        public int EmployerKey { get; set; }
        public int EmployeeKey { get; set; }
        public int PayDateKey { get; set; }
        public int DeliveryMethodKey { get; set; }
        public int PayrollMetricKey { get; set; }
        public int ContributionTypeKey { get; set; }
        public int PayrollAuditKey { get; set; }
        public string DocumentID { get; set; }
        public DateTime PayDate { get; set; }
        public DateTime PayPeriodEndDate { get; set; }
        public bool CurrentPayPeriodInd { get; set; }
        public decimal PerPeriodQty { get; set; }
        public decimal PerPeriodAmt { get; set; }
        public decimal YTDQty { get; set; }
        public decimal YTDAmt { get; set; }

        public class PayrollConfiguration: EntityTypeConfiguration<Payroll>
        {
            public PayrollConfiguration()
            {
                ToTable("Payroll_f");
                HasKey(k => new {
                    k.EmployerKey,
                    k.EmployeeKey,
                    k.PayDateKey,
                    k.PayrollMetricKey,
                    k.ContributionTypeKey,
                    k.DocumentID
                });
            }
        }
    }
}
