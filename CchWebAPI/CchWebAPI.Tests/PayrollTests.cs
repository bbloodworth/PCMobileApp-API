using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClearCost.UnitTesting;
using System.Net;
using ClearCost.Net;
using CchWebAPI.Controllers;
using CchWebAPI.Payroll.Dispatchers;
using CchWebAPI.Payroll.Data;
using System.Threading.Tasks;
using ClearCost.Platform;
using System.Linq;
using System.Web.Http.Results;
using CchWebAPI.Payroll.Models;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Summary description for Payroll
    /// </summary>
    [TestClass]
    public class PayrollTests {
        public PayrollTests() {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public async Task CanGetDatesPaid() {
            foreach (var testAccount in TestAccounts.TyLinAccounts.Accounts) {
                var employer = EmployerCache.Employers.FirstOrDefault(e =>
                    e.Id == testAccount.EmployerId);

                var repository = new PayrollRepository();
                repository.Initialize(employer.ConnectionString);

                var dispatcher = new PayrollDispatcher(repository);
                var controller = new PayrollController(dispatcher);

                var result = await controller.GetDatesPaidAsync(employer, testAccount.CchId)
                    as OkNegotiatedContentResult<List<DatePaid>>;

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Content);
            }
        }
        [TestMethod]
        public async Task CanGetPaycheck() {
            foreach (var testAccount in TestAccounts.TyLinAccounts.Accounts) {
                var employer = EmployerCache.Employers.FirstOrDefault(e =>
                    e.Id == testAccount.EmployerId);

                var repository = new PayrollRepository();
                repository.Initialize(employer.ConnectionString);

                var dispatcher = new PayrollDispatcher(repository);
                var controller = new PayrollController(dispatcher);

                var result = await controller.GetPaycheckAsync(employer, testAccount.PaycheckDocumentId)
                    as OkNegotiatedContentResult<Paycheck>;

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.Content);
            }
        }
    }
}
