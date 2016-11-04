using System.Data.Entity;
using System.Data.SqlClient;
using ClearCost.Data;
using CchWebAPI.BenefitContributions.Models;
//using CchWebAPI.Employee.Data;

namespace CchWebAPI.BenefitContributions.Data
{
    public class ContributionsContext: ClearCostContext<ContributionsContext>
    {
        public ContributionsContext(string connectionString) : 
            base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder)
        {
            builder.Configurations.Add(new Models.Payroll.PayrollConfiguration());
            builder.Configurations.Add(new Employee.Data.Employee.EmployeeConfiguration());
            builder.Configurations.Add(new Dates.DatesConfiguration());
            builder.Configurations.Add(new DeliveryMethod.DeliveryMethodConfiguration());
            builder.Configurations.Add(new ContributionType.ConfigurationTypeConfiguration());
            builder.Configurations.Add(new PayrollMetric.PayrollMetricConfiguration());
            builder.Configurations.Add(new PayrollAudit.PayrollAuditConfiguration());
        }

        public DbSet<Models.Payroll> Payroll { get; set; }
        public DbSet<Employee.Data.Employee> EmployeeMembers { get; set; }
        public DbSet<Dates> Dates { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<ContributionType> ContributionTypes { get; set; }
        public DbSet<PayrollMetric> PayrollMetrics { get; set; }
        public DbSet<PayrollAudit> PayrollAudits { get; set; }
    }
}
