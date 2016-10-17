using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClearCost.UnitTesting;
using ClearCost.Net;
using System.Diagnostics;
using System.Dynamic;
using System.Net;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Summary description for MessagingTests
    /// </summary>
    [TestClass]
    public class MessagingTests {
        public MessagingTests() {
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
        [TestCategory("Messaging")]
        public void CanUpdatePushPromptStatus() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi,
                "mary.smith@cchcaesars.com");

            dynamic payload = new ExpandoObject();
            payload.EmployerId = 21;
            payload.EventName = "Unit Test";
            payload.LogComment = "confirming route works.";
            payload.DeviceId = 0;

            var result = ApiUtil.PostJson<ApiResult<dynamic>>(ctx,
                String.Format("Animation/Messaging/PushPrompt/{0}",
                    ctx.Employer.Id),
                payload);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.Item1);
        }
        [TestMethod]
        [TestCategory("Messaging")]
        public void CanGetPushPromptStatus() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi,
                "mary.smith@cchcaesars.com");

            var result = ApiUtil.GetJsonResult<dynamic>(ctx,
                String.Format("Animation/Messaging/PushPrompt/{0}/{1}/{2}",
                    ctx.Employer.Id,
                    ctx.Employer.HandshakeId,
                    "96dcba2ce8e6a7d7ce57e05a75cf2902d721b47457fa64662ca2ae46e4ca9bce"
                    ));

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PromptStatus);
        }
        [TestMethod]
        [TestCategory("Messaging")]
        public void CanUpdateEmailAddress() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi,
                "mary.smith@cchcaesars.com");

            dynamic payload = new ExpandoObject();
            payload.NewEmail = "mary.smith@cchcaesars.com";

            var result = ApiUtil.PostJson<dynamic>(ctx,
                "Animation/Membership/Email",
                payload);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, result.Item1);
        }
    }
}
