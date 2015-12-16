using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace CchWebAPI.Support
{
    public class EmailMessenger
    {
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
        public static void Send(string to, string cc,
            string subject, string message, bool isHtml, string attachmentPath, bool isInternalServer)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("Email.SenderEmail".GetConfigurationValue(), "Email.SenderName".GetConfigurationValue()),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                Attachments = { new Attachment(attachmentPath, MediaTypeNames.Application.Pdf) }, 
                BodyEncoding = Encoding.ASCII, 
                Priority = MailPriority.High
            };
            char[] charSeparators = new char[] { ',', ' ' };
            var result = to.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string recipient in result)
            {
                mailMessage.To.Add(new MailAddress(recipient));
            }

            if (!string.IsNullOrEmpty(cc)) {
                result = cc.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ccRecipient in result) {
                    mailMessage.CC.Add(new MailAddress(ccRecipient));
                }
            }

            string smtpHost = "Email.Smtp".GetConfigurationValue();

            SmtpClient smtpClient;

            if (isInternalServer)
            {
                smtpClient = new SmtpClient(smtpHost, 25)
                {
                    Credentials = new NetworkCredential("SAMB01VMD01@clearcosthealth.com", "pricetransparency"),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = true,
                    EnableSsl = false
                };
            }
            else
            {
                smtpClient = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    //Credentials = new System.Net.NetworkCredential("rdavid@clearcosthealth.com", "chikusho"),
                    Credentials = new NetworkCredential("SAMB01VMD01@clearcosthealth.com", "pricetransparency"),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };
            }
            smtpClient.Send(mailMessage);
        }
    }
}
