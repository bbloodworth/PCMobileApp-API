using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI.Areas.Animation.Models {
    public class Settings {
        public Security Security { get; set; }

        public Settings() {
            Security = new Security();
        }
    }

    public class Security {
        public int MinimumSecurityAnswerLength { get; set; }
    }
}