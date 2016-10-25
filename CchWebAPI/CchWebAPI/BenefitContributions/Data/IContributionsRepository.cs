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
        Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchid);
    }

    public class ContributionsRepository: IContributionsRepository
    {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString)
        {
            _connectionString = connectionString;
            _connectionString = "Data Source=KERMITDB\\MSPIGGY;Initial Catalog=CCH_DemoDWH;Trusted_Connection=true;Asynchronous Processing=True; MultipleActiveResultSets=true";
        }

        public async Task<List<BenefitContribution>> GetContributionsByCchIdAsync(int cchid)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 0)
            {
                throw new InvalidOperationException("Invalid CCHID");
            }

            //List<BenefitContribution> contributions = new List<BenefitContribution>();

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

            using (var context = new ContributionsContext(_connectionString))
            {
                var contributions = await context.Database.SqlQuery<BenefitContribution>(sqlQuery).ToListAsync();

                return contributions;
            }
        }

        public List<BenefitContribution> GetContributionsByCchId(int cchid)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Failed to initialize Benefit Contributions repository");
            }
            if (cchid < 0)
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
                        PreTaxInd =true,
                        YTDAmt = 1660.8m
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