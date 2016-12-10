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
                    Join(context.Employees,
                        pto => pto.EmployeeKey, e => e.EmployeeKey,
                        (pto, e) => new {
                            CchId = e.CchId,
                            FirstName = e.EmployeeFirstName,
                            LastName = e.EmployeeLastName,
                            PaycheckDateKey = pto.PaycheckDateKey,
                            AccrualRate = pto.AccrualRate,
                            PerPeriodAccruedQty = pto.PerPeriodAccruedQty,
                            PerPeriodHoursQty = pto.PerPeriodHoursQty,
                            PerPeriodTakenQty = pto.PerPeriodTakenQty,
                            YTDAccruedQty = pto.YTDAccruedQty,
                            YTDTakenQty = pto.YTDTakenQty,
                            PayrollMetricKey = pto.PayrollMetricKey,
                            AvailableBalanceQty = pto.AvailableBalanceQty,
                            CurrentRecordInd = pto.CurrentRecordInd
                        }).
                    Join(context.PayrollMetrics,
                        a => a.PayrollMetricKey, p => p.PayrollMetricKey,
                        (a, p) => new PaidTimeOffDetail {
                            CchId = a.CchId,
                            PaycheckDateKey = a.PaycheckDateKey,
                            AccrualRate = a.AccrualRate,
                            PerPeriodAccruedQty = a.PerPeriodAccruedQty,
                            PerPeriodHoursQty = a.PerPeriodHoursQty,
                            PerPeriodTakenQty = a.PerPeriodTakenQty,
                            YTDAccruedQty = a.YTDAccruedQty,
                            YTDTakenQty = a.YTDTakenQty,
                            AvailableBalanceQty = a.AvailableBalanceQty,
                            CurrentRecordInd = a.CurrentRecordInd,
                            PayrollCategoryName = p.PayrollCategoryName,
                            ReportingCategoryCode = p.ReportingCategoryCode
                        }
                    ).Where(a =>
                       a.CurrentRecordInd.Equals(true)
                       && a.CchId.Equals(cchid)
                    );

                // GET data from the db
                dbResult = await query.ToListAsync();


                return dbResult;
            }
        }
    }
}

