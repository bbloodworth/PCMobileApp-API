using ClearCost.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.SqlClient;

namespace CchWebAPI.MedicalPlan.Data {
    public class MedicalPlanContext : ClearCostContext<MedicalPlanContext> {
        public MedicalPlanContext(string connectionString) : base(new SqlConnection(connectionString)) { }

        public override void ConfigureModel(DbModelBuilder builder) {
            builder.Configurations.Add(new MedicalPlanOption.MedicalPlanOptionConfiguration());
            builder.Configurations.Add(new MedicalPlanAccumulation.MedicalPlanAccumulationConfiguration());
            builder.Configurations.Add(new PlanYear.PlanYearConfiguration());
        }

        public DbSet<MedicalPlanOption> MedicalPlanOptions { get; set; }
        public DbSet<MedicalPlanAccumulation> MedicalPlanAccumulations { get; set; }
        public DbSet<PlanYear> PlanYears { get; set; }

    }

    public class MedicalPlanAccumulation {
        public int EmployerKey { get; set; } // EmployerKey (Primary key)
        public int EnrolledMemberKey { get; set; } // EnrolledMemberKey (Primary key)
        public int BenefitPlanOptionKey { get; set; } // BenefitPlanOptionKey (Primary key)
        public int CoverageTierKey { get; set; } // CoverageTierKey (Primary key)
        public int PlanYearKey { get; set; } // PlanYearKey (Primary key)
        //public int AsOfDateKey { get; set; } // AsOfDateKey (Primary key)
        public string AsOfDateKey { get; set; } // AsOfDateKey (Primary key)
        //public int MedicalPlanAccumulationAuditKey { get; set; } // MedicalPlanAccumulationAuditKey
        public decimal? IndividualDeductiblePaidAmt { get; set; } // IndividualDeductiblePaidAmt
        public decimal? FamilyDeductiblePaidAmt { get; set; } // FamilyDeductiblePaidAmt
        public decimal? IndividualMaxOopPaidAmt { get; set; } // IndividualMaxOOPPaidAmt
        public decimal? FamilyMaxOopPaidAmt { get; set; } // FamilyMaxOOPPaidAmt
        public decimal? IndividualDeductibleRemainingAmt { get; set; } // IndividualDeductibleRemainingAmt
        public decimal? FamilyDeductibleRemainingAmt { get; set; } // FamilyDeductibleRemainingAmt
        public decimal? IndividualMaxOopRemainingAmt { get; set; } // IndividualMaxOOPRemainingAmt
        public decimal? FamilyMaxOopRemainingAmt { get; set; } // FamilyMaxOOPRemainingAmt
        public decimal? IndividualDeductibleAmt { get; set; } // IndividualDeductibleAmt
        public decimal? FamilyDeductibleAmt { get; set; } // FamilyDeductibleAmt
        public decimal? IndividualMaxOOPAmt { get; set; } // IndividualMaxOOPAmt
        public decimal? FamilyMaxOOPAmt { get; set; } // FamilyMaxOOPAmt

        //Relationships
        public virtual PlanYear PlanYear { get; set; }

