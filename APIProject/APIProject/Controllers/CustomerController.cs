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
        private readonly IContactService _contactService;
        private readonly IIssueService _issueService;
        private readonly IOpportunityService _opportunityService;
        private readonly IActivityService _activityService;

        public CustomerController(ICustomerService _customerService,
            IIssueService _issueService,
            IActivityService _activityService,
            IContactService _contactService,
            IOpportunityService _opportunityService,
            IUploadNamingService _uploadNamingService)
        {
            this._issueService = _issueService;
            this._activityService = _activityService;
            this._customerService = _customerService;
            this._opportunityService = _opportunityService;
            this._uploadNamingService = _uploadNamingService;
            this._contactService = _contactService;
        }

        [Route("PostLeadCustomer")]
        [ResponseType(typeof(PostLeadCustomerResponseViewModel))]
        public IHttpActionResult PostLeadCustomer(PostLeadCustomerViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var response = new PostLeadCustomerResponseViewModel();
            if (request.Avatar != null)
            {
                String[] splitBase64 = request.Avatar.Base64Content.Split(',');

                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                //string avatarExtension = ".jpg";
                request.Avatar.Name = _uploadNamingService.GetCustomerAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                //saveFileHelper.SaveCustomerImage(request.Avatar.Name, request.Avatar.Base64Content);
                saveFileHelper.SaveCustomerImage(request.Avatar.Name, splitBase64.ToList().Last());
                response.CustomerAvatarUpdated = true;
            }
            try
            {
                if(DateTime.Compare(DateTime.Now,request.EstablishedDate)< 0)
                {
                    throw new Exception("Ngày thành lập không được sau thời gian hiện tại");
                }
                var insertedCustomer = _customerService.Add(request.ToCustomerModel());
                response.CustomerCreated = true;
                response.CustomerID = insertedCustomer.ID;
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PostMarketingCustomerAndContact")]
        public IHttpActionResult PostMarketingCustomerAndContact(PostMarketingCustomerAndContactViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var insertedCustomer = _customerService.Add(request.ToCustomerModel());
            var _contact = request.ToContactModel();
            _contact.CustomerID = insertedCustomer.ID;
            var insertedContact = _contactService.Add(_contact);
            return Ok(new
            {
                CustomerID = insertedCustomer.ID,
                ContactID = insertedContact.ID
            });
        }

        [Route("PutLeadInformation")]
        [ResponseType(typeof(PutCustomerViewModel))]
        public IHttpActionResult PutLeadInformation(PutLeadInformationViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var response = new PutCustomerViewModel();
            if (request.Avatar != null)
            {
                string avatarExtension = Path.GetExtension(request.Avatar.Name).ToLower();
                request.Avatar.Name = _uploadNamingService.GetCustomerAvatarNaming() + avatarExtension;
                SaveFileHelper saveFileHelper = new SaveFileHelper();
                saveFileHelper.SaveCustomerImage(request.Avatar.Name, request.Avatar.Base64Content);
                response.CustomerImageUpdated = true;
            }
            try
            {
                var foundCustomer = _customerService.Get(request.CustomerID);
                _customerService.UpdateInfo(request.ToCustomerModel());
                _customerService.SaveChanges();
                response.CustomerUpdated = true;
                return Ok(response);
            }
            catch (Exception exceptionFromService)
            {
                return BadRequest(exceptionFromService.Message);
            }
        }

        [Route("PutCustomerInformation")]
        [ResponseType(typeof(PutCustomerViewModel))]
        public IHttpActionResult PutCustomerInformation(PutCustomerInformationViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            var response = new PutCustomerViewModel();
            try
            {
                var foundCustomer = _customerService.Get(request.CustomerID);
                _customerService.UpdateType(request.ToCustomerModel());
                response.CustomerUpdated = true;
                _customerService.SaveChanges();
                return Ok(response);

            }
            catch (Exception exceptionFromService)
            {
                return Content(HttpStatusCode.BadRequest, exceptionFromService.Message);
            }
        }

        [Route("GetCustomerList")]
        [ResponseType(typeof(CustomerViewModel))]
        public IHttpActionResult GetCustomerList(int page = 1, int pageSize = 100)
        {

            var customers = _customerService.GetAll().Skip(pageSize * (page - 1)).Take(pageSize)
                .ToList();
            customers.ForEach(c => _uploadNamingService.ConcatCustomerAvatar(c));
            return Ok(customers.Select(c => new CustomerViewModel(c)));
        }

        [Route("GetOfficialCustomers")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetOfficialCustomers()
        {
            var entities = _customerService.GetOfficial().ToList();
            entities.ForEach(c => _uploadNamingService.ConcatCustomerAvatar(c));
            return Ok(entities.Select(c => new CustomerDetailViewModel(c)));
        }

        [Route("GetSimilarCustomers")]
        [ResponseType(typeof(CustomerDetailViewModel))]
        public IHttpActionResult GetSimilarCustomers(string customerName, string customerAddress)
        {
            CompareHelper compareHelper = new CompareHelper();
            var compareResult = compareHelper.StringCompare(customerAddress, customerName);
            var similarCustomers = _customerService.GetAll().Where(c => compareHelper.StringCompare(customerName, c.Name) >= 75
            && compareHelper.StringCompare(customerAddress, c.Address) >= 75).ToList();
            similarCustomers.ForEach(c => _uploadNamingService.ConcatCustomerAvatar(c));
            var result = similarCustomers.Select(c => new CustomerDetailViewModel(c));
            return Ok(result);
        }



        [Route("GetCustomerDetails")]
        [ResponseType(typeof(CustomerDetailsViewModel))]
        public IHttpActionResult GetCustomerDetails(int ID = 0)
        {
            if (ID == 0)
            {
                return BadRequest();
            }
            try
            {
                var foundCustomer = _customerService.Get(ID);
                _uploadNamingService.ConcatCustomerAvatar(foundCustomer);
                var customerContacts = _contactService.GetByCustomer(ID).ToList();
                customerContacts.ForEach(c => _uploadNamingService.ConcatContactAvatar(c));
                var customerIssues = _issueService.GetByCustomer(ID).ToList();
                var customerOppors = _opportunityService.GetByCustomer(ID).ToList();
                var customerActivities = _activityService.GetByCustomer(ID).ToList();
                var response = new CustomerDetailsViewModel(
                    foundCustomer,
                    customerContacts,
                    customerIssues,
                    customerOppors,
                    customerActivities);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

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
            var foundCustomer = _customerService.GetAll().Where(c => c.ID == ID).SingleOrDefault();
            if (foundCustomer != null)
            {
                _uploadNamingService.ConcatCustomerAvatar(foundCustomer);
                return Ok(new CustomerDetailViewModel(foundCustomer));
            }
            return NotFound();
        }

    }
}
