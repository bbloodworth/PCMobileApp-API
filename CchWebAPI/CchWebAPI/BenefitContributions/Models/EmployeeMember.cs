using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContributions.Models
{
    public class EmployeeMember
    {
        public int EmployeeKey { get; set; }
        public int CCHID { get; set; }
        public string SourceEmployeeNum { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public string PayGroupCode { get; set; }
        public string PayGroupName { get; set; }

        public class EmployeeMemberConfiguration: EntityTypeConfiguration<EmployeeMember>
        {
            public EmployeeMemberConfiguration()
            {
                ToTable("Employee_d");
                HasKey(k => k.EmployeeKey);
            }
        }
    }
}