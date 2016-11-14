using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using ClearCost.UnitTesting;
using System.Net;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Summary description for UserContentTests
    /// </summary>
    [TestClass]
    public class UserContentTests {
        public UserContentTests() {
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
        [TestCategory("UserContent")]
        public void CanGetUserContentLocale() {

            var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(), "mary.smith@cchcaesars.com");
            string result = null;

            using (var webClient = new WebClient()) {
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                webClient.Headers.Add("ApiKey", "DB366C62-88B6-402D-BCB7-E3FC384776E1");
                webClient.Headers.Add("AuthHash", "631B756ADC1A2347E1815DD719BB59E1A032C345AE8C6539C22C568CC5DCBBF0D55A0BF28C3A0AC638D7C399E469A0658CB2E028517F521B98DE3D6192BA5AB08D52BF36035C6FB812F53F89F1EB83BF5BEDC6688BEDACFAB657C15673335CA64FD7BF1C1334354B89E41E025E8479D0");

                result = webClient.DownloadString(String.Format("{0}/v1/Animation/UserContent/UserContentLocale/en/0/", ctx.RootUri));
            }
            

            Assert.IsNotNull(result);
        }
    }
}
