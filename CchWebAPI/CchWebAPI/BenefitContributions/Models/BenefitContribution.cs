﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace CchWebAPI.BenefitContributions.Models
{
    public class BenefitContribution {
        public string BenefitContributionType;
        public DateTime PayrollFileReceivedDate;
        public List<BenefitContributionDetail> BenefitContributions;
        public List<PercentageElected> PercentageElected;
    }

    public class BenefitContributionDetail
    {
        [JsonProperty("EmployeeId")]
        public int CCHID;
        public string EmployeeFirstName;
        public string EmployeeLastName;
        public DateTime AsOfDate;
        public DateTime DWCreateDate;
        public string PayrollCategoryName;
        public string PayrollMetricName;
        public bool PreTaxInd;
        public string ContributionTypeCode;
        public string ContributionTypeName;
        public decimal? PerPeriodAmt;
        public decimal? YTDAmt;
        public string ReportingCategoryCode;
        [JsonProperty("IsCurrent")]
        public bool CurrentPayPeriodInd;


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