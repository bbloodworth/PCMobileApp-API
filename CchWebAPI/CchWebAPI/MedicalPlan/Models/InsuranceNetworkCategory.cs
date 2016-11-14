using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.MedicalPlan.Models {
    public abstract class InsuranceNetworkCategory {
        public InsuranceCoverageCategory InNetwork { get; set; }
        public InsuranceCoverageCategory OutOfNetwork { get; set; }

        public InsuranceNetworkCategory() {
            InNetwork = new InsuranceCoverageCategory();
            OutOfNetwork = new InsuranceCoverageCategory();
        }
    }
}