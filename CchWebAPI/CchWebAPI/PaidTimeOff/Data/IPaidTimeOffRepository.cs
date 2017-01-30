using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;



using CchWebAPI.PaidTimeOff.Models;

namespace CchWebAPI.PaidTimeOff.Data {
    public interface IPaidTimeOffRepository {
        void Initialize(string connectionString);
        Task<List<PaidTimeOffDetail>> GetPaidTimeOffDetailsByCchIdAsync(int cchid);

    }

    public class PaidTimeOffRepository : IPaidTimeOffRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<List<PaidTimeOffDetail>> GetPaidTimeOffDetailsByCchIdAsync(int cchid) {
            // USE this object to get data from the db
            List<PaidTimeOffDetail> dbResult;

            using (var context = new PaidTimeOffContext(_connectionString)) {
                // QUERY the data warehouse for the users PTO details
                var query = context.PTOSnapshots.
                    Join(
                        context.Employees,
                        pto => pto.EmployeeKey,
                        employee => employee.EmployeeKey,
                        (paidTimeOff, employee) => new {
                            PaidTimeOff = paidTimeOff,
                            Employee = employee
                        })
                    .Join(
                        context.Dates,
                        paidTimeOff => paidTimeOff.PaidTimeOff.AccruedThroughDateKey,
                        date => date.DateKey,
                        (paidTimeOff, date) => new {
                            PaidTimeOff = paidTimeOff.PaidTimeOff,
                            Employee = paidTimeOff.Employee,
                            Date = date
                        })
                    .Join(
                        context.PayrollMetrics,
                        paidTimeOff => paidTimeOff.PaidTimeOff.PayrollMetricKey,
                        payrollMetric => payrollMetric.PayrollMetricKey,
                        (paidTimeOff, payrollMetric) => new {
                            PaidTimeOff = paidTimeOff.PaidTimeOff,
                            Employee = paidTimeOff.Employee,
                            Date = paidTimeOff.Date,
                            PayrollMetric = payrollMetric
                        })
                    .Where(a =>
                       a.PaidTimeOff.CurrentRecordInd.Equals(true)
                       && a.Employee.CchId.Equals(cchid)
                    )
                    .Select(
                        a => new PaidTimeOffDetail {
                            CchId = a.Employee.CchId,
                            AccruedThroughDate = a.Date.FullDate,
                            AccrualRate = a.PaidTimeOff.AccrualRate,
                            PerPeriodAccruedQty = a.PaidTimeOff.PerPeriodAccruedQty,
                            PerPeriodHoursQty = a.PaidTimeOff.PerPeriodHoursQty,
                            PerPeriodTakenQty = a.PaidTimeOff.PerPeriodTakenQty,
                            YTDAccruedQty = a.PaidTimeOff.YTDAccruedQty,
                            YTDTakenQty = a.PaidTimeOff.YTDTakenQty,
                            AvailableBalanceQty = a.PaidTimeOff.AvailableBalanceQty,
                            CurrentRecordInd = a.PaidTimeOff.CurrentRecordInd,
                            PayrollCategoryName = a.PayrollMetric.PayrollCategoryName,
                            ReportingCategoryCode = a.PayrollMetric.ReportingCategoryCode
                        });

                // GET data from the db
                dbResult = await query.ToListAsync();

                return dbResult;
            }
        }
    }
}

