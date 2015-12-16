using System.Web.Mvc;
using PreRendered.Common;
using PreRendered.Models;

namespace PreRendered.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //string videoCampaignMemberId = "DefaultVideoCampaignMemberId".GetConfigurationValue();
            //int employerId = "DefaultEmployerId".GetConfigurationNumericValue();
            UserSessionVideoModel userSessionVideo = UserSessionVideoModel.Current;

            if (null != Request.QueryString["vid"])
            {
                string qparam = Request.QueryString["vid"];
                string[] qparams = qparam.Split('|');

                if (qparams.Length == 2)
                {
                    string videoCampaignMemberId = qparams[0];
                    int employerId = int.Parse(qparams[1]);

                    userSessionVideo = WebApiService.GetUserSessionVideoData(
                        videoCampaignMemberId, employerId);

                    UserSessionVideoModel.Current = userSessionVideo;
                }
                else
                {
                    HelperService.LogAnEvent(LogEvents.InvalidQueryParameters, qparam);
                }
            }
            else
            {
                if (userSessionVideo.VideoCampaignMemberId.Equals("DefaultVideoCampaignMemberId".GetConfigurationValue()))
                {
                    HelperService.LogAnEvent(LogEvents.NoQueryParameters);
                }
            }
            ViewBag.Poster = userSessionVideo.PublicPosterUrl;
            ViewBag.Vid = userSessionVideo.PublicIntroVideoUrl;

            if (!string.IsNullOrEmpty(userSessionVideo.VideoCampaignFileId))
            {
                HelperService.LogAnEvent(LogEvents.StartIntro);
            }

            return View();
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
            HelperService.LogAnEvent(LogEvents.EndIntro);

            var userSessionVideo = UserSessionVideoModel.Current;

            ViewBag.Poster = userSessionVideo.PublicPosterUrl;
            ViewBag.Vid = userSessionVideo.PublicIntroVideoUrl;

            var signIn = new SignInModel
            {
                Id = userSessionVideo.VideoCampaignMemberId
            };

            return View(signIn);
        }

        [HttpPost]
        public ActionResult SignIn(string id, FormCollection collection)
        {
            bool authenticated = false;
            var userSessionVideo = UserSessionVideoModel.Current;

            if (!string.IsNullOrEmpty(userSessionVideo.VideoCampaignMemberId))
            {
                var signIn = new SignInModel { Id = id };

                if (TryUpdateModel(signIn))
                {
                    string birthDate = signIn.BirthDate;
                    string ssn = signIn.Ssn;

                    if (!string.IsNullOrEmpty(userSessionVideo.DateOfBirth) &&
                        !string.IsNullOrEmpty(userSessionVideo.MemberSsn))
                    {
                        if ((userSessionVideo.DateOfBirth.Equals(birthDate)) &&
                            (userSessionVideo.MemberSsn.Equals(ssn)))
                        {
                            authenticated = true;
                        }
                    }
                }
            }
            if (authenticated)
            {
                HelperService.LogAnEvent(LogEvents.AuthenticateSuccess);

                return RedirectToAction("Private");
            }
            HelperService.LogAnEvent(LogEvents.AuthenticateFail);

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

            var userSessionVideo = UserSessionVideoModel.Current;

            ViewBag.Poster = userSessionVideo.PublicPosterUrl;
            int replayed = 0;

            string personalVid = userSessionVideo.VideoWithSignedAccessSignature;
            ViewBag.Vid = personalVid;

            if (null != Request.QueryString["replay"])
            {
                HelperService.LogAnEvent(Request.QueryString["replay"].Equals("1") ?
                    LogEvents.Replay : LogEvents.StartVideo);
                replayed = Request.QueryString["replay"].Equals("1") ? 1 : 0;
            }
            else
            {
                HelperService.LogAnEvent(LogEvents.StartVideo);
            }
            ViewBag.Replayed = replayed;

            return View();
        }

        public ActionResult Survey()
        {
            ViewBag.Message = "Survey";
            var userSessionVideo = UserSessionVideoModel.Current;

            if (null != Request.QueryString["initial"])
            {
                if (Request.QueryString["initial"].Equals("1"))
                {
                    HelperService.LogAnEvent(LogEvents.EndVideo);
                }
            }
            if (null != Request.QueryString["replay"])
            {
                string replay = Request.QueryString["replay"];
                if (replay.Equals("1"))
                {
                    ViewBag.Replay = 1;
                    HelperService.LogAnEvent(LogEvents.Replay);
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
                    HelperService.LogAnEvent(LogEvents.GoToCCH);

                    return Redirect(userSessionVideo.CchEmployerLink);
                }
            }
            if (null != Request.QueryString["goBenefits"])
            {
                if (Request.QueryString["goBenefits"].Equals("1"))
                {
                    HelperService.LogAnEvent(LogEvents.GoToPlan);

                    return Redirect(userSessionVideo.EmployerBenefitsLink);
                }
            }
            ViewBag.Poster = userSessionVideo.PublicPosterUrl;

            string personalVid = userSessionVideo.VideoWithSignedAccessSignature;
            ViewBag.Vid = personalVid;
            ViewBag.ShowModal = 0;

            return View(userSessionVideo);
        }

        [HttpPost]
        public ActionResult Survey(string button, FormCollection formCollection)
        {
            HelperService.LogAnEvent(button.Equals("YES") ? LogEvents.VideoHelpfulYes : LogEvents.VideoHelpfulNo);

            var userSessionVideo = UserSessionVideoModel.Current;

            ViewBag.Poster = userSessionVideo.PublicPosterUrl;

            string personalVid = userSessionVideo.VideoWithSignedAccessSignature;
            ViewBag.Vid = personalVid;
            ViewBag.ShowModal = 1;

            return View(userSessionVideo);
        }
    }
}
