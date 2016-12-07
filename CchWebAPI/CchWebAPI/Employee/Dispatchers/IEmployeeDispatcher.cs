using CchWebAPI.Employee.Data.V1;
using CchWebAPI.Employee.Data.V2;
using CchWebAPI.Employee.Models;
using ClearCost.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace CchWebAPI.Employee.Dispatchers {
    public interface IEmployeeDispatcher {
        Task<Models.Employee> GetEmployeeAsync(ClearCost.Platform.Employer employer, int cchId);
        Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(ClearCost.Platform.Employer employer, int cchId, int planId);
        Task<List<BenefitPlan>> GetEmployeeBenefitsEnrolled(ClearCost.Platform.Employer employer, int cchId, int year);
        Task<List<BenefitPlan>> GetEmployeeBenefitsEligible(ClearCost.Platform.Employer employer, int cchId);
    }

    public class EmployeeDispatcher : IEmployeeDispatcher {
        private Data.V1.IEmployeeRepository _repositoryV1;
        private Data.V2.IEmployeeRepository _repositoryV2;
        public EmployeeDispatcher(Data.V1.IEmployeeRepository repository) {
            _repositoryV1 = repository;
        }
        public EmployeeDispatcher(Data.V2.IEmployeeRepository repository) {
            _repositoryV2 = repository;
        }

        public async Task<Models.Employee> GetEmployeeAsync(ClearCost.Platform.Employer employer, int cchId) {
            if (cchId < 1)
                throw new ArgumentException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new ArgumentException("Invalid employer context.");

            _repositoryV2.Initialize(employer.ConnectionString);

            var employee = new Models.Employee();

            // Check to see if this database has data warhouse tables.
            if (await _repositoryV2.IsExistingTable("dbo", "Employee_d")) {
                var employeeData = await _repositoryV2.GetEmployeeByCchIdAsync(cchId);
                employee.Merge(employeeData);
                if (await _repositoryV2.IsExistingTable("dbo", "Member_d"))
                {
                    var memberData = await _repositoryV2.GetMemberByCchIdAsync(cchId);
                    employee.Merge(memberData);
                }
            }
            else {
                _repositoryV1 = new Data.V1.EmployeeRepository();
                _repositoryV1.Initialize(employer.ConnectionString);

                var employeeData = await _repositoryV1.GetEmployeeAsync(cchId);
                employee.Merge(employeeData);
            }


            return employee;
        }

        public async Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(ClearCost.Platform.Employer employer, int cchId, int planId) {
            if (cchId < 1)
                throw new ArgumentException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new ArgumentException("Invalid employer context.");

            _repositoryV2.Initialize(employer.ConnectionString);

            var result = await _repositoryV2.GetEmployeeBenefitPlanMembersAsync(cchId, planId);

            return result;
        }

        public async Task<List<BenefitPlan>> GetEmployeeBenefitsEnrolled(ClearCost.Platform.Employer employer, int cchId, int year) {
            if (cchId < 1)
                throw new ArgumentException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new ArgumentException("Invalid employer context.");

            _repositoryV2.Initialize(employer.ConnectionString);

            var result = await _repositoryV2.GetEmployeeBenefitsEnrolled(cchId, year);

            return result;
        }

        public async Task<List<BenefitPlan>> GetEmployeeBenefitsEligible(ClearCost.Platform.Employer employer, int cchId) {
            if (cchId < 1)
                throw new ArgumentException("Invalid member context.");

            if (employer == null || string.IsNullOrEmpty(employer.ConnectionString))
                throw new ArgumentException("Invalid employer context.");

            _repositoryV2.Initialize(employer.ConnectionString);

            var result = await _repositoryV2.GetEmployeeBenefitsEligible(cchId);

            return result;
        }
    }
}
