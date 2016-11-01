using CchWebAPI.EmployeeDW.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CchWebAPI.EmployeeDW.Data {
    public interface IEmployeeRepository {
        void Initialize(string connectionString);
        Task<Employee> GetEmployeeByKeyAsync(int employeeKey);
        Task<Employee> GetEmployeeByCchIdAsync(int cchId);
        Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(int cchId, int planId);
    }

    public class EmployeeRepository : IEmployeeRepository {
        private string _connectionString = string.Empty;

        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }

        public async Task<Employee> GetEmployeeByKeyAsync(int employeeKey) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            Employee employee = null;

            using (var ctx = new EmployeeContext(_connectionString)) {
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.EmployeeKey == employeeKey);
            }

            return employee;
        }

        public async Task<Employee> GetEmployeeByCchIdAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            Employee employee = null;

            using (var ctx = new EmployeeContext(_connectionString)) {
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.CCHID == cchId && p.CurrentRecordInd.Value);
            }

            return employee;
        }

        public async Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(int cchId, int planId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            //var employee = await GetEmployeeAsync(cchId);

            //if (employee == null)
            //    throw new ArgumentException("Invalid cchId");

            List<PlanMember> planMembers = new List<PlanMember>();

            using (var ctx = new EmployeeContext(_connectionString)) {
                planMembers = await (
                    from benefitEnrollment in ctx.BenefitEnrollments
                        join employee in ctx.Members
                            on benefitEnrollment.SubscriberMemberKey equals employee.MemberKey
                        join dependent in ctx.Members
                            on benefitEnrollment.EnrolledMemberKey equals dependent.MemberKey
                    where
                        employee.CCHID == cchId
                        && benefitEnrollment.BenefitPlanOptionKey == planId
                        // Needs to be added after data is present.
                        //&& benefitEnrollment.CurrentRecordInd == true
                    select new PlanMember {
                        // the employee data is also returned in the dependent join.
                        CchId = dependent.CCHID,
                        FirstName = dependent.MemberFirstName,
                        LastName = dependent.MemberLastName
                    }).ToListAsync();
            }

            return planMembers;
        }
    }
}