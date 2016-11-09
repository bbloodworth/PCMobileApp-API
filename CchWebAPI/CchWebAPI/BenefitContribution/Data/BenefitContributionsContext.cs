using System.Data.Entity;
using System.Data.SqlClient;
using ClearCost.Data;
using CchWebAPI.BenefitContribution.Models;

namespace CchWebAPI.BenefitContribution.Data
{
    public class BenefitContributionsContext: ClearCostContext<BenefitContributionsContext>
    {
        public BenefitContributionsContext(string connectionString) : 
            base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder)
        {
            builder.Configurations.Add(new Models.Payroll.PayrollConfiguration());
            builder.Configurations.Add(new EmployeeMember.EmployeeMemberConfiguration());
            builder.Configurations.Add(new Dates.DatesConfiguration());
            builder.Configurations.Add(new DeliveryMethod.DeliveryMethodConfiguration());
            builder.Configurations.Add(new ContributionType.ConfigurationTypeConfiguration());
            builder.Configurations.Add(new PayrollMetric.PayrollMetricConfiguration());
            builder.Configurations.Add(new PayrollAudit.PayrollAuditConfiguration());
            builder.Configurations.Add(new BenefitEnrollment.BenefitEnrollmentConfiguration());
            builder.Configurations.Add(new Member.MemberConfiguration());
            builder.Configurations.Add(new BenefitPlanOption.BenefitPlanOptionConfiguration());
            
        }

        public DbSet<Models.Payroll> Payroll { get; set; }
        public DbSet<Models.EmployeeMember> EmployeeMembers { get; set; }
        public DbSet<Dates> Dates { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<ContributionType> ContributionTypes { get; set; }
        public DbSet<PayrollMetric> PayrollMetrics { get; set; }
        public DbSet<PayrollAudit> PayrollAudits { get; set; }
        public DbSet<BenefitEnrollment> BenefitEnrollments { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BenefitPlanOption> BenefitPlanOptions { get; set; }
    }
}
