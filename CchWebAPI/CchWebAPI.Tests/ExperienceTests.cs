using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ClearCost.UnitTesting;
using System.Net.Http;
using System.Net;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Summary description for ExperienceTests
    /// </summary>
    [TestClass]
    public class ExperienceTests {
        public ExperienceTests() {
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
        public void CanLogInitialExperience() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi,
                "mary.smith@cchcaesars.com");

            var experienceLogRequest = new ExperienceLogRequest {
                EmployerId = 21,
                EventName = "Unit Test",
                LogComment = "confirming route works.",
                DeviceId = "0"
            };

            var urlResult = ApiUtil.PostJson<HttpResponseMessage>(ctx,
                String.Format("Animation/Experience/LogInitial/{0}", ctx.Employer.HandshakeId), 
                experienceLogRequest);

            Assert.IsNotNull(urlResult);
            Assert.AreEqual(HttpStatusCode.OK, urlResult.Item1);
        }
    }
}
