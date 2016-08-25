using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using CchWebAPI.Configuration;
using NLog;

namespace CchWebAPI.Support {
    public class EmailMessenger {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Sends the message with attachment via e-mail.
        /// </summary>
        /// <param name="to">the e-mail recipients.</param>
        /// <param name="cc">the e-mail copy recipients.</param>
        /// <param name="subject">The subject line.</param>
        /// <param name="message">The message content.</param>
        /// <param name="isHtml">if set to <c>true</c> [is HTML].</param>
        /// <param name="attachmentPath">The file system path to the attachment file.</param>
        /// <param name="isInternalServer">if set to <c>true</c> [we use the internal SMTP server of CCH] else [we use the Gmail public server].</param>
        public static bool Send(string to, string cc,
            string subject, string message, bool isHtml, string attachmentPath, bool isInternalServer) {
            var sentEmailSuccessfully = true;

            MailMessage mailMessage = new MailMessage {
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                Attachments = { new Attachment(attachmentPath, MediaTypeNames.Application.Pdf) },
                BodyEncoding = Encoding.ASCII,
                Priority = MailPriority.High
            };

            if (EmailConfiguration.Settings.UseInternalServer) {
                mailMessage.From = new MailAddress(EmailConfiguration.InternalServerSettings.From);
            }
            else {
                mailMessage.From = new MailAddress(EmailConfiguration.ExternalServerSettings.From);
            }

            char[] charSeparators = new char[] { ',', ' ' };
            var result = to.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string recipient in result) {
                mailMessage.To.Add(new MailAddress(recipient));
            }

            if (!string.IsNullOrEmpty(cc)) {
                result = cc.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ccRecipient in result) {
                    mailMessage.CC.Add(new MailAddress(ccRecipient));
                }
            }

            SmtpClient smtpClient;

            if (isInternalServer) {
                smtpClient = EmailConfiguration.GetInternalServerSmtpClient();
            }
            else {
                smtpClient = EmailConfiguration.GetExternalServerSmtpClient();
            }

            try {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex) {
                sentEmailSuccessfully = false;

                string emailTo;

                if (mailMessage.To.Count > 0) {
                    emailTo = mailMessage.To[0].Address;
                }
                else {
                    emailTo = "";
                }

                logger.Error("Failed to send email. from={0}, to={1}, subject={2}, server={3}, exception={4}, innerException={5}, stackTrace = {6}",
                    mailMessage.From, emailTo, mailMessage.Subject, smtpClient.Host, ex.Message, ex.InnerException, ex.StackTrace);
            }

            return sentEmailSuccessfully;
        }
    }
}
