using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    class EmailService : IEmailService
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="message">
        /// new MailMessage(
        ///         "senderEmail@gmail.com",
        ///         "receiverEmail@gmail.com",
        ///         "TestCRMSubject",
        ///         "TestCRMBody"
        ///       );
        /// </param>
        /// <param name="credential">
        /// new NetworkCredential(
        ///         "senderEmail@gmail.com",
        ///         "kenejnzwmzwboknd"
        ///     );
        /// </param>
        public void SendEmail(MailMessage message, NetworkCredential credential)
        {
            SmtpClient smtpobj = new SmtpClient("smtp.gmail.com", 25)
            {
                EnableSsl = true,
                Credentials = credential
            };

            
            smtpobj.Send(message);
        }
    }

    public interface IEmailService
    {
        void SendEmail(MailMessage message, NetworkCredential credential);
    }
}