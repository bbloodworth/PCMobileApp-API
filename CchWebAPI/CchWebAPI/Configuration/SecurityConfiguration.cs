using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CchWebAPI.Configuration {
    public class SecurityConfiguration : ConfigurationSection {
        private static SecurityConfiguration settings 
            = ConfigurationManager.GetSection("securitySettings") as SecurityConfiguration;

        public static SecurityConfiguration Settings {
            get {
                return settings;
            }
        }

        [ConfigurationProperty("minimumSecretAnswerLength", DefaultValue = 5, IsRequired = true)]
        public int MinimumSecretAnswerLength {
            get { return (int)this["minimumSecretAnswerLength"]; }
            set { this["minimumSecretAnswerLength"] = value; }
        }
    }
}