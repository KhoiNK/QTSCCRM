using APIProject.GlobalVariables;
using APIProject.Helper;
using APIProject.Model.Models;
using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/marketingresult")]
    public class MarketingResultController : ApiController
    {
        private readonly IMarketingResultService _marketingResultService;
        private readonly IMarketingPlanService _marketingPlanService;
        private readonly IContactService _contactService;
        private readonly ICustomerService _customerService;
        private readonly ICompareService _compareService;
        private readonly IEmailService _emailService;

        public MarketingResultController(IMarketingResultService _marketingResultService,
            IMarketingPlanService _marketingPlanService,
            IContactService _contactService,
            ICustomerService _customerService,
            ICompareService _compareService,
            IEmailService _emailService)
        {
            this._contactService = _contactService;
            this._marketingResultService = _marketingResultService;
            this._marketingPlanService = _marketingPlanService;
            this._customerService = _customerService;
            this._compareService = _compareService;
            this._emailService = _emailService;
        }

        [Route("GetMarketingResults")]
        //[ResponseType(MarketingResultViewModel)]
        public IHttpActionResult GetMarketingResults(int planID = 0)
        {
            if (planID == 0)
            {
                return BadRequest();
            }
            var list = _marketingResultService.GetMarketingResults();

            //check if specific plan
            list = list.Where(c => c.MarketingPlanID == planID);

            var resultList = list.Select(c => new MarketingResultViewModel(c));
            return Ok(resultList);
        }

        [Route("GetMarketingResultDetails")]
        public IHttpActionResult GetMarketingResultDetails(int planID = 0)
        {
            if (planID == 0)
            {
                return BadRequest();
            }
            try
            {
                var foundPlan = _marketingPlanService.Get(planID);
                var results = _marketingResultService.GetByPlan(planID);
                if (!results.Any())
                {
                    throw new Exception("Không có dữ liệu nào");
                }
                var facilityRate = results.Select(c => c.FacilityRate).ToList().Average();
                var arrangingRate = results.Select(c => c.ArrangingRate).ToList().Average();
                var servicingRate = results.Select(c => c.ServicingRate).ToList().Average();
                var indicatorRate = results.Select(c => c.IndicatorRate).ToList().Average();
                var otherRate = results.Select(c => c.OthersRate).ToList().Average();

                var invitationCount = results.Where(c => c.IsFromInvitation).Count();
                var mediaCount = results.Where(c => c.IsFromMedia).Count();
                var otherCount = results.Where(c => c.IsFromOthers).Count();
                var friendCount = results.Where(c => c.IsFromFriend).Count();
                var qtscSite = results.Where(c => c.IsFromWebsite).Count();

                var joinMoreCount = results.Where(c => c.IsWantMore).Count();

                var leadGeneratedCount = results.Where(c => c.Status == MarketingResultStatus.BecameNewLead).Count();
                var totalResultCount = results.Count();
                //customer generated count
                return Ok(new
                {
                    FacilityRate = facilityRate,
                    ArrangingRate = arrangingRate,
                    ServicingRate = servicingRate,
                    IndicatorRate = indicatorRate,
                    OtherRate = otherRate,
                    InvitationCount = invitationCount,
                    MediaCount = mediaCount,
                    FriendCount = friendCount,
                    QtscSite = qtscSite,
                    OtherCount = otherCount,
                    JoinMoreCount = joinMoreCount,
                    LeadGeneratedCount = leadGeneratedCount,
                    TotalParticipants = totalResultCount
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("GetMarketingResultParticipants")]
        public IHttpActionResult GetMarketingResultParticipants(int planID = 0)
        {
            if (planID == 0)
            {
                return BadRequest();
            }
            try
            {
                var fountPlan = _marketingPlanService.Get(planID);
                var allCustomers = _customerService.GetAll().ToList();
                var planResults = _marketingResultService.GetByPlan(planID);
                var response = new List<MarketingResultParticipantViewModel>();
                foreach (var result in planResults)
                {
                    var responseItem = new MarketingResultParticipantViewModel();
                    responseItem.ID = result.ID;
                    responseItem.CustomerID = result.CustomerID;
                    responseItem.CustomerName = result.CustomerName;
                    responseItem.CustomerAddress = result.CustomerAddress;
                    responseItem.ContactName = result.ContactName;
                    responseItem.Email = result.Email;
                    responseItem.Phone = result.Phone;
                    responseItem.AverageRate = (result.ArrangingRate +
                                                result.FacilityRate +
                                                result.IndicatorRate +
                                                result.OthersRate +
                                                result.ServicingRate) / 5;
                    responseItem.IsWantMore = result.IsWantMore;
                    responseItem.Notes = result.Notes;
                    responseItem.Status = result.Status;
                    response.Add(responseItem);
                }
                //todo next
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PostMarketingResults")]
        public IHttpActionResult PostMarketingResults(PostMarketingResultViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            //bool requestDone = _marketingResultService.CreateResults(request.ToMarketingResultModels(), request.IsFinished, request.StaffID);
            try
            {
                var foundPlan = _marketingPlanService.Get(request.PlanID);
                var doingPlans = _marketingPlanService.GetDoing().Where(plan => plan.ID != foundPlan.ID);
                var addedResult = _marketingResultService.Add(request.ToResultModel());
                _marketingResultService.SaveChanges();
                var similarCustomers = _compareService.GetSimilarCustomers(new Customer
                {
                    Name = addedResult.CustomerName,
                    Address = addedResult.CustomerAddress
                });
                if (similarCustomers.Any())
                {
                    _marketingResultService.UpdateSimilar(addedResult);
                }
                _emailService.SendThankEmail(addedResult.CustomerName,
                    addedResult.ContactName,
                    addedResult.Email,
                    foundPlan.Title, doingPlans);
                _marketingResultService.SaveChanges();
                return Ok(addedResult.ID);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("PostCustomerAndContact")]
        public IHttpActionResult PostCustomerAndContact(int planResultID = 0)
        {
            if (planResultID == 0)
            {
                return BadRequest();
            }
            try
            {
                var foundResult = _marketingResultService.Get(planResultID);
                if (foundResult.Status == MarketingResultStatus.BecameNewContact ||
                    foundResult.Status == MarketingResultStatus.BecameNewLead)
                {
                    throw new Exception("Không thể tạo mới thêm");
                }
                var newCus = new Customer
                {
                    Name = foundResult.CustomerName,
                    Address = foundResult.CustomerAddress
                };
                var addedCus = _customerService.Add(newCus);
                var newContact = new Contact
                {
                    CustomerID = addedCus.ID,
                    Name = foundResult.ContactName,
                    Email = foundResult.Email,
                    Phone = foundResult.Phone
                };
                var addedContact = _contactService.Add(newContact);
                foundResult.CustomerID = addedCus.ID;
                _marketingResultService.UpdateLeadGenerated(foundResult, addedCus, addedContact);
                _marketingResultService.SaveChanges();
                return Ok(new
                {
                    CustomerID = addedCus.ID,
                    ContactID = addedContact.ID
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}