using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CchWebAPI.Areas.Animation.Models;
using CchWebAPI.Properties;
using CchWebAPI.Support;
using ClearCost.IO.Log;
using NLog;
using System.Dynamic;
using System.Data;
using CchWebAPI.Services;
using System.Web.Http.Cors;

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class EmployeeController : ApiController
    {
        [HttpGet]
        [EnableCors(origins: "https://dev.hrinfoapp.com,https://alpha.hrinfoapp.com,https://alpha2.hrinfoapp.com", headers: "*", methods: "get")]
        public HttpResponseMessage GetDependents()
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);
            dynamic data = new ExpandoObject();

            EmployeeServices services = new EmployeeServices();

            MemberDetail keibc = services.GetKeyEmployeeInfo(Request.EmployerID(), Request.CCHID());

            if (keibc != null)
            {
                Dependents deps = new Dependents();
                Dependent member = new Dependent();

                member.CCHID = Request.CCHID();
                member.Email = keibc.Email;
                member.SubscriberMedicalId = keibc.SubscriberMedicalId;
                member.FullName = string.Format("{0} {1}", keibc.FirstName, keibc.LastName);
                member.Age = 0;
                member.IsAdult = true;
                member.RelationshipText = "Employee";

                deps.Add(member);

                if (keibc.Dependents.Count > 0)
                {
                    foreach (DependentDetail detail in keibc.Dependents)
                    {
                        Dependent dep = new Dependent();
                        dep.CCHID = detail.CCHID;
                        dep.Email = detail.Email;
                        dep.SubscriberMedicalId = detail.SubscriberMedicalId;
                        dep.FullName = string.Format("{0} {1}", detail.FirstName, detail.LastName);
                        dep.Age = detail.Age;
                        dep.IsAdult = detail.IsAdult;
                        dep.RelationshipText = detail.RelationshipText;

                        deps.Add(dep);
                    }
                    data.Dependents = deps;

                    hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                }
            }

            //using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            //{
            //    using (GetKeyEmployeeInfoByCchId gkeibc = new GetKeyEmployeeInfoByCchId())
            //    {
            //        gkeibc.CchId = Request.CCHID();
            //        gkeibc.GetData(gecs.ConnString);

            //        if (gkeibc.Tables.Count > 0 && gkeibc.Tables[1].Rows.Count > 0)
            //        {
            //            Dependents deps = new Dependents();
            //            Dependent dep = null;

            //            if (gkeibc.EmployeeTable.TableName != "Empty" && 
            //                gkeibc.EmployeeTable.Rows.Count > 0)
            //            {
            //                dep = new Dependent();

            //                dep.CCHID = int.Parse(gkeibc["CCHID"].ToString());
            //                dep.Email = gkeibc["Email"].ToString();
            //                dep.SubscriberMedicalId = gkeibc["SubscriberMedicalID"].ToString();
            //                dep.FullName = string.Format("{0} {1}", gkeibc["FirstName"].ToString(), gkeibc["LastName"].ToString());
            //                dep.Age = 0;
            //                dep.IsAdult = true;
            //                dep.RelationshipText = "Employee";

            //                deps.Add(dep);
            //            }

            //            if (gkeibc.DependentTable.TableName != "EmptyTable")
            //            {

            //                gkeibc.ForEachDependent(delegate (DataRow dr)
            //                {
            //                    dep = new Dependent();
            //                    dep.CCHID = int.Parse(dr["CCHID"].ToString());
            //                    dep.Email = dr["Email"].ToString();
            //                    dep.SubscriberMedicalId = dr["SubscriberMedicalID"].ToString();
            //                    dep.FullName = string.Format("{0} {1}", dr["FirstName"].ToString(), dr["LastName"].ToString());
            //                    dep.Age = int.Parse(dr["Age"].ToString());
            //                    dep.IsAdult = int.Parse(dr["Adult"].ToString()) == 1 ? true : false;
            //                    dep.RelationshipText = dr["RelationshipText"].ToString();

            //                    deps.Add(dep);
            //                });

            //                data.Dependents = deps;

            //                hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
            //            }
            //        }
            //    }
            //}
            return hrm;
        }
    }
}
