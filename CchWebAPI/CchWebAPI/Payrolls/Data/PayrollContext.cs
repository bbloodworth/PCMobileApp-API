using ClearCost.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace CchWebAPI.Payrolls.Data
{
    public class PayrollContext : ClearCostContext<PayrollContext>
    {
        public PayrollContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder)
        {
            builder.Configurations.Add(new Payroll.PayrollConfiguration());
            builder.Configurations.Add(new Employee.EmployeeConfiguration());
            builder.Configurations.Add(new DateOfInterest.DateOfInterestConfiguration());
        }

        public DbSet<Payroll> Payrolls { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<DateOfInterest> Dates { get; set; }
    }

    [Table("Payroll_f")]
    public class Payroll {
        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployerKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int EmployeeKey { get; set; }

        public int PayPeriodStartDateKey { get; set; }

        public int PayPeriodEndDateKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayDateKey { get; set; }

        public int DeliveryMethodKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PayrollMetricKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContributionTypeKey { get; set; }

        public int PayrollAuditKey { get; set; }

        [Key]
        [System.ComponentModel.DataAnnotations.Schema.Column(Order = 5)]
        [StringLength(20)]
        public string DocumentID { get; set; }

        public DateTime? PayPeriodStartDate { get; set; }

        public DateTime? PayPeriodEndDate { get; set; }

        public DateTime? PayDate { get; set; }

        public bool CurrentPayPeriodInd { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "money")]
        public decimal? PayrollMetricRate { get; set; }

        public decimal? PerPeriodQty { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "money")]
        public decimal? PerPeriodAmt { get; set; }

        public decimal? YTDQty { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "money")]
        public decimal? YTDAmt { get; set; }

        public class PayrollConfiguration : EntityTypeConfiguration<Payroll> {
            public PayrollConfiguration() {
                ToTable("Payroll_f");
            }

        }
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

        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "date")]
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
            }
        }
    }
    [Table("Date_d")]
    public class DateOfInterest {
        [Key]
        public int DateKey { get; set; }

        public DateTime FullDate { get; set; }

        [StringLength(50)]
        public string DayOfWeekName { get; set; }

        [StringLength(20)]
        public string DayOfWeekNum { get; set; }

        [StringLength(20)]
        public string DayInMonthNum { get; set; }

        [StringLength(20)]
        public string JulianDayNum { get; set; }

        [StringLength(20)]
        public string WeekInYearNum { get; set; }

        [StringLength(50)]
        public string MonthName { get; set; }

        [StringLength(20)]
        public string MonthInYearNum { get; set; }

        [StringLength(50)]
        public string QuarterName { get; set; }

        [StringLength(20)]
        public string QuarterInYearNum { get; set; }

        [StringLength(50)]
        public string CalendarYearName { get; set; }

        [StringLength(20)]
        public string CalendarYearNum { get; set; }

        public bool WeekdayInd { get; set; }

        public bool LastDayInMonthInd { get; set; }

        [StringLength(20)]
        public string DWCreateUserID { get; set; }

        public DateTime? DWCreateDate { get; set; }

        public int? ETLControlID { get; set; }

        public class DateOfInterestConfiguration : EntityTypeConfiguration<DateOfInterest> {
            public DateOfInterestConfiguration() {
                ToTable("Date_d");
                HasKey(k => k.DateKey);
            }
        }
    }
}