using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CchWebAPI.BenefitContribution.Models;

namespace CchWebAPI.BenefitContribution.Data
{
    public interface IBenefitContributionsRepository
    {
        void Initialize(string connectionString);
        Task<List<BenefitContributionDetail>> GetContributionsByCchIdAsync(int cchid, string categoryCode);
        Task<DateTime> GetMaxPayrollDate();
        Task<String> GetBenefitName(int cchid, string categoryCode);
        Task<List<PercentageElected>> GetPercentageElected(int cchid, string categoryCode);

    }

    public class BenefitContributionsRepository: IBenefitContributionsRepository
    {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<String> GetBenefitName(int cchid, string categoryCode) {
            if (string.IsNullOrEmpty(_connectionString)) {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }

            using (var context = new BenefitContributionsContext(_connectionString)) {

                var x = context.BenefitEnrollments.
                    Join(context.Members,
                    be => be.EnrolledMemberKey, m => m.MemberKey,
                    (be, m) => new {
                        MemberKey = be.EnrolledMemberKey,
                        BenefitPlanOptionKey = be.BenefitPlanOptionKey,
                        CCHID = m.CCHID
                    }).Join(context.BenefitPlanOptions,
                    a => a.BenefitPlanOptionKey, bpo => bpo.BenefitPlanOptionKey,
                    (a, bpo) => new {
                        PayerName = bpo.PayerName,
                        BenefitPlanOptionName = bpo.BenefitPlanOptionName,
                        BenefitPlanTypeCode = bpo.BenefitPlanTypeCode,
                        MemberId = a.MemberKey,
                        CCHID = a.CCHID
                    }).Where(
                        a => 
                        a.CCHID.Equals(cchid)
                        && a.BenefitPlanTypeCode.Equals(categoryCode)
                    ).FirstOrDefault();


                return String.Format("{0} {1}", x.PayerName, categoryCode);

            }


        }

        public async Task<List<PercentageElected>> GetPercentageElected(int cchid, string categoryCode) {
            List<PercentageElected> result = new List<PercentageElected>();

            if (string.IsNullOrEmpty(_connectionString)) {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 1) {
                throw new InvalidOperationException("Invalid CCHID");
            }

            using (var context = new BenefitContributionsContext(_connectionString)) {
                var query = context.BenefitEnrollments.
                    Join(context.Members,
                    be => be.EnrolledMemberKey, m => m.MemberKey,
                    (be, m) => new {
                        MemberKey = be.EnrolledMemberKey,
                        BenefitPlanOptionKey = be.BenefitPlanOptionKey,
                        CCHID = m.CCHID,
                        Percent = be.EmployeeAnnualContributionPct,

                    }).
                    Join(context.BenefitPlanOptions,
                    a => a.BenefitPlanOptionKey, bpo => bpo.BenefitPlanOptionKey,
                    (a, bpo) => new PercentageElected {
                        ContributionName = bpo.BenefitTypeName,
                        Percentage = a.Percent,
                        CCHID = a.CCHID,
                        BenefitPlanTypeCode = bpo.BenefitPlanTypeCode
                    }).
                    Where(
                        a =>
                        a.CCHID.Equals(cchid)
                        && a.BenefitPlanTypeCode.Equals(categoryCode)
                    );
                result = query.ToList();
            }

            return result;
            /*

                 SELECT 
                  BenefitTypeName, EmployeeAnnualContributionPct
                FROM BenefitEnrollment_f f
                INNER JOIN Member_d m
                  ON f.EnrolledMemberKey = m.MemberKey 
                INNER JOIN BenefitPlanOption_d bpo
                  ON f.BenefitPlanOptionKey = bpo.BenefitPlanOptionKey
                WHERE 
                  m.CCHID = 63841
                  AND BenefitPlanTypeCode = '401K'
                             */

            }


        public async Task<DateTime> GetMaxPayrollDate() {
            if (string.IsNullOrEmpty(_connectionString)) {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }

            using (var context = new BenefitContributionsContext(_connectionString)) {
                return context.Payroll.Max(p => p.PayPeriodEndDate);
            }
        }

