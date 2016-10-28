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
                case "http://localhost:8083":
                    return ClearCost.UnitTesting.Environment.LocalWapi;
                case "https://dwapi.clearcosthealth.com":
                    return ClearCost.UnitTesting.Environment.dwapi;
                case "https://wapi.clearcosthealth.com":
                    return ClearCost.UnitTesting.Environment.wapi;
                case "https://la2wapi.clearcosthealth.com":
                    return ClearCost.UnitTesting.Environment.la2wapi;
                case "https://lawapi.clearcosthealth.com":
                    return ClearCost.UnitTesting.Environment.lawapi;
                case "https://pwapi.clearcosthealth.com":
                    throw new NotImplementedException("Processing environment isn't supported currently.");
                default:
                    throw new NotImplementedException(String.Format("There's no mapping for the apiBaseAddress: {0}", apiBaseAddress));
            }
        }
        public static string GetApiBaseAddress() {
            return ConfigurationManager.AppSettings["APIBaseAddress"].ToString();
        }
    }
}
