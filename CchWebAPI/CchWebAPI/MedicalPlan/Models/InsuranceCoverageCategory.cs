using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.MedicalPlan.Models {
    public sealed class InsuranceCoverageCategory {
        public decimal? Individual { get; set; }
        public decimal? Family { get; set; }
    }
}