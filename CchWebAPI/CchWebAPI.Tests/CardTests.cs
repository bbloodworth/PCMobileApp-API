using System;
using System.Collections.Generic;
using System.Diagnostics; 
using System.Net;

using CchWebAPI.Areas.Animation.Models; 
using CchWebAPI.Services;
using ClearCost.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CchWebAPI.Tests {
    [TestClass]
    public class CardTests {
        [TestMethod]
        public void CanEmailIdCard() {
            //Have to find a way to make this execute successfully on the CI server.
            //Without significant work, we'd need to setup and instance of the media service
            //on devweb that this can call.


            //var tuple = new CardService().GetMemberCardUrls("en", 11, 171230); 

            //Assert.IsNotNull(tuple);
            //Assert.AreEqual(true, tuple.Item1);

            //dynamic memberCardUrls = tuple.Item2;
            //Assert.IsNotNull(memberCardUrls);

            //List<CardResult> cardResults = memberCardUrls.Results;

            //cardResults.ForEach(r => {
            //    Debug.WriteLine(r.CardUrl);
            //});

            //var cardWebRequest = new MemberCardWebRequest() {
            //    CardToken = (memberCardUrls.Results as List<CardResult>)[0].SecurityToken,
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
            //var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi,
            //    "mary.apptest@cch.com");

            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.lawapi,
                "mary.apptest@cch.com");

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
    }
}
