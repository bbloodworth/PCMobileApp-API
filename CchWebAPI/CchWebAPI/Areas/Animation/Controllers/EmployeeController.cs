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

namespace CchWebAPI.Areas.Animation.Controllers
{
    public class EmployeeController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetDependents()
        {
            HttpResponseMessage hrm = Request.CreateResponse(HttpStatusCode.NoContent);

            dynamic data = new ExpandoObject();
            using (GetEmployerConnString gecs = new GetEmployerConnString(Request.EmployerID()))
            {
                using (GetKeyEmployeeInfoByCchId gkeibc = new GetKeyEmployeeInfoByCchId())
                {
                    gkeibc.CchId = Request.CCHID();
                    gkeibc.GetData(gecs.ConnString);

                    if (gkeibc.Tables.Count > 0 && gkeibc.Tables[1].Rows.Count > 0)
                    {
                        if (gkeibc.DependentTable.TableName != "EmptyTable")
                        {
                            Dependents deps = new Dependents();
                            Dependent dep = null;

                            gkeibc.ForEachDependent(delegate (DataRow dr)
                            {
                                dep = new Dependent();
                                dep.CCHID = int.Parse(dr["CCHID"].ToString());
                                dep.Email = dr["Email"].ToString();
                                dep.SubscriberMedicalId = dr["SubscriberMedicalID"].ToString();
                                dep.FullName = string.Format("{0} {1}", dr["FirstName"].ToString(), dr["LastName"].ToString());
                                dep.Age = int.Parse(dr["Age"].ToString());
                                dep.IsAdult = int.Parse(dr["Adult"].ToString()) == 1 ? true : false;
                                dep.RelationshipText = dr["RelationshipText"].ToString();

                                deps.Add(dep);
                            });

                            data.Dependents = deps;

                            hrm = Request.CreateResponse(HttpStatusCode.OK, (object)data);
                        }
                    }
                }
            }
            return hrm;
        }
    }
}
