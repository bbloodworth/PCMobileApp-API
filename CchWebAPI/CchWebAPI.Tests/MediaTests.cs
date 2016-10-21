using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ClearCost.UnitTesting;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Summary description for MediaTests
    /// </summary>
    [TestClass]
    public class MediaTests {
        public MediaTests() {
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
        [TestCategory("Media Tests")]
        public void CanGetMediaUrl() {

            var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(),
                "mary.smith@cchcaesars.com");

            // This needs to be made dynamic somehow.  Also, the api returns a string even if
            // there's no matching record in the database.  The api should be updated to return a
            // 404 if there's no matching database record.
            var results = ApiUtil.GetJsonResult<dynamic>(ctx,
                String.Format("Animation/Media/MediaUrl/{0}",
                "14.38"));
            
            Assert.IsNotNull(results);
        }
    }
}
