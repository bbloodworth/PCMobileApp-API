using ClearCost.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace CchWebAPI.Employee.Data {
    public class EmployeeContext : ClearCostContext<EmployeeContext> {
        public EmployeeContext(string connectionString) 
            : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new Employee.EmployeeConfiguration());
            builder.Configurations.Add(new Member.MemberConfiguration());
            builder.Configurations.Add(new BenefitEnrollment.BenefitEnrollmentConfiguration());
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<BenefitEnrollment> BenefitEnrollments { get; set; }
    }

    public class Employee {
        public int EmployeeKey { get; set; } // EmployeeKey (Primary key)
        public int Cchid { get; set; } // CCHID
        public string SourceEmployeeNum { get; set; } // SourceEmployeeNum (length: 20)
        public string EmployeeFirstName { get; set; } // EmployeeFirstName (length: 50)
        public string EmployeePreferredFirstName { get; set; } // EmployeePreferredFirstName (length: 50)
        public string EmployeeLastName { get; set; } // EmployeeLastName (length: 50)
        public string PrimaryWorkLocationCode { get; set; } // PrimaryWorkLocationCode (length: 10)
        public string PrimaryWorkLocationName { get; set; } // PrimaryWorkLocationName (length: 50)
        public string CompanyAddress1Text { get; set; } // CompanyAddress1Text (length: 100)
        public string CompanyAddress2Text { get; set; } // CompanyAddress2Text (length: 100)
        public string CompanyCityName { get; set; } // CompanyCityName (length: 50)
        public string CompanyStateCode { get; set; } // CompanyStateCode (length: 10)
        public string CompanyPostalCode { get; set; } // CompanyPostalCode (length: 10)
        public string OrgLevel1Name { get; set; } // OrgLevel1Name (length: 50)
        public string OrgLevel1ValueCode { get; set; } // OrgLevel1ValueCode (length: 10)
        public string OrgLevel1ValueName { get; set; } // OrgLevel1ValueName (length: 50)
        public string OrgLevel2Name { get; set; } // OrgLevel2Name (length: 50)
        public string OrgLevel2ValueCode { get; set; } // OrgLevel2ValueCode (length: 10)
        public string OrgLevel2ValueName { get; set; } // OrgLevel2ValueName (length: 50)
        public string OrgLevel3Name { get; set; } // OrgLevel3Name (length: 50)
        public string OrgLevel3ValueCode { get; set; } // OrgLevel3ValueCode (length: 10)
        public string OrgLevel3ValueName { get; set; } // OrgLevel3ValueName (length: 50)
        public string OrgLevel4Name { get; set; } // OrgLevel4Name (length: 50)
        public string OrgLevel4ValueCode { get; set; } // OrgLevel4ValueCode (length: 10)
        public string OrgLevel4ValueName { get; set; } // OrgLevel4ValueName (length: 50)
        public string OrgLevel5Name { get; set; } // OrgLevel5Name (length: 50)
        public string OrgLevel5ValueCode { get; set; } // OrgLevel5ValueCode (length: 10)
        public string OrgLevel5ValueName { get; set; } // OrgLevel5ValueName (length: 50)
        public string PayGroupCode { get; set; } // PayGroupCode (length: 10)
        public string PayGroupName { get; set; } // PayGroupName (length: 50)
        public string JobCode { get; set; } // JobCode (length: 10)
        public string JobName { get; set; } // JobName (length: 50)
        public string JobTitleName { get; set; } // JobTitleName (length: 50)
        public string RecognitionTitleName { get; set; } // RecognitionTitleName (length: 50)
        public System.DateTime? OriginalHireDate { get; set; } // OriginalHireDate
        public System.DateTime? LastHireDate { get; set; } // LastHireDate
        public System.DateTime? SeniorityDate { get; set; } // SeniorityDate
        public string EmploymentTypeDesc { get; set; } // EmploymentTypeDesc (length: 100)
        public string EmploymentHoursDesc { get; set; } // EmploymentHoursDesc (length: 100)
        public string EmployeeClassificationDesc { get; set; } // EmployeeClassificationDesc (length: 100)
        public string TaxableStatusDesc { get; set; } // TaxableStatusDesc (length: 100)
        public string PayFrequencyDesc { get; set; } // PayFrequencyDesc (length: 100)
        public System.DateTime? FlsaStatusDesc { get; set; } // FLSAStatusDesc
        public decimal? AnnualBaseSalaryAmt { get; set; } // AnnualBaseSalaryAmt
        public string EmploymentStatusCode { get; set; } // EmploymentStatusCode (length: 10)
        public string EmploymentStatusDesc { get; set; } // EmploymentStatusDesc (length: 100)
        public string FederalTaxElectionCode { get; set; } // FederalTaxElectionCode (length: 10)
        public string StateOfWorkElectionCode { get; set; } // StateOfWorkElectionCode (length: 10)
        public string StateOfResidenceElectionCode { get; set; } // StateOfResidenceElectionCode (length: 10)
        public System.DateTime? EffectiveFromDate { get; set; } // EffectiveFromDate
        public System.DateTime? EffectiveToDate { get; set; } // EffectiveToDate
        public bool? CurrentRecordInd { get; set; } // CurrentRecordInd
        public string SourceCreateUserId { get; set; } // SourceCreateUserID (length: 20)
        public System.DateTime? SourceCreateDate { get; set; } // SourceCreateDate
        public string SourceUpdateUserId { get; set; } // SourceUpdateUserID (length: 20)
        public System.DateTime? SourceUpdateDate { get; set; } // SourceUpdateDate
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 20)
        public System.DateTime? DwCreateDate { get; set; } // DWCreateDate
        public string DwUpdateUserId { get; set; } // DWUpdateUserID (length: 20)
        public System.DateTime? DwUpdateDate { get; set; } // DWUpdateDate
        public int? EtlControlId { get; set; } // ETLControlID
        public string SupervisorFullName { get; set; } // SupervisorFullName (length: 50)

        public class EmployeeConfiguration : EntityTypeConfiguration<Employee> {
            public EmployeeConfiguration() {
                ToTable("Employee_d");
                HasKey(x => x.EmployeeKey);

                Property(x => x.EmployeeKey).HasColumnName(@"EmployeeKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
                Property(x => x.Cchid).HasColumnName(@"CCHID").IsRequired().HasColumnType("int");
                Property(x => x.SourceEmployeeNum).HasColumnName(@"SourceEmployeeNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.EmployeeFirstName).HasColumnName(@"EmployeeFirstName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.EmployeePreferredFirstName).HasColumnName(@"EmployeePreferredFirstName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.EmployeeLastName).HasColumnName(@"EmployeeLastName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.PrimaryWorkLocationCode).HasColumnName(@"PrimaryWorkLocationCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PrimaryWorkLocationName).HasColumnName(@"PrimaryWorkLocationName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.CompanyAddress1Text).HasColumnName(@"CompanyAddress1Text").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.CompanyAddress2Text).HasColumnName(@"CompanyAddress2Text").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.CompanyCityName).HasColumnName(@"CompanyCityName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.CompanyStateCode).HasColumnName(@"CompanyStateCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.CompanyPostalCode).HasColumnName(@"CompanyPostalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.OrgLevel1Name).HasColumnName(@"OrgLevel1Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel1ValueCode).HasColumnName(@"OrgLevel1ValueCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.OrgLevel1ValueName).HasColumnName(@"OrgLevel1ValueName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel2Name).HasColumnName(@"OrgLevel2Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel2ValueCode).HasColumnName(@"OrgLevel2ValueCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.OrgLevel2ValueName).HasColumnName(@"OrgLevel2ValueName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel3Name).HasColumnName(@"OrgLevel3Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel3ValueCode).HasColumnName(@"OrgLevel3ValueCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.OrgLevel3ValueName).HasColumnName(@"OrgLevel3ValueName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel4Name).HasColumnName(@"OrgLevel4Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel4ValueCode).HasColumnName(@"OrgLevel4ValueCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.OrgLevel4ValueName).HasColumnName(@"OrgLevel4ValueName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel5Name).HasColumnName(@"OrgLevel5Name").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OrgLevel5ValueCode).HasColumnName(@"OrgLevel5ValueCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.OrgLevel5ValueName).HasColumnName(@"OrgLevel5ValueName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.PayGroupCode).HasColumnName(@"PayGroupCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PayGroupName).HasColumnName(@"PayGroupName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.JobCode).HasColumnName(@"JobCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.JobName).HasColumnName(@"JobName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.JobTitleName).HasColumnName(@"JobTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.RecognitionTitleName).HasColumnName(@"RecognitionTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.OriginalHireDate).HasColumnName(@"OriginalHireDate").IsOptional().HasColumnType("datetime");
                Property(x => x.LastHireDate).HasColumnName(@"LastHireDate").IsOptional().HasColumnType("datetime");
                Property(x => x.SeniorityDate).HasColumnName(@"SeniorityDate").IsOptional().HasColumnType("datetime");
                Property(x => x.EmploymentTypeDesc).HasColumnName(@"EmploymentTypeDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.EmploymentHoursDesc).HasColumnName(@"EmploymentHoursDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.EmployeeClassificationDesc).HasColumnName(@"EmployeeClassificationDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.TaxableStatusDesc).HasColumnName(@"TaxableStatusDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.PayFrequencyDesc).HasColumnName(@"PayFrequencyDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.FlsaStatusDesc).HasColumnName(@"FLSAStatusDesc").IsOptional().HasColumnType("datetime");
                Property(x => x.AnnualBaseSalaryAmt).HasColumnName(@"AnnualBaseSalaryAmt").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
                Property(x => x.EmploymentStatusCode).HasColumnName(@"EmploymentStatusCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.EmploymentStatusDesc).HasColumnName(@"EmploymentStatusDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.FederalTaxElectionCode).HasColumnName(@"FederalTaxElectionCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.StateOfWorkElectionCode).HasColumnName(@"StateOfWorkElectionCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.StateOfResidenceElectionCode).HasColumnName(@"StateOfResidenceElectionCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.EffectiveFromDate).HasColumnName(@"EffectiveFromDate").IsOptional().HasColumnType("date");
                Property(x => x.EffectiveToDate).HasColumnName(@"EffectiveToDate").IsOptional().HasColumnType("date");
                Property(x => x.CurrentRecordInd).HasColumnName(@"CurrentRecordInd").IsOptional().HasColumnType("bit");
                Property(x => x.SourceCreateUserId).HasColumnName(@"SourceCreateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.SourceCreateDate).HasColumnName(@"SourceCreateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.SourceUpdateUserId).HasColumnName(@"SourceUpdateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.SourceUpdateDate).HasColumnName(@"SourceUpdateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DwCreateUserId).HasColumnName(@"DWCreateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwCreateDate).HasColumnName(@"DWCreateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DwUpdateUserId).HasColumnName(@"DWUpdateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwUpdateDate).HasColumnName(@"DWUpdateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.EtlControlId).HasColumnName(@"ETLControlID").IsOptional().HasColumnType("int");
                Property(x => x.SupervisorFullName).HasColumnName(@"SupervisorFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            }
        }
    }

    public class Member {
        public int MemberKey { get; set; } // MemberKey (Primary key)
        public int Cchid { get; set; } // CCHID
        public string MemberNum { get; set; } // MemberNum (length: 20)
        public string MemberNumTypeDesc { get; set; } // MemberNumTypeDesc (length: 100)
        public string MemberEdiNum { get; set; } // MemberEDINum (length: 20)
        public int? CchApplicationUserId { get; set; } // CCHApplicationUserID
        public string SocialSecurityNum { get; set; } // SocialSecurityNum (length: 20)
        public string SocialSecurityLastFourNum { get; set; } // SocialSecurityLastFourNum (length: 20)
        public string MemberFirstName { get; set; } // MemberFirstName (length: 50)
        public string MemberPreferredFirstName { get; set; } // MemberPreferredFirstName (length: 50)
        public string MemberLastName { get; set; } // MemberLastName (length: 50)
        public string MemberMiddleName { get; set; } // MemberMiddleName (length: 50)
        public string RegistrationEmail { get; set; } // RegistrationEmail (length: 150)
        public System.DateTime? BirthDate { get; set; } // BirthDate
        public string GenderCode { get; set; } // GenderCode (length: 10)
        public string GenderDesc { get; set; } // GenderDesc (length: 100)
        public string Address1Text { get; set; } // Address1Text (length: 250)
        public string Address2Text { get; set; } // Address2Text (length: 250)
        public string CityName { get; set; } // CityName (length: 50)
        public string StateCode { get; set; } // StateCode (length: 10)
        public string PostalCode { get; set; } // PostalCode (length: 10)
        public double? LatitudeDegrees { get; set; } // LatitudeDegrees
        public double? LongitudeDegrees { get; set; } // LongitudeDegrees
        public string PreferredPhoneNum { get; set; } // PreferredPhoneNum (length: 20)
        public string MobilePhoneNum { get; set; } // MobilePhoneNum (length: 20)
        public string PropertyCode { get; set; } // PropertyCode (length: 10)
        public string RegistrationStatusDesc { get; set; } // RegistrationStatusDesc (length: 100)
        public System.DateTime? RegistrationDate { get; set; } // RegistrationDate
        public System.DateTime? PtFirstSearchDate { get; set; } // PTFirstSearchDate
        public System.DateTime? RegistrationPenaltyDate { get; set; } // RegistrationPenaltyDate
        public bool RegisteredInd { get; set; } // RegisteredInd
        public bool WebRegisteredInd { get; set; } // WebRegisteredInd
        public bool PhoneRegisteredInd { get; set; } // PhoneRegisteredInd
        public bool AutoRegisteredInd { get; set; } // AutoRegisteredInd
        public bool MemberTerminatedInd { get; set; } // MemberTerminatedInd
        public bool TestMemberInd { get; set; } // TestMemberInd
        public bool EmployeeInd { get; set; } // EmployeeInd
        public bool KeyMemberInd { get; set; } // KeyMemberInd
        public bool PtAppAccessInd { get; set; } // PTAppAccessInd
        public bool MpmAccessInd { get; set; } // MPMAccessInd
        public System.DateTime? EffectiveFromDate { get; set; } // EffectiveFromDate
        public System.DateTime? EffectiveToDate { get; set; } // EffectiveToDate
        public bool CurrentRecordInd { get; set; } // CurrentRecordInd
        public string SourceCreateUserId { get; set; } // SourceCreateUserID (length: 20)
        public System.DateTime? SourceCreateDate { get; set; } // SourceCreateDate
        public string SourceUpdateUserId { get; set; } // SourceUpdateUserID (length: 20)
        public System.DateTime? SourceUpdateDate { get; set; } // SourceUpdateDate
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 20)
        public System.DateTime? DwCreateDate { get; set; } // DWCreateDate
        public string DwUpdateUserId { get; set; } // DWUpdateUserID (length: 20)
        public System.DateTime? DwUpdateDate { get; set; } // DWUpdateDate
        public int? EtlControlId { get; set; } // ETLControlID

        public class MemberConfiguration : EntityTypeConfiguration<Member> {
            public MemberConfiguration() {
                ToTable("Member_d");
                HasKey(x => x.MemberKey);

                Property(x => x.MemberKey).HasColumnName(@"MemberKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
                Property(x => x.Cchid).HasColumnName(@"CCHID").IsRequired().HasColumnType("int");
                Property(x => x.MemberNum).HasColumnName(@"MemberNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.MemberNumTypeDesc).HasColumnName(@"MemberNumTypeDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.MemberEdiNum).HasColumnName(@"MemberEDINum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.CchApplicationUserId).HasColumnName(@"CCHApplicationUserID").IsOptional().HasColumnType("int");
                Property(x => x.SocialSecurityNum).HasColumnName(@"SocialSecurityNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.SocialSecurityLastFourNum).HasColumnName(@"SocialSecurityLastFourNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.MemberFirstName).HasColumnName(@"MemberFirstName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.MemberPreferredFirstName).HasColumnName(@"MemberPreferredFirstName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.MemberLastName).HasColumnName(@"MemberLastName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.MemberMiddleName).HasColumnName(@"MemberMiddleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.RegistrationEmail).HasColumnName(@"RegistrationEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
                Property(x => x.BirthDate).HasColumnName(@"BirthDate").IsOptional().HasColumnType("datetime");
                Property(x => x.GenderCode).HasColumnName(@"GenderCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.GenderDesc).HasColumnName(@"GenderDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.Address1Text).HasColumnName(@"Address1Text").IsOptional().HasColumnType("nvarchar").HasMaxLength(250);
                Property(x => x.Address2Text).HasColumnName(@"Address2Text").IsOptional().HasColumnType("nvarchar").HasMaxLength(250);
                Property(x => x.CityName).HasColumnName(@"CityName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.StateCode).HasColumnName(@"StateCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PostalCode).HasColumnName(@"PostalCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.LatitudeDegrees).HasColumnName(@"LatitudeDegrees").IsOptional().HasColumnType("float");
                Property(x => x.LongitudeDegrees).HasColumnName(@"LongitudeDegrees").IsOptional().HasColumnType("float");
                Property(x => x.PreferredPhoneNum).HasColumnName(@"PreferredPhoneNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.MobilePhoneNum).HasColumnName(@"MobilePhoneNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.PropertyCode).HasColumnName(@"PropertyCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.RegistrationStatusDesc).HasColumnName(@"RegistrationStatusDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.RegistrationDate).HasColumnName(@"RegistrationDate").IsOptional().HasColumnType("datetime");
                Property(x => x.PtFirstSearchDate).HasColumnName(@"PTFirstSearchDate").IsOptional().HasColumnType("datetime");
                Property(x => x.RegistrationPenaltyDate).HasColumnName(@"RegistrationPenaltyDate").IsOptional().HasColumnType("datetime");
                Property(x => x.RegisteredInd).HasColumnName(@"RegisteredInd").IsRequired().HasColumnType("bit");
                Property(x => x.WebRegisteredInd).HasColumnName(@"WebRegisteredInd").IsRequired().HasColumnType("bit");
                Property(x => x.PhoneRegisteredInd).HasColumnName(@"PhoneRegisteredInd").IsRequired().HasColumnType("bit");
                Property(x => x.AutoRegisteredInd).HasColumnName(@"AutoRegisteredInd").IsRequired().HasColumnType("bit");
                Property(x => x.MemberTerminatedInd).HasColumnName(@"MemberTerminatedInd").IsRequired().HasColumnType("bit");
                Property(x => x.TestMemberInd).HasColumnName(@"TestMemberInd").IsRequired().HasColumnType("bit");
                Property(x => x.EmployeeInd).HasColumnName(@"EmployeeInd").IsRequired().HasColumnType("bit");
                Property(x => x.KeyMemberInd).HasColumnName(@"KeyMemberInd").IsRequired().HasColumnType("bit");
                Property(x => x.PtAppAccessInd).HasColumnName(@"PTAppAccessInd").IsRequired().HasColumnType("bit");
                Property(x => x.MpmAccessInd).HasColumnName(@"MPMAccessInd").IsRequired().HasColumnType("bit");
                Property(x => x.EffectiveFromDate).HasColumnName(@"EffectiveFromDate").IsOptional().HasColumnType("date");
                Property(x => x.EffectiveToDate).HasColumnName(@"EffectiveToDate").IsOptional().HasColumnType("date");
                Property(x => x.CurrentRecordInd).HasColumnName(@"CurrentRecordInd").IsRequired().HasColumnType("bit");
                Property(x => x.SourceCreateUserId).HasColumnName(@"SourceCreateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.SourceCreateDate).HasColumnName(@"SourceCreateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.SourceUpdateUserId).HasColumnName(@"SourceUpdateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.SourceUpdateDate).HasColumnName(@"SourceUpdateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DwCreateUserId).HasColumnName(@"DWCreateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwCreateDate).HasColumnName(@"DWCreateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DwUpdateUserId).HasColumnName(@"DWUpdateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwUpdateDate).HasColumnName(@"DWUpdateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.EtlControlId).HasColumnName(@"ETLControlID").IsOptional().HasColumnType("int");
            }
        }
    }

    public class BenefitEnrollment {
        public int EmployerKey { get; set; } // EmployerKey (Primary key)
        public int PlanYearKey { get; set; } // PlanYearKey (Primary key)
        public int SubscriberMemberKey { get; set; } // SubscriberMemberKey (Primary key)
        public int SubscriberEmployeeKey { get; set; } // SubscriberEmployeeKey (Primary key)
        public int EnrolledMemberKey { get; set; } // EnrolledMemberKey (Primary key)
        public int RelationshipKey { get; set; } // RelationshipKey (Primary key)
        public int BenefitPlanOptionKey { get; set; } // BenefitPlanOptionKey (Primary key)
        public int CoverageTierKey { get; set; } // CoverageTierKey (Primary key)
        public int BenefitEnrollmentStatusKey { get; set; } // BenefitEnrollmentStatusKey (Primary key)
        public int EffectiveFromDateKey { get; set; } // EffectiveFromDateKey (Primary key)
        public int EffectiveToDateKey { get; set; } // EffectiveToDateKey (Primary key)
        public int EnrollmentChangeDateKey { get; set; } // EnrollmentChangeDateKey (Primary key)
        public int BenefitEnrollmentAuditKey { get; set; } // BenefitEnrollmentAuditKey (Primary key)
        public string SubscriberPlanId { get; set; } // SubscriberPlanID (length: 20)
        public string MemberPlanId { get; set; } // MemberPlanID (length: 20)
        public string DependentNum { get; set; } // DependentNum (length: 5)
        public string ContractSuffixCode { get; set; } // ContractSuffixCode (length: 10)
        public string PrimaryCareProviderName { get; set; } // PrimaryCareProviderName (length: 50)
        public string PrimaryCareProviderNpi { get; set; } // PrimaryCareProviderNPI (length: 10)
        public string PrimaryCareDentistName { get; set; } // PrimaryCareDentistName (length: 50)
        public decimal? EmployeeAnnualContributionAmt { get; set; } // EmployeeAnnualContributionAmt
        public decimal? EmployeeAnnualContributionPct { get; set; } // EmployeeAnnualContributionPct
        public bool CoverageTerminatedInd { get; set; } // CoverageTerminatedInd
        public bool? CurrentRecordInd { get; set; } // CurrentRecordInd

        public class BenefitEnrollmentConfiguration : EntityTypeConfiguration<BenefitEnrollment> {
            public BenefitEnrollmentConfiguration() {
                ToTable("BenefitEnrollment_f");
                HasKey(x => new { x.EmployerKey, x.SubscriberMemberKey, x.EffectiveFromDateKey, x.EffectiveToDateKey, x.BenefitPlanOptionKey, x.BenefitEnrollmentStatusKey, x.EnrolledMemberKey, x.SubscriberEmployeeKey, x.PlanYearKey, x.CoverageTierKey, x.RelationshipKey, x.EnrollmentChangeDateKey, x.BenefitEnrollmentAuditKey });

                Property(x => x.EmployerKey).HasColumnName(@"EmployerKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.PlanYearKey).HasColumnName(@"PlanYearKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.SubscriberMemberKey).HasColumnName(@"SubscriberMemberKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.SubscriberEmployeeKey).HasColumnName(@"SubscriberEmployeeKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.EnrolledMemberKey).HasColumnName(@"EnrolledMemberKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.RelationshipKey).HasColumnName(@"RelationshipKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.BenefitPlanOptionKey).HasColumnName(@"BenefitPlanOptionKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.CoverageTierKey).HasColumnName(@"CoverageTierKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.BenefitEnrollmentStatusKey).HasColumnName(@"BenefitEnrollmentStatusKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.EffectiveFromDateKey).HasColumnName(@"EffectiveFromDateKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.EffectiveToDateKey).HasColumnName(@"EffectiveToDateKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.EnrollmentChangeDateKey).HasColumnName(@"EnrollmentChangeDateKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.BenefitEnrollmentAuditKey).HasColumnName(@"BenefitEnrollmentAuditKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.SubscriberPlanId).HasColumnName(@"SubscriberPlanID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.MemberPlanId).HasColumnName(@"MemberPlanID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DependentNum).HasColumnName(@"DependentNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(5);
                Property(x => x.ContractSuffixCode).HasColumnName(@"ContractSuffixCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PrimaryCareProviderName).HasColumnName(@"PrimaryCareProviderName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.PrimaryCareProviderNpi).HasColumnName(@"PrimaryCareProviderNPI").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PrimaryCareDentistName).HasColumnName(@"PrimaryCareDentistName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.EmployeeAnnualContributionAmt).HasColumnName(@"EmployeeAnnualContributionAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.EmployeeAnnualContributionPct).HasColumnName(@"EmployeeAnnualContributionPct").IsOptional().HasColumnType("decimal").HasPrecision(6, 4);
                Property(x => x.CoverageTerminatedInd).HasColumnName(@"CoverageTerminatedInd").IsRequired().HasColumnType("bit");
                Property(x => x.CurrentRecordInd).HasColumnName(@"CurrentRecordInd").IsOptional().HasColumnType("bit");
            }
        }
    }
}