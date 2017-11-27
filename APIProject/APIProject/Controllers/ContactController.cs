using APIProject.Helper;
using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/contact")]
    public class ContactController : ApiController
    {
        private readonly IContactService _contactService;
        private readonly ICustomerService _customerService;
        private readonly IUploadNamingService _uploadNamingService;

        public ContactController(IContactService _contactService,
            ICustomerService _customerService,
            IUploadNamingService _uploadNamingService)
        {
            this._customerService = _customerService;
            this._contactService = _contactService;
            this._uploadNamingService = _uploadNamingService;
        }

        [Route("GetCustomerContacts")]
        [ResponseType(typeof(ContactViewModel))]
        public IHttpActionResult GetCustomerContacts(int customerID = 0)
        {
            if (customerID == 0)
            {
                return BadRequest();
            }
            var foundContacts = _contactService.GetByCustomer(customerID).ToList();
            foundContacts.ForEach(c => _uploadNamingService.ConcatContactAvatar(c));
            return Ok(foundContacts.Select(c => new ContactViewModel(c)));
        }

        [Route("PostNewContact")]
        [ResponseType(typeof(PostContactResponseViewModel))]
        public IHttpActionResult PostNewContact(PostContactViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var response = new PostContactResponseViewModel();
            if (request.Avatar != null)
            {
                String[] splitBase64 = request.Avatar.Base64Content.Split(',');

                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                request.Avatar.Name = _uploadNamingService.GetContactAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                //saveFileHelper.SaveContactImage(request.Avatar.Name, request.Avatar.Base64Content);
                saveFileHelper.SaveContactImage(request.Avatar.Name, splitBase64.ToList().Last());
                response.ContactAvatarUpdated = true;
            }
            try
            {
                MailAddress m = new MailAddress(request.Email);

                var foundCustomer = _customerService.Get(request.CustomerID);
                var insertedContact = _contactService.Add(request.ToContactModel());
                response.ContactCreated = true;
                response.ContactID = insertedContact.ID;
                return Ok(response);

            }catch(FormatException ex)
            {
                return BadRequest("Email không đúng định dạng");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PutContactInformation")]
        [ResponseType(typeof(PutContactResponseViewModel))]
        public IHttpActionResult PutContactInformation(PutContactInformationViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var response = new PutContactResponseViewModel();
            if(request.Avatar != null)
            {
                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                request.Avatar.Name = _uploadNamingService.GetContactAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                saveFileHelper.SaveContactImage(request.Avatar.Name, request.Avatar.Base64Content);
                response.ContactAvatarUpdated = true;
            }
            try
            {
                var foundContact = _contactService.Get(request.ID);
                _contactService.UpdateInfo(request.ToContactModel());
                _contactService.SaveChanges();
                response.ContactUpdated = true;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetOpportunityContact")]
        [ResponseType(typeof(ContactViewModel))]
        public IHttpActionResult GetOpportunityContact(int opportunityID = 0)
        {
            if(opportunityID == 0)
            {
                return BadRequest();
            }
            var foundContact = _contactService.GetContactByOpportunity(opportunityID);
            if(foundContact != null)
            {
                _uploadNamingService.ConcatContactAvatar(foundContact);
                return Ok(new ContactViewModel(foundContact));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("GetActivityContact")]
        [ResponseType(typeof(ContactViewModel))]
        public IHttpActionResult GetActivityContact(int activityID = 0)
        {
            if (activityID == 0)
            {
                return BadRequest();
            }
            var foundContact = _contactService.GetContactByActivity(activityID);
            if (foundContact != null)
            {
                _uploadNamingService.ConcatContactAvatar(foundContact);
                return Ok(new ContactViewModel(foundContact));
            }
            else
            {
                return NotFound();
            }
        }

        

        [Route("GetIssueContact")]
        [ResponseType(typeof(ContactViewModel))]
        public IHttpActionResult GetIssueContact(int issueID = 0)
        {
            if (issueID == 0)
            {
                return BadRequest();
            }
            var foundContact = _contactService.GetByIssue(issueID);
            if (foundContact != null)
            {
                _uploadNamingService.ConcatContactAvatar(foundContact);
                return Ok(new ContactViewModel(foundContact));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
