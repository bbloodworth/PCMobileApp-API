using CchWebAPI.EmployeeDW.Models;
using CchWebAPI.Payrolls.Data;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace CchWebAPI.Payrolls.Dispatchers {
    public interface IPayrollDispatcher {
        Task<List<DatePaid>> ExecuteAsync(Employer employer, int cchId);
    }

    public class PayrollDispatcher : IPayrollDispatcher {
        private IPayrollRepository _repository;
        public PayrollDispatcher(IPayrollRepository repository) {
            _repository = repository;
        }

        public async Task<List<DatePaid>> ExecuteAsync(Employer employer, int cchId) {
            if (cchId < 1)
                throw new InvalidOperationException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            //_repository.Initialize(employer.ConnectionString);
            _repository.Initialize(DataWarehouse.GetEmployerConnectionString(employer.Id));

            var result = await _repository.GetDatesPaidAsync(cchId);

            return result;
        }
    }
}