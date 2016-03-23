using System;
using System.Collections.Generic;

namespace CchWebAPI.Areas.Animation.Models {
    public class CardResult {
        public string CardName;
        public string ViewMode;
        public string CardUrl;
        public string SecurityToken;
    }

    public class CardUrlResult {
        public List<CardResult> Results { get; set; }
        public int TotalCount {
            get {
                return Results.Count;
            }
        }
    }

    public class CardToken {
        public int EmployerId { get; set; }
        public DateTime Expires { get; set; }
        public CardDetail CardDetail { get; set; }
    }
}