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

    public class PartnerHandshakeController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PostHash(String hsID, [FromBody]HandshakeRequest hsRequest)
        {
            Handshake h = new Handshake();
            Boolean providerActive = false, providerIsPartner = false;
            int employerID = 0;
            string cnxString = "";
            CCHEncrypt e = new CCHEncrypt();

            using (ValidateMobilePartner vmp = new ValidateMobilePartner(hsID, hsRequest.OrganizationID))
            {
                vmp.ForEachProvider(delegate(Boolean valid, Boolean isPartner, int empId, string cnx, string un)
                {
                    providerActive = valid;
                    providerIsPartner = isPartner;
                    employerID = empId;
                    cnxString = cnx;
                    Request.UserName(un);
                    MembershipUser mu = Membership.GetUser(un, true);
                    Request.UserID(mu.ProviderUserKey.ToString());
                });
            }

            if (providerActive && providerIsPartner)
            {
                e.UserKey = Request.EncryptionKey();
                e.SecretKey = Properties.Settings.Default.SecretKey;
                e.Add("UserID", Request.UserID());

                e.Add("EmployerID", employerID.ToString());
                using (GetPartnerEmployeeInfoByName gpeibn = new GetPartnerEmployeeInfoByName())
                {
                    gpeibn.FirstName = hsRequest.FirstName;
                    gpeibn.LastName = hsRequest.LastName;
                    gpeibn.DOB = hsRequest.DOB;
                    gpeibn.SubscriberMedicalID = hsRequest.MedicalID;
                    //gpeibn.RelationshipCode = hsRequest.RelationshipCode;

                    gpeibn.GetData(cnxString);

                    if (gpeibn.Tables.Count == 0 ||
                        gpeibn.Tables[0].Rows.Count == 0 ||
                        gpeibn.Tables[0].Rows[0][0].ToString() == string.Empty)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NoContent, new Exception("User Not Found"));
                    }

                    //UserAccess Check dstrickland 7/8/2015
                    using (var cpaa = new CheckPersonApplicationAccess(gpeibn.CCHID, cnxString))
                    {
                        if (!cpaa.HasAccess)
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                                new Exception(cpaa.ErrorMessage));
                    }

                    e.Add("CCHID", gpeibn.CCHID.ToString());
                    gpeibn.ForEach<Handshake.EmployeeInfoData>(
                        delegate(Handshake.EmployeeInfoData eid)
                        {
                            h.EmployeeInfo = eid;
                        }
                    );

                    //CreateLoginAudit(Request.UserName(), Request.RequestUri.Host.ToString(), gpeibn.CCHID, cnxString);
                    using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
                    {
                        iulh.UserName = Request.UserName();
                        iulh.CCHID = gpeibn.CCHID;
                        iulh.Domain = Request.RequestUri.Host;
                        iulh.CchApplicationId = 2;  // 1 is for Transparency App; 2 is for HR App
                        iulh.PostData(cnxString);
                    }
                }

                h.AuthHash = e.ToString();
                return this.Request.CreateResponse<Handshake>(HttpStatusCode.OK, h);
            }
            else
            {
                return this.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        public HttpResponseMessage GetHash(String hsID, String orgID, String fieldValue)
        {
            Handshake h = new Handshake();
            Boolean providerActive = false, providerIsPartner = false;
            int employerID = 0;
            string cnxString = "";
            CCHEncrypt e = new CCHEncrypt();

            using (ValidateMobilePartner vmp = new ValidateMobilePartner(hsID, orgID))
                vmp.ForEachProvider(delegate(Boolean valid, Boolean isPartner,int empId, string cnx, string un)
                { 
                    providerActive = valid; 
                    providerIsPartner = isPartner; 
                    employerID = empId; 
                    cnxString = cnx;
                    Request.UserName(un);
                    MembershipUser mu = Membership.GetUser(un, true);
                    Request.UserID(mu.ProviderUserKey.ToString());
                });
            
            if (providerActive && providerIsPartner)
            {
                e.UserKey = Request.EncryptionKey(); 
                e.SecretKey = Properties.Settings.Default.SecretKey;
                e.Add("UserID", Request.UserID());

                e.Add("EmployerID", employerID.ToString());
                using (GetPartnerEmployeeInfo gpei = new GetPartnerEmployeeInfo())
                {
                    gpei.HandShakeKey = hsID;
                    gpei.OrganizationID = orgID;
                    gpei.PartnerIDValue = fieldValue;
                    gpei.GetData(cnxString);

                    if (gpei.Tables.Count == 0 || gpei.Tables[0].Rows.Count == 0 ||
                        gpei.Tables[0].Rows[0][0].ToString() == string.Empty)
                        return this.Request.CreateErrorResponse(HttpStatusCode.NoContent,
                            new Exception("User Not Found"));

                    //UserAccess Check dstrickland 7/8/2015
                    using (var cpaa = new CheckPersonApplicationAccess(gpei.CCHID, cnxString))
                    {
                        if (!cpaa.HasAccess)
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                                new Exception(cpaa.ErrorMessage));
                    }

                    e.Add("CCHID", gpei.CCHID.ToString());
                    gpei.ForEach<Handshake.EmployeeInfoData>(
                        delegate(Handshake.EmployeeInfoData eid)
                        {
                            h.EmployeeInfo = eid;
                        }
                        );

                    // CreateLoginAudit(Request.UserName(), Request.RequestUri.Host.ToString(), gpei.CCHID, cnxString);
                    using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
                    {
                        iulh.UserName = Request.UserName();
                        iulh.Domain = Request.RequestUri.Host;
                        iulh.CchApplicationId = 2;  // 1 is for Transparency App; 2 is for HR App
                        iulh.PostData(cnxString);
                    }
                }
                h.AuthHash = e.ToString();
                return this.Request.CreateResponse<Handshake>(HttpStatusCode.OK, h);
            }
            else
            {
                return this.Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
        }

        private void CreateLoginAudit(string UN, string D, int cchid, string cx)
        {
            using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
            {
                iulh.UserName = UN;
                iulh.Domain = D;
                iulh.CCHID = cchid;
                iulh.CchApplicationId = 2;  // 1 is for Transparency App; 2 is for HR App
                iulh.PostData(cx);
            }
        }
    }
}
