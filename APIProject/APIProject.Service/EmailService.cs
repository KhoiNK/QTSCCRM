using System;
using System.Collections;
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
                //UserName = "phongtlse61770@fpt.edu.vn",
                UserName="qtsccrmemailsender@gmail.com",
                //Password = "ugodedyupyaccrbl"
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
        
//        public void SendTestEmail(MailMessage message)
//        {
//            SmtpClient smtpobj = new SmtpClient("localhost", 25)
//            {
//                EnableSsl = false
//            };
//            smtpobj.Send(message);
//        }
        
        public void SendQuoteEmail(Contact contact, Staff staff,string[] attactments, IEnumerable<QuoteItemMapping> quoteItemMappings,
            Quote quote)
        {
            Template template = GetTemplate("PriceEmailTemplate.html");
            var salesItems = quoteItemMappings.Select(quoteItem => Hash.FromAnonymousObject(new
            {
                quoteItem.SalesItem.Name,
                quoteItem.SalesItem.Price,
                quoteItem.SalesItem.Unit
            }));
            object data = new
            {
                customerName = contact.Name,
                contactName = contact.Name,
                salesItems = salesItems,
                tax = quote.Tax,
                discount = quote.Discount,
                staffName = staff.Name,
                staffEmail = staff.Email,
                staffPhonenumber = staff.Phone
            };
            
            NetworkCredential networkCredential = GetDefaultNetworkCredential();
            MailMessage[] mailMessages = new EmailBuilder()
                .SetSubject($"QTSC - Báo giá ngày {DateTime.Now.ToShortDateString()}")
                .SetBody(template,data,new[] {typeof(CustomFilters)})
                .SetFrom(networkCredential.UserName)
                .SetTO(new[]{contact.Email})
                .SetAttachments(attactments)
                .getEmails();
            foreach (MailMessage mailMessage in mailMessages)
            {
                SendEmail(mailMessage, networkCredential);
            }
        }

        public void SendThankEmail(string customerName,string contactName,string customerEmail, string marketingPlanTitle,
            IEnumerable<MarketingPlan> doingPlans)
        {
            Template template = GetTemplate("ThankEmailTemplate.html");
            var marketingPlans = doingPlans.Select(marketingPlan => Hash.FromAnonymousObject(new
            {
                name = marketingPlan.Title
            }));
            object data = new
            {
                customerName,
                contactName,
                eventName = marketingPlanTitle,
                marketingPlans
            };
            NetworkCredential networkCredential = GetDefaultNetworkCredential();
            MailMessage[] mailMessages = new EmailBuilder()
                .SetSubject($"Sự kiện {marketingPlanTitle} - Lời cảm ơn")
                .SetBody(template,data)
                .SetFrom(networkCredential.UserName)
                .SetTO(new[]{customerEmail})
                .getEmails();
            foreach (MailMessage mailMessage in mailMessages)
            {
                SendEmail(mailMessage, networkCredential);                
            }
        }

        public void SendNewMarketingPlan(IEnumerable<Contact> contacts,MarketingPlan marketingPlan)
        {
            Template template = GetTemplate("NewMarketingPlanTemplate.html");
            object data = new
            {
                eventName = marketingPlan.Title,
                eventDescription = marketingPlan.Description,
                linkEventPage = "https://qtsc.com.vn/web/guest/home"
            };
            NetworkCredential networkCredential = GetDefaultNetworkCredential();
            MailMessage[] messages = new EmailBuilder()
                .SetSubject("Thư mời tham gia sự kiện của QTSC")
                .SetBody(template, data)
                .SetFrom(networkCredential.UserName)
                .SetBCC(contacts.Select(contact => contact.Email).ToArray())
                .getEmails();
            foreach (MailMessage mailMessage in messages)
            {
                SendEmail(mailMessage, networkCredential);
            }
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
        void SendQuoteEmail(Contact contact, Staff staff,string[] attactments,
            IEnumerable<QuoteItemMapping> quoteItemMappings,
            Quote quote);

        void SendThankEmail(String customerName,String contactName, String customerEmail, String marketingPlanTitle,
            IEnumerable<MarketingPlan> marketingPlans);

        void SendNewMarketingPlan(IEnumerable<Contact> contacts, MarketingPlan marketingPlan);
    }
}