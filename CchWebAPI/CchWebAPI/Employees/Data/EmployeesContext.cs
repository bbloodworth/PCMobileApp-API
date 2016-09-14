using ClearCost.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace CchWebAPI.Employees.Data {
    public class EmployeesContext : ClearCostContext<EmployeesContext> {
        public EmployeesContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new Employee.EmployeeConfiguration());
        }

        public DbSet<Employee> Employees { get; set; }
    }

    public class Employee {
        public int MemberId { get; set; }
        public int EmployerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobTitle { get; set; }

        public class EmployeeConfiguration : EntityTypeConfiguration<Employee> {
            public EmployeeConfiguration() {
                ToTable("Enrollments");
                HasKey(k => k.MemberId);
                Property(p => p.MemberId).HasColumnName("CCHID");
                Property(p => p.FirstName).HasColumnName("FirstName");
                Property(p => p.LastName).HasColumnName("LastName");
            }
        }
    }
}