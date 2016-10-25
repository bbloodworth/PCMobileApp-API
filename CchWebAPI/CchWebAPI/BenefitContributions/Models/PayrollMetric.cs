using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContributions.Models
{
    public class PayrollMetric
    {
        public int PayrollMetricKey { get; set; }
        public string PayrollMetricCode { get; set; }
        public string PayrollMetricName { get; set; }
        public string PayrollMetricDesc { get; set; }
        public string PayrollCategoryName { get; set; }
        public string ReportingCategoryCode { get; set; }
        public bool PreTaxInd { get; set; }

        public class PayrollMetricConfiguration: EntityTypeConfiguration<PayrollMetric>
        {
            public PayrollMetricConfiguration()
            {
                ToTable("PayrollMetric_d");
                HasKey(k => k.PayrollMetricKey);
            }
        }
    }
}
