using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using APIProject.Model.Models;
using APIProject.Service.DotliquidFilters;
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
        public void SendEmail(MailMessage message, NetworkCredential credential = null)
        {
            if (credential == null)
            {
                credential = GetDefaultNetworkCredential();
            }
            SmtpClient smtpobj = new SmtpClient("smtp.gmail.com", 25)
            {
                EnableSsl = true,
                Credentials = credential
            };

            smtpobj.Send(message);
        }

        public void SendQuoteEmail(Contact contact, Staff staff, IEnumerable<QuoteItemMapping> quoteItemMappings,
            Quote quote)
        {
            Template template = GetTemplate("PriceEmailTemplate.html");
            var salesItems = quoteItemMappings.Select(quoteItem => Hash.FromAnonymousObject(new
            {
                quoteItem.SalesItem.Name,
                quoteItem.SalesItem.Price,
                quoteItem.SalesItem.Unit
            }));
            RenderParameters renderParameters = new RenderParameters(CultureInfo.CurrentCulture)
            {
                Filters = new[] {typeof(CustomFilters)},
                LocalVariables = Hash.FromAnonymousObject(new
                {
                    customerName = contact.Name,
                    salesItems,
                    tax = quote.Tax,
                    discount = quote.Discount,
                    staffName = staff.Name,
                    staffEmail = staff.Email,
                    staffPhonenumber = staff.Phone
                })
            };
            string body = template.Render(renderParameters);
            NetworkCredential networkCredential = GetDefaultNetworkCredential();
            MailMessage message = new MailMessage
            {
                From = new MailAddress(networkCredential.UserName),
                Subject = $"QTSC - Báo giá ngày {DateTime.Now.ToShortDateString()}",
                To = {contact.Email},
                Body = body,
                IsBodyHtml = true
            };
            SendEmail(message, networkCredential);
        }

        public void SendThankEmail(string customerName,string contactName,string customerEmail, string marketingPlanTitle,
            IEnumerable<MarketingPlan> doingPlans)
        {
            Template template = GetTemplate("ThankEmailTemplate.html");
            var marketingPlans = doingPlans.Select(marketingPlan => Hash.FromAnonymousObject(new
            {
                name = marketingPlan.Title
            }));
            string body = template.Render(Hash.FromAnonymousObject(new
            {
                customerName,
                contactName,
                eventName = marketingPlanTitle,
                marketingPlans
            }));
            NetworkCredential networkCredential = GetDefaultNetworkCredential();
            MailMessage message = new MailMessage
            {
                From = new MailAddress(networkCredential.UserName),
                Subject = $"Sự kiện {marketingPlanTitle} - Lời cảm ơn",
                To = {customerEmail},
                Body = body,
                IsBodyHtml = true
            };
            SendEmail(message, networkCredential);
        }

        private Template GetTemplate(String fileName)
        {
            string templateString =
                File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/EmailTemplate/" + fileName);
            return Template.Parse(templateString);
        }
    }

    public interface IEmailService
    {
        void SendEmail(MailMessage message, NetworkCredential credential);

        void SendQuoteEmail(Contact contact, Staff staff, IEnumerable<QuoteItemMapping> quoteItemMappings, Quote quote);

        void SendThankEmail(String customerName,String contactName, String customerEmail, String marketingPlanTitle,
            IEnumerable<MarketingPlan> marketingPlans);
    }
}