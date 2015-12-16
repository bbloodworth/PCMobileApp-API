using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using System.Text;

namespace CchWebAPI.Controllers
{
    using Models;
    using Support;

    public class HandshakeController : ApiController
    {
        public HttpResponseMessage GetHash(String hsID)
        {
            HandshakeMobile h = new HandshakeMobile();
            Boolean providerActive = false;
            CCHEncrypt e = new CCHEncrypt();
            
            using (ValidateMobileProvider vmp = new ValidateMobileProvider(hsID))
                vmp.ForEachProvider(delegate(Boolean valid) { if (valid) providerActive = true; });

            if (providerActive)
            {
                e.UserKey = Request.EncryptionKey();
                e.SecretKey = Properties.Settings.Default.SecretKey;
                e.Add("UserID", Request.UserID());

                using (GetKeyUserInfo gkui = new GetKeyUserInfo(Request.UserName()))
                {
                    e.Add("EmployerID", gkui.EmployerID);
                    h.EmployerName = gkui.EmployerName;
                    using (GetKeyEmployeeInfo gkei = new GetKeyEmployeeInfo())
                    {
                        //UserAccess Check dstrickland 7/8/2015
                        using (var cpaa = new CheckPersonApplicationAccess(gkei.CCHID, gkui.CnxString))
                        {
                            if (!cpaa.HasAccess)
                                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                                    new Exception(cpaa.ErrorMessage));
                        }

                        gkei.Email = Request.UserName();
                        gkei.GetData(gkui.CnxString);
                        e.Add("CCHID", gkei.CCHID.ToString());
                        gkei.ForEach<HandshakeMobile.EmployeeInfoData>(
                            delegate(HandshakeMobile.EmployeeInfoData eid)
                            {
                                h.EmployeeInfo = eid;
                            }
                        );
                    }
                }

                using(GetEmployerConnString gecs = new GetEmployerConnString(Convert.ToInt32(e["EmployerID"])))
                {
                    using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
                    {
                        iulh.UserName = Request.UserName();
                        iulh.Domain = Request.RequestUri.Host;
                        iulh.CchApplicationId = 2;  // 1 is for Transparency App; 2 is for HR App
                        iulh.PostData(gecs.ConnString);
                    }
                }

                h.AuthHash = e.ToString();
                return this.Request.CreateResponse<HandshakeMobile>(HttpStatusCode.OK, h);
            }
            else
            {
                return this.Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }
    }
}
