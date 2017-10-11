using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface IMarketingResultService
    {
        IEnumerable<MarketingResult> GetMarketingResults();
        bool CreateResults(List<MarketingResult> list, bool isFinished);
    }

    public class MarketingResultService: IMarketingResultService
    {
        private readonly IMarketingResultRepository _marketingResultRepository;
        private readonly IMarketingPlanRepository _marketingPlanRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;

        private const string RunningName = "Running";
        private const string ReportingName = "Reporting";
        private const string FinishedName = "Finished";


        public MarketingResultService(IMarketingResultRepository _marketingResultRepository, IMarketingPlanRepository _marketingPlanRepository,
            ICustomerRepository _customerRepository, IContactRepository _contactRepository, IUnitOfWork _unitOfWork)
        {
            this._marketingResultRepository = _marketingResultRepository;
            this._marketingPlanRepository = _marketingPlanRepository;
            this._customerRepository = _customerRepository;
            this._contactRepository = _contactRepository;
            this._unitOfWork = _unitOfWork;
        }

        public bool CreateResults(List<MarketingResult> list, bool isFinished)
        {
            int planID = list.First().MarketingPlanID;
            var foundPlan = _marketingPlanRepository.GetById(planID);
            foreach(var item in list)
            {
                //verify customer exist
                if (item.CustomerID.HasValue)
                {
                    var foundCustomer = _customerRepository.GetById(item.CustomerID.Value);
                    if(foundCustomer == null)
                    {
                        return false;
                    }
                }
                //verify contact exist and in right customer
                if (item.ContactID.HasValue)
                {
                    var foundContact = _contactRepository.GetById(item.ContactID.Value);
                    if(foundContact == null)
                    {
                        return false;
                    }
                    else
                    {
                        if (item.CustomerID.HasValue)
                        {
                            if(foundContact.CustomerID != item.CustomerID)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            //verify plan exist
            if(foundPlan == null)
            {
                return false;
            }
            //verify stage
            if(foundPlan.Stage != RunningName && foundPlan.Stage != ReportingName)
            {
                return false;
            }
            //verify stage and is finished
            if (isFinished)
            {
                if(foundPlan.Stage == RunningName)
                {
                    return false;
                }
            }


            //start adding result code here
            return true;
        }

        public IEnumerable<MarketingResult> GetMarketingResults()
        {
            return _marketingResultRepository.GetAll();
        }
    }

    
}
