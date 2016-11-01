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
            builder.Configurations.Add(new Member.MemberConfiguration());
            builder.Configurations.Add(new BenefitEnrollment.BenefitEnrollmentConfiguration());
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BenefitEnrollment> BenefitEnrollments { get; set; }
    }

    [Table("Employee_d")]
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
            }
        }
    }

    [Table("Member_d")]
    public class Member {
        [Key]
        public int MemberKey { get; set; }

        public int CCHID { get; set; }

        [StringLength(20)]
        public string MemberNum { get; set; }

        [StringLength(100)]
        public string MemberNumTypeDesc { get; set; }

        [StringLength(20)]
        public string MemberEDINum { get; set; }

        public int? CCHApplicationUserID { get; set; }

        [StringLength(20)]
        public string SocialSecurityNum { get; set; }

        [StringLength(20)]
        public string SocialSecurityLastFourNum { get; set; }

        [StringLength(50)]
        public string MemberFirstName { get; set; }

        [StringLength(50)]
        public string MemberPreferredFirstName { get; set; }

        [StringLength(50)]
        public string MemberLastName { get; set; }

        [StringLength(50)]
        public string MemberMiddleName { get; set; }

        [StringLength(150)]
        public string RegistrationEmail { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(10)]
        public string GenderCode { get; set; }

        [StringLength(100)]
        public string GenderDesc { get; set; }

        [StringLength(250)]
        public string Address1Text { get; set; }

        [StringLength(250)]
        public string Address2Text { get; set; }

        [StringLength(50)]
        public string CityName { get; set; }

        [StringLength(10)]
        public string StateCode { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        public double? LatitudeDegrees { get; set; }

        public double? LongitudeDegrees { get; set; }

        [StringLength(20)]
        public string PreferredPhoneNum { get; set; }

        [StringLength(20)]
        public string MobilePhoneNum { get; set; }

        [StringLength(10)]
        public string PropertyCode { get; set; }

        [StringLength(100)]
        public string RegistrationStatusDesc { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public DateTime? PTFirstSearchDate { get; set; }

        public DateTime? RegistrationPenaltyDate { get; set; }

        public bool RegisteredInd { get; set; }

        public bool WebRegisteredInd { get; set; }

        public bool PhoneRegisteredInd { get; set; }

        public bool AutoRegisteredInd { get; set; }

        public bool MemberTerminatedInd { get; set; }

        public bool TestMemberInd { get; set; }

        public bool EmployeeInd { get; set; }

        public bool KeyMemberInd { get; set; }

        public bool PTAppAccessInd { get; set; }

        public bool MPMAccessInd { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
        public DateTime? EffectiveFromDate { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
        public DateTime? EffectiveToDate { get; set; }

        public bool CurrentRecordInd { get; set; }

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

        public class MemberConfiguration : EntityTypeConfiguration<Member> {
            public MemberConfiguration() {
            }
        }
    }

    [Table("BenefitEnrollment_f")]
    public class BenefitEnrollment {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployerKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlanYearKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SubscriberMemberKey { get; set; }

        public int SubscriberEmployeeKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EnrolledMemberKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BenefitPlanOptionKey { get; set; }

        public int CoverageTierKey { get; set; }

        public int BenefitEnrollmentStatusKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EffectiveFromDateKey { get; set; }

        public int EffectiveToDateKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EnrollmentChangeDateKey { get; set; }

        [Required]
        [StringLength(10)]
        public string BenefitEnrollmentAuditKey { get; set; }

        public int? SubscriberPlanID { get; set; }

        public int? MemberPlanID { get; set; }

        [StringLength(5)]
        public string DependentNum { get; set; }

        [StringLength(10)]
        public string ContractSuffixCode { get; set; }

        [StringLength(50)]
        public string PrimaryCareProviderName { get; set; }

        [StringLength(10)]
        public string PrimaryCareProviderNPI { get; set; }

        [StringLength(50)]
        public string PrimaryCareDentistName { get; set; }

        public decimal? EmployeeAnnualContributionAmt { get; set; }

        [StringLength(10)]
        public string EmployeeAnnualContributionPct { get; set; }

        public bool CoverageTerminatedInd { get; set; }

        public bool? CurrentRecordInd { get; set; }

        public int RelationshipKey { get; set; }

        public class BenefitEnrollmentConfiguration : EntityTypeConfiguration<BenefitEnrollment> {
            public BenefitEnrollmentConfiguration() {
            }
        }
    }
}