using CchWebAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CchWebAPI {
    /// <summary>
    /// This is a temporary class that will be replaced in the future when ClearCost.Platform.Employer
    /// is updated to return data warehouse connection strings.
    /// </summary>
    public class EmployerConnectionString {
        public static EmployerConfigElement GetConnectionString(int employerId) {

            foreach (EmployerConfigElement employer in EmployerConnectionStringConfiguration.ConnectionStrings.Employers) {
                if (employer.Id == employerId) {
                    return employer;
                }
            }

            return new EmployerConfigElement();
        }
    }
}