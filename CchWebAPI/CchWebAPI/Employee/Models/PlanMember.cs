using System.Collections.Generic;
using Newtonsoft.Json;

namespace CchWebAPI.Employee.Models {
    public class PlanMember {
        public int CchId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        public int BenefitPlanOptionKey { get; set; }
        [JsonIgnore]
        public string BenefitTypeCode { get; set; }
        public int? MED { get; set; }
        public int? DEN { get; set; }
        public int? VIS { get; set; }
        public int? RX { get; set; }
    }
}