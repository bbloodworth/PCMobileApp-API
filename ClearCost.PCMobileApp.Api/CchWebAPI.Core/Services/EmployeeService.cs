using CchWebAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Core.Services {
    public class EmployeeService {

    }
    #region Data Access
    internal class EmployeeContext : ClearCostContext<EmployeeContext> {
        public EmployeeContext(Employer employer) : base(ConnectionFactory.Get(employer.connectionString)) {
            Employer = employer;
        }
    }
    #endregion
}
