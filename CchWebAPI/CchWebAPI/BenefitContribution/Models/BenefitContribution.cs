using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace CchWebAPI.BenefitContribution.Models
{
    public class BenefitContribution {
        public string PayerName;
        public string BenefitTypeName;
        public DateTime PayrollFileReceivedDate;
        public List<BenefitContributionDetail> BenefitContributions;
        public List<PercentageElected> PercentageElected;
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
        public string PayrollMetricDisplayName;


        public class BenefitContributionDetailConfiguration : EntityTypeConfiguration<BenefitContributionDetail> {
            public BenefitContributionDetailConfiguration() {
            }
        }
    }

    public class PercentageElected {
        [JsonIgnore]
        public int CCHID;
        [JsonIgnore]
        public string BenefitPlanTypeCode;
        public string ContributionName;
        public float? Percentage;
    }


}