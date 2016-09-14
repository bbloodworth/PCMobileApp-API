using CchWebAPI.Core.DataTransferObjects;
using CchWebAPI.Core.Services;
using CchWebAPI.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CchWebAPI.Controllers
{
    [RoutePrefix("v2")]
    public class MemberCardController : ApiController
    {
        public List<MemberCard> GetMemberCards(CchUser cchUser) {
            var db = new HealthPlanRepository("");
            var healthPlanService = new HealthPlanService(db);

            return healthPlanService.GetMemberCards(cchId);
        }
    }
}
