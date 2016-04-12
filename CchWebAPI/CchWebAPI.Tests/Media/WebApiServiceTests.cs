using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CchWebAPI.Services;
using ClearCost.Security.JWT;
using CchWebAPI.Areas.Animation.Models;
using DynamicAnimation.Common;
using DynamicAnimation.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CchWebAPI.Tests.Media {
    [TestClass]
    public class WebApiServiceTests {
        [TestMethod]
        public void CanGetAuthorizationByCchId() {
            var response = WebApiService.GetAuthorizationByCchId(11, 57020);
            Assert.IsNotNull(response);
            Assert.IsFalse(string.IsNullOrEmpty(response.AuthHash));
        }

        [TestMethod]
        public void CanGetCampaignIntro() {
            var response = WebApiService.GetCampaignIntro(11, 1);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void CanGetCampaignSession() {
            return;
            //This will not work without refactoring away from sessions
            var response = WebApiService.GetCampaignSession(CampaignSessionModel.Current);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void CanGetAuthorization() {
            var response = WebApiService.GetAuthorization("SmithCaesars", "1970/01/01", "4321");
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void CanGetVideoCampaign() {
            var response = WebApiService.GetVideoCampaign(1, 11);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void CanGetVideoCampaignMemberData() {
            var response = WebApiService.GetVideoCampaignMemberData("57020", 11);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void CanGetUserSessionVideoData() {
            var response = WebApiService.GetUserSessionVideoData("57020", 11);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public void CanLogAnonEvent() {
            var result = WebApiService.LogAnonEvent(new DynamicAnimation.Models.ExperienceLogRequest() {
                CchId = 57020,
                ContentId = "unittest",
                EmployerId = 11,
                EventId = 0,
                EventName = "UnitTest",
                ExperienceUserId = "unittest",
                LogComment = "CanLogAnonEvent UnitTest Run"
            });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CanLogUserEvent() {
            var response = WebApiService.GetAuthorizationByCchId(11, 57020);

            var result = WebApiService.LogUserEvent(new DynamicAnimation.Models.ExperienceLogRequest() {
                CchId = 57020,
                ContentId = "unittest",
                EmployerId = 11,
                EventId = 0,
                EventName = "UnitTest",
                ExperienceUserId = "unittest",
                LogComment = "CanLogUserEvent UnitTest Run"
            }, response.AuthHash);

            Assert.IsNotNull(result);
        }


        [TestMethod]
        public void CanLogInitialEvent() {
            var response = WebApiService.GetAuthorizationByCchId(11, 57020);

            var result = WebApiService.LogUserEvent(new DynamicAnimation.Models.ExperienceLogRequest() {
                CchId = 57020,
                ContentId = "unittest",
                EmployerId = 11,
                EventId = 0,
                EventName = "UnitTest",
                ExperienceUserId = "unittest",
                LogComment = "CanLogInitialEvent UnitTest Run"
            }, response.AuthHash);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CanGetCardDetailWithValidJwt() {
            var service = new CardService();
            var results = service.GetMemberCardUrls("en", 11, 57020);

            var response = WebApiService.GetCardDetail(11, results.Results[0].SecurityToken);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task CannotGetCardDetailWithExpiredJwt() {
            //This check can be removed after successfull deployment.
            if (!Debugger.IsAttached)
                return;

            var service = new CardService();
            var cardToken = JwtService.EncryptPayload(JsonConvert.SerializeObject(new CardToken() {
                Expires = DateTime.UtcNow.AddMinutes(-2),
                EmployerId = 11
            }));

            Debug.WriteLine(cardToken);
            var response = await WebApiService.GetCardDetail(11, cardToken);
            Assert.IsTrue(response.Expired);
        }

        [TestMethod]
        public async Task CannotGetCardDetailWithMismatchedEmployerId() {
            //This check can be removed after successfull deployment.
            if (!Debugger.IsAttached)
                return;

            var service = new CardService();
            var cardToken = JwtService.EncryptPayload(JsonConvert.SerializeObject(new CardToken() {
                Expires = DateTime.UtcNow.AddMinutes(15),
                EmployerId = 11
            }));

            var response = await WebApiService.GetCardDetail(12, cardToken);
            Assert.IsTrue(response.Invalid);
        }
    }
}
