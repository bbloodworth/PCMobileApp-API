using ClearCost.Data;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace CchWebAPI.Employees.Data
{
    public class EmployeesContext : ClearCostContext<EmployeesContext>
    {
        public EmployeesContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder)
        {
            builder.Configurations.Add(new Employee.EmployeeConfiguration());
        }

        public DbSet<Employee> Employees { get; set; }
    }

    public class Employee
    {
        public int MemberId { get; set; }
        [NotMapped]
        public int EmployerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string JobTitle { get; set; }
        public string EmployeeID { get; set; }
        public string MemberMedicalID { get; set; }
        public string MemberRXID { get; set; }
        public string SubscriberMedicalID { get; set; }
        public string SubscriberRXID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string RelationshipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string HealthPlanType { get; set; }
        public string MedicalPlanType { get; set; }
        public string RXPlanType { get; set; }

        public class EmployeeConfiguration : EntityTypeConfiguration<Employee>
        {
            public EmployeeConfiguration()
            {
                ToTable("Enrollments");
                HasKey(k => k.MemberId);
                Property(p => p.MemberId).HasColumnName("CCHID");
                Property(p => p.FirstName).HasColumnName("FirstName");
                Property(p => p.LastName).HasColumnName("LastName");
                Property(p => p.EmployeeID).HasColumnName("EmployeeID");
                Property(p => p.MemberMedicalID).HasColumnName("MemberMedicalID");
                Property(p => p.MemberRXID).HasColumnName("MemberRXID");
                Property(p => p.SubscriberMedicalID).HasColumnName("SubscriberMedicalID");
                Property(p => p.SubscriberRXID).HasColumnName("SubscriberRXID");
                Property(p => p.DateOfBirth).HasColumnName("DateOfBirth");
                Property(p => p.Gender).HasColumnName("Gender");
                Property(p => p.RelationshipCode).HasColumnName("RelationshipCode");
                Property(p => p.City).HasColumnName("City");
                Property(p => p.State).HasColumnName("State");
                Property(p => p.ZipCode).HasColumnName("ZipCode");
                Property(p => p.Email).HasColumnName("Email");
                Property(p => p.Longitude).HasColumnName("Longitude");
                Property(p => p.Latitude).HasColumnName("Latitude");
                Property(p => p.HealthPlanType).HasColumnName("HealthPlanType");
                Property(p => p.MedicalPlanType).HasColumnName("MedicalPlanType");
                Property(p => p.RXPlanType).HasColumnName("RXPlanType");
            }
        }
    }
}