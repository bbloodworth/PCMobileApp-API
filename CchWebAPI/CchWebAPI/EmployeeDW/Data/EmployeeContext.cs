using ClearCost.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace CchWebAPI.EmployeeDW.Data
{
    public class EmployeeContext : ClearCostContext<EmployeeContext>
    {
        public EmployeeContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder)
        {
            builder.Configurations.Add(new Employee.EmployeeConfiguration());
        }

        public DbSet<Employee> Employees { get; set; }
    }

    public class Employee {
        [Key]
        public int EmployeeKey { get; set; }

        public int CCHID { get; set; }

        [StringLength(20)]
        public string SourceEmployeeNum { get; set; }

        [StringLength(50)]
        public string EmployeeFirstName { get; set; }

        [StringLength(50)]
        public string EmployeePreferredFirstName { get; set; }

        [StringLength(50)]
        public string EmployeeLastName { get; set; }

        [StringLength(10)]
        public string PrimaryWorkLocationCode { get; set; }

        [StringLength(50)]
        public string PrimaryWorkLocationName { get; set; }

        [StringLength(100)]
        public string CompanyAddress1Text { get; set; }

        [StringLength(100)]
        public string CompanyAddress2Text { get; set; }

        [StringLength(50)]
        public string CompanyCityName { get; set; }

        [StringLength(10)]
        public string CompanyStateCode { get; set; }

        [StringLength(10)]
        public string CompanyPostalCode { get; set; }

        [StringLength(50)]
        public string OrgLevel1Name { get; set; }

        [StringLength(10)]
        public string OrgLevel1ValueCode { get; set; }

        [StringLength(50)]
        public string OrgLevel1ValueName { get; set; }

        [StringLength(50)]
        public string OrgLevel2Name { get; set; }

        [StringLength(10)]
        public string OrgLevel2ValueCode { get; set; }

        [StringLength(50)]
        public string OrgLevel2ValueName { get; set; }

        [StringLength(50)]
        public string OrgLevel3Name { get; set; }

        [StringLength(10)]
        public string OrgLevel3ValueCode { get; set; }

        [StringLength(50)]
        public string OrgLevel3ValueName { get; set; }

        [StringLength(50)]
        public string OrgLevel4Name { get; set; }

        [StringLength(10)]
        public string OrgLevel4ValueCode { get; set; }

        [StringLength(50)]
        public string OrgLevel4ValueName { get; set; }

        [StringLength(50)]
        public string OrgLevel5Name { get; set; }

        [StringLength(10)]
        public string OrgLevel5ValueCode { get; set; }

        [StringLength(50)]
        public string OrgLevel5ValueName { get; set; }

        [StringLength(10)]
        public string PayGroupCode { get; set; }

        [StringLength(50)]
        public string PayGroupName { get; set; }

        [StringLength(10)]
        public string JobCode { get; set; }

        [StringLength(50)]
        public string JobName { get; set; }

        [StringLength(50)]
        public string JobTitleName { get; set; }

        [StringLength(50)]
        public string RecognitionTitleName { get; set; }

        public DateTime? OriginalHireDate { get; set; }

        public DateTime? LastHireDate { get; set; }

        public DateTime? SeniorityDate { get; set; }

        [StringLength(100)]
        public string EmploymentTypeDesc { get; set; }

        [StringLength(100)]
        public string EmploymentHoursDesc { get; set; }

        [StringLength(100)]
        public string EmployeeClassificationDesc { get; set; }

        [StringLength(100)]
        public string TaxableStatusDesc { get; set; }

        [StringLength(100)]
        public string PayFrequencyDesc { get; set; }

        public DateTime? FLSAStatusDesc { get; set; }

        public decimal? AnnualBaseSalaryAmt { get; set; }

        [StringLength(10)]
        public string EmploymentStatusCode { get; set; }

        [StringLength(100)]
        public string EmploymentStatusDesc { get; set; }

        [StringLength(10)]
        public string FederalTaxElectionCode { get; set; }

        [StringLength(10)]
        public string StateOfWorkElectionCode { get; set; }

        [StringLength(10)]
        public string StateOfResidenceElectionCode { get; set; }

        public DateTime? EffectiveFromDate { get; set; }

        //[Column(TypeName = "date")]
        public DateTime? EffectiveToDate { get; set; }

        public bool? CurrentRecordInd { get; set; }

        [StringLength(20)]
        public string SourceCreateUserID { get; set; }

        public DateTime? SourceCreateDate { get; set; }

        [StringLength(20)]
        public string SourceUpdateUserID { get; set; }

        public DateTime? SourceUpdateDate { get; set; }

        [StringLength(20)]
        public string DWCreateUserID { get; set; }

        public DateTime? DWCreateDate { get; set; }

        [StringLength(20)]
        public string DWUpdateUserID { get; set; }

        public DateTime? DWUpdateDate { get; set; }

        public int? ETLControlID { get; set; }

        [StringLength(50)]
        public string SupervisorFullName { get; set; }

        public class EmployeeConfiguration : EntityTypeConfiguration<Employee> {
            public EmployeeConfiguration() {
                ToTable("Employee_d");
                HasKey(k => k.EmployeeKey);
                Property(p => p.EmployeeKey).HasColumnName("EmployeeKey");
                Property(p => p.CCHID).HasColumnName("CCHID");
                Property(p => p.SourceEmployeeNum).HasColumnName("SourceEmployeeNum");
                Property(p => p.EmployeeFirstName).HasColumnName("EmployeeFirstName");
                Property(p => p.EmployeePreferredFirstName).HasColumnName("EmployeePreferredFirstName");
                Property(p => p.EmployeeLastName).HasColumnName("EmployeeLastName");
                Property(p => p.PrimaryWorkLocationCode).HasColumnName("PrimaryWorkLocationCode");
                Property(p => p.PrimaryWorkLocationName).HasColumnName("PrimaryWorkLocationName");
                Property(p => p.CompanyAddress1Text).HasColumnName("CompanyAddress1Text");
                Property(p => p.CompanyAddress2Text).HasColumnName("CompanyAddress2Text");
                Property(p => p.CompanyCityName).HasColumnName("CompanyCityName");
                Property(p => p.CompanyStateCode).HasColumnName("CompanyStateCode");
                Property(p => p.CompanyPostalCode).HasColumnName("CompanyPostalCode");
                Property(p => p.OrgLevel1Name).HasColumnName("OrgLevel1Name");
                Property(p => p.OrgLevel1ValueCode).HasColumnName("OrgLevel1ValueCode");
                Property(p => p.OrgLevel1ValueName).HasColumnName("OrgLevel1ValueName");
                Property(p => p.OrgLevel2Name).HasColumnName("OrgLevel2Name");
                Property(p => p.OrgLevel2ValueCode).HasColumnName("OrgLevel2ValueCode");
                Property(p => p.OrgLevel2ValueName).HasColumnName("OrgLevel2ValueName");
                Property(p => p.OrgLevel3Name).HasColumnName("OrgLevel3Name");
                Property(p => p.OrgLevel3ValueCode).HasColumnName("OrgLevel3ValueCode");
                Property(p => p.OrgLevel3ValueName).HasColumnName("OrgLevel3ValueName");
                Property(p => p.OrgLevel4Name).HasColumnName("OrgLevel4Name");
                Property(p => p.OrgLevel4ValueCode).HasColumnName("OrgLevel4ValueCode");
                Property(p => p.OrgLevel4ValueName).HasColumnName("OrgLevel4ValueName");
                Property(p => p.OrgLevel5Name).HasColumnName("OrgLevel5Name");
                Property(p => p.OrgLevel5ValueCode).HasColumnName("OrgLevel5ValueCode");
                Property(p => p.OrgLevel5ValueName).HasColumnName("OrgLevel5ValueName");
                Property(p => p.PayGroupCode).HasColumnName("PayGroupCode");
                Property(p => p.PayGroupName).HasColumnName("PayGroupName");
                Property(p => p.JobCode).HasColumnName("JobCode");
                Property(p => p.JobName).HasColumnName("JobName");
                Property(p => p.JobTitleName).HasColumnName("JobTitleName");
                Property(p => p.RecognitionTitleName).HasColumnName("RecognitionTitleName");
                Property(p => p.OriginalHireDate).HasColumnName("OriginalHireDate");
                Property(p => p.LastHireDate).HasColumnName("LastHireDate");
                Property(p => p.SeniorityDate).HasColumnName("SeniorityDate");
                Property(p => p.EmploymentTypeDesc).HasColumnName("EmploymentTypeDesc");
                Property(p => p.EmploymentHoursDesc).HasColumnName("EmploymentHoursDesc");
                Property(p => p.EmployeeClassificationDesc).HasColumnName("EmployeeClassificationDesc");
                Property(p => p.TaxableStatusDesc).HasColumnName("TaxableStatusDesc");
                Property(p => p.PayFrequencyDesc).HasColumnName("PayFrequencyDesc");
                Property(p => p.FLSAStatusDesc).HasColumnName("FLSAStatusDesc");
                Property(p => p.AnnualBaseSalaryAmt).HasColumnName("AnnualBaseSalaryAmt");
                Property(p => p.EmploymentStatusCode).HasColumnName("EmploymentStatusCode");
                Property(p => p.EmploymentStatusDesc).HasColumnName("EmploymentStatusDesc");
                Property(p => p.FederalTaxElectionCode).HasColumnName("FederalTaxElectionCode");
                Property(p => p.StateOfWorkElectionCode).HasColumnName("StateOfWorkElectionCode");
                Property(p => p.StateOfResidenceElectionCode).HasColumnName("StateOfResidenceElectionCode");
                Property(p => p.EffectiveFromDate).HasColumnName("EffectiveFromDate");
                Property(p => p.EffectiveToDate).HasColumnName("EffectiveToDate");
                Property(p => p.CurrentRecordInd).HasColumnName("CurrentRecordInd");
                Property(p => p.SourceCreateUserID).HasColumnName("SourceCreateUserID");
                Property(p => p.SourceCreateDate).HasColumnName("SourceCreateDate");
                Property(p => p.SourceUpdateUserID).HasColumnName("SourceUpdateUserID");
                Property(p => p.SourceUpdateDate).HasColumnName("SourceUpdateDate");
                Property(p => p.DWCreateUserID).HasColumnName("DWCreateUserID");
                Property(p => p.DWCreateDate).HasColumnName("DWCreateDate");
                Property(p => p.DWUpdateUserID).HasColumnName("DWUpdateUserID");
                Property(p => p.DWUpdateDate).HasColumnName("DWUpdateDate");
                Property(p => p.ETLControlID).HasColumnName("ETLControlID");
                Property(p => p.SupervisorFullName).HasColumnName("SupervisorFullName");
            }
        }
    }
}