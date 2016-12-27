using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.MedicalPlan.Models {
    public class MedicalPlanAccumulation {
        public InsuranceCoverageCategory DeductibleAmountPaid { get; set; }
        public InsuranceCoverageCategory DeductibleRemainingAmount { get; set; }
        public InsuranceCoverageCategory OutOfPocketMaximumPaid { get; set; }
        public InsuranceCoverageCategory OutOfPocketMaximumRemainingAmount { get; set; }
        public InsuranceCoverageCategory PlanAmount { get; set; }
        public InsuranceCoverageCategory PlanOutOfPocketMaximum { get; set; }

        public MedicalPlanAccumulation() {
            InitProperties();
        }

        public MedicalPlanAccumulation(Data.MedicalPlanAccumulation medicalPlanAccumulation) {
            InitProperties();
            Merge(medicalPlanAccumulation);
        }

        private void InitProperties() {
            DeductibleAmountPaid = new InsuranceCoverageCategory();
            DeductibleRemainingAmount = new InsuranceCoverageCategory();
            OutOfPocketMaximumPaid = new InsuranceCoverageCategory();
            OutOfPocketMaximumRemainingAmount = new InsuranceCoverageCategory();
            PlanAmount = new InsuranceCoverageCategory();
            PlanOutOfPocketMaximum = new InsuranceCoverageCategory();
        }

        private void Merge(Data.MedicalPlanAccumulation medicalPlanAccumulation) {
            if (medicalPlanAccumulation == null) {
                return;
            }

            DeductibleAmountPaid.Family = medicalPlanAccumulation.FamilyDeductiblePaidAmt;
            DeductibleAmountPaid.Individual = medicalPlanAccumulation.IndividualDeductiblePaidAmt;

            DeductibleRemainingAmount.Family = medicalPlanAccumulation.FamilyDeductibleRemainingAmt;
            DeductibleRemainingAmount.Individual = medicalPlanAccumulation.IndividualDeductibleRemainingAmt;

            OutOfPocketMaximumPaid.Family = medicalPlanAccumulation.FamilyMaxOopPaidAmt;
            OutOfPocketMaximumPaid.Individual = medicalPlanAccumulation.IndividualMaxOopPaidAmt;

            OutOfPocketMaximumRemainingAmount.Family = medicalPlanAccumulation.FamilyMaxOopRemainingAmt;
            OutOfPocketMaximumRemainingAmount.Individual = medicalPlanAccumulation.IndividualMaxOopRemainingAmt;

            PlanAmount.Individual = medicalPlanAccumulation.IndividualDeductibleAmt;
            PlanAmount.Family = medicalPlanAccumulation.FamilyDeductibleAmt;

            PlanOutOfPocketMaximum.Individual = medicalPlanAccumulation.IndividualMaxOOPAmt;
            PlanOutOfPocketMaximum.Family = medicalPlanAccumulation.FamilyMaxOOPAmt;
        }
    }
}
 