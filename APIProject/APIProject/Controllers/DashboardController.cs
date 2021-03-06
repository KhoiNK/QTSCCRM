﻿using APIProject.GlobalVariables;
using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/dashbboard")]
    public class DashboardController : ApiController
    {
        private readonly IOpportunityService _opportunityService;
        private readonly ICustomerService _customerService;
        private readonly IMarketingResultService _marketingResultService;
        private readonly IMarketingPlanService _marketingPlanService;
        private readonly IContractService _contractService;
        private readonly ISalesCategoryService _salesCategoryService;
        private readonly IActivityService _activityService;
        private readonly IIssueService _issueService;
        private readonly IIssueCategoryMappingService _issueCategoryMappingService;
        public DashboardController(ICustomerService _customerService,
            IActivityService _activityService,
            IContractService _contractService,
            ISalesCategoryService _salesCategoryService,
            IOpportunityService _opportunityService,
            IMarketingResultService _marketingResultService,
            IMarketingPlanService _marketingPlanService,
            IIssueCategoryMappingService _issueCategoryMappingService,
            IIssueService _issueService)
        {
            this._salesCategoryService = _salesCategoryService;
            this._activityService = _activityService;
            this._marketingResultService = _marketingResultService;
            this._marketingPlanService = _marketingPlanService;
            this._contractService = _contractService;
            this._opportunityService = _opportunityService;
            this._customerService = _customerService;
            this._issueService = _issueService;
            this._issueCategoryMappingService = _issueCategoryMappingService;
        }

        [Route("GetDashboard")]
        [ResponseType(typeof(DashboardViewModel))]
        public IHttpActionResult GetDashboard(int monthRange = 12,
            int month = 0)
        {

            #region background loading
            //business logic variables
            int contractRemindDays = 10;

            //cập nhật tình trạng gia hạn sử dụng dịch vụ
            _contractService.BackgroundUpdateStatus(contractRemindDays);

            //cập nhật trạng thái khiếu nại
            _issueService.BackgroundUpdateStatus();

            //cập nhật trình trạng lịch hẹn
            _activityService.BackgroundUpdateStatus();
            //commit all background
            _contractService.SaveChanges();
            #endregion

            month = (month == 0) ? DateTime.Now.Month : month;

            int considerOppCount = _opportunityService.CountConsider();
            int makeQuoteOppCount = _opportunityService.CountMakeQuote();
            int validateOppCount = _opportunityService.CountValidateQuote();
            int sendQuoteOppCount = _opportunityService.CountSendQuote();
            int wonOppCount = _opportunityService.CountWon();
            int lostOppCount = _opportunityService.CountLost();

            double considerDuration = _opportunityService.AverageConsider();
            double makeQuoteDuration = _opportunityService.AverageMakeQuote();
            double validateQuoteDuration = _opportunityService.AverageValidateQuote();
            double sendQuoteDuration = _opportunityService.AverageSendQuote();
            double negotiationDuration = _opportunityService.AverageNegotiation();

            var OppCreatedRates = _opportunityService.GetCreatedRates(monthRange);

            int contractCount = _contractService.GetAll().Count();
            int contractNeedActionCount = _contractService.GetNeedAction().Count();
            List<int> contractUsingYears = new List<int>
            {
                1,2,5
            };
            var AllContractUsingRates = _contractService.GetAllUsingRates(contractUsingYears);
            var allCategories = _salesCategoryService.GetAll();
            var individualUsingRatesList = new List<Dictionary<string, int>>();
            foreach (var category in allCategories)
            {
                individualUsingRatesList.Add(_contractService.GetUsingRates(category, contractUsingYears));
            }
            var contractUsingRatesList = new List<UsingRateCharts>();
            for (int i = 0; i < allCategories.Count(); i++)
            {
                contractUsingRatesList.Add(new UsingRateCharts
                {
                    CategoryID = allCategories.ElementAt(i).ID,
                    Chart = new ChartViewModel
                    {
                        Labels = individualUsingRatesList.ElementAt(i).Keys.ToList(),
                        Values = individualUsingRatesList.ElementAt(i).Values.ToList()
                    }
                });

            }
            int customerCount = _customerService.GetOfficial().Count();
            int leadCount = _customerService.GetLead().Count();

            //List<string> xLabels = new List<string>();
            //DateTime startTime = DateTime.Now.AddMonths(-(monthRange-1));
            //for (int i = 1; i <= monthRange; i++)
            //{
            //    xLabels.Add(startTime.Month + "/" + startTime.Year);
            //    startTime.AddMonths(1);
            //}
            Dictionary<string, int> customerRates = _customerService.GetCustomerRates(monthRange);
            Dictionary<string, int> leadRates = _customerService.GetLeadRates(monthRange);

            var doingIssueCount = _issueService.GetDoing().Count();
            var failedIssueCount = _issueService.GetFailed().Count();
            var doneIssueCount = _issueService.GetDone().Count();

            Dictionary<string, int> issueRates = _issueService.GetRates(monthRange);
            Dictionary<string, int> issueCategoryCount = _issueCategoryMappingService.GetCounts(monthRange);

            var doingMarketings = _marketingPlanService.GetDoing().Count();
            Dictionary<string, int> marketingRates = _marketingPlanService.GetRates(monthRange);
            Dictionary<string, int> marketingLeadCount = _marketingResultService.GetLeadRates(monthRange);
            Dictionary<string, int> LeadSourceCount = _marketingResultService.GetSourceRates(monthRange);
            var response = new DashboardViewModel
            {
                Opportunity = new DashboardOpportunity
                {
                    ConsiderCount = considerOppCount,
                    MakeQuoteCount = makeQuoteOppCount,
                    ValidateCount = validateOppCount,
                    SendCount = sendQuoteOppCount,
                },
                Activity = new DashboardActivity
                {
                    //todo next 
                },
                Contract = new DashboardContract
                {
                    TotalCount = contractCount,
                    NeedActionCount = contractNeedActionCount,
                    AllUsingRates = new ChartViewModel
                    {
                        Labels = AllContractUsingRates.Keys.ToList(),
                        Values = AllContractUsingRates.Values.ToList()
                    },
                    UsingRatesList = contractUsingRatesList
                },
                Customer = new DashboardCustomer
                {
                    CustomerCount = customerCount,
                    LeadCount = leadCount,
                },
                Issue = new DashboardIssue
                {
                    DoingIssuesCount = doingIssueCount,
                },
                Marketing = new DashboardMarketing
                {
                    ExecutingCount = doingMarketings,
                    Rates = new ChartViewModel
                    {
                        Labels = marketingRates.Keys.ToList(),
                        Values = marketingRates.Values.ToList()
                    },
                    LeadCountChart = new ChartViewModel
                    {
                        Labels = marketingLeadCount.Keys.ToList(),
                        Values = marketingLeadCount.Values.ToList(),
                    },
                    LeadSourceCountChart = new ChartViewModel
                    {
                        Labels = LeadSourceCount.Keys.ToList(),
                        Values = LeadSourceCount.Values.ToList()
                    }
                }
            };
            return Ok(response);

        }

        [Route("GetOpportunityDashboard")]
        [ResponseType(typeof(DashboardOpportunity))]
        public IHttpActionResult GetOpportunityDashboard(int monthRange = 12,
            int month = 0)
        {
            int considerOppCount = _opportunityService.CountConsider();
            int makeQuoteOppCount = _opportunityService.CountMakeQuote();
            int validateOppCount = _opportunityService.CountValidateQuote();
            int sendQuoteOppCount = _opportunityService.CountSendQuote();
            int negotiateOppCount = _opportunityService.CountNegotiation();
            int wonOppCount = _opportunityService.CountWon();
            int lostOppCount = _opportunityService.CountLost();

            double considerDuration = _opportunityService.AverageConsider();
            double makeQuoteDuration = _opportunityService.AverageMakeQuote();
            double validateQuoteDuration = _opportunityService.AverageValidateQuote();
            double sendQuoteDuration = _opportunityService.AverageSendQuote();
            double negotiationDuration = _opportunityService.AverageNegotiation();

            var OppCreatedRates = _opportunityService.GetCreatedRates(monthRange);

            var response = new DashboardOpportunity
            {
                ConsiderCount = considerOppCount,
                MakeQuoteCount = makeQuoteOppCount,
                ValidateCount = validateOppCount,
                SendCount = sendQuoteOppCount,
                NegotiateCount = negotiateOppCount,
                WonLostRate = new ChartViewModel
                {
                    Labels = new List<string>
                    {
                        OpportunityStatus.Won,
                        OpportunityStatus.Lost
                    },
                    Values = new List<int>
                    {
                        wonOppCount,
                        lostOppCount
                    }
                },
                StageDurationRates = new DoubleTypeChartViewModel
                {
                    Labels = new List<string>
                    {
                        OpportunityStage.Consider,
                        OpportunityStage.MakeQuote,
                        OpportunityStage.ValidateQuote,
                        OpportunityStage.SendQuote,
                        OpportunityStage.Negotiation
                    },
                    Values = new List<double>
                    {
                        Math.Ceiling(considerDuration),
                        Math.Ceiling(makeQuoteDuration),
                        Math.Ceiling(validateQuoteDuration),
                        Math.Ceiling(sendQuoteDuration),
                        Math.Ceiling(negotiationDuration)
                    }
                },
                CreateRates = new ChartViewModel
                {
                    Labels = OppCreatedRates.Keys.ToList(),
                    Values = OppCreatedRates.Values.ToList()
                }
            };
            return Ok(response);
        }

        [Route("GetCustomerDashboard")]
        [ResponseType(typeof(DashboardCustomer))]
        public IHttpActionResult GetCustomerDashboard(int monthRange = 12)
        {
            var customerCount = _customerService.GetOfficial().Count();
            var leadCount = _customerService.GetLead().Count();
            var customerConvertRates = _customerService.GetConvertRates(monthRange);
            var response = new DashboardCustomer
            {
                CustomerCount = customerCount,
                LeadCount = leadCount,
                ConvertRates = new ChartViewModel
                {
                    Labels = customerConvertRates.Keys.ToList(),
                    Values = customerConvertRates.Values.ToList()
                }
            };
            return Ok(response);
        }

        [Route("GetActivityDashboard")]
        [ResponseType(typeof(DashboardActivity))]
        public IHttpActionResult GetActivityDashboard(int monthRange = 12)
        {
            _activityService.BackgroundUpdateStatus();
            _activityService.SaveChanges();

            var overdueActivities = _activityService.GetOverdue();
            var todayActivities = _activityService.GetByDate(DateTime.Now).Where(c=>c.Status==ActivityStatus.Open||c.Status==ActivityStatus.Overdue);
            var futureActivities = _activityService.GetFuture();
            var response = new DashboardActivity();
            if (overdueActivities.Any())
            {
                response.OverdueActivities = overdueActivities.Select(c => new ActivityViewModel(c)).ToList();
                response.OverdueActivities.Sort((x, y) => DateTime.Compare(x.TodoTime, y.TodoTime));
            }
            if (todayActivities.Any())
            {
                response.TodayActivities = todayActivities.Select(c => new ActivityViewModel(c)).ToList();
                response.TodayActivities.Sort((x, y) => DateTime.Compare(x.TodoTime, y.TodoTime));

            }
            if (futureActivities.Any())
            {
                response.FutureActivities = futureActivities.Select(c => new ActivityViewModel(c)).ToList();
                response.FutureActivities.Sort((x, y) => DateTime.Compare(x.TodoTime, y.TodoTime));

            }
            return Ok(response);
        }

        [Route("GetContractDashboard")]
        [ResponseType(typeof(DashboardContract))]
        public IHttpActionResult GetContractDashboard(int monthRange = 12)
        {
            int contractRemindDays = 10;
            _contractService.BackgroundUpdateStatus(contractRemindDays);
            _contractService.SaveChanges();

            var totalCount = _contractService.GetAll().Count();
            var needActionCount = _contractService.GetNeedAction().Count();
            List<int> usingYears = new List<int>
            {
                1,2,5
            };
            var allCategoriesUsingRates = _contractService.GetAllUsingRates(usingYears);
            var allCategories = _salesCategoryService.GetAll();
            var individualUsingRatesList = new List<Dictionary<string, int>>();
            foreach (var category in allCategories)
            {
                individualUsingRatesList.Add(_contractService.GetUsingRates(category, usingYears));
            }
            var contractUsingRatesList = new List<UsingRateCharts>();
            for (int i = 0; i < allCategories.Count(); i++)
            {
                contractUsingRatesList.Add(new UsingRateCharts
                {
                    CategoryID = allCategories.ElementAt(i).ID,
                    Chart = new ChartViewModel
                    {
                        Labels = individualUsingRatesList.ElementAt(i).Keys.ToList(),
                        Values = individualUsingRatesList.ElementAt(i).Values.ToList()
                    }
                });

            }
            var response = new DashboardContract
            {
                TotalCount = totalCount,
                NeedActionCount = needActionCount,
                AllUsingRates = new ChartViewModel
                {
                    Labels = allCategoriesUsingRates.Keys.ToList(),
                    Values = allCategoriesUsingRates.Values.ToList()
                },
                UsingRatesList = contractUsingRatesList
            };
            return Ok(response);
        }

        [Route("GetMarketingDashboard")]
        [ResponseType(typeof(DashboardMarketing))]
        public IHttpActionResult GetMarketingDashboard(int monthRange = 12)
        {
            _marketingPlanService.BackgroundUdate();
            _marketingPlanService.SaveChanges();
            var executingCount = _marketingPlanService.GetDoing().Count();
            var createRates = _marketingPlanService.GetRates(monthRange);
            var leadGenerateRates = _marketingResultService.GetLeadRates(monthRange);
            var leadSourceRates = _marketingResultService.GetSourceRates(monthRange);
            var marketingRatings = _marketingResultService.GetRatings();
            var response = new DashboardMarketing
            {
                ExecutingCount = executingCount,
                Rates = new ChartViewModel
                {
                    Labels = createRates.Keys.ToList(),
                    Values = createRates.Values.ToList()
                },
                LeadCountChart = new ChartViewModel
                {
                    Labels = leadGenerateRates.Keys.ToList(),
                    Values = leadGenerateRates.Values.ToList()
                },
                LeadSourceCountChart = new ChartViewModel
                {
                    Labels = leadSourceRates.Keys.ToList(),
                    Values = leadSourceRates.Values.ToList()
                },
                MarketingRatingMaxValue = 5,
                MarketingRatings = new DoubleTypeChartViewModel
                {
                    Labels=marketingRatings.Keys.ToList(),
                    Values=marketingRatings.Values.ToList(),
                }
            };
            return Ok(response);
        }

        [Route("GetIssueDashboard")]
        [ResponseType(typeof(DashboardIssue))]
        public IHttpActionResult GetIssueDashboard(int monthRange = 12)
        {
            _issueService.BackgroundUpdateStatus();
            _issueService.SaveChanges();
            var doingIssueCount = _issueService.GetDoing().Count();
            var failedIssueCount = _issueService.GetFailed().Count();
            var doneIssueCount = _issueService.GetDone().Count();

            Dictionary<string, int> issueRates = _issueService.GetRates(monthRange);
            Dictionary<string, int> issueCategoryCount = _issueCategoryMappingService.GetCounts(monthRange);
            var response = new DashboardIssue
            {
                DoingIssuesCount = doingIssueCount,
                DoneFailedRate = new ChartViewModel
                {
                    Labels = new List<string>
                    {
                        IssueStatus.Done,
                        IssueStatus.Failed
                    },
                    Values = new List<int>
                    {
                        doneIssueCount,
                        failedIssueCount
                    }
                },
                CreateRates = new ChartViewModel
                {
                    Labels = issueRates.Keys.ToList(),
                    Values = issueRates.Values.ToList(),
                },
                IssueCategoryRates = new ChartViewModel
                {
                    Labels = issueCategoryCount.Keys.ToList(),
                    Values = issueCategoryCount.Values.ToList()
                }
            };
            return Ok(response);
        }
    }
}
