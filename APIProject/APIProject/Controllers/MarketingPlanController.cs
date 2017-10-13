using APIProject.Service;
using APIProject.ViewModels;
using System;
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

namespace APIProject.Controllers
{
    //[Authorize]
    [RoutePrefix("api/marketingplan")]
    public class MarketingPlanController : ApiController
    {
        private readonly IMarketingPlanService _marketingPlanService;

        public MarketingPlanController(IMarketingPlanService _marketingPlanService)
        {
            this._marketingPlanService = _marketingPlanService;
        }

        //[Authorize(Roles = "Admin,Employee")]
        [Route("GetMarketingPlanList")]
        [ResponseType(typeof(MarketingPlanViewModel))]
        public async Task<IHttpActionResult> GetMarketingPlanList()
        {
            var task = Task.Factory.StartNew(() => {
                var list = _marketingPlanService.GetMarketingPlans().Select(c => new MarketingPlanViewModel(c));
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
                if (plan != null) {
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //test code here
            string fileRoot = HttpContext.Current.Server.MapPath("~/MarketingPlanFiles");
            string budgetFileSrc = null;
            string scheduleFileSrc = null;
            string taskAssignFileSrc = null;
            string licenseFileSrc = null;
            if (request.BudgetFile.HasValue)
            {
                budgetFileSrc = fileRoot + $@"\\{request.BudgetFile.Value.Name}";
                File.WriteAllBytes(budgetFileSrc, Convert.FromBase64String(request.BudgetFile.Value.Base64Content));

            }
            if (request.EventScheduleFile.HasValue)
            {
                scheduleFileSrc = fileRoot + $@"\\{request.EventScheduleFile.Value.Name}";
                File.WriteAllBytes(budgetFileSrc, Convert.FromBase64String(request.EventScheduleFile.Value.Base64Content));

            }
            if (request.TaskAssignFile.HasValue)
            {
                taskAssignFileSrc = fileRoot + $@"\\{request.TaskAssignFile.Value.Name}";
                File.WriteAllBytes(budgetFileSrc, Convert.FromBase64String(request.TaskAssignFile.Value.Base64Content));

            }
            if (request.LicenseFile.HasValue)
            {
                licenseFileSrc = fileRoot + $@"\\{request.LicenseFile.Value.Name}";
                File.WriteAllBytes(budgetFileSrc, Convert.FromBase64String(request.LicenseFile.Value.Base64Content));

            }
            int requestID = _marketingPlanService.CreateNewPlan(request.ToMarketingPlanModel(fileRoot), request.IsFinished);

            return Ok(requestID);
        }

        [Route("PutMarketingFiles")]
        public async Task<HttpResponseMessage> PutFormData()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/MarketingPlanFiles");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                
                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                    
                }


                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("PutDraftingMarketingPlan")]
        public IHttpActionResult PutDraftingMarketingPlan([FromBody]PutDraftingMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_marketingPlanService.UpdatePlan(request.ToMarketingPlanModel(), request.IsFinished));

        }

        [Route("PutValidateMarketingPlan")]
        public IHttpActionResult PutValidateMarketingPlan([FromBody]PutValidateMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(_marketingPlanService.ValidatePlan(request.ToMarketingPlanModel(), request.Validate));
        }

        [Route("PutAcceptMarketingPlan")]
        public IHttpActionResult PutAcceptMarketingPlan([FromBody]PutAcceptMarketingPlanViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_marketingPlanService.AcceptPlan(request.ToMarketingPlanModel(), request.Accept));
        }

        [Route("DeleteMarketingPlan")]
        public IHttpActionResult DeleteMarketingPlan()
        {
            return Ok();
        }

    }
}
