using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CchWebAPI.MedicalPlan.Data;
using CchWebAPI.MedicalPlan.Dispatchers;
using CchWebAPI.Controllers;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class MedicalPlanTests {
        public MedicalPlanTests() {
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
        public async Task CanGetMedicalPlan() {
            foreach (var testAccount in TestAccounts.DemoAccounts.Accounts) {
                var repository = new MedicalPlanRepository();
                repository.Initialize(EmployerConnectionString.GetConnectionString(testAccount.EmployerId).DataWarehouse);

                var dispatcher = new MedicalPlanDispatcher(repository);
                var controller = new MedicalPlansController(dispatcher);

                var result = await controller.GetMedicalPlanAsync(testAccount.EmployerId, testAccount.MedicalPlanId);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.IsSuccess);
            }
        }
        [TestMethod]
        public async Task CanGetMedicalPlanAccumulation() {
            foreach (var testAccount in TestAccounts.DemoAccounts.Accounts) {
                var repository = new MedicalPlanRepository();
                repository.Initialize(EmployerConnectionString.GetConnectionString(testAccount.EmployerId).DataWarehouse);

                var dispatcher = new MedicalPlanDispatcher(repository);
                var controller = new MedicalPlansController(dispatcher);

                var result = await controller.GetMedicalPlanAccumulationAsync(
                    testAccount.EmployerId,
                    testAccount.CchId,
                    testAccount.MedicalPlanId,
                    DateTime.UtcNow.Year);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.IsSuccess);
            }
        }
    }
}
