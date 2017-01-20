using CchWebAPI.Employee.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace CchWebAPI.Employee.Data {
    // For testing the existence of data warehouse tables.
    public class SqlRepository {
        protected string _connectionString = string.Empty;
        public void Initialize(string connectionString) {
            _connectionString = connectionString;
        }
    }
    namespace V1 {
        public interface IEmployeeRepository {
            Task<bool> IsExistingTableAsync(string schema, string table);
            void Initialize(string connectionString);
            Task<Employee> GetEmployeeAsync(int cchId);
        }

        public class EmployeeRepository : SqlRepository, IEmployeeRepository {
            //private string _connectionString = string.Empty;

            //public void Initialize(string connectionString) {
            //    _connectionString = connectionString;
            //}
            public async Task<bool> IsExistingTableAsync(string schema, string table) {
                bool tableExists = false;

                using (var ctx = new EmployeeContext(_connectionString)) {
                    tableExists = await ctx.Database
                         .SqlQuery<int?>(@"
                         SELECT 1 FROM sys.tables AS T
                         INNER JOIN sys.schemas AS S ON T.schema_id = S.schema_id
                         WHERE S.Name = '@schema' AND T.Name = '@table'",
                             new SqlParameter("schema", schema),
                             new SqlParameter("table", table))
                         .FirstOrDefaultAsync() != null;
                }

                return tableExists;
            }

            public async Task<Employee> GetEmployeeAsync(int cchId) {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

                Employee employee = null;

                using (var ctx = new EmployeeContext(_connectionString)) {
                    employee = await ctx.Employees.FirstOrDefaultAsync(p => p.CchId == cchId);
                }

                return employee;
            }

        }
    }
    namespace V2 {
        public interface IEmployeeRepository {
            Task<bool> IsExistingTableAsync(string schema, string table);
            void Initialize(string connectionString);
            Task<Employee> GetEmployeeByKeyAsync(int employeeKey);
            Task<Employee> GetEmployeeByCchIdAsync(int cchId);
            Task<Member> GetMemberByCchIdAsync(int cchId);
            Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(int cchId, int planId, int year);
            Task<List<PlanMember>> GetEmployeeDependentsAsync(int cchId);
            Task<List<BenefitPlan>> GetEmployeeBenefitsEnrolledAsync(int cchId, int year);
            Task<List<BenefitPlan>> GetEmployeeBenefitsEligibleAsync(int cchId);
            Task<BenefitMedicalPlan> GetEmployeeBenefitEnrollmentMedicalPlanAsync(int memberKey);
        }

        public class EmployeeRepository : SqlRepository, IEmployeeRepository {
            public async Task<bool> IsExistingTableAsync(string schema, string table) {
                bool tableExists = false;

                using (var ctx = new EmployeeContext(_connectionString)) {
                    tableExists = await ctx.Database
                         .SqlQuery<int?>(@"
                         SELECT 1 FROM sys.synonyms AS T
                         INNER JOIN sys.schemas AS S ON T.schema_id = S.schema_id
                         WHERE S.Name = @schema AND T.Name = @table",
                             new SqlParameter("schema", schema),
                             new SqlParameter("table", table))
                         .FirstOrDefaultAsync() != null;
                }

                return tableExists;
            }

            public async Task<Employee> GetEmployeeByKeyAsync(int employeeKey) {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

                Employee employee = null;

                using (var ctx = new EmployeeContext(_connectionString)) {
                    employee = await ctx.Employees.FirstOrDefaultAsync(p => p.EmployeeKey.Equals(employeeKey));
                }

                return employee;
            }

            public async Task<Employee> GetEmployeeByCchIdAsync(int cchId) {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

                Employee employee = null;

                using (var ctx = new EmployeeContext(_connectionString)) {
                    employee = await ctx.Employees.FirstOrDefaultAsync(p => p.CchId.Equals(cchId) && p.CurrentRecordInd.Equals(1));
                }

                return employee;
            }

            public async Task<Member> GetMemberByCchIdAsync(int cchId)
            {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

                Member member = null;

                using (var ctx = new EmployeeContext(_connectionString))
                {
                    member = await ctx.Members.FirstOrDefaultAsync(p => p.CchId.Equals(cchId) && p.CurrentRecordInd.Equals(1));
                }

                return member;
            }

            public async Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(int cchId, int planId, int year) {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

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
                        .Join(
                            ctx.PlanYears,
                            benefitEnrollment => benefitEnrollment.BenefitEnrollment.PlanYearKey,
                            planYears => planYears.PlanYearKey,
                            (benefitEnrollment, planYears) => new {
                                BenefitEnrollment = benefitEnrollment.BenefitEnrollment,
                                Member = benefitEnrollment.Member,
                                Dependent = benefitEnrollment.Dependent,
                                PlanYear = planYears
                            })
                        .Join(
                            ctx.BenefitEnrollmentStatus,
                            benefitEnrollment => benefitEnrollment.BenefitEnrollment.BenefitEnrollmentStatusKey,
                            benefitEnrollmentStatus => benefitEnrollmentStatus.BenefitEnrollmentStatusKey,
                            (benefitEnrollment, benefitEnrollmentStatus) => new {
                                BenefitEnrollment = benefitEnrollment.BenefitEnrollment,
                                PlanYear = benefitEnrollment.PlanYear,
                                Member = benefitEnrollment.Member,
                                Dependent = benefitEnrollment.Dependent,
                                BenefitEnrollmentStatuses = benefitEnrollmentStatus
                            })
                        .Where(
                            p => p.Member.CchId.Equals(cchId)
                            && p.BenefitEnrollment.BenefitPlanOptionKey.Equals(planId)
                            && p.PlanYear.PlanYearName.Equals(year.ToString())
                            && p.BenefitEnrollmentStatuses.BenefitEnrollmentStatusName.Equals("Active")
                        )
                        .Select(
                            p => new PlanMember {
                                CchId = p.BenefitEnrollment.EnrolledMemberKey,
                                FirstName = p.Dependent.MemberFirstName,
                                LastName = p.Dependent.MemberLastName
                            }
                        )
                        .ToListAsync();
                }

                return planMembers;
            }

            public async Task<List<PlanMember>> GetEmployeeDependentsAsync(int cchId) {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

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
                        .Join(
                            ctx.BenefitPlanOptions,
                            benefitEnrollment => benefitEnrollment.BenefitEnrollment.BenefitPlanOptionKey,
                            benefitPlanOption => benefitPlanOption.BenefitPlanOptionKey,
                            (benefitEnrollment, benefitPlanOption) => new {
                                BenefitEnrollment = benefitEnrollment.BenefitEnrollment,
                                Member = benefitEnrollment.Member,
                                Dependent = benefitEnrollment.Dependent,
                                BenefitPlanOption = benefitPlanOption
                            })
                        .Join(
                            ctx.BenefitEnrollmentStatus,
                            benefitEnrollment => benefitEnrollment.BenefitEnrollment.BenefitEnrollmentStatusKey,
                            benefitEnrollmentStatus => benefitEnrollmentStatus.BenefitEnrollmentStatusKey,
                            (benefitEnrollment, benefitEnrollmentStatus) => new {
                                BenefitEnrollment = benefitEnrollment.BenefitEnrollment,
                                Member = benefitEnrollment.Member,
                                Dependent = benefitEnrollment.Dependent,
                                BenefitPlanOption = benefitEnrollment.BenefitPlanOption,
                                BenefitEnrollmentStatus = benefitEnrollmentStatus
                            }
                        )
                        .Where(
                            p => p.Member.CchId.Equals(cchId)
                            && p.BenefitEnrollmentStatus.BenefitEnrollmentStatusKey.Equals(1)
                        )
                        .Select(
                            p => new PlanMember {
                                CchId = p.BenefitEnrollment.EnrolledMemberKey,
                                FirstName = p.Dependent.MemberFirstName,
                                LastName = p.Dependent.MemberLastName,
                                BenefitPlanOptionKey = p.BenefitPlanOption.BenefitPlanOptionKey,
                                BenefitTypeCode = p.BenefitPlanOption.BenefitTypeCode
                            }
                        )
                        .ToListAsync();
                }

                List<string> benefitTypes = new List<string>(new string[] { "MED", "DEN", "VIS", "RX" });

                List<PlanMember> distinctMembers = planMembers
                  .GroupBy(p => new { p.FirstName, p.LastName })
                  .Select(g => g.First())
                  .ToList();


                List<PlanMember> dependents = new List<PlanMember>();

                foreach (PlanMember member in distinctMembers) {
                    List<PlanMember> thisMember = planMembers
                        .Where(i => i.FirstName == member.FirstName && i.LastName == member.LastName)
                        .ToList();

                    PlanMember dependent = new PlanMember {
                        CchId = member.CchId,
                        FirstName = member.FirstName,
                        LastName = member.LastName
                    };

                    Dictionary<string, int> benefit = new Dictionary<string, int>();
                    foreach (PlanMember line in thisMember) {
                        //benefit.Add(line.BenefitTypeCode, line.BenefitPlanOptionKey);
                        if (benefitTypes.Contains(line.BenefitTypeCode)) {
                            PropertyInfo prop = dependent.GetType().GetProperty(line.BenefitTypeCode, BindingFlags.Public | BindingFlags.Instance);
                            if (null != prop && prop.CanWrite) {
                                prop.SetValue(dependent, line.BenefitPlanOptionKey);
                            }
                        }
                    }

                    dependents.Add(dependent);
                }

                return dependents;
            }

            public async Task<List<BenefitPlan>> GetEmployeeBenefitsEnrolledAsync(int cchId, int year) {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

                List<BenefitPlan> benefitPlans = new List<BenefitPlan>();

                using (var ctx = new EmployeeContext(_connectionString)) {
                    var employee = await ctx.Employees
                        .FirstOrDefaultAsync(p => p.CchId.Equals(cchId));

                    if (employee != null) {
                        benefitPlans = await ctx.BenefitEnrollments
                        .Join(
                            ctx.PlanYears,
                            benefitEnrollments => benefitEnrollments.PlanYearKey,
                            planYears => planYears.PlanYearKey,
                            (benefitEnrollments, planYears) => new {
                                BenefitEnrollments = benefitEnrollments,
                                PlanYears = planYears
                            })
                        .Join(
                            ctx.BenefitPlanOptions,
                            benefitEnrollments => benefitEnrollments.BenefitEnrollments.BenefitPlanOptionKey,
                            benefitPlanOptions => benefitPlanOptions.BenefitPlanOptionKey,
                            (benefitEnrollments, benefitPlanOptions) => new {
                                BenefitEnrollments = benefitEnrollments.BenefitEnrollments,
                                PlanYears = benefitEnrollments.PlanYears,
                                BenefitPlanOptions = benefitPlanOptions
                            })
                        .Join(
                            ctx.Members,
                            benefitEnrollments => benefitEnrollments.BenefitEnrollments.EnrolledMemberKey,
                            members => members.MemberKey,
                            (benefitEnrollments, members) => new {
                                BenefitEnrollments = benefitEnrollments.BenefitEnrollments,
                                PlanYears = benefitEnrollments.PlanYears,
                                BenefitPlanOptions = benefitEnrollments.BenefitPlanOptions,
                                Members = members
                            })
                        .Join(
                            ctx.BenefitEnrollmentStatus,
                            benefitEnrollments => benefitEnrollments.BenefitEnrollments.BenefitEnrollmentStatusKey,
                            benefitEnrollmentStatuses => benefitEnrollmentStatuses.BenefitEnrollmentStatusKey,
                            (benefitEnrollments, benefitEnrollmentStatuses) => new {
                                BenefitEnrollments = benefitEnrollments.BenefitEnrollments,
                                PlanYears = benefitEnrollments.PlanYears,
                                BenefitPlanOptions = benefitEnrollments.BenefitPlanOptions,
                                Members = benefitEnrollments.Members,
                                BenefitEnrollmentStatuses = benefitEnrollmentStatuses
                            })
                        .Where(
                            p =>
                                p.Members.CchId.Equals(cchId)
                                && p.PlanYears.PlanYearName.Equals(year.ToString())
                                && p.BenefitEnrollmentStatuses.BenefitEnrollmentStatusName.Equals("Active")
                        )
                        .Select(
                            p => new BenefitPlan {
                                Id = p.BenefitPlanOptions.SourcePlanOptionCode,
                                Name = p.BenefitPlanOptions.BenefitPlanOptionName
                            })
                        .Distinct()
                        .ToListAsync();

                        benefitPlans.Add(new BenefitPlan
                        {
                            Id = employee.BenefitGroupCode,
                            Name = "Benefit Group"
                        });
                        benefitPlans.Add(new BenefitPlan
                        {
                            Id = employee.EarningsGroupCode,
                            Name = "Earnings Group"
                        });
                    }
                }
                return benefitPlans;
            }
            public async Task<List<BenefitPlan>> GetEmployeeBenefitsEligibleAsync(int cchId) {
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

                List<BenefitPlan> benefitPlans = new List<BenefitPlan>();

                using (var ctx = new EmployeeContext(_connectionString)) {
                    var employee = await ctx.Employees
                        .FirstOrDefaultAsync(
                            p => p.CchId.Equals(cchId)
                            && p.CurrentRecordInd.Equals(1));

                    if (employee != null) {
                        benefitPlans.Add(new BenefitPlan
                        {
                            Id = employee.BenefitGroupCode,
                            Name = "Benefit Group"
                        });
                        benefitPlans.Add(new BenefitPlan
                        {
                            Id = employee.EarningsGroupCode,
                            Name = "Earnings Group"
                        });
                    }
                }
                return benefitPlans;
            }

            public async Task<BenefitMedicalPlan> GetEmployeeBenefitEnrollmentMedicalPlanAsync(int memberKey)
            {
                //This is a one-off method because of the "MED" and "EXPAT" criteria and is not re-usable
                //TODO: refactor where clause if other BenefitEnrollment types are needed beyond just medical
                if (string.IsNullOrEmpty(_connectionString))
                    throw new InvalidOperationException("Failed to initialize repository");

                BenefitMedicalPlan plan = new BenefitMedicalPlan();

                using (var ctx = new EmployeeContext(_connectionString))
                {
                    var date = DateTime.UtcNow;
                    plan = await ctx.BenefitEnrollments
                    .Join(
                        ctx.PlanYears,
                        benefitEnrollments => benefitEnrollments.PlanYearKey,
                        planYears => planYears.PlanYearKey,
                        (benefitEnrollments, planYears) => new
                        {
                            BenefitEnrollments = benefitEnrollments,
                            PlanYears = planYears
                        })
                    .Join(
                        ctx.BenefitPlanOptions,
                        benefitEnrollments => benefitEnrollments.BenefitEnrollments.BenefitPlanOptionKey,
                        benefitPlanOptions => benefitPlanOptions.BenefitPlanOptionKey,
                        (benefitEnrollments, benefitPlanOptions) => new
                        {
                            BenefitEnrollments = benefitEnrollments,
                            BenefitPlanOptions = benefitPlanOptions
                        })
                    .Join(
                        ctx.BenefitEnrollmentStatus,
                        benefitEnrollments => benefitEnrollments.BenefitEnrollments.BenefitEnrollments.BenefitEnrollmentStatusKey,
                        benefitEnrollmentStatus => benefitEnrollmentStatus.BenefitEnrollmentStatusKey,
                        (benefitEnrollments, benefitEnrollmentStatus) => new
                        {
                            BenefitEnrollments = benefitEnrollments,
                            BenefitEnrollmentStatus = benefitEnrollmentStatus
                        })
                    .Where(
                            p =>
                                p.BenefitEnrollments.BenefitPlanOptions.BenefitTypeCode.Equals("MED")
                                && !p.BenefitEnrollments.BenefitPlanOptions.BenefitPlanTypeCode.Equals("EXPAT")
                                && p.BenefitEnrollments.BenefitEnrollments.BenefitEnrollments.EnrolledMemberKey.Equals(memberKey)
                                && p.BenefitEnrollments.BenefitEnrollments.PlanYears.PlanYearStartDate.Value <= date
                                && p.BenefitEnrollments.BenefitEnrollments.PlanYears.PlanYearEndDate.Value >= date
                                && p.BenefitEnrollmentStatus.BenefitEnrollmentStatusName.Equals("Active")
                        )
                    .Select(
                            p => new BenefitMedicalPlan
                            {
                                MemberPlanId = p.BenefitEnrollments.BenefitEnrollments.BenefitEnrollments.BenefitPlanOptionKey,
                                PlanType = p.BenefitEnrollments.BenefitPlanOptions.BenefitTypeCode,
                                SubscriberPlanId = p.BenefitEnrollments.BenefitEnrollments.BenefitEnrollments.SubscriberPlanId
                            }
                        )
                    .FirstOrDefaultAsync();
                }
                return plan;
            }
        }
    }
}