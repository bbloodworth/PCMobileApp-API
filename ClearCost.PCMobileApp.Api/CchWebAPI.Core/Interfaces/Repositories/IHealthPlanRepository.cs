using CchWebAPI.Core.Entities;
using System.Collections.Generic;

namespace CchWebAPI.Core.Interfaces.Repositories {
    public interface IHealthPlanRepository {
        IEnumerable<MemberCard> GetMemberCards(int id);
    }
}
