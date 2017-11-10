using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using APIProject.Model.Models;
using DotLiquid;

namespace APIProject.Service
{
    class EmailService : IEmailService
    {
        private NetworkCredential GetDefaultNetworkCredential()
        {
            return null;
        }   
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

        public MailMessage BuildQuoteInfomationBody(MailMessage message, string customerName, Staff staff,
            List<SalesItem> salesItems,
            Quote quote)
        {
            if (message == null)
            {
                message = new MailMessage();
            }
            string file = AppDomain.CurrentDomain.BaseDirectory + "/EmailTemplate/PriceEmailTemplate.html";
            string templateString = File.ReadAllText(file);
            Template template = Template.Parse(templateString);
            var objects = salesItems.Select(item => Hash.FromAnonymousObject(new
            {
                item.Name,
                item.Price,
                item.Unit
            }));
            string body = template.Render(Hash.FromAnonymousObject(new
            {
                customerName,
                salesItems = objects,
                tax = quote.Tax,
                discount = quote.Discount,
                staffName = staff.Name,
                staffEmail = staff.Email,
                staffPhonenumber = staff.Phone
            }));
            message.Body = body;
            message.IsBodyHtml = true;
            return message;
        }
    }

    public interface IEmailService
    {
        void SendEmail(MailMessage message, NetworkCredential credential);

        MailMessage BuildQuoteInfomationBody(MailMessage message, string customerName, Staff staff,
            List<SalesItem> salesItems, Quote quote);
    }
}