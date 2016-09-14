using CchWebAPI.Core.Entities;
using CchWebAPI.Employees.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CchWebAPI.Employees.Dispatchers {
    public interface IEmployeesDispatcher {
        TaskEventHandler<Employee> ExecuteAsync(int cchId, Employer employer);
    }

    public class EmployeesDispatcher : IEmployeesDispatcher {
        private IEmployeesRepository _repository;
        public EmployeesDispatcher(IEmployeesRepository repository) {
            _repository = repository;
        }

        public async Task<Employee> ExecuteAsync(int cchid, Employer employer) {
            Contract.Requires<InvalidOperationException>(cchid > 1, "Invalid member context.");
            Contract.Requires<InvalidOperationException>(employer != null 
                || !string.IsNullOrEmpty(employer.ConnectionString), "Invalid employer context.");

            _repository.Initialize(employer.ConnectionString);

            var result = await _repository.GetEmployeeAsync(cchid);

            return result;
        }
    }
}