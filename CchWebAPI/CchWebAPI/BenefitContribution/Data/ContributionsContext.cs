using System.Data.Entity;
using System.Data.SqlClient;
using ClearCost.Data;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace CchWebAPI.BenefitContribution.Data {
    public class ContributionsContext : ClearCostContext<ContributionsContext> {
        public ContributionsContext(string connectionString) :
            base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new Payroll.PayrollConfiguration());
            builder.Configurations.Add(new Employee.EmployeeConfiguration());
            builder.Configurations.Add(new Dates.DatesConfiguration());
            builder.Configurations.Add(new DeliveryMethod.DeliveryMethodConfiguration());
            builder.Configurations.Add(new ContributionType.ConfigurationTypeConfiguration());
            builder.Configurations.Add(new PayrollMetric.PayrollMetricConfiguration());
            builder.Configurations.Add(new PayrollAudit.PayrollAuditConfiguration());
        }

        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<Employee> EmployeeMembers { get; set; }
        public DbSet<Dates> DatesOfInterest { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<ContributionType> ContributionTypes { get; set; }
        public DbSet<PayrollMetric> PayrollMetrics { get; set; }
        public DbSet<PayrollAudit> PayrollAudits { get; set; }

        #region POCO classes
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
        public class ContributionType {
            public int ContributionTypeKey { get; set; }
            public string ContributionTypeCode { get; set; }
            public string ContributionTypeName { get; set; }

            public class ConfigurationTypeConfiguration : EntityTypeConfiguration<ContributionType> {
                public ConfigurationTypeConfiguration() {
                    ToTable("ContributionType_d");
                    HasKey(k => k.ContributionTypeKey);
                }
            }
        }
        public class Dates {
            public int DateKey { get; set; }
            public DateTime FullDate { get; set; }

            public class DatesConfiguration : EntityTypeConfiguration<Dates> {
                public DatesConfiguration() {
                    ToTable("Date_d");
                    HasKey(k => k.DateKey);
                }
            }
        }
        public class DeliveryMethod {
            public int DeliveryMethodKey { get; set; }
            public string DeliveryMethodCode { get; set; }
            public string DeliveryMethodName { get; set; }

            public class DeliveryMethodConfiguration : EntityTypeConfiguration<DeliveryMethod> {
                public DeliveryMethodConfiguration() {
                    ToTable("DeliveryMethod_d");
                    HasKey(k => k.DeliveryMethodKey);
                }
            }
        }
        public class EmployeeMember {
            public int EmployeeKey { get; set; }
            public int CCHID { get; set; }
            public string SourceEmployeeNum { get; set; }
            public string EmployeeFirstName { get; set; }
            public string EmployeeLastName { get; set; }
            public string PayGroupCode { get; set; }
            public string PayGroupName { get; set; }

            public class EmployeeMemberConfiguration : EntityTypeConfiguration<EmployeeMember> {
                public EmployeeMemberConfiguration() {
                    ToTable("Employee_d");
                    HasKey(k => k.EmployeeKey);
                }
            }
        }
        public class Payroll {
            public int EmployerKey { get; set; }
            public int EmployeeKey { get; set; }
            public int PayDateKey { get; set; }
            public int DeliveryMethodKey { get; set; }
            public int PayrollMetricKey { get; set; }
            public int ContributionTypeKey { get; set; }
            public int PayrollAuditKey { get; set; }
            public string DocumentID { get; set; }
            public DateTime PayDate { get; set; }
            public bool CurrentPayPeriodInd { get; set; }
            public decimal PerPeriodQty { get; set; }
            public decimal PerPeriodAmt { get; set; }
            public decimal YTDQty { get; set; }
            public decimal YTDAmt { get; set; }

            public class PayrollConfiguration : EntityTypeConfiguration<Payroll> {
                public PayrollConfiguration() {
                    ToTable("Payroll_f");
                    HasKey(k => new {
                        k.EmployerKey,
                        k.EmployeeKey,
                        k.PayDateKey,
                        k.PayrollMetricKey,
                        k.ContributionTypeKey,
                        k.DocumentID
                    });
                }
            }
        }
        public class PayrollAudit {
            public int PayrollAuditKey { get; set; }
            public int ETLVersionID { get; set; }
            public DateTime DWCreateDate { get; set; }

            public class PayrollAuditConfiguration : EntityTypeConfiguration<PayrollAudit> {
                public PayrollAuditConfiguration() {
                    ToTable("PayrollAudit_d");
                    HasKey(k => k.PayrollAuditKey);
                }
            }
        }
        public class PayrollMetric {
            public int PayrollMetricKey { get; set; }
            public string PayrollMetricCode { get; set; }
            public string PayrollMetricName { get; set; }
            public string PayrollMetricDesc { get; set; }
            public string PayrollCategoryName { get; set; }
            public string ReportingCategoryCode { get; set; }
            public bool PreTaxInd { get; set; }

            public class PayrollMetricConfiguration : EntityTypeConfiguration<PayrollMetric> {
                public PayrollMetricConfiguration() {
                    ToTable("PayrollMetric_d");
                    HasKey(k => k.PayrollMetricKey);
                }
            }
        }
        #endregion
    }
}