        public async Task<List<BenefitContributionDetail>> GetContributionsByCchIdAsync(int cchid, string categoryCode)
        {
            if (string.IsNullOrEmpty(_connectionString)){
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 1){
                throw new InvalidOperationException("Invalid CCHID");
            }

            using (var context = new BenefitContributionsContext(_connectionString))
            {
                // TODO remove extraneous info ie FirstName/LastName

                IQueryable<BenefitContributionDetail> contributions =
                    context.Payroll
                    .Join(
                        context.Dates, 
                        p => p.PayDateKey, d => d.DateKey, (p, d) => new 
                        {
                            p.EmployeeKey,
                            p.PayDateKey,
                            p.DeliveryMethodKey,
                            p.ContributionTypeKey,
                            p.PayrollMetricKey,
                            p.PayrollAuditKey,
                            p.PerPeriodAmt,
                            p.YTDAmt,
                            p.CurrentPayPeriodInd,
                            d.DateKey,
                            d.FullDate
                        }).Join(
                            context.EmployeeMembers, 
                            p => p.EmployeeKey, e => e.EmployeeKey, 
                            (p, e) => new 
                            {
                                e.CchId,
                                e.EmployeeFirstName,
                                e.EmployeeLastName,
                                p.EmployeeKey,
                                p.PayDateKey,
                                p.DeliveryMethodKey,
                                p.ContributionTypeKey,
                                p.PayrollMetricKey,
                                p.PayrollAuditKey,
                                p.PerPeriodAmt,
                                p.YTDAmt,
                                p.CurrentPayPeriodInd,
                                p.DateKey,
                                p.FullDate
                            })
                            .Join(
                                context.DeliveryMethods, 
                                p => p.DeliveryMethodKey, dm => dm.DeliveryMethodKey, 
                                (p, dm) => new 
                                {
                                    p.CchId,
                                    p.EmployeeFirstName,
                                    p.EmployeeLastName,
                                    p.EmployeeKey,
                                    p.PayDateKey,
                                    p.DeliveryMethodKey,
                                    p.ContributionTypeKey,
                                    p.PayrollMetricKey,
                                    p.PayrollAuditKey,
                                    p.PerPeriodAmt,
                                    p.YTDAmt,
                                    p.CurrentPayPeriodInd,
                                    p.DateKey,
                                    p.FullDate,
                                    dm.DeliveryMethodName
                                })
                                .Join(context.ContributionTypes, 
                                    p => p.ContributionTypeKey, ct => ct.ContributionTypeKey, 
                                    (p, ct) => new
                                    {
                                        p.CchId,
                                        p.EmployeeFirstName,
                                        p.EmployeeLastName,
                                        p.EmployeeKey,
                                        p.PayDateKey,
                                        p.DeliveryMethodKey,
                                        p.ContributionTypeKey,
                                        p.PayrollMetricKey,
                                        p.PayrollAuditKey,
                                        p.PerPeriodAmt,
                                        p.YTDAmt,
                                        p.CurrentPayPeriodInd,
                                        p.DateKey,
                                        p.FullDate,
                                        p.DeliveryMethodName,
                                        ct.ContributionTypeCode,
                                        ct.ContributionTypeName
                                    })
                                    .Join(context.PayrollMetrics, 
                                        p => p.PayrollMetricKey, pm => pm.PayrollMetricKey, 
                                        (p, pm) => new 
                                        {
                                            p.CchId,
                                            p.EmployeeFirstName,
                                            p.EmployeeLastName,
                                            p.EmployeeKey,
                                            p.PayDateKey,
                                            p.DeliveryMethodKey,
                                            p.ContributionTypeKey,
                                            p.PayrollMetricKey,
                                            p.PayrollAuditKey,
                                            p.PerPeriodAmt,
                                            p.YTDAmt,
                                            p.CurrentPayPeriodInd,
                                            p.DateKey,
                                            p.FullDate,
                                            p.DeliveryMethodName,
                                            p.ContributionTypeCode,
                                            p.ContributionTypeName,
                                            pm.PayrollMetricCode,
                                            pm.PayrollMetricName,
                                            pm.PayrollCategoryName,
                                            pm.ReportingCategoryCode,
                                            pm.PreTaxInd
                                        })
                                        .Join(context.PayrollAudits, 
                                            p => p.PayrollAuditKey, pa => pa.PayrollAuditKey,
                    (p, pa) => new BenefitContributionDetail
                    {
                        CchId = p.CchId,
                        AsOfDate = p.FullDate,
                        ContributionTypeCode = p.ContributionTypeCode,
                        ContributionTypeName = p.ContributionTypeName,
                        DWCreateDate = pa.DWCreateDate,
                        EmployeeFirstName = p.EmployeeFirstName,
                        EmployeeLastName = p.EmployeeLastName,
                        PayrollCategoryName = p.PayrollCategoryName,
                        PayrollMetricName = p.PayrollMetricName,
                        PerPeriodAmt = p.PerPeriodAmt,
                        PreTaxInd = p.PreTaxInd,
                        YTDAmt = p.YTDAmt,
                        ReportingCategoryCode = p.ReportingCategoryCode,
                        CurrentPayPeriodInd = p.CurrentPayPeriodInd
                                            }
                        ).Where(
                            p => p.CchId == cchid 
                            && ( string.IsNullOrEmpty(categoryCode) || p.ReportingCategoryCode.Equals(categoryCode) ) 
                            && p.CurrentPayPeriodInd
                        )
                    ;

                return contributions.ToList();
            }
        }
    }
}

