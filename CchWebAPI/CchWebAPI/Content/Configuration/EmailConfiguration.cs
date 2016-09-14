using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;

namespace CchWebAPI.Configuration {
    /// <summary>
    /// Provides strongly typed references and intellisense for email settings.
    /// </summary>
    public class EmailConfiguration : ConfigurationSection {
        /// <summary>
        /// For accessing settings that don't apply to a specific server setting.
        /// </summary>
        public static EmailConfiguration Settings {
            get { return ConfigurationManager.GetSection("emailSettings/settings") as EmailConfiguration; }
        }
        /// <summary>
        /// For accessing configuration settings for the internal email server.
        /// </summary>
        public static SmtpSection InternalServerSettings {
            get {
                return ConfigurationManager.GetSection("emailSettings/internalServer") as SmtpSection;
            }
        }
        /// <summary>
        /// For accessing configuration settings for the external email server.
        /// </summary>
        public static SmtpSection ExternalServerSettings {
            get {
                return ConfigurationManager.GetSection("emailSettings/externalServer") as SmtpSection;
            }
        }
        /// <summary>
        /// For accessing the useInternalServer property.
        /// </summary>
        [ConfigurationProperty("useInternalServer", DefaultValue = true, IsRequired = true)]
        public bool UseInternalServer {
            get { return (bool)this["useInternalServer"]; }
            set { this["useInternalServer"] = value; }
        }
        /// <summary>
        /// Convenience method for returning an SmtpClient configured with the internalServer settings in web.config.
        /// </summary>
        /// <returns>An SmtpClient configured with the internalServer settings.</returns>
        public static SmtpClient GetInternalServerSmtpClient() {
            return GetSmtpClient(InternalServerSettings);
        }
        /// <summary>
        /// Convenience method for returning an SmtpClient configured with the externalServer settings in web.config.
        /// </summary>
        /// <returns>An SmtpClient configured with the externalServer settings.</returns>
        public static SmtpClient GetExternalServerSmtpClient() {
            return GetSmtpClient(ExternalServerSettings);
        }
        /// <summary>
        /// Method for internal use to create an SmtpClient from the values specified in web.config.
        /// </summary>
        /// <param name="settings">An SmtpSection</param>
        /// <returns>An SmtpClient configured with the settings passed in the parameter.</returns>
        private static SmtpClient GetSmtpClient(SmtpSection settings) {
            return new SmtpClient() {
                Host = settings.Network.Host,
                Port = settings.Network.Port,
                Credentials = new NetworkCredential(
                    settings.Network.UserName,
                    settings.Network.Password
                ),
                DeliveryMethod = settings.DeliveryMethod,
                UseDefaultCredentials = settings.Network.DefaultCredentials,
                EnableSsl = settings.Network.EnableSsl
            };
        }
    }
}