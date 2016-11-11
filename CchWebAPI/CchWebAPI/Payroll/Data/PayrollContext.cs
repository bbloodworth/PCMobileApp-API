using ClearCost.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace CchWebAPI.Payroll.Data
{
    public class PayrollContext : ClearCostContext<PayrollContext>
    {
        public PayrollContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder)
        {
            builder.Configurations.Add(new Payroll.PayrollConfiguration());
            builder.Configurations.Add(new Employee.EmployeeConfiguration());
            builder.Configurations.Add(new DateOfInterest.DateOfInterestConfiguration());
            builder.Configurations.Add(new DeliveryMethod.DeliveryMethodConfiguration());
            builder.Configurations.Add(new ContributionType.ContributionTypeConfiguration());
            builder.Configurations.Add(new PayrollMetric.PayrollMetricConfiguration());
        }

        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<DateOfInterest> Dates { get; set; }

        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<ContributionType> ContributionTypes { get; set; }
        public DbSet<PayrollMetric> PayrollMetrics { get; set; }
    }

    public class Payroll {
        public int EmployerKey { get; set; } // EmployerKey (Primary key)
        public int EmployeeKey { get; set; } // EmployeeKey (Primary key)
        public int PayPeriodStartDateKey { get; set; } // PayPeriodStartDateKey
        public int PayPeriodEndDateKey { get; set; } // PayPeriodEndDateKey
        public int PayDateKey { get; set; } // PayDateKey (Primary key)
        public int DeliveryMethodKey { get; set; } // DeliveryMethodKey
        public int PayrollMetricKey { get; set; } // PayrollMetricKey (Primary key)
        public int ContributionTypeKey { get; set; } // ContributionTypeKey (Primary key)
        public int PayrollAuditKey { get; set; } // PayrollAuditKey
        public string DocumentId { get; set; } // DocumentID (Primary key) (length: 20)
        public System.DateTime? PayPeriodStartDate { get; set; } // PayPeriodStartDate
        public System.DateTime? PayPeriodEndDate { get; set; } // PayPeriodEndDate
        public System.DateTime? PayDate { get; set; } // PayDate
        public bool CurrentPayPeriodInd { get; set; } // CurrentPayPeriodInd
        public decimal? PayrollMetricRate { get; set; } // PayrollMetricRate
        public decimal? PerPeriodQty { get; set; } // PerPeriodQty
        public decimal? PerPeriodAmt { get; set; } // PerPeriodAmt
        public decimal? YtdQty { get; set; } // YTDQty
        public decimal? YtdAmt { get; set; } // YTDAmt

        public class PayrollConfiguration : EntityTypeConfiguration<Payroll> {
            public PayrollConfiguration() {
                ToTable("Payroll_f");
                HasKey(x => new { x.EmployerKey, x.PayDateKey, x.PayrollMetricKey, x.EmployeeKey, x.ContributionTypeKey, x.DocumentId });

                Property(x => x.EmployerKey).HasColumnName(@"EmployerKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                Property(x => x.EmployeeKey).HasColumnName(@"EmployeeKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                Property(x => x.PayPeriodStartDateKey).HasColumnName(@"PayPeriodStartDateKey").IsRequired().HasColumnType("int");
                Property(x => x.PayPeriodEndDateKey).HasColumnName(@"PayPeriodEndDateKey").IsRequired().HasColumnType("int");
                Property(x => x.PayDateKey).HasColumnName(@"PayDateKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                Property(x => x.DeliveryMethodKey).HasColumnName(@"DeliveryMethodKey").IsRequired().HasColumnType("int");
                Property(x => x.PayrollMetricKey).HasColumnName(@"PayrollMetricKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                Property(x => x.ContributionTypeKey).HasColumnName(@"ContributionTypeKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                Property(x => x.PayrollAuditKey).HasColumnName(@"PayrollAuditKey").IsRequired().HasColumnType("int");
                Property(x => x.DocumentId).HasColumnName(@"DocumentID").IsRequired().HasColumnType("nvarchar").HasMaxLength(20).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                Property(x => x.PayPeriodStartDate).HasColumnName(@"PayPeriodStartDate").IsOptional().HasColumnType("datetime");
                Property(x => x.PayPeriodEndDate).HasColumnName(@"PayPeriodEndDate").IsOptional().HasColumnType("datetime");
                Property(x => x.PayDate).HasColumnName(@"PayDate").IsOptional().HasColumnType("datetime");
                Property(x => x.CurrentPayPeriodInd).HasColumnName(@"CurrentPayPeriodInd").IsRequired().HasColumnType("bit");
                Property(x => x.PayrollMetricRate).HasColumnName(@"PayrollMetricRate").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.PerPeriodQty).HasColumnName(@"PerPeriodQty").IsOptional().HasColumnType("decimal").HasPrecision(6, 2);
                Property(x => x.PerPeriodAmt).HasColumnName(@"PerPeriodAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.YtdQty).HasColumnName(@"YTDQty").IsOptional().HasColumnType("decimal").HasPrecision(6, 2);
                Property(x => x.YtdAmt).HasColumnName(@"YTDAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
            }

        }
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
        public string FlsaStatusDesc { get; set; } // FLSAStatusDesc
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
                Property(x => x.FlsaStatusDesc).HasColumnName(@"FLSAStatusDesc").IsOptional().HasColumnType("nvarchar");
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
    public class DateOfInterest {
        public int DateKey { get; set; } // DateKey (Primary key)
        public System.DateTime? FullDate { get; set; } // FullDate
        public string DayOfWeekName { get; set; } // DayOfWeekName (length: 10)
        public int? DayOfWeekNum { get; set; } // DayOfWeekNum
        public int? DayInMonthNum { get; set; } // DayInMonthNum
        public int? JulianDayNum { get; set; } // JulianDayNum
        public int? WeekInYearNum { get; set; } // WeekInYearNum
        public string MonthName { get; set; } // MonthName (length: 10)
        public int? MonthInYearNum { get; set; } // MonthInYearNum
        public string QuarterName { get; set; } // QuarterName (length: 10)
        public int? QuarterInYearNum { get; set; } // QuarterInYearNum
        public string CalendarYearName { get; set; } // CalendarYearName (length: 4)
        public int? CalendarYearNum { get; set; } // CalendarYearNum
        public bool? WeekdayInd { get; set; } // WeekdayInd
        public bool? LastDayInMonthInd { get; set; } // LastDayInMonthInd
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 6)
        public System.DateTime DwCreateDate { get; set; } // DWCreateDate

        public class DateOfInterestConfiguration : EntityTypeConfiguration<DateOfInterest> {
            public DateOfInterestConfiguration() {
                ToTable("Date_d");
                HasKey(x => x.DateKey);

                Property(x => x.DateKey).HasColumnName(@"DateKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.FullDate).HasColumnName(@"FullDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DayOfWeekName).HasColumnName(@"DayOfWeekName").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.DayOfWeekNum).HasColumnName(@"DayOfWeekNum").IsOptional().HasColumnType("int");
                Property(x => x.DayInMonthNum).HasColumnName(@"DayInMonthNum").IsOptional().HasColumnType("int");
                Property(x => x.JulianDayNum).HasColumnName(@"JulianDayNum").IsOptional().HasColumnType("int");
                Property(x => x.WeekInYearNum).HasColumnName(@"WeekInYearNum").IsOptional().HasColumnType("int");
                Property(x => x.MonthName).HasColumnName(@"MonthName").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.MonthInYearNum).HasColumnName(@"MonthInYearNum").IsOptional().HasColumnType("int");
                Property(x => x.QuarterName).HasColumnName(@"QuarterName").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.QuarterInYearNum).HasColumnName(@"QuarterInYearNum").IsOptional().HasColumnType("int");
                Property(x => x.CalendarYearName).HasColumnName(@"CalendarYearName").IsOptional().IsFixedLength().IsUnicode(false).HasColumnType("char").HasMaxLength(4);
                Property(x => x.CalendarYearNum).HasColumnName(@"CalendarYearNum").IsOptional().HasColumnType("int");
                Property(x => x.WeekdayInd).HasColumnName(@"WeekdayInd").IsOptional().HasColumnType("bit");
                Property(x => x.LastDayInMonthInd).HasColumnName(@"LastDayInMonthInd").IsOptional().HasColumnType("bit");
                Property(x => x.DwCreateUserId).HasColumnName(@"DWCreateUserID").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(6);
                Property(x => x.DwCreateDate).HasColumnName(@"DWCreateDate").IsRequired().HasColumnType("datetime");
            }
        }
    }

    public class DeliveryMethod {
        public int DeliveryMethodKey { get; set; } // DeliveryMethodKey (Primary key)
        public string DeliveryMethodCode { get; set; } // DeliveryMethodCode (length: 10)
        public string DeliveryMethodName { get; set; } // DeliveryMethodName (length: 50)
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 20)
        public System.DateTime? DwCreateDate { get; set; } // DWCreateDate
        public string DwUpdateUserId { get; set; } // DWUpdateUserID (length: 20)
        public System.DateTime? DwUpdateDate { get; set; } // DWUpdateDate
        public int? EtlControlId { get; set; } // ETLControlID

        public class DeliveryMethodConfiguration : EntityTypeConfiguration<DeliveryMethod> {
            public DeliveryMethodConfiguration() {
                ToTable("DeliveryMethod_d");
                HasKey(x => x.DeliveryMethodKey);

                Property(x => x.DeliveryMethodKey).HasColumnName(@"DeliveryMethodKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
                Property(x => x.DeliveryMethodCode).HasColumnName(@"DeliveryMethodCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.DeliveryMethodName).HasColumnName(@"DeliveryMethodName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.DwCreateUserId).HasColumnName(@"DWCreateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwCreateDate).HasColumnName(@"DWCreateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DwUpdateUserId).HasColumnName(@"DWUpdateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwUpdateDate).HasColumnName(@"DWUpdateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.EtlControlId).HasColumnName(@"ETLControlID").IsOptional().HasColumnType("int");
            }
        }
    }

    public class ContributionType {
        public int ContributionTypeKey { get; set; } // ContributionTypeKey (Primary key)
        public string ContributionTypeCode { get; set; } // ContributionTypeCode (length: 10)
        public string ContributionTypeName { get; set; } // ContributionTypeName (length: 50)
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 20)
        public System.DateTime? DwCreateDate { get; set; } // DWCreateDate
        public string DwUpdateUserId { get; set; } // DWUpdateUserID (length: 20)
        public System.DateTime? DwUpdateDate { get; set; } // DWUpdateDate
        public int? EtlControlId { get; set; } // ETLControlID

        public class ContributionTypeConfiguration : EntityTypeConfiguration<ContributionType> {
            public ContributionTypeConfiguration() {
                ToTable("ContributionType_d");
                HasKey(x => x.ContributionTypeKey);

                Property(x => x.ContributionTypeKey).HasColumnName(@"ContributionTypeKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
                Property(x => x.ContributionTypeCode).HasColumnName(@"ContributionTypeCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.ContributionTypeName).HasColumnName(@"ContributionTypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.DwCreateUserId).HasColumnName(@"DWCreateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwCreateDate).HasColumnName(@"DWCreateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DwUpdateUserId).HasColumnName(@"DWUpdateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwUpdateDate).HasColumnName(@"DWUpdateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.EtlControlId).HasColumnName(@"ETLControlID").IsOptional().HasColumnType("int");
            }
        }
    }

    public class PayrollMetric {
        public int PayrollMetricKey { get; set; } // PayrollMetricKey (Primary key)
        public string PayrollMetricCode { get; set; } // PayrollMetricCode (length: 10)
        public string PayrollMetricName { get; set; } // PayrollMetricName (length: 50)
        public string PayrollMetricDesc { get; set; } // PayrollMetricDesc (length: 100)
        public string PayrollCategoryName { get; set; } // PayrollCategoryName (length: 50)
        public string ReportingCategoryCode { get; set; } // ReportingCategoryCode (length: 10)
        public bool PreTaxInd { get; set; } // PreTaxInd
        public string SourceCreateUserId { get; set; } // SourceCreateUserID (length: 20)
        public System.DateTime? SourceCreateDate { get; set; } // SourceCreateDate
        public string SourceUpdateUserId { get; set; } // SourceUpdateUserID (length: 20)
        public System.DateTime? SourceUpdateDate { get; set; } // SourceUpdateDate
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 20)
        public System.DateTime? DwCreateDate { get; set; } // DWCreateDate
        public string DwUpdateUserId { get; set; } // DWUpdateUserID (length: 20)
        public System.DateTime? DwUpdateDate { get; set; } // DWUpdateDate
        public int? EtlControlId { get; set; } // ETLControlID

        public class PayrollMetricConfiguration : EntityTypeConfiguration<PayrollMetric> {
            public PayrollMetricConfiguration() {
                ToTable("PayrollMetric_d");
                HasKey(x => x.PayrollMetricKey);

                Property(x => x.PayrollMetricKey).HasColumnName(@"PayrollMetricKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
                Property(x => x.PayrollMetricCode).HasColumnName(@"PayrollMetricCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PayrollMetricName).HasColumnName(@"PayrollMetricName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.PayrollMetricDesc).HasColumnName(@"PayrollMetricDesc").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
                Property(x => x.PayrollCategoryName).HasColumnName(@"PayrollCategoryName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.ReportingCategoryCode).HasColumnName(@"ReportingCategoryCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PreTaxInd).HasColumnName(@"PreTaxInd").IsRequired().HasColumnType("bit");
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
}