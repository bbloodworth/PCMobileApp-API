using System.Data.Entity;
using System.Data.SqlClient;
using ClearCost.Data;
using CchWebAPI.PaidTimeOff.Models;
using CchWebAPI.EmployeeDW.Data;

namespace CchWebAPI.PaidTimeOff.Data
{
    public class PaidTimeOffContext: ClearCostContext<PaidTimeOffContext>
    {
        public PaidTimeOffContext(string connectionString) : 
            base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder)
        {
            builder.Configurations.Add(new PTOSnapshot.PTOSnapshotConfiguration());
            builder.Configurations.Add(new PayrollMetric.PayrollMetricConfiguration());
            builder.Configurations.Add(new Dates.DatesConfiguration());
            builder.Configurations.Add(new Employee.EmployeeConfiguration());

        }

        public DbSet<PTOSnapshot> PTOSnapshots { get; set; }
        public DbSet<PayrollMetric> PayrollMetrics { get; set; }
        public DbSet<Dates> Dates { get; set; }
        public DbSet<Employee> Employees { get; set; }

    }
}
