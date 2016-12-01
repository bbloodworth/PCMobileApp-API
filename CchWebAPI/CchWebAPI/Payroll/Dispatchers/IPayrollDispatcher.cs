using CchWebAPI.Payroll.Models;
using CchWebAPI.Payroll.Data;
using ClearCost.Platform;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace CchWebAPI.Payroll.Dispatchers {
    public interface IPayrollDispatcher {
        Task<List<DatePaid>> GetDatePaidAsync(Employer employer, int cchId);
        Task<Paycheck> GetPaycheckAsync(Employer employer, string documentId);
    }

    public class PayrollDispatcher : IPayrollDispatcher {
        private IPayrollRepository _repository;
        public PayrollDispatcher(IPayrollRepository repository) {
            _repository = repository;
        }

        public async Task<List<DatePaid>> GetDatePaidAsync(Employer employer, int cchId) {
            if (cchId < 1)
                throw new InvalidOperationException("Invalid member context.");

            if (employer == null || string.IsNullOrWhiteSpace(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            _repository.Initialize(employer.ConnectionString);

            var result = await _repository.GetDatesPaidAsync(cchId);

            return result;
        }
        public async Task<Paycheck> GetPaycheckAsync(Employer employer, string documentId) {
            if (String.IsNullOrWhiteSpace(documentId))
                throw new InvalidOperationException("Invalid documentId.");

            if (employer == null || string.IsNullOrWhiteSpace(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            _repository.Initialize(employer.ConnectionString);

            var result = await _repository.GetPaycheckAsync(documentId);

            Paycheck paycheck = null;

            if (result != null) {
                paycheck = new Paycheck(result);
            }

            return paycheck;
        }
    }
}