using CchWebAPI.Payrolls.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CchWebAPI.Payrolls.Data {
    public interface IPayrollRepository {
        void Initialize(string connectionString);
        Task<List<DatePaid>> GetDatesPaidAsync(int cchId);
        Task<List<PaycheckDetailsQueryResult>> GetPaycheckAsync(string documentId);
    }

    public class PayrollRepository : IPayrollRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<List<DatePaid>> GetDatesPaidAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialize repository");

            using (var ctx = new PayrollContext(_connectionString)) {
                List<DatePaid> payrolls = await (
                    from payroll in ctx.Payrolls
                        join employee in ctx.Employees 
                            on payroll.EmployeeKey equals employee.EmployeeKey
                        join dateTable in ctx.Dates 
                            on payroll.PayDateKey equals dateTable.DateKey
                    where employee.CCHID == cchId
                    select new DatePaid {
                        CchId = employee.CCHID,
                        DocumentId = payroll.DocumentID,
                        PaycheckDate = dateTable.FullDate.Value
                    })
                    .Distinct()
                    .ToListAsync();

                return payrolls;
            }
        }

        public async Task<List<PaycheckDetailsQueryResult>> GetPaycheckAsync(string documentId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialize repository");

            using (var ctx = new PayrollContext(_connectionString)) {
                var query = await (
                    from payroll in ctx.Payrolls
                        join dateTable in ctx.Dates 
                            on payroll.PayDateKey equals dateTable.DateKey
                        join employee in ctx.Employees 
                            on payroll.EmployeeKey equals employee.EmployeeKey
                        join deliveryMethod in ctx.DeliveryMethods
                            on payroll.DeliveryMethodKey equals deliveryMethod.DeliveryMethodKey
                        join contributionType in ctx.ContributionTypes 
                            on payroll.ContributionTypeKey equals contributionType.ContributionTypeKey
                        join payrollMetric in ctx.PayrollMetrics
                            on payroll.PayrollMetricKey equals payrollMetric.PayrollMetricKey
                    where payroll.DocumentID == documentId
                    select new PaycheckDetailsQueryResult {
                        CchId = employee.CCHID,
                        FirstName = employee.EmployeeFirstName,
                        LastName = employee.EmployeeLastName,
                        PrimaryWorkLocationCode = employee.PrimaryWorkLocationCode,
                        FederalTaxElectionCode = employee.FederalTaxElectionCode,
                        StateOfWorkElectionCode = employee.StateOfWorkElectionCode,
                        StateOfResidenceElectionCode = employee.StateOfResidenceElectionCode,
                        PayDate = dateTable.FullDate,
                        DocumentId = payroll.DocumentID,
                        DeliveryMethodCode = deliveryMethod.DeliveryMethodCode,
                        PayrollCategoryName = payrollMetric.PayrollCategoryName,
                        PayrollMetricName = payrollMetric.PayrollMetricName,
                        PreTaxInd = payrollMetric.PreTaxInd,
                        ContributionTypeCode = contributionType.ContributionTypeCode,
                        PayrollMetricRate = payroll.PayrollMetricRate,
                        PerPeriodQty = payroll.PerPeriodQty,
                        PerPeriodAmt = payroll.PerPeriodAmt,
                        YTDQty = payroll.YTDQty,
                        YTDAmt = payroll.YTDAmt
                    }).ToListAsync();
                return query;
            }
        }
    }
}