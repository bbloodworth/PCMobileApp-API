using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Services;
using ClearCost.Security.JWT;
using ClearCost.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CchWebAPI.Tests {
    [TestClass]
    public class CardTests {
        [TestMethod]
        public void CanEmailIdCard() {
            //THIS IS going to fail during transition in the JWT token swap. Will readress once that's
            //working and deployed.

            //Have to find a way to make this execute successfully on the CI server.
            //Without significant work, we'd need to setup and instance of the media service
            //on devweb that this can call.

            //if (!Debugger.IsAttached)
            //    return;

            //var memberCardUrlResult = new CardService().GetMemberCardUrls("en", 11, 171230);

            //Assert.IsNotNull(memberCardUrlResult);
            //Assert.AreNotEqual(0, memberCardUrlResult.TotalCount);

            //memberCardUrlResult.Results.ForEach(r => {
            //    Debug.WriteLine(r.CardUrl);
            //});

            //var cardWebRequest = new MemberCardWebRequest() {
            //    CardToken = memberCardUrlResult.Results[0].SecurityToken,
            //    ToEmail = "dstrickland@clearcosthealth.com",
            //    Subject = "CanEmailIdCard Unit Test",
            //    Message = "Unit test result"
            //};

            //var result = new CardService().SendIdCardEmail(11, cardWebRequest);
            //Assert.IsNotNull(result);
            //Assert.AreEqual(true, result.Item1);
            //Assert.IsTrue(result.Item2.SvgSuccess);
            //Assert.AreEqual(string.Empty, result.Item3);
        }

        [TestMethod]
        public void CanUseWapiToEmailIdCard() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi,
                "mary.smith@cchcaesars.com");

            var memberUrlsResult = ApiUtil.GetJsonResult<dynamic>(ctx, "Animation/Card/CardUrls/en");

            Assert.IsNotNull(memberUrlsResult);

            var cardWebRequest = new MemberCardWebRequest() {
                CardToken = memberUrlsResult.Results[0].SecurityToken,
                ToEmail = "dstrickland@clearcosthealth.com",
                Subject = "CanEmailIdCard Unit Test",
                Message = "Unit test result"
            };

            var emailResult = ApiUtil.PostJson<MemberCardWebRequest>(ctx, "Animation/Card/Email", cardWebRequest);

            Assert.IsNotNull(emailResult);
            Assert.AreEqual(HttpStatusCode.OK, emailResult.Item1);
        }

        [TestMethod]
        public void CanUseWapiToViewIdCard() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.dwapi,
                "mary.smith@cchcaesars.com");

            dynamic memberUrlsResult = ApiUtil.GetJsonResult<dynamic>(ctx, "Animation/Card/CardUrls/en");

            Assert.IsNotNull(memberUrlsResult);

            Assert.IsNotNull(memberUrlsResult.Results);
            List<CardResult> cardResults = memberUrlsResult.Results.ToObject<List<CardResult>>();

            var authResult = ctx.GetAuthResult();

            cardResults.ForEach(r => { 
                Debug.WriteLine(r.CardUrl);
                var result = ApiUtil.GetContentResult(ctx, r.CardUrl, false, string.Empty, authResult);

                Assert.IsFalse(string.IsNullOrEmpty(result));
                Assert.IsFalse(result.Contains("Something went wrong"));
            });
        }

        [TestMethod]
        public void CanGetCardTokens() {
            var service = new CardService();
            var results = service.GetMemberCardUrls("en", 11, 57020);

            Assert.IsNotNull(results);
            Assert.AreNotEqual(0, results.TotalCount);

            results.Results.ForEach(c => {
                var cardToken = JsonConvert.DeserializeObject<CardToken>(JwtService.DecryptPayload(c.SecurityToken));

                Assert.IsNotNull(cardToken);
                Assert.IsNotNull(cardToken.CardDetail);
                Assert.IsFalse(string.IsNullOrEmpty(cardToken.CardDetail.CardTypeFileName));
                Assert.IsTrue(DateTime.UtcNow < cardToken.Expires);
                Assert.AreNotEqual(0, cardToken.CardDetail.CardTypeId);
                Assert.AreNotEqual(0, cardToken.CardDetail.CardViewModeId);
            });
        }

        [TestMethod]
        public void CanGetCardDetailFromJwt() {
            var service = new CardService();
            var results = service.GetMemberCardUrls("en", 11, 57020);

            Assert.IsNotNull(results);

            var cardDetail = service.GetCardDetail(11, results.Results[0].SecurityToken);
            Assert.IsNotNull(cardDetail);
            Assert.IsFalse(string.IsNullOrEmpty(cardDetail.CardTypeFileName)); 
            Assert.AreNotEqual(0, cardDetail.CardTypeId);
            Assert.AreNotEqual(0, cardDetail.CardViewModeId);
        }

        [TestMethod]
        public void CannotGetCardDetailForExpiredJwt() {
            var service = new CardService();
            var cardToken = JwtService.EncryptPayload(JsonConvert.SerializeObject(new CardToken() {
                Expires = DateTime.UtcNow.AddMinutes(-2),
                EmployerId = 11
            }));

            var cardDetail = service.GetCardDetail(11, cardToken);
            Assert.IsTrue(cardDetail.Expired);
        }

        [TestMethod]
        public void CannotGetCardDetailForMismatchedEmployerId() {
            var service = new CardService();
            var cardToken = JwtService.EncryptPayload(JsonConvert.SerializeObject(new CardToken() {
                Expires = DateTime.UtcNow.AddMinutes(15),
                EmployerId = 11
            }));

            var cardDetail = service.GetCardDetail(12, cardToken);
            Assert.IsTrue(cardDetail.Invalid);
        }
    }
}
