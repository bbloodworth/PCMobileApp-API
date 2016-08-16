using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClearCost.Data;
using CchWebAPI.PComm.Models;
using CchWebAPI.Services;
using Dapper;
using ClearCost.UnitTesting;
using System.Net;
using System.Diagnostics;

namespace CchWebAPI.Tests {
    [TestClass]
    public class PlanInfoTests {
        [TestMethod]
        public void CanGetPlanInfo() {
            SqlMapper.SetTypeMap(typeof(HealthPlanSummary), new ColumnAttributeTypeMapper<HealthPlanSummary>());
            var service = new PlanInfoService();
            var summary = service.GetHealthPlanSummaryAsync(11, 57020).Result;

            Assert.IsNotNull(summary);
            Assert.IsFalse(string.IsNullOrEmpty(summary.PlanName));
            Assert.AreNotEqual(0, summary.Deductible);
            Assert.IsFalse(summary.AsOfDate.HasValue);
            Assert.IsFalse(summary.YearToDateSpent.HasValue);
            Assert.AreNotEqual(0, summary.Deductible);
            Assert.IsTrue(!summary.Coinsurance.HasValue || !summary.Coinsurance.Value.Equals(0));
            Assert.AreNotEqual(0, summary.CoinsuranceComplement);
            Assert.IsTrue(!summary.Copay.HasValue || !summary.Copay.Value.Equals(0));
            Assert.AreNotEqual(0, summary.OutOfPocketMax);

            summary = service.GetHealthPlanSummaryAsync(11, 21).Result;

            Assert.IsNotNull(summary);
            Assert.IsFalse(string.IsNullOrEmpty(summary.PlanName));
            Assert.AreNotEqual(0, summary.Deductible);
            Assert.IsTrue(summary.AsOfDate.HasValue);
            Assert.IsTrue(summary.YearToDateSpent.HasValue);
            Assert.AreNotEqual(0, summary.Deductible);
            Assert.IsTrue(!summary.Coinsurance.HasValue || !summary.Coinsurance.Value.Equals(0));
            Assert.AreNotEqual(0, summary.CoinsuranceComplement);
            Assert.IsTrue(!summary.Copay.HasValue || !summary.Copay.Value.Equals(0));
            Assert.AreNotEqual(0, summary.OutOfPocketMax);
        }

        [TestMethod]
        public void CanGetPlanInfoFromWapi() {
            if (!Debugger.IsAttached)
                return;

            SqlMapper.SetTypeMap(typeof(HealthPlanSummary), new ColumnAttributeTypeMapper<HealthPlanSummary>());
            var ctx = UnitTestContext.Get(ClearCost.UnitTesting.Environment.LocalWapi,
                "mary.smith@cchcaesars.com");

            var summary = ApiUtil.GetJsonResult<HealthPlanSummary>(ctx, "Animation/HealthPlanSummary");

            Assert.IsNotNull(summary); 
            Assert.IsFalse(string.IsNullOrEmpty(summary.PlanName));
            Assert.AreNotEqual(0, summary.Deductible);
            Assert.IsFalse(summary.AsOfDate.HasValue);
            Assert.IsFalse(summary.YearToDateSpent.HasValue);
            Assert.AreNotEqual(0, summary.Deductible);
            Assert.IsTrue(!summary.Coinsurance.HasValue || !summary.Coinsurance.Value.Equals(0));
            Assert.AreNotEqual(0, summary.CoinsuranceComplement);
            Assert.IsTrue(!summary.Copay.HasValue || !summary.Copay.Value.Equals(0));
            Assert.AreNotEqual(0, summary.OutOfPocketMax);
        }

        [TestMethod]
        public void CanSubmitBenefitInquiry() {
            return;
            var service = new PlanInfoService();
            service.SubmitBenefitInquiry(11, 57020);
        }
    }
}
