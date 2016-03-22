using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Security;
using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Properties;
using CchWebAPI.Support;

using ClearCost.IO.Log;
using NLog;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class MembershipController : ApiController
    {
        //^(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])((19|20)\d\d)$
        private static readonly Regex RegDateOfBirth = new Regex(@"^(19|20)\d\d[- /.](0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])$", RegexOptions.None);
        private static readonly Regex RegPhone = new Regex(@"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$", RegexOptions.None);

        private static readonly string EMPLOYER_QUERY = "select e.connectionString, up.employerId from userprofile up join employers e on up.employerid = e.employerid where email = @Email";
        /// <summary>
        /// This service Authenticates,  given a user's last name, date of birth, and last 4 of SSN
        /// If authenticated successfully, it returns the User's Authorization Hash Token, which will be used for the 2nd part of the registration
        /// </summary>
        /// <param name="hsId">Handshake Id for client</param>
        /// <param name="hsRequest">Request object containing authentication data</param>
        /// <returns>Authorization Hash Token</returns>
        [HttpPost]
        public HttpResponseMessage Register1(String hsId, [FromBody] UserAuthenticationRequest hsRequest)
        {
            dynamic eo = new ExpandoObject();
            HttpResponseMessage hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new Exception("Client Handshake is Not Authorized"));
            var e = new CCHEncrypt();

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new Exception("User was Not Found"));

                if (RegDateOfBirth.IsMatch(hsRequest.DateOfBirth))
                {
                    using (var gefae = new GetEnrollmentsForAllEmployers())
                    {
                        hsRequest.LastFourSsn = hsRequest.LastFourSsn.Trim().Length > 4
                            ? hsRequest.LastFourSsn.Substring(hsRequest.LastFourSsn.Length - 4, 4)
                            : hsRequest.LastFourSsn;

                        gefae.LastName = hsRequest.LastName;
                        gefae.LastFour = hsRequest.LastFourSsn;
                        //DateTime birthDate = DateTime.Parse(hsRequest.DateOfBirth);
                        //gefae.DateOfBirth = string.Format("{0}-{1}-{2}", birthDate.Year, birthDate.Month, birthDate.Day);
                        gefae.DateOfBirth = hsRequest.DateOfBirth;
                        gefae.GetFrontEndData();

                        if (gefae.Tables.Count > 0 &&
                            gefae.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = gefae.Tables[0].Rows[0];
                            int cchid = dr.GetData<int>("CCHID");
                            int employerId = dr.GetData<int>("employerid");
                            string connString = dr.GetData("connectionstring");

                            //UserAccess Check dstrickland 7/8/2015
                            using (var cpaa = new CheckPersonApplicationAccess(cchid, connString))
                            {
                                if (!cpaa.HasAccess)
                                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                                        new Exception(cpaa.ErrorMessage));
                            }

                            e.UserKey = Request.EncryptionKey();
                            e.SecretKey = Settings.Default.SecretKey;
                            e.Add("CCHID", cchid.ToString(CultureInfo.InvariantCulture));
                            e.Add("EmployerID", employerId.ToString(CultureInfo.InvariantCulture));
                            e.Add("UserID", hsId);

                            ((IDictionary<string, object>) eo)["AuthHash"] = e.ToString();
                            hrm = Request.CreateResponse(HttpStatusCode.OK, (eo as ExpandoObject));

                            //LogUserLoginHistory(null, cchid, connString);
                        }
                    }
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage Register2(String hsId, [FromBody] UserAuthenticationRequest hsRequest)
        {
            dynamic eo = new ExpandoObject();
            HttpResponseMessage hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                new Exception("Client Handshake is Not Authorized"));
            var e = new CCHEncrypt();

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new Exception("User was Not Found"));

                using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
                {
                    const string employeeQuery =
                        "SELECT CONVERT(varchar(10), DateOfBirth, 111) AS DateOfBirthText, * FROM Enrollments WHERE CCHID = @CchId";
                    using (var employeeDb = new DataBase(employeeQuery, true))
                    {
                        employeeDb.AddParameter("CchId", Request.CCHID());
                        employeeDb.GetData(gecs.ConnString);

                        if (employeeDb.Tables.Count > 0 && employeeDb.Tables[0].Rows.Count > 0)
                        {
                            hsRequest.FirstName = employeeDb.Tables[0].Rows[0].GetData("FirstName");
                            hsRequest.LastName = employeeDb.Tables[0].Rows[0].GetData("LastName");
                            hsRequest.LastName = hsRequest.LastName.Replace("_", " ");
                            hsRequest.DateOfBirth = employeeDb.Tables[0].Rows[0].GetData("DateOfBirthText");
                            hsRequest.LastFourSsn = employeeDb.Tables[0].Rows[0].GetData("MemberSsn");
                            hsRequest.MedicalId = employeeDb.Tables[0].Rows[0].GetData("MemberMedicalId");
                            
                            string fullName = string.Format("{0} {1}", hsRequest.FirstName, hsRequest.LastName);
                            string mobilePhone = hsRequest.MobilePhone;
                            string alternatePhone = hsRequest.Phone;

                            eo.UserName = hsRequest.UserName;
                            eo.DisplayName = fullName;
                            eo.MobilePhone = mobilePhone;
                            eo.AlternatePhone = alternatePhone;

                            using (GetUserContentPreference gucp = new GetUserContentPreference())
                            {
                                gucp.CCHID = Request.CCHID();
                                gucp.GetData(gecs.ConnString);

                                eo.SmsInd = gucp.SmsInd;
                                eo.EmailInd = gucp.EmailInd;
                                eo.OsBasedAlertInd = gucp.OsBasedAlertInd;
                                eo.LocaleCode = gucp.LocaleCode;
                                eo.PreferredContact = gucp.ContactPhoneNumber;
                            }
                            
                            MembershipCreateStatus status;
                            if (CreateNewMemberAccount(email: hsRequest.UserName, firstName: hsRequest.FirstName,
                                lastName: hsRequest.LastName, phone: hsRequest.Phone,
                                secretQuestionId: hsRequest.SecretQuestionId, secretAnswer: hsRequest.SecretAnswer,
                                password: hsRequest.Password, mobilePhone: hsRequest.MobilePhone, 
                                cchid: Request.CCHID(), employerId: Request.EmployerID(),
                                cnxString: gecs.ConnString, status: out status))
                            {
                                var membershipUser = Membership.GetUser(hsRequest.UserName);
                                if (membershipUser != null)
                                {
                                    if (membershipUser.ProviderUserKey != null)
                                    {
                                        eo.Question = membershipUser.PasswordQuestion;

                                        string aspUserId = membershipUser.ProviderUserKey.ToString();
                                        e.UserKey = Request.EncryptionKey();
                                        e.SecretKey = Settings.Default.SecretKey;
                                        e.Add("UserID", aspUserId);
                                        e.Add("CCHID", Request.CCHID().ToString());
                                        e.Add("EmployerID", Request.EmployerID().ToString());
                                        e.Add("UserName", hsRequest.UserName);

                                        //((IDictionary<string, object>) eo)["AuthHash"] = e.ToString();
                                        //hrm = Request.CreateResponse(HttpStatusCode.OK, (eo as ExpandoObject));
                                        eo.AuthHash = e.ToString();
                                        hrm = Request.CreateResponse(HttpStatusCode.OK, (object)eo);

                                        //InsertAuditTrail(Request.CCHID(), aspUserId,
                                        //    "Animation Register", Request.RequestUri.Host, gecs.ConnString);
                                        LogUserLoginHistory(hsRequest.UserName, Request.CCHID(), gecs.ConnString);
                                    }
                                }
                            }
                            else
                            {
                                switch (status)
                                {
                                    case MembershipCreateStatus.DuplicateUserName:
                                        hrm = Request.CreateErrorResponse(HttpStatusCode.Conflict,
                                            new Exception("Member Account already exists"));
                                        break;
                                    default:
                                        hrm = Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                                            new Exception("Error in creating new Member Account"));
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return hrm;
        }

        /// <summary>
        /// This service Authenticates and Logs in a User
        /// </summary>
        /// <param name="hsId">Handshake Id for client</param>
        /// <param name="hsRequest">Request object containing authentication data</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Login(String hsId, [FromBody] UserAuthenticationRequest hsRequest) {
            var hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new Exception("Client Handshake is Not Authorized"));
            var e = new CCHEncrypt();
            dynamic data = new ExpandoObject();

            if (!ValidateConsumer.IsValidConsumer(hsId)) {
                LogUtil.Log(string.Format("Login failed. Inavlid Handshake Id {0}", hsId), LogLevel.Info);
                return hrm;
            }

            hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new Exception("User Name and Password Do Not Match"));

            if (!Membership.ValidateUser(hsRequest.UserName, hsRequest.Password)) {
                LogUtil.Log(string.Format("Login failed for user {0}.  Credentials failed membership validation.",
                    hsRequest.UserName), LogLevel.Info);
                return hrm;
            }

            using (var employerDb = new DataBase(EMPLOYER_QUERY, true)) {

                employerDb.AddParameter("Email", hsRequest.UserName);
                employerDb.GetFrontEndData();

                hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, 
                    new Exception("User Profile was Not Found"));

                if (employerDb.Tables.Count < 1 || employerDb.Tables[0].Rows.Count < 1) {
                    LogUtil.Log(string.Format("Login failed for user {0}.  User Profile was not found.",
                    hsRequest.UserName), LogLevel.Info);
                    return hrm;
                }

                e.Add("EmployerID", employerDb.Tables[0].Rows[0].GetData("employerId"));

                using (var gkei = new GetKeyEmployeeInfo()) {

                    gkei.Email = hsRequest.UserName;
                    string cnxString = employerDb.Tables[0].Rows[0].GetData("connectionString");

                    gkei.GetData(cnxString);

                    hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, 
                        new Exception("Employee Info on User Name was Not Found"));

                    if (gkei.Tables.Count < 1 || gkei.Tables[0].Rows.Count < 1) {
                        LogUtil.Log(string.Format("Login failed for user {0}.  Employee Info was not found.",
                        hsRequest.UserName), LogLevel.Info);
                        return hrm;
                    }

                    //UserAccess Check dstrickland 7/7/2015
                    using (var cpaa = new CheckPersonApplicationAccess(gkei.CCHID, cnxString)) {
                        if (!cpaa.HasAccess) {
                            LogUtil.Log(string.Format("Login failed for user {0}.  User does not have acces to AppId 2.",
                                hsRequest.UserName), LogLevel.Info);
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                                new Exception(cpaa.ErrorMessage));
                        }
                    }

                    var firstName = gkei.Tables[0].Rows[0].GetData("FirstName");
                    var lastName = gkei.Tables[0].Rows[0].GetData("LastName");
                    var fullName = string.Format("{0} {1}", firstName, lastName);
                    var mobilePhone = gkei.Tables[0].Rows[0].GetData("MobilePhone");
                    var alternatePhone = gkei.Tables[0].Rows[0].GetData("Phone");

                    using (var gucp = new GetUserContentPreference()) {
                        gucp.CCHID = gkei.CCHID;
                        gucp.GetData(cnxString);

                        data.SmsInd = gucp.SmsInd;
                        data.EmailInd = gucp.EmailInd;
                        data.OsBasedAlertInd = gucp.OsBasedAlertInd;
                        data.LocaleCode = gucp.LocaleCode;
                        data.PreferredContact = gucp.ContactPhoneNumber;

                        hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                    }

                    var membershipUser = Membership.GetUser(hsRequest.UserName);
                    if (membershipUser != null && membershipUser.ProviderUserKey != null) {
                        e.UserKey = Request.EncryptionKey();
                        e.SecretKey = Settings.Default.SecretKey;
                        e.Add("UserName", hsRequest.UserName);
                        e.Add("CCHID", gkei.CCHID.ToString());

                        string aspUserId = membershipUser.ProviderUserKey.ToString();
                        e.Add("UserID", aspUserId);
                        string authHash = e.ToString();

                        data.AuthHash = authHash;
                        data.UserName = hsRequest.UserName;
                        data.DisplayName = fullName;
                        data.MobilePhone = mobilePhone;
                        data.AlternatePhone = alternatePhone;
                        data.Question = membershipUser.PasswordQuestion;

                        hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);

                        LogUserLoginHistory(hsRequest.UserName, gkei.CCHID, cnxString);
                    }
                }
            }

            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserEmail([FromBody] AccountRequest accountRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(
                HttpStatusCode.NoContent, "Unexpected Error changing the Email Address");

            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (UpdateUserEmail uue = new UpdateUserEmail())
                {
                    uue.Email = accountRequest.NewEmail;
                    uue.UserID = Request.UserID();
                    uue.PostFrontEndData();
                    switch (uue.ReturnStatus)
                    {
                        case 0:
                            uue.UpdateClientSide(accountRequest.NewEmail, Request.CCHID(), gecs.ConnString);

                            hrm = Request.CreateResponse(HttpStatusCode.OK);
                            break;
                    }
                }
            }
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage UserMobilePhone()
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserMobilePhone gump = new GetUserMobilePhone())
                {
                    gump.CchId = Request.CCHID();
                    gump.GetData(gecs.ConnString);
                    data.MobilePhone = gump.MobilePhone;

                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage UserMobilePhone([FromBody] AccountRequest accountRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(
                HttpStatusCode.NoContent, "Unexpected Error changing the Mobile Phone Number");

            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (UpdateUserMobilePhone uump = new UpdateUserMobilePhone())
                {
                    uump.CCHID = Request.CCHID();
                    uump.MobilePhone = accountRequest.NewMobilePhone;
                    uump.PostData(gecs.ConnString);

                    if (uump.PostReturn == 1)
                    {
                        hrm = Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage UserAlternatePhone([FromBody] AccountRequest accountRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(
                HttpStatusCode.NoContent, "Unexpected Error changing the Alternate Phone Number");

            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (UpdateUserPhone uup = new UpdateUserPhone())
                {
                    uup.CCHID = Request.CCHID();
                    uup.Phone = accountRequest.NewAlternatePhone;
                    uup.PostData(gecs.ConnString);

                    hrm = Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage UserPhoneNumbers()
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetEmployeeByCchIdForCallCenter gebcfcc = new GetEmployeeByCchIdForCallCenter())
                {
                    gebcfcc.CchId = Request.CCHID();
                    gebcfcc.GetData(gecs.ConnString);
                    data.MobilePhone = gebcfcc.MobilePhone;
                    data.AlternatePhone = gebcfcc.AlternatePhone;

                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object) data);
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage PasswordReset0(UserAuthenticationRequest request)
        {
            var e = new CCHEncrypt();
            dynamic data = new ExpandoObject();
            using (GetUserProfileByEmail gupbe = new GetUserProfileByEmail())
            {
                gupbe.Email = request.UserName;
                gupbe.GetFrontEndData();

                int employerId = Convert.ToInt32(gupbe.EmployerId);

                using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
                {
                    using (GetKeyEmployeeInfo gkei = new GetKeyEmployeeInfo())
                    {
                        gkei.Email = request.UserName;
                        gkei.GetData(gecs.ConnString);

                        if (gkei.Tables.Count > 0 && gkei.Tables[0].Rows.Count > 0)
                        {
                            int cchId = gkei.Tables[0].Rows[0].GetData<int>("CCHID");

                            using (GetUserContentPreference gucp = new GetUserContentPreference())
                            {
                                gucp.CCHID = cchId;
                                gucp.GetData(gecs.ConnString);

                                data.ContactPhoneNumber = gucp.ContactPhoneNumber;
                            }

                            using (GetEmployeeByCchIdForCallCenter gebcfcc = new GetEmployeeByCchIdForCallCenter())
                            {
                                gebcfcc.CchId = cchId;
                                gebcfcc.GetData(gecs.ConnString);

                                if (request.UserName.ToLower() == gebcfcc.Email.ToLower() &&
                                    request.FullSsn.Trim() == gebcfcc.MemberFullSsn)
                                {
                                    var membershipUser = Membership.GetUser(request.UserName);
                                    if (membershipUser != null)
                                    {
                                        if (membershipUser.ProviderUserKey != null)
                                        {
                                            e.UserKey = Request.EncryptionKey();
                                            e.SecretKey = Settings.Default.SecretKey;
                                            e.Add("UserName", request.UserName);
                                            e.Add("CCHID", gkei.CCHID.ToString());
                                            e.Add("EmployerID", employerId.ToString());

                                            string aspUserId = membershipUser.ProviderUserKey.ToString();
                                            e.Add("UserID", aspUserId);
                                            data.AuthHash = e.ToString();

                                            data.Question = membershipUser.PasswordQuestion;
                                            data.Success = true;
                                        }
                                        else
                                        {
                                            data.Fail = true;
                                            data.ErrorMessage = "Provider User Key does Not Exist";
                                        }
                                    }
                                    else
                                    {
                                        data.Fail = true;
                                        data.ErrorMessage = "Member Account does Not Exist";
                                    }
                                }
                                else
                                {
                                    data.Fail = true;
                                    data.ErrorMessage = "Email or SSN does Not Match";
                                }
                            }
                        }
                        else
                        {
                            data.Fail = true;
                            data.ErrorMessage = "Key Employee Info is Missing";
                        }
                    }
                }
            }
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage PasswordReset1(UserAuthenticationRequest request)
        {
            var e = new CCHEncrypt();
            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserContentPreference gucp = new GetUserContentPreference())
                {
                    gucp.CCHID = Request.CCHID();
                    gucp.GetData(gecs.ConnString);

                    data.ContactPhoneNumber = gucp.ContactPhoneNumber;
                }

                using (GetEmployeeByCchIdForCallCenter gebcfcc = new GetEmployeeByCchIdForCallCenter())
                {
                    gebcfcc.CchId = Request.CCHID();
                    gebcfcc.GetData(gecs.ConnString);

                    if (request.UserName == gebcfcc.Email &&
                        request.LastFourSsn == gebcfcc.MemberSsn)
                    {
                        data.Success = true;

                        e.UserKey = Request.EncryptionKey();
                        e.SecretKey = Settings.Default.SecretKey;
                        e.Add("EmployerID", Request.EmployerID().ToString());
                        e.Add("UserID", Request.UserID());
                        e.Add("UserName", request.UserName);
                        e.Add("CCHID", Request.CCHID().ToString());

                        string authHash = e.ToString();
                        //data.AuthHash = authHash;
                    }
                    else
                    {
                        data.Fail = true;
                        data.ErrorMessage = "Email or SSN does Not Match";
                    }
                }
            }
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage PasswordReset2(UserAuthenticationRequest request)
        {
            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetUserContentPreference gucp = new GetUserContentPreference())
                {
                    gucp.CCHID = Request.CCHID();
                    gucp.GetData(gecs.ConnString);

                    data.ContactPhoneNumber = gucp.ContactPhoneNumber;
                }
            }
            string userName = Request.UserName();
            MembershipUser mu = Membership.GetUser(userName);

            if (mu == null)
            {
                data.Fail = true;
                data.ErrorMessage = "Unexpected Error locating User Membership";
            }
            else
            {
                if (string.IsNullOrEmpty(request.SecretAnswer))
                {
                    data.Fail = true;
                    data.ErrorMessage = "Missing Password Security Secret Answer";
                }
                else
                {
                    try
                    {
                        // Reset the password using the secret answer; the new password is a system-generated one
                        string newPwd = mu.ResetPassword(request.SecretAnswer);

                        // Use the system-generated password to change to the new user-provided password
                        if (mu.ChangePassword(newPwd, request.NewPassword))
                        {
                            mu.Comment = string.Empty;
                            Membership.UpdateUser(mu);

                            data.Success = true;
                        }
                        else
                        {
                            data.Fail = true;
                            data.ErrorMessage = "Wrong Password Security Secret Answer";
                        }
                    }
                    catch (Exception exc)
                    {
                        data.Fail = true;
                        data.ErrorMessage = exc.Message;

                    }
                }
            }
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage GetMemberAuthorization(int employerId, int cchId, String hsId)
        {
            dynamic eo = new ExpandoObject();
            HttpResponseMessage hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                new Exception("Client Handshake is Not Authorized"));
            var e = new CCHEncrypt();

            if (ValidateConsumer.IsValidConsumer(hsId))
            {
                hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, 
                    new Exception("Member with CCH ID was Not Found"));

                using (GetEmployerConnString gecs = new GetEmployerConnString(employerId))
                {
                    //UserAccess Check dstrickland 7/8/2015
                    using (var cpaa = new CheckPersonApplicationAccess(cchId, gecs.ConnString))
                    {
                        if (!cpaa.HasAccess)
                            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                                new Exception(cpaa.ErrorMessage));
                    }

                    using (GetKeyEmployeeInfoByCchId gkeibc = new GetKeyEmployeeInfoByCchId())
                    {
                        gkeibc.CchId = cchId;
                        gkeibc.GetData(gecs.ConnString);

                        if (gkeibc.Tables.Count > 0 && gkeibc.Tables[0].Rows.Count > 0)
                        {
                            e.UserKey = Request.EncryptionKey();
                            e.SecretKey = Settings.Default.SecretKey;
                            e.Add("CCHID", cchId.ToString());
                            e.Add("EmployerID", employerId.ToString());
                            e.Add("UserID", hsId);

                            ((IDictionary<string, object>)eo)["AuthHash"] = e.ToString();
                            hrm = Request.CreateResponse(HttpStatusCode.OK, (eo as ExpandoObject));

                            //InsertAuditTrail(cchId, hsId,
                            //    "Animation CCHID Login", Request.RequestUri.Host, gecs.ConnString);

                            string userName = gkeibc.Tables[0].Rows[0].GetData("Email");
                            LogUserLoginHistory(userName, cchId, gecs.ConnString);
                        }
                    }
                }
            }
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage SecurityQuestions()
        {
            dynamic data = new ExpandoObject();

            List<string> securityQuestions = GetPasswordQuestions().Select(q => q.Value.ToString()).ToList();

            data.Questions = securityQuestions;

            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage SetSecurityAnswer(UserAuthenticationRequest securityRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(
                HttpStatusCode.PreconditionFailed, "Unexpected Error locating user membership");

            string userName = Request.UserName();
            MembershipUser mu = Membership.GetUser(userName);

            if (mu != null)
            {
                string secretQuestion = string.IsNullOrEmpty(securityRequest.SecretQuestion)
                    ? mu.PasswordQuestion
                    : securityRequest.SecretQuestion;

                hrm = Request.CreateErrorResponse(
                    HttpStatusCode.PreconditionFailed, "Missing Password or Secret Answer");

                if (!string.IsNullOrEmpty(securityRequest.Password) &&
                    !string.IsNullOrEmpty(secretQuestion) &&
                    !string.IsNullOrEmpty(securityRequest.SecretAnswer))
                {
                    try
                    {
                        if (mu.ChangePasswordQuestionAndAnswer(
                            securityRequest.Password,
                            secretQuestion,
                            securityRequest.SecretAnswer))
                        {
                            mu.Comment = string.Empty;
                            Membership.UpdateUser(mu);

                            if (!Roles.IsUserInRole(userName, "Customer"))
                            {
                                Roles.AddUserToRole(userName, "Customer");
                            }
                            hrm = Request.CreateResponse(HttpStatusCode.OK);
                        }
                        else
                        {
                            hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Wrong Password");
                        }
                    }
                    catch (Exception exc)
                    {
                        hrm = Request.CreateErrorResponse(HttpStatusCode.NoContent, exc.Message);
                    }
                }
            }
            return hrm;
        }

        [HttpPost]
        public HttpResponseMessage ChangePassword(UserAuthenticationRequest passwordRequest)
        {
            HttpResponseMessage hrm = Request.CreateErrorResponse(
                HttpStatusCode.PreconditionFailed, "Unexpected Error locating user membership");

            string userName = Request.UserName();
            MembershipUser mu = Membership.GetUser(userName);

            if (mu != null)
            {
                if (mu.ChangePassword(passwordRequest.Password, passwordRequest.NewPassword))
                {
                    mu.Comment = string.Empty;
                    Membership.UpdateUser(mu);

                    hrm = Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    hrm = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Wrong Password");
                }
            }
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage GetCurrentSecurityQuestion()
        {
            dynamic data = new ExpandoObject();
            MembershipUser mu = Membership.GetUser(Request.UserName());
            data.Question = mu.PasswordQuestion;

            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            return hrm;
        }

        [HttpGet]
        public HttpResponseMessage GetEmployeeSegment()
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetEmployeeSegment ges = new GetEmployeeSegment())
                {
                    ges.CchId = Request.CCHID();
                    ges.EmployerId = Request.EmployerID();
                    ges.GetData(gecs.ConnString);

                    data.CchId = Request.CCHID();
                    data.Segment = ges.PropertyCode;
                    data.HealthPlan = ges.Insurer;
                    data.BirthYear = ges.BirthYear;

                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                }
            }
            return hrm;
        }

        private void LogUserLoginHistory(string userName, int cchId, string connString)
        {
            using (InsertUserLoginHistory iulh = new InsertUserLoginHistory())
            {
                string aspNetUserName = "AspNetUserName".GetConfigurationValue();
                MembershipUser mu = Membership.GetUser(aspNetUserName, true);
                if (mu != null) if (mu.ProviderUserKey != null) Request.UserID(mu.ProviderUserKey.ToString());

                if (!string.IsNullOrEmpty(userName))
                {
                    mu = Membership.GetUser(userName, true);
                    if (mu != null)
                    {
                        aspNetUserName = userName;
                        if (mu.ProviderUserKey != null) Request.UserID(mu.ProviderUserKey.ToString());
                    }
                }
                Request.UserName(aspNetUserName);

                iulh.UserName = Request.UserName();
                iulh.CCHID = cchId;
                iulh.Domain = Request.RequestUri.Host;
                iulh.CchApplicationId = 2;  // 1 is for Transparency App; 2 is for HR App
                iulh.PostData(connString);
            }
        }

        private bool CreateNewMemberAccount(string email,
            string firstName, string lastName, string phone, string mobilePhone, 
            int secretQuestionId, string secretAnswer, string password,
            int cchid, int employerId, string cnxString, out MembershipCreateStatus status)
        {
            bool isSuccessful = false;
            string securityQuestion = GetPasswordQuestions()
                .Where(q => q.Key == secretQuestionId.ToString())
                .Select(q => q.Value.ToString())
                .First();
            
            MembershipUser newUser = Membership.CreateUser(
                email, password, email, securityQuestion, secretAnswer, true, out status);

            if (newUser != null && status.Equals(MembershipCreateStatus.Success))
            {
                Roles.AddUserToRole(email, "Customer");

                using (InsertUserProfile iup = new InsertUserProfile())
                {
                    var providerUserKey = newUser.ProviderUserKey;
                    if (providerUserKey != null)
                    {
                        iup.UserID = (Guid) providerUserKey;
                        iup.EmployerID = employerId;
                        iup.FirstName = firstName;
                        iup.LastName = lastName;
                        iup.Email = email;
                        iup.MessageCode = "RegConfirmationMsg";
                        iup.PostFrontEndData();

                        if (!iup.HasThrownError)
                        {
                            using (UpdateUserEmail uue = new UpdateUserEmail())
                            {
                                uue.UpdateClientSide(email, cchid, cnxString);
                                if (!uue.HasThrownError)
                                {
                                    using (UpdateUserPhone uup = new UpdateUserPhone())
                                    {
                                        uup.Phone = phone;
                                        uup.CCHID = cchid;
                                        uup.PostData(cnxString);
                                        if (!uup.HasThrownError)
                                        {
                                            using (UpdateUserMobilePhone uump = new UpdateUserMobilePhone())
                                            {
                                                uump.MobilePhone = mobilePhone;
                                                uump.CCHID = cchid;
                                                uump.PostData(cnxString);
                                                if (!uump.HasThrownError)
                                                {
                                                    isSuccessful = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return isSuccessful;
        }

        private IDictionary<string, object> GetPasswordQuestions()
        {
            const string strQuestionQuery = @"select row_number() over (order by passwordquestion asc) as id, passwordquestion from passwordquestions";
            using (DataBase dbQuestions = new DataBase(strQuestionQuery, true))
            {
                dbQuestions.GetFrontEndData();
                if (dbQuestions.Tables.Count > 0 && dbQuestions.Tables[0].Rows.Count > 0)
                {
                    return dbQuestions.Tables[0].AsEnumerable().ToDictionary(q => q.GetData("id"), q => (object)q.GetData("passwordquestion"));
                }
            }
            return null;
        }

        private void InsertAuditTrail(int cchId, string sessionId, string action, string domain, string connection)
        {
            using (var iat = new InsertAuditTrail())
            {
                iat.CchId = cchId;
                iat.SessionId = sessionId;
                iat.Action = action;
                iat.Domain = domain;
                iat.PostData(connection);
            }
        }
    }
}
