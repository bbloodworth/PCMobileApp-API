﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClearCost.UnitTesting;
using CchWebAPI.Areas.Animation.Models;
using System.Net;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Http;

namespace CchWebAPI.Tests {
    [TestClass]
    public class MembershipTests {

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void CanGetWapiAuthResult() {

            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.lawapi, @"mary.smith@cchcaesars.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(), @"mary.smith@cchcaesars.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(), @"mikew@cchdemo.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(), @"mary.s@cchdemo.com").GetAuthResult("password1");
            //var authResult = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(), @"mary.smith@cchcaesars.com").GetAuthResult();

            foreach (var testUser in new TestAccounts().Accounts) {
                var authResult = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(), testUser.Username).GetAuthResult();
                Assert.IsNotNull(authResult);
                Assert.IsFalse(string.IsNullOrEmpty(authResult.AuthHash));
            }
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void CanExecuteResetPasswordStep1() {

            foreach (var testUser in new TestAccounts().Accounts) {
                var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(),
                    testUser.Username);

                var payload = new UserAuthenticationRequest() {
                    UserName = testUser.Username,
                    FullSsn = testUser.FullSsn
                };

                var step1Result = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password/Reset0", payload);

                Assert.IsNotNull(step1Result);
                Assert.AreEqual(HttpStatusCode.OK, step1Result.Item1);
                Assert.IsNotNull(step1Result.Item2);
                string authHash = step1Result.Item2.AuthHash;
                Assert.IsFalse(string.IsNullOrEmpty(authHash));
            }
        }

        [TestMethod]
        [TestCategory("Membership Tests")]
        public void CanExecuteResetPasswordStep2() {
            foreach (var testUser in new TestAccounts().Accounts) {
                var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(),
                    testUser.Username);

                var payload = new UserAuthenticationRequest() {
                    UserName = testUser.Username,
                    FullSsn = testUser.FullSsn
                };

                var step1Result = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password/Reset0", payload);

                Assert.IsNotNull(step1Result);
                Assert.AreEqual(HttpStatusCode.OK, step1Result.Item1);
                Assert.IsNotNull(step1Result.Item2);

                payload.SecretQuestion = step1Result.Item2.Question;
                payload.SecretAnswer = testUser.SecretAnswer;
                payload.NewPassword = testUser.Password;

                var step2Result = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password/Reset2", payload);

                Assert.IsNotNull(step2Result);
                Assert.AreEqual(HttpStatusCode.OK, step2Result.Item1);
                Assert.IsNotNull(step2Result.Item2);
            }
        }
        [TestMethod]
        [TestCategory("Membership Tests")]
        //[TestCategory("Integration Tests")]
        public void CanLogin() {

            foreach (var testUser in new TestAccounts().Accounts) {
                var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(),
                    testUser.Username);

                dynamic payload = new ExpandoObject();
                payload.username = testUser.Username;
                payload.password = testUser.Password;

                var urlResults = ApiUtil.PostJson<dynamic>(ctx,
                    String.Format("Animation/Membership/Login/{0}", ctx.Employer.HandshakeId),
                    payload);

                Assert.IsNotNull(urlResults);
                Assert.AreEqual(HttpStatusCode.OK, urlResults.Item1);
            }
        }
        [TestMethod]
        [TestCategory("Membership Tests")]
        public void LoginRequiresHandshakeId() {

            foreach (var testUser in new TestAccounts().Accounts) {
                var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(),
                    testUser.Username);

                dynamic payload = new ExpandoObject();
                payload.username = testUser.Username;
                payload.password = testUser.Password;

                var urlResults = ApiUtil.PostJson<dynamic>(ctx,
                    "Animation/Membership/Login/",
                    payload);

                Assert.IsNotNull(urlResults);
                Assert.AreEqual(HttpStatusCode.Unauthorized, urlResults.Item1);
            }
        }
        [TestMethod]
        //[TestCategory("MPM-1673")]
        //[TestCategory("Integration Tests")]
        [TestCategory("Membership Tests")]
        public void CanGetGoogleSegment() {

            foreach (var testUser in new TestAccounts().Accounts) {
                var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(),
                    testUser.Username);

                var urlResult = ApiUtil.GetJsonResult<dynamic>(ctx, "Animation/Membership/Google/Segment");

                Assert.IsNotNull(urlResult);
            }
        }
        [TestMethod]
        [TestCategory("Membership Tests")]
        public void CanChangePassword() {

            foreach(var testUser in new TestAccounts().Accounts) {
                var ctx = UnitTestContext.Get(EnvironmentHelper.GetEnvironment(),
                    testUser.Username);

                var payload = new UserAuthenticationRequest();
                payload.Password = testUser.Password;
                payload.NewPassword = testUser.Password;

                var urlResult = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password",
                    payload, testUser.Password);

                Assert.IsNotNull(urlResult);
                Assert.AreEqual(HttpStatusCode.OK, urlResult.Item1);
            }
        }
    }
}
