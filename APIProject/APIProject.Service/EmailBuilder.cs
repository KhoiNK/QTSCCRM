using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using DotLiquid;

namespace APIProject.Service
{
    public class EmailBuilder
    {
        private MailAddress _from;
        private MailAddressCollection _to;
        private MailAddressCollection _bcc;
        private String _subject;
        private String _body;
        private bool _isHtml;
        private string[] _attachments;

        public EmailBuilder SetFrom(string emailAddress)
        {
            _from = new MailAddress(emailAddress);
            return this;
        }

        public EmailBuilder SetTO(string[] mailAddresses)
        {
            SetTO(StringArrayToMailAddressCollection(mailAddresses));
            return this;
        }

        public EmailBuilder SetBCC(string[] mailAddresses)
        {
            SetBCC(StringArrayToMailAddressCollection(mailAddresses));
            return this;
        }


        public EmailBuilder SetTO(MailAddressCollection mailAddressCollection)
        {
            if (mailAddressCollection != null && _bcc != null && mailAddressCollection.Count + _bcc.Count >= 100)
            {
                throw new NotImplementedException();
            }

            _to = mailAddressCollection;
            return this;
        }

        public EmailBuilder SetBCC(MailAddressCollection mailAddressCollection)
        {
            if (_to != null && mailAddressCollection != null && _to.Count + mailAddressCollection.Count >= 100)
            {
                throw new NotImplementedException();
            }

            _bcc = mailAddressCollection;
            return this;
        }

        public EmailBuilder SetSubject(String subject)
        {
            _subject = subject;
            return this;
        }

        public EmailBuilder SetBody(String body)
        {
            _body = body;
            _isHtml = false;
            return this;
        }

        public EmailBuilder SetBody(Template template, object data)
        {
            _body = template.Render(Hash.FromAnonymousObject(data));
            _isHtml = true;
            return this;
        }

        public EmailBuilder SetBody(Template template, object data, IEnumerable<Type> filters)
        {
            RenderParameters renderParameters = new RenderParameters(CultureInfo.CurrentCulture)
            {
                Filters = filters,
                LocalVariables = Hash.FromAnonymousObject(data)
            };
            _body = template.Render(renderParameters);
            _isHtml = true;
            return this;
        }

        public EmailBuilder SetAttachments(string[] attachments)
        {
            _attachments = attachments;
            return this;
        }

        public MailMessage[] getEmails()
        {
            List<MailMessage> mailMessages = new List<MailMessage>();
            if (_bcc != null && _bcc.Count > 100)
            {
                mailMessages.AddRange(AddBcc(_bcc));
            }
            else if (_to != null && _to.Count > 100)
            {
                mailMessages.AddRange(AddTo(_to));
            }
            else
            {
                MailMessage mailMessage = GetEmailWithoutRecipients();
                if (_to != null)
                {
                    foreach (MailAddress mailAddress in _to)
                    {
                        mailMessage.To.Add(mailAddress);
                    }
                }

                if (_bcc != null)
                {
                    foreach (MailAddress mailAddress in _bcc)
                    {
                        mailMessage.Bcc.Add(mailAddress);
                    }
                }

                mailMessages.Add(mailMessage);
            }

            return mailMessages.ToArray();
        }

        private List<MailMessage> AddBcc(IEnumerable<MailAddress> addresses)
        {
            List<MailMessage> mailMessages = new List<MailMessage>();
            Queue<MailAddress> queue = new Queue<MailAddress>(addresses);
            MailMessage mailMessage = GetEmailWithoutRecipients();
            int count = 0;
            while (queue.Count > 0)
            {
                mailMessage.Bcc.Add(queue.Dequeue());
                count++;
                if (count == 100 || queue.Count == 0)
                {
                    mailMessages.Add(mailMessage);
                    mailMessage = GetEmailWithoutRecipients();
                    count = 0;
                }
            }

            return mailMessages;
        }

        private List<MailMessage> AddTo(IEnumerable<MailAddress> addresses)
        {
            List<MailMessage> mailMessages = new List<MailMessage>();
            Queue<MailAddress> queue = new Queue<MailAddress>(addresses);
            MailMessage mailMessage = GetEmailWithoutRecipients();
            int count = 0;
            while (queue.Count > 0)
            {
                mailMessage.To.Add(queue.Dequeue());
                count++;
                if (count == 100 || queue.Count == 0)
                {
                    mailMessages.Add(mailMessage);
                    mailMessage = GetEmailWithoutRecipients();
                    count = 0;
                }
            }

            return mailMessages;
        }

        private MailMessage GetEmailWithoutRecipients()
        {
            var message = new MailMessage
            {
                From = _from,
                Subject = _subject,
                Body = _body,
                IsBodyHtml = _isHtml
            };

            if (_attachments != null)
            {
                foreach (string attachment in _attachments)
                {
                    message.Attachments.Add(new Attachment(attachment));
                }
            }

            return message;
        }

        private MailAddressCollection StringArrayToMailAddressCollection(string[] mailAddresses)
        {
            MailAddressCollection addressCollection = new MailAddressCollection();
            foreach (string address in mailAddresses)
            {
                try
                {
                    addressCollection.Add(address);
                }
                catch (FormatException)
                {
                    //skip malform email
                }
            }

            return addressCollection;
        }
    }
}