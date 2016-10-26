using CchWebAPI.EmployeeDW.Data;
using ClearCost.Platform;
using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace CchWebAPI.EmployeeDW.Dispatchers {
    public interface IEmployeeDispatcher {
        Task<Employee> ExecuteAsync(int cchId, Employer employer);
    }

    public class EmployeeDispatcher : IEmployeeDispatcher {
        private IEmployeeRepository _repository;
        public EmployeeDispatcher(IEmployeeRepository repository) {
            _repository = repository;
        }

        public async Task<Employee> ExecuteAsync(int cchId, Employer employer) {
            if (cchId < 1)
                throw new InvalidOperationException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new InvalidOperationException("Invalid employer context.");

            //_repository.Initialize(employer.ConnectionString);
            _repository.Initialize(DataWarehouse.GetEmployerConnectionString(employer.Id));

            var result = await _repository.GetEmployeeByCchIdAsync(cchId);

            return result;
        }
    }
}