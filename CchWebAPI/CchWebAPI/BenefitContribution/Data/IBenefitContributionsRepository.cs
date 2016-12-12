using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CchWebAPI.BenefitContribution.Models;
using System.Data.Entity;

namespace CchWebAPI.BenefitContribution.Data
{
    public interface IBenefitContributionsRepository
    {
        void Initialize(string connectionString);
        Task<List<BenefitContributionDetail>> GetContributionsByCchIdAsync(int cchid, string categoryCode);
        Task<DateTime> GetMaxPayrollDateAsync();
        Task<List<String>> GetBenefitNameAsync(int cchid, string categoryCode);
        Task<List<PercentageElected>> GetPercentageElectedAsync(int cchid, string categoryCode);

    }

    public class BenefitContributionsRepository: IBenefitContributionsRepository
    {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<String>> GetBenefitNameAsync(int cchid, string categoryCode) {
            if (string.IsNullOrEmpty(_connectionString)) {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }

            List<String> headers = new List<String>();

            using (var context = new BenefitContributionsContext(_connectionString)) {

                var res = await context.BenefitEnrollments.
                    Join(context.Members,
                        be => be.EnrolledMemberKey, m => m.MemberKey,
                        (be, m) => new {
                            MemberKey = be.EnrolledMemberKey,
                            BenefitPlanOptionKey = be.BenefitPlanOptionKey,
                            CCHID = m.CCHID })
                    .Join(context.BenefitPlanOptions,
                        a => a.BenefitPlanOptionKey, bpo => bpo.BenefitPlanOptionKey,
                        (a, bpo) => new {
                            PayerName = bpo.PayerName,
                            BenefitTypeCode = bpo.BenefitTypeCode,
                            BenefitPlanOptionName = bpo.BenefitPlanOptionName,
                            BenefitPlanTypeCode = bpo.BenefitPlanTypeCode,
                            MemberId = a.MemberKey,
                            CCHID = a.CCHID,
                            BenefitTypeName = bpo.BenefitTypeName
                        }
                        )
                    .Where(
                        a => 
                        a.CCHID.Equals(cchid)
                        && a.BenefitTypeCode.Equals(categoryCode)
                    ).FirstOrDefaultAsync();

                String payerName = String.Empty;
                String benefitTypeName = categoryCode;

                if (res != null) {
                    payerName = res.PayerName;
                    benefitTypeName = res.BenefitTypeName;
                }
                headers.Add(payerName);
                headers.Add(benefitTypeName);
                return headers;

            }


        }

        public async Task<List<PercentageElected>> GetPercentageElectedAsync(int cchid, string categoryCode) {
            List<PercentageElected> results = new List<PercentageElected>();

            if (string.IsNullOrEmpty(_connectionString)) {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 1) {
                throw new InvalidOperationException("Invalid CCHID");
            }

            using (var context = new BenefitContributionsContext(_connectionString)) {
                results = await context.BenefitEnrollments
                    .Join(context.Members,
                    be => be.EnrolledMemberKey, m => m.MemberKey,
                    (be, m) => new {
                        MemberKey = be.EnrolledMemberKey,
                        BenefitPlanOptionKey = be.BenefitPlanOptionKey,
                        CCHID = m.CCHID,
                        Percent = be.EmployeeAnnualContributionPct,

                    })
                    .Join(context.BenefitPlanOptions,
                    a => a.BenefitPlanOptionKey, bpo => bpo.BenefitPlanOptionKey,
                    (a, bpo) => new PercentageElected {
                        ContributionName = bpo.BenefitPlanOptionName,
                        Percentage = a.Percent,
                        CCHID = a.CCHID,
                        BenefitPlanTypeCode = bpo.BenefitPlanTypeCode
                    })
                    .Where(
                        a =>
                        a.CCHID.Equals(cchid)
                        && a.BenefitPlanTypeCode.Equals(categoryCode)
                    ).ToListAsync();
                
            }

            return results;

            }


        public async Task<DateTime> GetMaxPayrollDateAsync() {
            if (string.IsNullOrEmpty(_connectionString)) {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }

            using (var context = new BenefitContributionsContext(_connectionString)) {
                return await context.Payroll.MaxAsync(p => p.PayPeriodEndDate);
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

            List<BenefitContributionDetail> contributions = new List<BenefitContributionDetail>();

            using (var context = new BenefitContributionsContext(_connectionString))
            {
                // TODO remove extraneous info ie FirstName/LastName

                contributions = await context.Payroll
                    .Join(
                        context.Dates,
                        p => p.PayDateKey, d => d.DateKey, (p, d) => new {
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
                            (p, e) => new {
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
                                (p, dm) => new {
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
                                    (p, ct) => new {
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
                                        (p, pm) => new {
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
                                            pm.DashboardDisplayName,
                                            pm.PayrollMetricCode,
                                            pm.PayrollMetricName,
                                            pm.PayrollCategoryName,
                                            pm.ReportingCategoryCode,
                                            pm.PreTaxInd
                                        })
                                        .Join(context.PayrollAudits,
                                            p => p.PayrollAuditKey, pa => pa.PayrollAuditKey,
                    (p, pa) => new BenefitContributionDetail {
                        CchId = p.CchId,
                        AsOfDate = p.FullDate,
                        ContributionTypeCode = p.ContributionTypeCode,
                        ContributionTypeName = p.ContributionTypeName,
                        DWCreateDate = pa.DWCreateDate,
                        EmployeeFirstName = p.EmployeeFirstName,
                        EmployeeLastName = p.EmployeeLastName,
                        PayrollCategoryName = p.PayrollCategoryName,
                        PayrollMetricName = p.PayrollMetricName,
                        PayrollMetricCode = p.PayrollMetricCode,
                        PayrollMetricDisplayName = p.DashboardDisplayName,
                        PerPeriodAmt = p.PerPeriodAmt,
                        PreTaxInd = p.PreTaxInd,
                        YTDAmt = p.YTDAmt,
                        ReportingCategoryCode = p.ReportingCategoryCode,
                        CurrentPayPeriodInd = p.CurrentPayPeriodInd
                    }
                        ).Where(
                            p => p.CchId == cchid
                            && (string.IsNullOrEmpty(categoryCode) || p.ReportingCategoryCode.Equals(categoryCode))
                            && p.CurrentPayPeriodInd
                        ).ToListAsync();
                    ;

                return contributions;
            }
        }
    }
}