        public class MedicalPlanAccumulationConfiguration : EntityTypeConfiguration<MedicalPlanAccumulation> {
            public MedicalPlanAccumulationConfiguration() {
                ToTable("MedicalPlanAccumulation_f");
                HasKey(x => new { x.EnrolledMemberKey, x.BenefitPlanOptionKey, x.PlanYearKey, x.AsOfDateKey, x.EmployerKey, x.CoverageTierKey });

                Property(x => x.EmployerKey).HasColumnName(@"EmployerKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.EnrolledMemberKey).HasColumnName(@"EnrolledMemberKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.BenefitPlanOptionKey).HasColumnName(@"BenefitPlanOptionKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.CoverageTierKey).HasColumnName(@"CoverageTierKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.PlanYearKey).HasColumnName(@"PlanYearKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                //Property(x => x.AsOfDateKey).HasColumnName(@"AsOfDateKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.AsOfDateKey).HasColumnName(@"AsOfDateKey").HasColumnType("nvarchar").HasMaxLength(30);
                //Property(x => x.MedicalPlanAccumulationAuditKey).HasColumnName(@"MedicalPlanAccumulationAuditKey").IsRequired().HasColumnType("int");
                Property(x => x.IndividualDeductiblePaidAmt).HasColumnName(@"IndividualDeductiblePaidAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.FamilyDeductiblePaidAmt).HasColumnName(@"FamilyDeductiblePaidAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.IndividualMaxOopPaidAmt).HasColumnName(@"IndividualMaxOOPPaidAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.FamilyMaxOopPaidAmt).HasColumnName(@"FamilyMaxOOPPaidAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.IndividualDeductibleRemainingAmt).HasColumnName(@"IndividualDeductibleRemainingAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.FamilyDeductibleRemainingAmt).HasColumnName(@"FamilyDeductibleRemainingAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.IndividualMaxOopRemainingAmt).HasColumnName(@"IndividualMaxOOPRemainingAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.FamilyMaxOopRemainingAmt).HasColumnName(@"FamilyMaxOOPRemainingAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.IndividualDeductibleAmt).HasColumnName(@"IndividualDeductibleAmt").IsOptional().HasColumnType("money").HasPrecision(18, 20);
                Property(x => x.FamilyDeductibleAmt).HasColumnName(@"FamilyDeductibleAmt").IsOptional().HasColumnType("money").HasPrecision(18, 20);
                Property(x => x.IndividualMaxOOPAmt).HasColumnName(@"IndividualMaxOOPAmt").IsOptional().HasColumnType("money").HasPrecision(18, 20);
                Property(x => x.FamilyMaxOOPAmt).HasColumnName(@"FamilyMaxOOPAmt").IsOptional().HasColumnType("money").HasPrecision(18, 20);

                //Relationships
                HasRequired(t => t.PlanYear);
            }

        }
    }

    public class MedicalPlanOption {
        public int BenefitPlanOptionKey { get; set; } // BenefitPlanOptionKey (Primary key)
        public string SourcePlanOptionCode { get; set; } // SourcePlanOptionCode (length: 10)
        public string ContractPrefixCode { get; set; } // ContractPrefixCode (length: 10)
        public string BenefitTypeName { get; set; } // BenefitTypeName (length: 50)
        public string PayrollMetricCode { get; set; } // PayrollMetricCode (length: 10)
        public string BenefitPlanOptionName { get; set; } // BenefitPlanOptionName (length: 50)
        public string BenefitClassName { get; set; } // BenefitClassName (length: 50)
        public string BenefitPlanName { get; set; } // BenefitPlanName (length: 50)
        public string BenefitPlanTypeName { get; set; } // BenefitPlanTypeName (length: 50)
        public string BenefitPlanTypeCode { get; set; } // BenefitPlanTypeCode (length: 10)
        public int? PayerKey { get; set; } // PayerKey
        public string PayerName { get; set; } // PayerName (length: 50)
        public int? IssuerNum { get; set; } // IssuerNum
        //public string NetworkName { get; set; } // NetworkName (length: 50)
        //public string MedicalGroupNum { get; set; } // MedicalGroupNum (length: 20)
        public System.DateTime? AccumulatorStartDate { get; set; } // AccumulatorStartDate
        public decimal? IndividualDeductibleAmt { get; set; } // IndividualDeductibleAmt
        public decimal? FamilyDeductibleAmt { get; set; } // FamilyDeductibleAmt
        public decimal? OonIndividualDeductibleAmt { get; set; } // OONIndividualDeductibleAmt
        public decimal? OonFamilyDeductibleAmt { get; set; } // OONFamilyDeductibleAmt
        public decimal? CoinsurancePct { get; set; } // CoinsurancePct
        public decimal? OonCoinsurancePct { get; set; } // OONCoinsurancePct
        public decimal? CopayAmt { get; set; } // CopayAmt
        public decimal? OonCopayAmt { get; set; } // OONCopayAmt
        public decimal? OutsideUsCopayAmt { get; set; } // OutsideUSCopayAmt
        public decimal? IndividualMaxOopAmt { get; set; } // IndividualMaxOOPAmt
        public decimal? FamilyMaxOopAmt { get; set; } // FamilyMaxOOPAmt
        public decimal? OonIndividualMaxOopAmt { get; set; } // OONIndividualMaxOOPAmt
        public decimal? OonFamilyMaxOopAmt { get; set; } // OONFamilyMaxOOPAmt
        public decimal? SpouseSurchargeAmt { get; set; } // SpouseSurchargeAmt
        public decimal? InNetworkOopMaxAmount { get; set; } // InNetworkOOPMaxAmount
        public int RxPlanOptionKey { get; set; } // RxPlanOptionKey
        public int DentalPlanOptionKey { get; set; } // DentalPlanOptionKey
        public int VisionPlanOptionKey { get; set; } // VisionPlanOptionKey
        public bool IdCardInd { get; set; } // IDCardInd
        public string SourceCreateUserId { get; set; } // SourceCreateUserID (length: 20)
        public System.DateTime? SourceCreateDate { get; set; } // SourceCreateDate
        public string SourceUpdateUserId { get; set; } // SourceUpdateUserID (length: 20)
        public System.DateTime? SourceUpdateDate { get; set; } // SourceUpdateDate
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 20)
        public System.DateTime? DwCreateDate { get; set; } // DWCreateDate
        public string DwUpdateUserId { get; set; } // DWUpdateUserID (length: 20)
        public System.DateTime? DwUpdateDate { get; set; } // DWUpdateDate
        public int? EtlControlId { get; set; } // ETLControlID

