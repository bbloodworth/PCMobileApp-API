using System;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebPages;
using DynamicAnimation.Common;
using DynamicAnimation.Models;

namespace DynamicAnimation.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            CampaignSessionModel campaignSession = CampaignSessionModel.Current;
            
            if (string.IsNullOrEmpty(campaignSession.ExperienceUserId))
            {
                ExperienceLogResponse logResponse = HelperService.LogInitialEvent(campaignSession.EmployerId);
                if (logResponse != null)
                {
                    campaignSession.ExperienceUserId = logResponse.ExperienceUserId;
                    CampaignSessionModel.Current = campaignSession;
                }
            }

            if (null != Request.QueryString["cid"])
            {
                string qparam = Request.QueryString["cid"];
                string[] qparams = qparam.Split('|');

                if (qparams.Length >= 3 && qparams.Length <= 4)
                {
                    int employerId = int.Parse(qparams[0]);
                    int campaignId = int.Parse(qparams[1]);
                    int contentId = int.Parse(qparams[2]);

                    if (campaignSession.EmployerId != employerId)
                    {
                        ExperienceLogResponse logResponse = HelperService.LogInitialEvent(employerId);
                        campaignSession.ExperienceUserId = logResponse.ExperienceUserId;
                        CampaignSessionModel.Current = campaignSession;
                    }

                    campaignSession.EmployerId = employerId;
                    campaignSession.CampaignId = campaignId;
                    campaignSession.ContentId = contentId;

                    var campaignIntro = WebApiService.GetCampaignIntro(
                        campaignSession.EmployerId,
                        campaignSession.CampaignId);

                    campaignSession.IntroAnimationType = campaignIntro.ContentType;
                    campaignSession.IntroAnimationName = campaignIntro.ContentName;
                    campaignSession.IntroContentId = campaignIntro.ContentId;

                    CampaignSessionModel.Current = campaignSession;

                    if (qparams.Length == 4)
                    {
                        int cchId = int.Parse(qparams[3]);
                        campaignSession.CchId = cchId;

                        AuthorizationResponse authResponse = WebApiService.GetAuthorizationByCchId(
                            campaignSession.EmployerId,
                            campaignSession.CchId);

                        if (!string.IsNullOrEmpty(authResponse.AuthHash))
                        {
                            campaignSession.AuthorizationHash = authResponse.AuthHash;

                            CampaignSessionModel.Current = campaignSession;

                            campaignSession = WebApiService.GetCampaignSession(CampaignSessionModel.Current);

                            if (!string.IsNullOrEmpty(campaignSession.JavaScriptFileName))
                            {
                                campaignSession.IntroAnimationName = "NONE";
                            }
                            CampaignSessionModel.Current = campaignSession;

                            HelperService.LogUserEvent(ExperienceEvents.AuthenticationSuccess, campaignSession.CchId.ToString());
                        }
                        else
                        {
                            HelperService.LogAnonEvent(ExperienceEvents.AuthenticationFail, campaignSession.CchId.ToString());
                        }
                    }
                }
                else
                {
                    HelperService.LogAnonEvent(ExperienceEvents.InvalidQueryParameters, qparam);
                }
            }
            else
            {
                HelperService.LogAnonEvent(ExperienceEvents.NoQueryParameters);
            }
            ViewBag.Vid = campaignSession.PublicIntroVideoUrl;

            if (campaignSession.IntroAnimationName.Equals("NONE"))
            {
                return RedirectToAction("Dynamic");
            }

            if (!string.IsNullOrEmpty(campaignSession.IntroAnimationName))
            {
                HelperService.LogAnonEvent(ExperienceEvents.StartIntro,
                    campaignSession.IntroAnimationName);
            }
            return View();
        }

        public ActionResult Dynamic()
        {
            ViewBag.Message = "Dynamic Video";
            var campaignSession = CampaignSessionModel.Current;

            ViewBag.AnimationTopic = campaignSession.AnimationTopic;
            ViewBag.AnimationScript = campaignSession.JavaScriptFileName;
            ViewBag.BannerImage = campaignSession.BannerImageName;
            ViewBag.PosterImageURL = campaignSession.BannerImageName;

            ViewBag.ApiKey = "AnzovinApiKey".GetConfigurationValue();
            ViewBag.CampaignId = campaignSession.CampaignId;
            ViewBag.EmployerId = campaignSession.EmployerId;
            ViewBag.LastName = campaignSession.LastName;
            ViewBag.DateOfBirth = campaignSession.DateOfBirth;
            ViewBag.Ssn = campaignSession.LastFour;
            ViewBag.CampaignContentId = campaignSession.CampaignContentId;
            ViewBag.AuthHash = campaignSession.AuthorizationHash;
            ViewBag.ExperienceUserId = campaignSession.ExperienceUserId;

            ViewBag.MemberContentData = Uri.EscapeDataString(campaignSession.MemberContentData);

            ViewBag.EventLogURL = campaignSession.UserEventLogUrl;
            ViewBag.ShowModal = 0;

            switch (campaignSession.AnimationTopic)
            {
                case "benefit_plan":
                    return View("benefit_plan");

                case "savings_alert":
                    return View("savings_alert");

                case "savings_alert_avaya":
                    return View("savings_alert_avaya");

                default:
                    // return View("Dynamic", "~/Views/Shared/_CanvasLayout.cshtml");
                    return View("Index");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "About ClearCost Health";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact ClearCost Health";

            return View();
        }

        public ActionResult SignIn()
        {
            ViewBag.Message = "Sign In";
            var campaignSession = CampaignSessionModel.Current;

            HelperService.LogAnonEvent(ExperienceEvents.EndIntro, campaignSession.IntroAnimationName);

            var signIn = new SignInModel
            {
                Id = campaignSession.ExperienceUserId
            };

            return View(signIn);
        }

        [HttpPost]
        public ActionResult SignIn(string id, FormCollection collection)
        {
            bool authenticated = false;
            var campaignSession = CampaignSessionModel.Current;

            if (!string.IsNullOrEmpty(campaignSession.ExperienceUserId))
            {
                var signIn = new SignInModel { Id = id };

                if (TryUpdateModel(signIn))
                {
                    campaignSession.LastName = signIn.LastName;
                    campaignSession.DateOfBirth = signIn.BirthDate;
                    campaignSession.LastFour = signIn.Ssn;

                    AuthorizationResponse authResponse =
                        WebApiService.GetAuthorization(lastName: campaignSession.LastName,
                            dateOfBirth: campaignSession.DateOfBirth, lastFour: campaignSession.LastFour);

                    if (!string.IsNullOrEmpty(authResponse.AuthHash))
                    {
                        campaignSession.AuthorizationHash = authResponse.AuthHash;
                        authenticated = true;
                    }
                    CampaignSessionModel.Current = campaignSession;
                }
            }
            if (authenticated)
            {
                HelperService.LogUserEvent(ExperienceEvents.AuthenticationSuccess, campaignSession.LastName);

                campaignSession = WebApiService.GetCampaignSession(CampaignSessionModel.Current);

                CampaignSessionModel.Current = campaignSession;

                return RedirectToAction("Dynamic");
            }
            HelperService.LogAnonEvent(ExperienceEvents.AuthenticationFail, campaignSession.LastName);

            return RedirectToAction("Oops");
        }

        public ActionResult Oops()
        {
            ViewBag.Message = "Oops!";

            return View();
        }

        public ActionResult Private()
        {
            ViewBag.Message = "Your Personalized Video";
            var campaignSession = CampaignSessionModel.Current;

            ViewBag.Poster = campaignSession.PublicPosterUrl;
            int replayed = 0;

            if (null != Request.QueryString["replay"])
            {
                HelperService.LogUserEvent(Request.QueryString["replay"].Equals("1") ?
                    ExperienceEvents.ReplayAnimation : ExperienceEvents.StartAnimation);
                replayed = Request.QueryString["replay"].Equals("1") ? 1 : 0;
            }
            else
            {
                HelperService.LogUserEvent(ExperienceEvents.StartAnimation);
            }
            ViewBag.Replayed = replayed;

            return View();
        }

        public ActionResult Helpful()
        {
            ViewBag.Message = "Helpful";
            if (null != Request.QueryString["r"])
            {
                string helpful = Request.QueryString["r"];
                HelperService.LogUserEvent(helpful.Equals("1")
                    ? ExperienceEvents.HelpfulYes
                    : ExperienceEvents.HelpfulNo);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Helpful(string button, FormCollection formCollection)
        {
            ViewBag.Message = "Helpful";

            if (!string.IsNullOrEmpty(button))
            {
                HelperService.LogUserEvent(button.Equals("YES") ? ExperienceEvents.HelpfulYes : ExperienceEvents.HelpfulNo);

                return View();
            }

            return RedirectToAction("Dynamic");
        }

        public ActionResult Survey()
        {
            ViewBag.Message = "Survey";
            var userSessionVideo = UserSessionVideoModel.Current;
            var campaignSession = CampaignSessionModel.Current;

            if (null != Request.QueryString["initial"])
            {
                if (Request.QueryString["initial"].Equals("1"))
                {
                    HelperService.LogUserEvent(ExperienceEvents.EndAnimation);
                }
            }
            if (null != Request.QueryString["replay"])
            {
                string replay = Request.QueryString["replay"];
                if (replay.Equals("1"))
                {
                    ViewBag.Replay = 1;
                    HelperService.LogUserEvent(ExperienceEvents.ReplayAnimation);
                }
                else
                {
                    ViewBag.Replay = 0;
                }
            }
            if (null != Request.QueryString["goCch"])
            {
                if (Request.QueryString["goCch"].Equals("1"))
                {
                    HelperService.LogUserEvent(ExperienceEvents.GoToCch);

                    return Redirect(campaignSession.CchLink);
                }
            }
            if (null != Request.QueryString["goBenefits"])
            {
                if (Request.QueryString["goBenefits"].Equals("1"))
                {
                    HelperService.LogUserEvent(ExperienceEvents.GoToPlan);

                    return Redirect(campaignSession.BenefitsLink);
                }
            }
            ViewBag.Poster = campaignSession.PublicPosterUrl;

            //string personalVid = userSessionVideo.VideoWithSignedAccessSignature;
            //ViewBag.Vid = personalVid;
            ViewBag.ShowModal = 0;

            return View(userSessionVideo);
        }

        [HttpPost]
        public ActionResult Survey(string button, FormCollection formCollection)
        {
            HelperService.LogAnonEvent(button.Equals("YES") ? ExperienceEvents.HelpfulYes : ExperienceEvents.HelpfulNo);

            var userSessionVideo = UserSessionVideoModel.Current;

            ViewBag.Poster = userSessionVideo.PublicPosterUrl;

            string personalVid = userSessionVideo.VideoWithSignedAccessSignature;
            ViewBag.Vid = personalVid;
            ViewBag.ShowModal = 1;

            return View(userSessionVideo);
        }
    }
}
