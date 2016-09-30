using CchWebAPI.Employees.Data;
using ClearCost.Platform;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace CchWebAPI.Employees.Dispatchers {
    public interface IEmployeesDispatcher {
        Task<Employee> ExecuteAsync(int cchId, Employer employer);
    }

    public class EmployeesDispatcher : IEmployeesDispatcher {
        private IEmployeesRepository _repository;
        public EmployeesDispatcher(IEmployeesRepository repository) {
            _repository = repository;
        }

        public async Task<Employee> ExecuteAsync(int cchId, Employer employer) {
            if (cchId < 1)
                throw new InvalidOperationException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            _repository.Initialize(employer.ConnectionString);

            var result = await _repository.GetEmployeeAsync(cchId);

            return result;
        }
    }
}