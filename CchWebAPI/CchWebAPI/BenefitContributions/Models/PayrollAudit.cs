using System;
using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContributions.Models
{
    public class PayrollAudit
    {
        public int PayrollAuditKey { get; set; }
        public int ETLVersionID { get; set; }
        public DateTime DWCreateDate { get; set; }

        public class PayrollAuditConfiguration: EntityTypeConfiguration<PayrollAudit>
        {
            public PayrollAuditConfiguration()
            {
                ToTable("PayrollAudit_d");
                HasKey(k => k.PayrollAuditKey);
            }
        }
    }
}