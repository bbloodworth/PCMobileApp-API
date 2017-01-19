using CchWebAPI.Controllers;
using CchWebAPI.Employee.Data.V1;
using CchWebAPI.Employee.Data.V2;
using CchWebAPI.Employee.Dispatchers;
using CchWebAPI.Employee.Models;
//using CchWebAPI.Employees.Data;
//using CchWebAPI.Employees.Dispatchers;
using ClearCost.Platform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace CchWebAPI.Tests {
    [TestClass]
    public class EmployeesTests {
        [TestMethod]
        [TestCategory("MPM-1486")]
        [TestCategory("Employees Tests")]
        public async Task CanGetEmployeeFromDB() {
            foreach(var testAccount in TestAccounts.Accounts) {
                var employer = EmployerCache.Employers.FirstOrDefault(e => 
                    e.Id == testAccount.EmployerId);

                var repository = new Employee.Data.V2.EmployeeRepository();
                repository.Initialize(employer.ConnectionString);

                var dispatcher = new EmployeeDispatcher(repository);
                var controller = new EmployeesController(dispatcher);

                var result = await controller.GetEmployeeAsync(employer, testAccount.CchId);

                Assert.IsNotNull(result);
                //Assert.IsNotNull(result.Content);
            }
        }

        [TestMethod]
        [TestCategory("Employees Tests")]
        public async Task CanGetBenefitPlanMembers() {
            foreach (var testAccount in TestAccounts.TyLinAccounts.Accounts) {
                foreach (var benefitPlanId in testAccount.BenefitPlans) {
                    var employer = EmployerCache.Employers.FirstOrDefault(e =>
                        e.Id == testAccount.EmployerId);

                    var repository = new Employee.Data.V2.EmployeeRepository();
                    repository.Initialize(employer.ConnectionString);

                    var dispatcher = new EmployeeDispatcher(repository);
                    var controller = new EmployeesController(dispatcher);

                    var result = await controller.GetEmployeeBenefitPlanMembersAsync(
                        employer,
                        testAccount.CchId,
                        benefitPlanId);

                    Assert.IsNotNull(result);
                    //Assert.IsNotNull(result.Content);
                }
            }
        }
        [TestMethod]
        [TestCategory("Employees Tests")]
        public async Task CanGetMembers() {
            foreach (var testAccount in TestAccounts.TyLinAccounts.Accounts) {
                    var employer = EmployerCache.Employers.FirstOrDefault(e =>
                        e.Id == testAccount.EmployerId);

                    var repository = new Employee.Data.V2.EmployeeRepository();
                    repository.Initialize(employer.ConnectionString);

                    var dispatcher = new EmployeeDispatcher(repository);
                    var controller = new EmployeesController(dispatcher);

                    var result = await controller.GetEmployeeDependentsAsync(
                        employer,
                        testAccount.CchId
                    );

                    Assert.IsNotNull(result);
                    //Assert.IsNotNull(result.Content);
            }
        }
    }
}