        public class MedicalPlanOptionConfiguration : EntityTypeConfiguration<MedicalPlanOption> {
            public MedicalPlanOptionConfiguration() {
                ToTable("MedicalPlanOption_d");
                HasKey(x => x.BenefitPlanOptionKey);

                Property(x => x.BenefitPlanOptionKey).HasColumnName(@"BenefitPlanOptionKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
                Property(x => x.SourcePlanOptionCode).HasColumnName(@"SourcePlanOptionCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.ContractPrefixCode).HasColumnName(@"ContractPrefixCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.BenefitTypeName).HasColumnName(@"BenefitTypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.PayrollMetricCode).HasColumnName(@"PayrollMetricCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.BenefitPlanOptionName).HasColumnName(@"BenefitPlanOptionName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.BenefitClassName).HasColumnName(@"BenefitClassName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.BenefitPlanName).HasColumnName(@"BenefitPlanName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.BenefitPlanTypeName).HasColumnName(@"BenefitPlanTypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.BenefitPlanTypeCode).HasColumnName(@"BenefitPlanTypeCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(10);
                Property(x => x.PayerKey).HasColumnName(@"PayerKey").IsOptional().HasColumnType("int");
                Property(x => x.PayerName).HasColumnName(@"PayerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.IssuerNum).HasColumnName(@"IssuerNum").IsOptional().HasColumnType("int");
                //Property(x => x.NetworkName).HasColumnName(@"NetworkName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                //Property(x => x.MedicalGroupNum).HasColumnName(@"MedicalGroupNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.AccumulatorStartDate).HasColumnName(@"AccumulatorStartDate").IsOptional().HasColumnType("datetime");
                Property(x => x.IndividualDeductibleAmt).HasColumnName(@"IndividualDeductibleAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.FamilyDeductibleAmt).HasColumnName(@"FamilyDeductibleAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.OonIndividualDeductibleAmt).HasColumnName(@"OONIndividualDeductibleAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.OonFamilyDeductibleAmt).HasColumnName(@"OONFamilyDeductibleAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.CoinsurancePct).HasColumnName(@"CoinsurancePct").IsOptional().HasColumnType("decimal").HasPrecision(6, 4);
                Property(x => x.OonCoinsurancePct).HasColumnName(@"OONCoinsurancePct").IsOptional().HasColumnType("decimal").HasPrecision(6, 4);
                Property(x => x.CopayAmt).HasColumnName(@"CopayAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.OonCopayAmt).HasColumnName(@"OONCopayAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.OutsideUsCopayAmt).HasColumnName(@"OutsideUSCopayAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.IndividualMaxOopAmt).HasColumnName(@"IndividualMaxOOPAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.FamilyMaxOopAmt).HasColumnName(@"FamilyMaxOOPAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.OonIndividualMaxOopAmt).HasColumnName(@"OONIndividualMaxOOPAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.OonFamilyMaxOopAmt).HasColumnName(@"OONFamilyMaxOOPAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.SpouseSurchargeAmt).HasColumnName(@"SpouseSurchargeAmt").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.InNetworkOopMaxAmount).HasColumnName(@"InNetworkOOPMaxAmount").IsOptional().HasColumnType("money").HasPrecision(19, 4);
                Property(x => x.RxPlanOptionKey).HasColumnName(@"RxPlanOptionKey").IsRequired().HasColumnType("int");
                Property(x => x.DentalPlanOptionKey).HasColumnName(@"DentalPlanOptionKey").IsRequired().HasColumnType("int");
                Property(x => x.VisionPlanOptionKey).HasColumnName(@"VisionPlanOptionKey").IsRequired().HasColumnType("int");
                Property(x => x.IdCardInd).HasColumnName(@"IDCardInd").IsRequired().HasColumnType("bit");
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
    public class PlanYear {
        public int PlanYearKey { get; set; } // PlanYearKey (Primary key)
        public string PlanYearNum { get; set; } // PlanYearNum (length: 20)
        public string PlanYearName { get; set; } // PlanYearName (length: 50)
        public System.DateTime? PlanYearStartDate { get; set; } // PlanYearStartDate
        public System.DateTime? PlanYearEndDate { get; set; } // PlanYearEndDate
        public System.DateTime? OpenEnrollmentStartDate { get; set; } // OpenEnrollmentStartDate
        public System.DateTime? OpenEnrollmentEndDate { get; set; } // OpenEnrollmentEndDate
        public string DwCreateUserId { get; set; } // DWCreateUserID (length: 20)
        public System.DateTime? DwCreateDate { get; set; } // DWCreateDate
        public int? EtlControlId { get; set; } // ETLControlID

        //Relationships
        public virtual ICollection<MedicalPlanAccumulation> MedicalPlanAccumulations { get; set; }

        public class PlanYearConfiguration : EntityTypeConfiguration<PlanYear> {
            public PlanYearConfiguration() {
                ToTable("PlanYear_d");
                HasKey(x => x.PlanYearKey);

                Property(x => x.PlanYearKey).HasColumnName(@"PlanYearKey").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
                Property(x => x.PlanYearNum).HasColumnName(@"PlanYearNum").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.PlanYearName).HasColumnName(@"PlanYearName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
                Property(x => x.PlanYearStartDate).HasColumnName(@"PlanYearStartDate").IsOptional().HasColumnType("datetime");
                Property(x => x.PlanYearEndDate).HasColumnName(@"PlanYearEndDate").IsOptional().HasColumnType("datetime");
                Property(x => x.OpenEnrollmentStartDate).HasColumnName(@"OpenEnrollmentStartDate").IsOptional().HasColumnType("datetime");
                Property(x => x.OpenEnrollmentEndDate).HasColumnName(@"OpenEnrollmentEndDate").IsOptional().HasColumnType("datetime");
                Property(x => x.DwCreateUserId).HasColumnName(@"DWCreateUserID").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
                Property(x => x.DwCreateDate).HasColumnName(@"DWCreateDate").IsOptional().HasColumnType("datetime");
                Property(x => x.EtlControlId).HasColumnName(@"ETLControlID").IsOptional().HasColumnType("int");
            }
        }
    }
}