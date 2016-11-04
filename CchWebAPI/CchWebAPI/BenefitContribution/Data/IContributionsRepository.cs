using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CchWebAPI.BenefitContribution.Models;

namespace CchWebAPI.BenefitContribution.Data
{
    public interface IContributionsRepository
    {
        void Initialize(string connectionString);
        Task<List<Models.BenefitContribution>> GetContributionsByCchIdAsync(int cchid, string categoryCode);
    }

    public class ContributionsRepository: IContributionsRepository
    {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString)
        {
            _connectionString = connectionString;
            //_connectionString = "Data Source=KERMITDB\\MSPIGGY;Initial Catalog=CCH_DemoDWH;Trusted_Connection=true;Asynchronous Processing=True; MultipleActiveResultSets=true";
        }

        public async Task<List<Models.BenefitContribution>> GetContributionsByCchIdAsync(int cchid, string categoryCode)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 1)
            {
                throw new InvalidOperationException("Invalid CCHID");
            }

            // for reference only
            string sqlQuery = @"SELECT
                e.CCHID
                ,e.EmployeeFirstName
                ,e.EmployeeLastName
                ,d.FullDate as AsOfDate
                ,pa.DWCreateDate
                ,pm.PayrollCategoryName
                ,pm.PayrollMetricName
                ,pm.PreTaxInd
                ,ct.ContributionTypeCode
                ,ct.ContributionTypeName
                ,f.PerPeriodAmt
                ,f.YTDAmt
                FROM
                Payroll_f f
                INNER JOIN Date_d d on f.PayDateKey = d.DateKey
                INNER JOIN Employee_d e on f.EmployeeKey = e.EmployeeKey
                INNER JOIN DeliveryMethod_d dm on f.DeliveryMethodKey = dm.DeliveryMethodKey
                INNER JOIN ContributionType_d ct on f.ContributionTypeKey = ct.ContributionTypeKey
                INNER JOIN PayrollMetric_d pm on f.PayrollMetricKey = pm.PayrollMetricKey
                INNER JOIN PayrollAudit_d pa on f.PayrollAuditKey = pa.PayrollAuditKey
                WHERE
                CCHID = 63841
                AND pm.ReportingCategoryCode = '401K'
                AND f.CurrentPayPeriodInd = 1";

            //var contributions = await context.Database.SqlQuery<BenefitContribution>(sqlQuery).ToListAsync();

            using (var context = new ContributionsContext(_connectionString))
            {
                IQueryable<Models.BenefitContribution> contributions =
                    context.Payrolls
                    .Join(
                        context.DatesOfInterest, 
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
                                e.Cchid,
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
                                    p.Cchid,
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
                                        p.Cchid,
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
                                            p.Cchid,
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
                                            (p, pa) => new Models.BenefitContribution
                                            {
                                                CCHID = p.Cchid,
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
                        ).Where(p => p.CCHID == cchid && 
                                ( string.IsNullOrEmpty(categoryCode) || 
                                  p.ReportingCategoryCode.Equals(categoryCode) ) && 
                                p.CurrentPayPeriodInd)
                    ;

                return contributions.ToList();
            }
        }
    }
}

