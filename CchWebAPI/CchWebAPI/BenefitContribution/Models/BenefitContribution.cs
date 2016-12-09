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
        [JsonProperty("EmployeeId")]
        public int CchId { get; set; }
        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }
        public DateTime AsOfDate { get; set; }
        public DateTime DWCreateDate { get; set; }
        public string PayrollCategoryName { get; set; }
        public string PayrollMetricName { get; set; }
        public bool PreTaxInd { get; set; }
        public string ContributionTypeCode { get; set; }
        public string ContributionTypeName { get; set; }
        public decimal? PerPeriodAmt { get; set; }
        public decimal? YTDAmt { get; set; }
        public string ReportingCategoryCode { get; set; }
        [JsonProperty("IsCurrent")]
        public bool CurrentPayPeriodInd { get; set; }
        public string PayrollMetricDisplayName { get; set; }


        public class BenefitContributionDetailConfiguration : EntityTypeConfiguration<BenefitContributionDetail> {
            public BenefitContributionDetailConfiguration() {
            }
        }
    }

    public class PercentageElected {
        [JsonIgnore]
        public int CCHID { get; set; }
        [JsonIgnore]
        public string BenefitPlanTypeCode { get; set; }
        public string ContributionName { get; set; }
        public float? Percentage { get; set; }
    }


}