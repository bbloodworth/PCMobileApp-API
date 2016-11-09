using System.Data.Entity.ModelConfiguration;

namespace CchWebAPI.BenefitContribution.Models
{
    public class EmployeeMember
    {
        public int EmployeeKey { get; set; }
        public int CchId { get; set; }
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