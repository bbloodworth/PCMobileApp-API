using System.Collections.Generic;

namespace CchWebAPI.Employee.Models {
    public class PlanMember {
        public int CchId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int BenefitPlanOptionKey { get; set; }
        public string BenefitTypeCode { get; set; }
    }



    public class Dependent {
        public int CchId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? MED { get; set; }
        public int? DEN { get; set; }
        public int? VIS { get; set; }
        public int? RX { get; set; }
    }
}