using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APIProject.Controllers
{
    [RoutePrefix("api/contact")]
    public class ContactController : ApiController
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService _contactService)
        {
            this._contactService = _contactService;
        }

        [Route("PostNewContact")]
        public IHttpActionResult PostNewContact(PostContactViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            string avatarName = null;
            string avatarB64 = null;
            if(request.Avatar != null)
            {
                avatarName = request.Avatar.Name;
                avatarB64 = request.Avatar.Base64Content;
            }
            int insertedContactID =  _contactService.CreateContact(request.ToContactModel(), avatarName, avatarB64);
            return Ok(insertedContactID);
        }

        [Route("PutContactInformation")]
        public IHttpActionResult PutContactInformation(PutContactInformationViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            string avatarName = null;
            string avatarB64 = null;
            if(request.Avatar != null)
            {
                avatarName = request.Avatar.Name;
                avatarB64 = request.Avatar.Base64Content;
            }

            bool isEdited = _contactService.EditContact(request.ToContactModel(), avatarName, avatarB64);

            return Ok(isEdited);
        }
    }
}
