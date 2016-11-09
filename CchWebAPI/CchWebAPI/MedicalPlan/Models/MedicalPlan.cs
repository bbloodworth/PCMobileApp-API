using CchWebAPI.MedicalPlan.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.MedicalPlan.Models {
    public class MedicalPlan {
        public int Id { get; set; }
        public string Name { get; set; }
        public Deductible Deductible { get; set; }
        public OutOfPocketMaximum OutOfPocketMaximum { get; set; }
        public Coinsurance Coinsurance { get; set; }
        public Copayment Copayment { get; set; }

        public MedicalPlan() {
            InitProperties();
        }
        public MedicalPlan(MedicalPlanOption medicalPlanOption) {
            InitProperties();

            Merge(medicalPlanOption);
        }

        private void InitProperties() {
            Deductible = new Deductible();
            OutOfPocketMaximum = new OutOfPocketMaximum();
            Coinsurance = new Coinsurance();
            Copayment = new Copayment();
        }

        private void Merge(MedicalPlanOption medicalPlanOption) {
            if (medicalPlanOption == null) {
                return;
            }

            Id = medicalPlanOption.BenefitPlanOptionKey;
            Name = medicalPlanOption.BenefitPlanOptionName;

            Deductible.InNetwork.Family = medicalPlanOption.FamilyDeductibleAmt;
            Deductible.InNetwork.Individual = medicalPlanOption.IndividualDeductibleAmt;
            Deductible.OutOfNetwork.Family = medicalPlanOption.OonFamilyDeductibleAmt;
            Deductible.OutOfNetwork.Individual = medicalPlanOption.OonIndividualDeductibleAmt;

            OutOfPocketMaximum.InNetwork.Family = medicalPlanOption.FamilyMaxOopAmt;
            OutOfPocketMaximum.InNetwork.Individual = medicalPlanOption.IndividualMaxOopAmt;
            OutOfPocketMaximum.OutOfNetwork.Family = medicalPlanOption.OonFamilyMaxOopAmt;
            OutOfPocketMaximum.OutOfNetwork.Individual = medicalPlanOption.OonIndividualMaxOopAmt;

            Coinsurance.InNetwork.PercentageCovered = medicalPlanOption.CoinsurancePct;
            Coinsurance.OutOfNetwork.PercentageCovered = medicalPlanOption.OonCoinsurancePct;

            Copayment.InNetwork.CopaymentAmount = medicalPlanOption.CopayAmt;
            Copayment.OutOfNetwork.CopaymentAmount = medicalPlanOption.OonCopayAmt;
        }
    }

    #region Coinsurance
    public sealed class Coinsurance {
        public CoinsuranceNetworkCoverage InNetwork { get; set; }
        public CoinsuranceNetworkCoverage OutOfNetwork { get; set; }
        public Coinsurance() {
            InNetwork = new CoinsuranceNetworkCoverage();
            OutOfNetwork = new CoinsuranceNetworkCoverage();
        }

    }
    public sealed class CoinsuranceNetworkCoverage {
        public decimal? PercentageCovered { get; set; }
    }
    #endregion

    #region Copayment
    public sealed class Copayment {
        public CopaymentNetworkCoverage InNetwork { get; set; }
        public CopaymentNetworkCoverage OutOfNetwork { get; set; }

        public Copayment() {
            InNetwork = new CopaymentNetworkCoverage();
            OutOfNetwork = new CopaymentNetworkCoverage();
        }
    }

    public sealed class CopaymentNetworkCoverage {
        public decimal? CopaymentAmount { get; set; }
    }
    #endregion
}