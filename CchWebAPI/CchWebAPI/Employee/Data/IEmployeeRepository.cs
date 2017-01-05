using CchWebAPI.Employee.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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
            Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(int cchId, int planId);
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
                    employee = await ctx.Employees.FirstOrDefaultAsync(p => p.CchId.Equals(cchId) && p.CurrentRecordInd.Value);
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
                    member = await ctx.Members.FirstOrDefaultAsync(p => p.Cchid.Equals(cchId) && p.CurrentRecordInd);
                }

                return member;
            }

            public async Task<List<PlanMember>> GetEmployeeBenefitPlanMembersAsync(int cchId, int planId) {
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
                        .Where(
                            //p => p.Dependent.Cchid.Equals(cchId)
                            p => p.BenefitEnrollment.SubscriberMemberKey.Equals(cchId)
                            && p.BenefitEnrollment.BenefitPlanOptionKey.Equals(planId)
                            //&& p.CurrentRecordInd
                        )
                        .Select(
                            p => new PlanMember {
                                //CchId = p.Dependent.Cchid,
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
                        .Where(
                            //p => p.Dependent.Cchid.Equals(cchId)
                            p => p.BenefitEnrollment.SubscriberMemberKey.Equals(cchId)

                        )
                        .Select(
                            p => new PlanMember {
                                CchId = p.Dependent.Cchid,
                                FirstName = p.Dependent.MemberFirstName,
                                LastName = p.Dependent.MemberLastName
                            }
                        )
                        .Distinct()
                        .ToListAsync();
                }

                return planMembers;
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
                                BenefitEnrollments = benefitEnrollments,
                                BenefitPlanOptions = benefitPlanOptions
                            })
                        .Where(
                            p =>
                                p.BenefitEnrollments.BenefitEnrollments.SubscriberMemberKey.Equals(cchId)
                                && p.BenefitEnrollments.PlanYears.PlanYearName.Equals(year.ToString())
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
                        .FirstOrDefaultAsync(p => p.CchId.Equals(cchId));

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