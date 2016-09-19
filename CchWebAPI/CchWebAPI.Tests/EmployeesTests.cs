using CchWebAPI.Employees.Data;
using CchWebAPI.Employees.Dispatchers;
using ClearCost.Platform;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Tests {
    [TestClass]
    public class EmployeesTests {
        [TestMethod]
        [TestCategory("MPM-1486")]
        [TestCategory("Integration Tests")]
        public async Task CanGetEmployeeFromDB() {
            var repo = new EmployeesRepository();
            repo.Initialize(ConfigurationManager.ConnectionStrings["Platform"].ConnectionString);
            var dispatcher = new EmployeesDispatcher(repo);

            var result = await dispatcher.ExecuteAsync(57020,
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(11)));

            Assert.IsNotNull(result);
        }
    }
}
