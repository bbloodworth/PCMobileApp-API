﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using CchWebAPI.PComm.Models;
using ClearCost.Data;
using Dapper;
using ClearCost.UnitTesting;
using System.Net.Http;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Summary description for SettingsTests
    /// </summary>
    [TestClass]
    public class SettingsTests {
        public SettingsTests() {
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
        [TestCategory("Settings Tests")]
        public void CanGetConfigurationValue() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi,
                "mary.smith@cchcaesars.com");

            var urlResults = ApiUtil.GetJsonResult<dynamic>(ctx,
                String.Format("Animation/Settings/{0}/{1}/ConfigValue/?configKey=NetworkLatencyThresholdJson", 
                ctx.Employer.HandshakeId,
                ctx.Employer.Id));

            Assert.IsNotNull(urlResults.Results.Id);
        }
        [TestMethod]
        [TestCategory("Settings Tests")]
        public void CanGetConfigurationValues() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi,
                "mary.smith@cchcaesars.com");

            var urlResults = ApiUtil.GetJsonResult<dynamic>(ctx,
                String.Format("Animation/Settings/GetConfigurationValues/{0}/{1}",
                ctx.Employer.Id,
                ctx.Employer.HandshakeId));

            Assert.IsNotNull(urlResults.Security);
        }
    }
}