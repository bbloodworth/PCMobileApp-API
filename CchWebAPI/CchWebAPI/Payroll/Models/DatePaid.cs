using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Payroll.Models {
    public class DatePaid {
        public int CchId { get; set; }
        public string DocumentId { get; set; }
        public DateTime PaycheckDate { get; set; }
    }
}