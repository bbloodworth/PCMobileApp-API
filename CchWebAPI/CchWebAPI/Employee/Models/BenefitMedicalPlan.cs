using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Employee.Models {
    public class BenefitMedicalPlan {
        public int MemberPlanId { get; set; }
        public string PlanType { get; set; }
        public string SubscriberPlanId { get; set; }
    }
}