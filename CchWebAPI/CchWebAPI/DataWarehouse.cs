using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CchWebAPI {
    /// <summary>
    /// This is a temporary class that will be replaced in the future when ClearCost.Platform.Employer
    /// is updated to return data warehouse connection strings.
    /// </summary>
    public class DataWarehouse {
        public static string GetEmployerConnectionString(int employerId) {
            switch (employerId) {
                // All tables aren't present for Caesars.
                //case 11:
                //    return "Data source=kermitdb.cch.clearcosthealth.com,49444;Initial Catalog=CCH_CaesarsDWH;Trusted_Connection=True;Asynchronous Processing=True; MultipleActiveResultSets=true";
                case 21:
                    return "Data source=kermitdb.cch.clearcosthealth.com,49444;Initial Catalog=CCH_DemoDWH;Trusted_Connection=True;Asynchronous Processing=True; MultipleActiveResultSets=true";
                case 2000:
                    return "Data source=kermitdb.cch.clearcosthealth.com,49444;Initial Catalog=CCH_TYLinDWH;Trusted_Connection=True;Asynchronous Processing=True; MultipleActiveResultSets=true";
                default:
                    return "";
            }
        }
    }
}