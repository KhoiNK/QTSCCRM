using APIProject.GlobalVariables;
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
    [RoutePrefix("api/customer")]
    public class CustomerController : ApiController
    {
        private readonly ICustomerService _customerService;
        private readonly IUploadNamingService _uploadNamingService;

        public CustomerController(ICustomerService _customerService, IUploadNamingService _uploadNamingService)
        {
            this._customerService = _customerService;
            this._uploadNamingService = _uploadNamingService;
        }

        [Route("PostLeadCustomer")]
        public IHttpActionResult PostLeadCustomer(PostLeadCustomerViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            if (request.Avatar != null)
            {
                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                request.Avatar.Name = _uploadNamingService.GetCustomerAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                saveFileHelper.SaveCustomerImage(request.Avatar.Name, request.Avatar.Base64Content);

            }

            return Ok(_customerService.CreateNewLead(request.ToCustomerModel()));
        }

        [Route("PutLeadInformation")]
        public IHttpActionResult PutLeadInformation(PutLeadInformationViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            if (request.Avatar != null)
            {
                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                request.Avatar.Name = _uploadNamingService.GetCustomerAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                saveFileHelper.SaveCustomerImage(request.Avatar.Name, request.Avatar.Base64Content);
            }

            return Ok(_customerService.EditLead(request.ToCustomerModel()));
        }

        [Route("PutCustomerInformation")]
        public IHttpActionResult PutCustomerInformation(PutCustomerInformationViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(_customerService.EditCustomer(request.ToCustomerModel()));
        }

        [Route("GetCustomerList")]
        [ResponseType(typeof(CustomerViewModel))]
        public IHttpActionResult GetCustomerList()
        {
            return Ok(_customerService.GetCustomerList().Select(x => new CustomerViewModel(x)));
        }
        [Route("GetCustomerDetail")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetCustomerDetail(int ID)
        {
            return Ok(_customerService.GetCustomerList().Where(x => x.ID == ID).Select(x => new CustomerDetailViewModel(x)));
        }

        [Route("GetOpportunityCustomer")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetOpportunityCustomer(int opportunityID = 0)
        {
            if (opportunityID == 0)
            {
                return BadRequest();
            }
            var foundCustomer = _customerService.GetByOpportunity(opportunityID);
            if (foundCustomer != null)
            {
                return Ok(new CustomerDetailViewModel(foundCustomer));
            }
            else
            {
                return NotFound();
            }
        }

        [Route("GetActivityCustomer")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetActivityCustomer(int activityID = 0)
        {
            if (activityID == 0)
            {
                return BadRequest();
            }

            var foundCustomer = _customerService.GetByActivity(activityID);
            if (foundCustomer != null)
            {
                return Ok(new CustomerDetailViewModel(foundCustomer));
            }
            else
            {
                return NotFound();
            }

        }

        [Route("GetCustomerTypes")]
        public IHttpActionResult GetCustomerTypes()
        {
            return Ok(_customerService.GetCustomerTypes());
        }
        [Route("GetIssueCustomer")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetIssueCustomer(int issueID = 0)
        {
            if (issueID == 0)
            {
                return BadRequest();
            }

            var foundCustomer = _customerService.GetByIssue(issueID);
            if (foundCustomer != null)
            {
                return Ok(new CustomerDetailViewModel(foundCustomer));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
