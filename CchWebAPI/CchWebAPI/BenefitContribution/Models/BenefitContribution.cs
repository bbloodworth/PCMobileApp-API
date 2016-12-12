using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace CchWebAPI.BenefitContribution.Models
{
    public class BenefitContribution {
        public string PayerName { get; set; }
        public string BenefitTypeName { get; set; }
        public DateTime PayrollFileReceivedDate { get; set; }
        public List<BenefitContributionDetail> BenefitContributions { get; set; }
        public List<PercentageElected> PercentageElected { get; set; }
    }

    public class BenefitContributionDetail
    {
        [JsonIgnore]
        public int CchId;
        [JsonIgnore]
        public string EmployeeFirstName;
        [JsonIgnore]
        public string EmployeeLastName;
        [JsonIgnore]
        public DateTime AsOfDate;
        [JsonIgnore]
        public DateTime DWCreateDate;
        public string PayrollCategoryName;
        public string PayrollMetricName;
        public bool PreTaxInd;
        public string ContributionTypeCode;
        public string ContributionTypeName;
        public decimal? PerPeriodAmt;
        public decimal? YTDAmt;
        public string ReportingCategoryCode;
        [JsonIgnore]
        public bool CurrentPayPeriodInd;
        public string PayrollMetricCode;
        public string PayrollMetricDisplayName;


        public class BenefitContributionDetailConfiguration : EntityTypeConfiguration<BenefitContributionDetail> {
            public BenefitContributionDetailConfiguration() {
            }
        }
    }

    public class PercentageElected {
        [JsonIgnore]
        public int CCHID { get; set; }
        public string BenefitPlanTypeCode { get; set; }
        public string BenefitTypeCode { get; set; }
        public string ContributionName { get; set; }
        public float? Percentage { get; set; }
    }

}