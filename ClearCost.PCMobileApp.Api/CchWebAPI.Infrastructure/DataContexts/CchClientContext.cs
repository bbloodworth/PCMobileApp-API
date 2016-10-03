using CchWebAPI.Core.Entities;
using System.Data.Entity;

namespace CchWebAPI.Infrastructure.DataContexts {
    public class CchClientContext : DbContext {
        public CchClientContext(string connectionString)
            : base(connectionString) { }

        public virtual DbSet<MemberCard> MemberCards { get; set; }

    }
}
