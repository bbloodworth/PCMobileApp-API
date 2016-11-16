using CchWebAPI.Employee.Data;
using CchWebAPI.Employee.Models;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace CchWebAPI.Employee.Dispatchers {
    public interface IEmployeeDispatcher {
        Task<Data.Employee> GetEmployeeAsync(Employer employer, int cchId);
        Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(Employer employer, int cchId, int planId);
    }

    public class EmployeeDispatcher : IEmployeeDispatcher {
        private IEmployeeRepository _repository;
        public EmployeeDispatcher(IEmployeeRepository repository) {
            _repository = repository;
        }

        public async Task<Data.Employee> GetEmployeeAsync(Employer employer, int cchId) {
            if (cchId < 1)
                throw new ArgumentException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new ArgumentException("Invalid employer context.");

            _repository.Initialize(EmployerConnectionString.GetConnectionString(employer.Id).DataWarehouse);

            var result = await _repository.GetEmployeeByCchIdAsync(cchId);

            return result;
        }

        public async Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(Employer employer, int cchId, int planId) {
            if (cchId < 1)
                throw new ArgumentException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new ArgumentException("Invalid employer context.");

            _repository.Initialize(EmployerConnectionString.GetConnectionString(employer.Id).DataWarehouse);

            var result = await _repository.GetEmployeeBenefitPlanMembersAsync(cchId, planId);

            return result;
        }
    }
}