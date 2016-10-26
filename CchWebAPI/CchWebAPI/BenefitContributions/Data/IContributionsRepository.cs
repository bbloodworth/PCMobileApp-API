using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CchWebAPI.BenefitContributions.Models;

namespace CchWebAPI.BenefitContributions.Data
{
    public interface IContributionsRepository
    {
        void Initialize(string connectionString);
        List<BenefitContribution> GetContributionsByCchId(int cchid);
        Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchid, string categoryCode);
    }

    public class ContributionsRepository: IContributionsRepository
    {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString)
        {
            _connectionString = connectionString;
            _connectionString = "Data Source=KERMITDB\\MSPIGGY;Initial Catalog=CCH_DemoDWH;Trusted_Connection=true;Asynchronous Processing=True; MultipleActiveResultSets=true";
        }

        public async Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchid, string categoryCode)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 1)
            {
                throw new InvalidOperationException("Invalid CCHID");
            }

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
                IQueryable<BenefitContribution> contributions =
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
                                e.CCHID,
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
                                    p.CCHID,
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
                                        p.CCHID,
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
                                            p.CCHID,
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
                                            (p, pa) => new BenefitContribution
                                            {
                                                CCHID = p.CCHID,
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
                                ( categoryCode.Equals(string.Empty) || 
                                  p.ReportingCategoryCode.Equals(categoryCode) ) && 
                                p.CurrentPayPeriodInd)
                    ;

                return contributions.ToList();
            }
        }

        public List<BenefitContribution> GetContributionsByCchId(int cchid)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 1)
            {
                throw new InvalidOperationException("Invalid CCHID");
            }

            List<BenefitContribution> benefitContributions = new List<BenefitContribution>();

            using (var context = new ContributionsContext(_connectionString))
            {
                benefitContributions.Add(
                    new BenefitContribution
                    {
                        CCHID = 63841,
                        AsOfDate = DateTime.Now,
                        ContributionTypeCode = "EE",
                        ContributionTypeName = "Employee",
                        DWCreateDate = DateTime.Now,
                        EmployeeFirstName = "MARY",
                        EmployeeLastName = "SMITH",
                        PayrollCategoryName = "Deduction",
                        PayrollMetricName = "Roth 401K",
                        PerPeriodAmt = 440.2m,
                        PreTaxInd = true,
                        YTDAmt = 1660.8m,
                        ReportingCategoryCode = "401K",
                        CurrentPayPeriodInd = true
                    });
            }
            return benefitContributions;
        }
    }
}

//        public async Task<List<IdCard>> GetIdCardsByCchIdAsync(int cchId)
//        {
//            if (string.IsNullOrEmpty(_connectionString))
//                throw new InvalidOperationException("Failed to initialized repository");

//            using (var itx = new IdCardsContext(_connectionString))
//            {
//                //This is not the final production code. It's illustrative at this point pending 
//                //the final data structures.

//                var enrollmentsQuery = @"SELECT CCHID, RelationshipCode
//                    FROM Enrollments
//                    WHERE SubscriberMedicalID IN
//                    (SELECT SubscriberMedicalID 
//	                    FROM Enrollments 
//	                    WHERE CCHID = @cchid)";


//                List<IdCard> results;
//                //if dependent only get dependent cards
//                if (!employee.CchId.Equals(cchId))
//                    results = await itx.IdCards
//                        .Include(p => p.CardType)
//                        .Where(id => id.MemberId.Equals(cchId)
//                            && id.LocaleId.Equals(1) //Only support English at this time
//                            && !string.IsNullOrEmpty(id.DetailText)).ToListAsync();
//                else //get employee and dependents
//                    results = await itx.IdCards
//                        .Include(p => p.CardType)
//                        .Where(id => id.LocaleId.Equals(1)
//                            && !string.IsNullOrEmpty(id.DetailText)
//                            && familyEnrollmentIds.Contains(id.MemberId)).ToListAsync();

//                var cardTypeIds = results.Select(r => r.CardType.Id).Distinct();

//                var translations = await itx.IdCardTypeTranslations
//                    .Where(t => t.LocaleId.Equals(1) && cardTypeIds.Contains(t.Id)).ToListAsync();

//                results.ForEach(r => {
//                    r.CardType.Translation = translations.FirstOrDefault(t => t.Id.Equals(r.CardType.Id)).CardTypeName;
//                    r.RequestContextMemberId = cchId;
//                });

//                return results;
//            }
//        }

//    }
//}