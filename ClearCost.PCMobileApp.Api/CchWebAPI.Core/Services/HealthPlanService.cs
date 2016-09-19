using AutoMapper;
using CchWebAPI.Core.DataTransferObjects;
using CchWebAPI.Core.Interfaces.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace CchWebAPI.Core.Services {
    public class HealthPlanService {
        private readonly IHealthPlanRepository DbContext;

        public HealthPlanService(IHealthPlanRepository db) {
            DbContext = db;
        }

        public List<DataTransferObjects.MemberCard> GetMemberCards(CchUser cchUser) {
            var memberCards = DbContext.GetMemberCards(cchUser.Id).ToList();

            return Mapper.Map<List<Entities.MemberCard>, List<DataTransferObjects.MemberCard>>(memberCards);
        }
    }
}
