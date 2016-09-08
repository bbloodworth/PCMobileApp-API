using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.IdCards.Models {
    public class CardToken {
        public int EmployerId { get; set; }
        public DateTime Expires { get; set; }
        public CardDetail CardDetail { get; set; }
    }
}