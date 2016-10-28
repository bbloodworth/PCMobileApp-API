using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.BenefitContributions.Models
{
    public class BenefitContribution
    {
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
        public bool CurrentPayPeriodInd;
    }
}