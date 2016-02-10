using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClearCost.UnitTesting;
using CchWebAPI.Areas.Animation.Models;
using System.Net;

namespace CchWebAPI.Tests {
    [TestClass]
    public class MembershipTests {

        [TestMethod]
        public void CanGetWapiAuthResult() {
            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.lawapi, @"mary.smith@cchcaesars.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi, @"mary.smith@cchcaesars.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi, @"mikew@cchdemo.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi, @"mary.s@cchdemo.com").GetAuthResult("password1");
            var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi, @"mary.smith@cchcaesars.com").GetAuthResult();
            Assert.IsNotNull(authResult);
            Assert.IsFalse(string.IsNullOrEmpty(authResult.AuthHash));
        }

        [TestMethod]
        public void CanExecuteResetPasswordStep1() {
            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.lawapi,
                "mary.apptest@cch.com");

            var payload = new UserAuthenticationRequest() {
                UserName = "mary.apptest@cch.com",
                FullSsn = "001020304"
            };

            var step1Result = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password/Reset0", payload);

            Assert.IsNotNull(step1Result);
            Assert.AreEqual(HttpStatusCode.OK, step1Result.Item1);
            Assert.IsNotNull(step1Result.Item2);
            string authHash = step1Result.Item2.AuthHash;
            Assert.IsFalse(string.IsNullOrEmpty(authHash));
        }

        [TestMethod]
        public void CanExecuteResetPasswordStep2() {
            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.lawapi,
                "mary.apptest@cch.com");

            var payload = new UserAuthenticationRequest() {
                UserName = "mary.apptest@cch.com",
                FullSsn = "001020304"
            };

            var step1Result = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password/Reset0", payload);

            Assert.IsNotNull(step1Result);
            Assert.AreEqual(HttpStatusCode.OK, step1Result.Item1);
            Assert.IsNotNull(step1Result.Item2);

            payload.SecretQuestion = step1Result.Item2.Question;
            payload.SecretAnswer = "crazytown";
            payload.NewPassword = "dem0-User";

            var step2Result = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password/Reset2", payload);

            Assert.IsNotNull(step2Result);
            Assert.AreEqual(HttpStatusCode.OK, step2Result.Item1);
            Assert.IsNotNull(step2Result.Item2);
        }
    }
}
