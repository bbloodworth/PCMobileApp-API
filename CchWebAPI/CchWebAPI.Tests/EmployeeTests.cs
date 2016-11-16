using CchWebAPI.Controllers;
using CchWebAPI.Employee.Data;
using CchWebAPI.Employee.Dispatchers;
using CchWebAPI.Employees.Data;
using CchWebAPI.Employees.Dispatchers;
using ClearCost.Platform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Tests {
    [TestClass]
    public class EmployeesTests {
        [TestMethod]
        [TestCategory("MPM-1486")]
        [TestCategory("Employees Tests")]
        public async Task CanGetEmployeeFromDB() {
            var repo = new EmployeesRepository();
            repo.Initialize(ConfigurationManager.ConnectionStrings["Platform"].ConnectionString);
            var dispatcher = new EmployeesDispatcher(repo);

            var result = await dispatcher.ExecuteAsync(57020,
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(11)));

            Assert.IsNotNull(result);
        }

        [TestMethod]
        [TestCategory("Employees Tests")]
        public async Task CanGetBenefitPlanMembers() {
            foreach (var testAccount in TestAccounts.DemoAccounts.Accounts) {
                foreach (var benefitPlanId in testAccount.BenefitPlans) {
                    var repository = new EmployeeRepository();
                    repository.Initialize(EmployerConnectionString.GetConnectionString(testAccount.EmployerId).DataWarehouse);

                    var dispatcher = new EmployeeDispatcher(repository);
                    var controller = new EmployeesController(dispatcher);

                    var result = await controller.GetEmployeeBenefitPlanMembers(
                        testAccount.EmployerId,
                        testAccount.CchId,
                        benefitPlanId);

                    Assert.IsNotNull(result);
                    Assert.IsTrue(result.IsSuccess);
                }
            }
        }
    }
}
