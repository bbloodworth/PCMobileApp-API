using System;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Areas.PComm.Models;
using CchWebAPI.Support;

namespace CchWebAPI.Areas.PComm.Controllers
{
    public class VideoAuthController : ApiController
    {
        /// <summary>
        /// This service does 2 things at once:
        /// a)  Authenticates the User, and
        /// b)  Returns the User's Member Content Data
        /// </summary>
        /// <param name="hsId">Handshake Id for client</param>
        /// <param name="hsRequest">Request object containing authentication data</param>
        /// <returns>Resultset in JSON with Member Content Data</returns>
        [HttpPost]
        public HttpResponseMessage GetAuthMemberData(String hsId, [FromBody] AuthMemberDataRequest hsRequest)
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.Unauthorized);
            var e = new CCHEncrypt();

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent, new Exception("User Not Found"));
                
                using (var gefae = new GetEnrollmentsForAllEmployers())
                {
                    gefae.LastName = hsRequest.LastName;
                    gefae.LastFour = hsRequest.LastFourSsn;
                    gefae.DateOfBirth = hsRequest.DateOfBirth;
                    gefae.GetFrontEndData();

                    if (gefae.Tables.Count > 0 &&
                        gefae.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = gefae.Tables[0].Rows[0];
                        int cchid = dr.GetData<int>("CCHID");
                        string cnxString = dr.GetData("ConnectionString");
                        int employerId = dr.GetData<int>("employerid");

                        //UserAccess Check dstrickland 7/7/2015
                        using (var cpaa = new CheckPersonApplicationAccess(cchid, cnxString))
                        {
                            if (!cpaa.HasAccess)
                                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                                    new Exception(cpaa.ErrorMessage));
                        }

                        e.UserKey = Request.EncryptionKey();
                        e.SecretKey = Properties.Settings.Default.SecretKey;
                        e.Add("CCHID", cchid.ToString(CultureInfo.InvariantCulture));
                        e.Add("EmployerID", employerId.ToString(CultureInfo.InvariantCulture));

                        string authHash = e.ToString();

                        if (employerId > 0)
                        {
                            CreateLoginAudit(hsId, 
                                Request.RequestUri.Host.ToString(CultureInfo.InvariantCulture),
                                cchid, cnxString);

                            hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent, new Exception("Video Data Not Found"));

                            using (var gvcmi = new GetVideoCampaignMemberIdByCchId())
                            {
                                gvcmi.CampaignId = hsRequest.CampaignId;
                                gvcmi.CchId = cchid;
                                gvcmi.GetData(cnxString);

                                if (!gvcmi.HasThrownError && !string.IsNullOrEmpty(gvcmi.VideoCampaignMemberId))
                                {
                                    using (var gvcmd = new GetVideoCampaignMemberDataById())
                                    {
                                        gvcmd.VideoCampaignMemberId = gvcmi.VideoCampaignMemberId;
                                        gvcmd.GetData(cnxString);

                                        if (!gvcmd.HasThrownError)
                                        {
                                            string videoMemberData = gvcmd.VideoMemberData;

                                            string resultset =
                                                string.Format("\"AuthHash\":\"{0}\",\"MemberData\":{1}",
                                                    authHash, videoMemberData);
                                            resultset = string.Concat("{", resultset, "}");

                                            hrm = new HttpResponseMessage(HttpStatusCode.OK)
                                            {
                                                RequestMessage = Request,
                                                Content = new StringContent(resultset), 
                                                StatusCode = HttpStatusCode.OK
                                            };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return hrm;
        }

        /// <summary>
        ///  This method will not work at this time because we do not have an AspNet User Id to log this event against
        /// </summary>
        /// <param name="handshakeId"></param>
        /// <param name="absUri"></param>
        /// <param name="cchid"></param>
        /// <param name="cx"></param>
        private void CreateLoginAudit(string handshakeId, string absUri, int cchid, string cx)
        {
            using (var iulh = new InsertUserLoginHistory())
            {
                iulh.UserName = handshakeId;
                iulh.Domain = string.Format("{0}:{1}", absUri, cchid);
                iulh.CchApplicationId = 2;  // 1 is for Transparency App; 2 is for HR App
                iulh.PostData(cx);
            }
        }
    }
}

