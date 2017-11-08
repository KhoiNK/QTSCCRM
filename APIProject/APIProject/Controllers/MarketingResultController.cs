using APIProject.GlobalVariables;
using APIProject.Helper;
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
        private readonly ICustomerService _customerService;
        private readonly ICompareService _compareService;

        public MarketingResultController(IMarketingResultService _marketingResultService,
            IMarketingPlanService _marketingPlanService,
            ICustomerService _customerService,
            ICompareService _compareService)
        {
            this._marketingResultService = _marketingResultService;
            this._marketingPlanService = _marketingPlanService;
            this._customerService = _customerService;
            this._compareService = _compareService;

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

                var leadGeneratedCount = results.Where(c => c.IsLeadGenerated).Count();
                //customer generated count
                return Ok(new
                {
                    FacilityRate= facilityRate,
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
                    LeadGeneratedCount = leadGeneratedCount
                });
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);

            }
        }

        [Route("GetMarketingResultParticipants")]
        public IHttpActionResult GetMarketingResultParticipants(int planID= 0)
        {
            if(planID == 0)
            {
                return BadRequest();
            }
            try
            {
                var fountPlan = _marketingPlanService.Get(planID);
                var allCustomers = _customerService.GetAll().ToList();
                var planResults = _marketingResultService.GetByPlan(planID);
                var response = new List<MarketingResultParticipantViewModel>();
                foreach(var result in planResults)
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
                    responseItem.Status = MarketingResultStatus.New;
                    if (responseItem.CustomerID.HasValue)
                    {
                        responseItem.Status = MarketingResultStatus.BecameNewLead;
                    }
                    else
                    {
                        foreach (var customer in allCustomers)
                        {
                            if (_compareService.StringCompare(result.CustomerName, customer.Name) >= 60)
                            {
                                if (_compareService.StringCompare(result.CustomerAddress, customer.Address) >= 60)
                                {
                                    responseItem.Status = MarketingResultStatus.HasSimilar;
                                    break;
                                }
                            }
                        }
                    }
                    response.Add(responseItem);
                }
                //todo next
                return Ok(response);

            }catch(Exception e)
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
                var addedResult = _marketingResultService.Add(request.ToResultModel());
                _marketingResultService.SaveChanges();
                return Ok(addedResult.ID);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
