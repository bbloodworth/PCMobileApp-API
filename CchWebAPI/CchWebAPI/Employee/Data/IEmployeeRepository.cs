using CchWebAPI.Employee.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace CchWebAPI.Employee.Data {
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
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.EmployeeKey.Equals(employeeKey));
            }

            return employee;
        }

        public async Task<Employee> GetEmployeeByCchIdAsync(int cchId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            Employee employee = null;

            using (var ctx = new EmployeeContext(_connectionString)) {
                employee = await ctx.Employees.FirstOrDefaultAsync(p => p.CchId.Equals(cchId) && p.CurrentRecordInd.Value);
            }

            return employee;
        }

        public async Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(int cchId, int planId) {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Failed to initialized repository");

            List<PlanMember> planMembers = new List<PlanMember>();

            using (var ctx = new EmployeeContext(_connectionString)) {
                planMembers = await ctx.BenefitEnrollments
                    .Join(
                        ctx.Members,
                        benefitEnrollment => benefitEnrollment.SubscriberMemberKey,
                        member => member.MemberKey,
                        (benefitEnrollment, member) => new {
                            BenefitEnrollment = benefitEnrollment,
                            Member = member
                        })
                    .Join(
                        ctx.Members,
                        benefitEnrollment => benefitEnrollment.BenefitEnrollment.EnrolledMemberKey,
                        dependent => dependent.MemberKey,
                        (benefitEnrollment, dependent) => new {
                            BenefitEnrollment = benefitEnrollment.BenefitEnrollment,
                            Member = benefitEnrollment.Member,
                            Dependent = dependent
                        })
                    .Where(
                        p => p.Dependent.CchId.Equals(cchId)
                        && p.BenefitEnrollment.BenefitPlanOptionKey.Equals(planId)
                        //&& p.CurrentRecordInd
                    )
                    .Select(
                        p => new PlanMember {
                            CchId = p.Dependent.CchId,
                            FirstName = p.Dependent.MemberFirstName,
                            LastName = p.Dependent.MemberLastName
                        }
                    )
                    .ToListAsync();
            }

            return planMembers;
        }
    }
}