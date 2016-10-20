using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CchWebAPI.Tests {
    /// <summary>
    /// Determines environment from app.config
    /// </summary>
    public class EnvironmentHelper {
        public static ClearCost.UnitTesting.Environment GetEnvironment() {
            var apiBaseAddress = ConfigurationManager.AppSettings["APIBaseAddress"].ToString();

            switch (apiBaseAddress) {
                case "https://dwapi.clearcosthealth.com":
                    return ClearCost.UnitTesting.Environment.dwapi;
                default:
                    throw new NotImplementedException(String.Format("There's no mapping for the apiBaseAddress: {0}", apiBaseAddress));
            }
        }
    }
}
