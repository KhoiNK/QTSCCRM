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
            try
            {
                return Ok(_customerService.EditLead(request.ToCustomerModel()));
            }
            catch (Exception exceptionFromService)
            {
                return BadRequest(exceptionFromService.Message);
            }
        }

        [Route("PutCustomerInformation")]
        public IHttpActionResult PutCustomerInformation(PutCustomerInformationViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _customerService.EditCustomer(request.ToCustomerModel());
                return Ok("Updated");

            }
            catch (Exception exceptionFromService)
            {
                return Content(HttpStatusCode.BadRequest, exceptionFromService.Message);
            }
        }

        [Route("GetCustomerList")]
        [ResponseType(typeof(CustomerViewModel))]
        public IHttpActionResult GetCustomerList()
        {

            var customers = _customerService.GetCustomerList();
            if (customers.Any())
            {
                foreach (var customer in customers)
                {
                    _uploadNamingService.ConcatCustomerAvatar(customer);
                }
                return Ok(customers.Select(c => new CustomerViewModel(c)));
            }
            return NotFound();

            //var customers = _customerService.GetAll().Where(c=>c.IsDelete==false);
            //if (onlyCustomer.Value)
            //{
            //    customers = customers.Where(c => c.CustomerType != CustomerType.Lead);
            //}
            // //results.ToList().ForEach(c => _uploadNamingService.ConcatCustomerAvatar(c));
            // customers.ToList()
            //return Ok()
        }

        [Route("GetOfficialCustomers")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetOfficialCustomers()
        {
            var entities = _customerService.GetOfficial().ToList();
            entities.ForEach(c => _uploadNamingService.ConcatCustomerAvatar(c));
            return Ok(entities.Select(c => new CustomerDetailViewModel(c)));
        }

        [Route("GetCustomerDetails")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetCustomerDetails(int ID)
        {
            var foundCustomer = _customerService.GetCustomerList().Where(c => c.ID == ID).SingleOrDefault();
            if (foundCustomer != null)
            {
                _uploadNamingService.ConcatCustomerAvatar(foundCustomer);
                foreach(var contact in foundCustomer.Contacts)
                {
                    _uploadNamingService.ConcatContactAvatar(contact);
                }
                return Ok(new CustomerDetailsViewModel(foundCustomer));
            }
            return NotFound();
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
                _uploadNamingService.ConcatCustomerAvatar(foundCustomer);
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
                _uploadNamingService.ConcatCustomerAvatar(foundCustomer);
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
                _uploadNamingService.ConcatCustomerAvatar(foundCustomer);
                return Ok(new CustomerDetailViewModel(foundCustomer));
            }
            else
            {
                return NotFound();
            }
        }


        [NonAction]
        [Route("GetCustomerDetail")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetCustomerDetail(int ID)
        {
            var foundCustomer = _customerService.GetCustomerList().Where(c => c.ID == ID).SingleOrDefault();
            if (foundCustomer != null)
            {
                _uploadNamingService.ConcatCustomerAvatar(foundCustomer);
                return Ok(new CustomerDetailViewModel(foundCustomer));
            }
            return NotFound();
        }

    }
}
