using CchWebAPI.Core.Entities;
using System.Data.Entity;

namespace CchWebAPI.Infrastructure.DataContexts {
    public class CchFrontEnd : DbContext {
        public CchFrontEnd(string connectionString)
            : base(connectionString) { }

        public virtual DbSet<Employer> Employers { get; set; }
    }
}
