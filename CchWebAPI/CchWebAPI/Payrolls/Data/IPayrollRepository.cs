using CchWebAPI.EmployeeDW.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CchWebAPI.Payrolls.Data {
    public interface IPayrollRepository {
        void Initialize(string connectionString);
        Task<List<DatePaid>> GetDatesPaidAsync(int cchId);
    }

    public class PayrollRepository : IPayrollRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<List<DatePaid>> GetDatesPaidAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            using (var ctx = new PayrollContext(_connectionString)) {
                List<DatePaid> payroll = await (
                    from p in ctx.Payrolls
                    join e in ctx.Employees on p.EmployeeKey equals e.EmployeeKey
                    join d in ctx.Dates on p.PayDateKey equals d.DateKey
                    where e.CCHID == cchId
                    select new DatePaid {
                        CchId = e.CCHID,
                        DocumentId = p.DocumentID,
                        PaycheckDate = d.FullDate
                    })
                    .Distinct()
                    .ToListAsync();

                return payroll;
            }
        }
    }
}