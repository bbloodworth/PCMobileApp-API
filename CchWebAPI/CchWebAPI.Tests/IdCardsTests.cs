using System.Configuration;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ClearCost.Net;
using ClearCost.Platform;
using ClearCost.UnitTesting;
using CchWebAPI.IdCards.Data;
using CchWebAPI.IdCards.Dispatchers;


namespace CchWebAPI.Tests {
    [TestClass]
    public class IdCardsTests {
        [TestMethod]
        [TestCategory("MPM-1370")]
        [TestCategory("Integration Tests")]
        public async Task CanGetIdCardsFromDB() {
            var repo = new IdCardsRepository();
            repo.Initialize(ConfigurationManager.ConnectionStrings["Platform"].ConnectionString);
            var dispatcher = new IdCardsDispatcher(repo);

            var result = await dispatcher.ExecuteAsync(57020, 
                EmployerCache.Employers.FirstOrDefault(e => e.Id.Equals(11)));

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod]
        [TestCategory("MPM-1370")]
        [TestCategory("Integration Tests")]
        public void CanGetIdCardsE2E() {
            if (!Debugger.IsAttached)
                return;

            var ctx = UnitTestContext.Get(Environment.LocalWapi, "mary.smith@cchcaesars.com");

            var result = ApiUtil.GetJsonResult<ApiResult<List<IdCard>>>(ctx, @"v2/IdCards");

            Assert.IsNotNull(result);
        }
    }
}
