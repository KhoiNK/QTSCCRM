using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace APIProject.Helper
{
    public class EmailHelper
    {
        public void SendQuote(string contactEmail, string contactName, string attachFileSrc)
        {
            MailMessage message = new MailMessage(
             "qtsccrmemailsender@gmail.com",
             contactEmail,
             "Báo giá",
             "Kính gửi " + contactName
                   );
            message.Attachments.Add(new Attachment(attachFileSrc));
            NetworkCredential credent = new NetworkCredential(
             "qtsccrmemailsender@gmail.com",
             "kenejnzwmzwboknd"
                 );

            SendEmail(message, credent);
        }

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
}