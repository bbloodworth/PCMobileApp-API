using CchWebAPI.Core.Entities;
using CchWebAPI.Core.Interfaces.Repositories;
using CchWebAPI.Infrastructure.DataContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Infrastructure.Repositories {
    public class HealthPlanRepository : IHealthPlanRepository, IDisposable {
        private CchClientContext DbContext;
        public HealthPlanRepository(string connectionString) {
            DbContext = new CchClientContext(connectionString);
        }
        public IEnumerable<MemberCard> GetMemberCards(int cchId) {
            return DbContext.MemberCards.Where(p => p.VisibleByUsers.Any(m => m.Id == cchId));
        }

        public void Dispose() {
            DbContext.Dispose();
        }
    }
}
