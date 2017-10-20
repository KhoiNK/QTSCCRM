using APIProject.Helper;
using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/contact")]
    public class ContactController : ApiController
    {
        private readonly IContactService _contactService;
        private readonly IUploadNamingService _uploadNamingService;

        public ContactController(IContactService _contactService,
            IUploadNamingService _uploadNamingService)
        {
            this._contactService = _contactService;
            this._uploadNamingService = _uploadNamingService;
        }

        [Route("PostNewContact")]
        public IHttpActionResult PostNewContact(PostContactViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            if(request.Avatar != null)
            {
                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                request.Avatar.Name = _uploadNamingService.GetContactAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                saveFileHelper.SaveContactImage(request.Avatar.Name, request.Avatar.Base64Content);
            }
            int insertedContactID =  _contactService.CreateContact(request.ToContactModel());
            return Ok(insertedContactID);
        }

        [Route("PutContactInformation")]
        public IHttpActionResult PutContactInformation(PutContactInformationViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            if(request.Avatar != null)
            {
                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                request.Avatar.Name = _uploadNamingService.GetContactAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                saveFileHelper.SaveContactImage(request.Avatar.Name, request.Avatar.Base64Content);

            }

            bool isEdited = _contactService.EditContact(request.ToContactModel());

            return Ok(isEdited);
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

        [Route("GetCustomerContacts")]
        [ResponseType(typeof(ContactViewModel))]
        public IHttpActionResult GetCustomerContacts(int customerID = 0)
        {
            if(customerID == 0)
            {
                return BadRequest();
            }
            var foundContacts = _contactService.GetByCustomer(customerID);
            if (foundContacts != null)
            {
                foreach (var contact in foundContacts)
                {
                    _uploadNamingService.ConcatContactAvatar(contact);
                }
                return Ok(foundContacts.Select(c => new ContactViewModel(c)));
            }
            return NotFound();
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
