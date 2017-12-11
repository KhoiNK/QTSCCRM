using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using APIProject.Model.Models;
using APIProject.GlobalVariables;

namespace APIProject.Controllers
{
    //[Authorize]
    [RoutePrefix("api/marketingplan")]
    public class MarketingPlanController : ApiController
    {
        private readonly IMarketingPlanService _marketingPlanService;
        private readonly IStaffService _staffService;
        private readonly IEmailService _emailService;
        private readonly IContactService _contactService;
        private readonly ICustomerService _customerService;

        public MarketingPlanController(IMarketingPlanService _marketingPlanService,
            IStaffService _staffService,
            IEmailService _emailService,
            IContactService _contactService,
            ICustomerService _customerService)
        {
            this._staffService = _staffService;
            this._marketingPlanService = _marketingPlanService;
            this._emailService = _emailService;
            this._contactService = _contactService;
            this._customerService = _customerService;
        }

        //[Authorize(Roles = "Admin,Employee")]
        [Route("GetMarketingPlanList")]
        [ResponseType(typeof(MarketingPlanViewModel))]
        public async Task<IHttpActionResult> GetMarketingPlanList(int page=1, int pageSize =100)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var list = _marketingPlanService.GetAll().Skip(pageSize * (page - 1)).Take(pageSize)
                .Select(c => new MarketingPlanViewModel(c));
                list.Reverse();
                return list;
            });
            return Ok(await task);
        }

        [Route("GetMarketingPlan")]
        [ResponseType(typeof(MarketingPlanDetailViewModel))]
        public IHttpActionResult GetMarketingPlan(int? id)
        {
            if (id.HasValue)
            {
                var plan = _marketingPlanService.GetMarketingPlan(id.Value);
                if (plan != null)
                {
                    MarketingPlanDetailViewModel item = new MarketingPlanDetailViewModel(plan);
                    return Ok(item);
                }
                else
                {
                    return NotFound();
                }
            }
            return BadRequest();
        }

        [Route("PostMarketingPlan")]
        public IHttpActionResult PostMarketingPlan([FromBody]PostMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            var addedPlan = _marketingPlanService.Add(request.ToMarketingPlanModel());
            _marketingPlanService.SaveChanges();
            var customers = _customerService.GetAll().Where(cus => !cus.IsDelete);
            List<Contact> contacts = new List<Contact>();
            foreach (Customer customer in customers)
            {
                var customerContacts = _contactService.GetByCustomer(customer.ID).Where(contact => !contact.IsDelete);
                foreach (Contact contact in customerContacts)
                {
                    if (!contacts.Exists(inlistContact => inlistContact.Email == contact.Email))
                    {
                        contacts.Add(contact);
                    }
                }
            }
            //_emailService.SendNewMarketingPlan(contacts,addedPlan);
            //insert plan and get plan id
            return Ok(new { PlanID = addedPlan.ID });
        }
        [Route("PostEditMarketingPlan")]
        public IHttpActionResult PostEditMarketingPlan(PutEditMarketingPlanViewModel request)
        {
            if(!ModelState.IsValid||request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if(DateTime.Compare(request.StartDate.Date,request.EndDate.Date) > 0)
                {
                    throw new Exception("Ngày bắt đầu không được quá ngày kết thúc");
                }
                var foundPlan = _marketingPlanService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                _marketingPlanService.UpdateInfo(request.ToMarketingPlanModel());
                _marketingPlanService.SaveChanges();
                return Ok(new { MarketingPlanUpdated = true });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        
        [Route("PostFinishMarketingPlan")]
        public IHttpActionResult PostFinishMarketingPlan(PutFinishMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var foundPlan = _marketingPlanService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                _marketingPlanService.Finish(request.ToMarketingPlanModel());
                _marketingPlanService.SaveChanges();
                return Ok(new { MarketingPlanUpdated = true });
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        //[Route("PutDraftingMarketingPlan")]
        //public IHttpActionResult PutDraftingMarketingPlan([FromBody]PutDraftingMarketingPlanViewModel request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    string budgetB64 = null;
        //    string eventB64 = null;
        //    string taskB64 = null;
        //    string licenseB64 = null;
        //    if (request.BudgetFile != null)
        //    {
        //        budgetB64 = request.BudgetFile.Base64Content;
        //    }
        //    if (request.EventScheduleFile != null)
        //    {
        //        eventB64 = request.EventScheduleFile.Base64Content;
        //    }
        //    if (request.TaskAssignFile != null)
        //    {
        //        taskB64 = request.TaskAssignFile.Base64Content;
        //    }
        //    if (request.LicenseFile != null)
        //    {
        //        licenseB64 = request.LicenseFile.Base64Content;
        //    }

        //    return Ok(_marketingPlanService.UpdatePlan(request.ToMarketingPlanModel(), request.IsFinished,
        //        budgetB64, taskB64, eventB64, licenseB64));

        //}

        //[Route("PutValidateMarketingPlan")]
        //public IHttpActionResult PutValidateMarketingPlan([FromBody]PutValidateMarketingPlanViewModel request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(_marketingPlanService.ValidatePlan(request.ToMarketingPlanModel(), request.Validate));
        //}

        //[Route("PutAcceptMarketingPlan")]
        //public IHttpActionResult PutAcceptMarketingPlan([FromBody]PutAcceptMarketingPlanViewModel request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    return Ok(_marketingPlanService.AcceptPlan(request.ToMarketingPlanModel(), request.Accept));
        //}
        [Route("GetMarketingStatus")]
        public IHttpActionResult GetMarketingStatus()
        {
            return Ok(new List<string>
            {
                MarketingStatus.Executing,
                MarketingStatus.Reporting,
                MarketingStatus.Finished
            });
        }

        [Route("DeleteMarketingPlan")]
        public IHttpActionResult DeleteMarketingPlan()
        {
            return Ok();
        }

        //[Route("PostMarketingResult")]
        //public async Task<IHttpActionResult> PostMarketingResultAsync([FromUri] int marketingID)
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        Request.CreateResponse(HttpStatusCode.UnsupportedMediaType, "Sai định dạng");
        //    }
        //    var fileRoot = HttpContext.Current.Server.MapPath("~/Resources/MarketingResultFiles");
        //    if (!Directory.Exists(fileRoot))
        //    {
        //        Directory.CreateDirectory(fileRoot);
        //    }

        //    //var provider = new MultipartFormDataContent(fileRoot);
        //    //var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();

        //    var provider = new MultipartFormDataStreamProvider(fileRoot);
        //    var result = await Request.Content.ReadAsMultipartAsync(provider);

        //    //foreach (var stream in filesReadToProvider.Contents)
        //    //{
        //    //    var fileBytes = await stream.ReadAsByteArrayAsync();
        //    //}

        //    var foundMarketing = _marketingPlanService.Get(marketingID);

        //    //Upload files
        //    int quotationCount = 0;
        //    int quotationItemCount = 0;

        //    foreach (MultipartFileData fileData in result.FileData)
        //    {
        //        if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
        //        {
        //            return BadRequest("Không đúng định dạng");
        //        }
        //        string fileName = fileData.Headers.ContentDisposition.FileName;

        //        if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
        //        {
        //            fileName = fileName.Trim('"');
        //        }
        //        if (fileName.Contains(@"/") || fileName.Contains(@"\"))
        //        {
        //            fileName = Path.GetFileName(fileName);
        //        }
        //        // Must be match with FileNameRFQ in PR
        //        var fullPath = Path.Combine(fileRoot, fileName);
        //        //File.Copy(fileData.LocalFileName, fullPath, true);
        //        var quotation = this.ReadQuotationFromExcel(fileData.LocalFileName, requisition, out quotationItemCount);

        //    }
        //}
    }
}
