﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using CchWebAPI.PaidTimeOff.Models;

namespace CchWebAPI.PaidTimeOff.Data {
    public interface IPaidTimeOffRepository {
        void Initialize(string connectionString);
        Task<List<PaidTimeOffTable>> GetPaidTimeOffDetailsByCchId(int cchid);

    }

    public class PaidTimeOffRepository : IPaidTimeOffRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<List<PaidTimeOffTable>> GetPaidTimeOffDetailsByCchId(int cchid) {
            List<PaidTimeOffTable> result;

            using (var context = new PaidTimeOffContext(_connectionString)) {
                var q = context.PTOSnapshots.
                    Join(context.Employees,
                        pto => pto.EmployeeKey, e => e.EmployeeKey,
                        (pto, e) => new {
                            CCHID = e.CCHID,
                            FirstName = e.EmployeeFirstName,
                            LastName = e.EmployeeLastName,
                            PaycheckDateKey = pto.PaycheckDateKey,
                            AccrualRate = pto.AccrualRate,
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
                        (a, p) => new PaidTimeOffTable {
                            CCHID = a.CCHID,
                            PaycheckDateKey = a.PaycheckDateKey,
                            AccrualRate = a.AccrualRate,
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
                       a.CurrentRecordInd.Equals(1)
                    // && a.CCHID.Equals(foobar)
                    );

                result = q.ToList();

            }

            return result;
        }
    }
}


