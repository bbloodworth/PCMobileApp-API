using System;
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
        [TestCategory("Membership")]
        public void CanGetWapiAuthResult() {
            if (!Debugger.IsAttached)
                return;

            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.lawapi, @"mary.smith@cchcaesars.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi, @"mary.smith@cchcaesars.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi, @"mikew@cchdemo.com").GetAuthResult();
            //var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi, @"mary.s@cchdemo.com").GetAuthResult("password1");
            var authResult = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi, @"mary.smith@cchcaesars.com").GetAuthResult();
            Assert.IsNotNull(authResult);
            Assert.IsFalse(string.IsNullOrEmpty(authResult.AuthHash));
        }

        [TestMethod]
        [TestCategory("Membership")]
        public void CanExecuteResetPasswordStep1() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi,
                "mary.smith@cchcaesars.com");

            var payload = new UserAuthenticationRequest() {
                UserName = "mary.smith@cchcaesars.com",
                FullSsn = "000004835"
            };

            var step1Result = ApiUtil.PostJson<dynamic>(ctx, "Animation/Membership/Password/Reset0", payload);

            Assert.IsNotNull(step1Result);
            Assert.AreEqual(HttpStatusCode.OK, step1Result.Item1);
            Assert.IsNotNull(step1Result.Item2);
            string authHash = step1Result.Item2.AuthHash;
            Assert.IsFalse(string.IsNullOrEmpty(authHash));
        }

        [TestMethod]
        [TestCategory("Membership")]
        public void CanExecuteResetPasswordStep2() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi,
                "mary.smith@cchcaesars.com");

            var payload = new UserAuthenticationRequest() {
                UserName = "mary.smith@cchcaesars.com",
                FullSsn = "000004835"
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
        [TestMethod]
        [TestCategory("Membership")]
        [TestCategory("Integration Tests")]
        public void CanLogin() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi,
                "mary.smith@cchcaesars.com");

            dynamic payload = new ExpandoObject();
            payload.username = "mary.smith@cchcaesars.com";
            payload.password = "dem0-User";

            var urlResults = ApiUtil.PostJson<dynamic>(ctx,
                "Animation/Membership/Login/182E533E-4488-4917-83B1-DB112DA71739",
                payload);

            Assert.IsNotNull(urlResults);
            Assert.AreEqual(HttpStatusCode.OK, urlResults.Item1);
        }
        [TestMethod]
        [TestCategory("Membership")]
        public void LoginRequiresHandshakeId() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi,
                "mary.smith@cchcaesars.com");

            dynamic payload = new ExpandoObject();
            payload.username = "mary.smith@cchcaesars.com";
            payload.password = "dem0-User";

            var urlResults = ApiUtil.PostJson<dynamic>(ctx,
                "Animation/Membership/Login/",
                payload);

            Assert.IsNotNull(urlResults);
            Assert.AreEqual(HttpStatusCode.Unauthorized, urlResults.Item1);
        }
    }
}
