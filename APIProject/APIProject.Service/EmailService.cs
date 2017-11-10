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
            return new NetworkCredential
            {
                UserName = "qtsccrmemailsender@gmail.com",
                Password = "kenejnzwmzwboknd"
            };
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

        public void SendQuoteEmail(Contact contact, Staff staff, IEnumerable<QuoteItemMapping> quoteItemMappings, Quote quote)
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + "/EmailTemplate/PriceEmailTemplate.html";
            string templateString = File.ReadAllText(file);
            Template template = Template.Parse(templateString);
            var salesItems = quoteItemMappings.Select(quoteItem => Hash.FromAnonymousObject(new
            {
                quoteItem.SalesItem.Name,
                quoteItem.SalesItem.Price,
                quoteItem.SalesItem.Unit
            }));
            string body = template.Render(Hash.FromAnonymousObject(new
            {
                customerName = contact.Name,
                salesItems,
                tax = quote.Tax,
                discount = quote.Discount,
                staffName = staff.Name,
                staffEmail = staff.Email,
                staffPhonenumber = staff.Phone
            }));
            NetworkCredential networkCredential = GetDefaultNetworkCredential();
            MailMessage message = new MailMessage();
            message.From = new MailAddress(networkCredential.UserName);
            message.To.Add(contact.Email);
            message.Subject = $"QTSC - Báo giá ngày {DateTime.Now.Date}";
            message.Body = body;
            message.IsBodyHtml = true;
            SendEmail(message, networkCredential);
        }
    }

    public interface IEmailService
    {
        void SendEmail(MailMessage message, NetworkCredential credential);

        void SendQuoteEmail(Contact contact, Staff staff, IEnumerable<QuoteItemMapping> quoteItemMappings, Quote quote);
    }
}