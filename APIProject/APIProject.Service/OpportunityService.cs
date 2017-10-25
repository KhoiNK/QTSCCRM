using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.GlobalVariables;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface IOpportunityService
    {
        IEnumerable<Opportunity> GetAllOpportunities();
        IEnumerable<Opportunity> GetByCustomer(int customerID);
        Dictionary<string, string> GetStageDescription();
        int CreateOpportunity(Opportunity newOpportunity);
        bool MapOpportunityActivity(int insertedOpportunityID, int insertedActivityID);
        Opportunity GetByID(int id);
        void EditInfo(Opportunity opportunity);
        void ProceedNextStage(Opportunity opportunity);
        void SetMakeQuoteStage(int opportunityID);
        Opportunity Get(int id);
        void Update(Opportunity opportunity);
        void SaveChanges();
    }
    public class OpportunityService : IOpportunityService
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OpportunityService(IOpportunityRepository _opportunityRepository,
            ICustomerRepository _customerRepository, IUnitOfWork _unitOfWork,
            IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository,
            IActivityRepository _activityRepository,
            IContactRepository _contactRepository,
            IQuoteRepository _quoteRepository)
        {
            this._opportunityRepository = _opportunityRepository;
            this._customerRepository = _customerRepository;
            this._opportunityCategoryMappingRepository = _opportunityCategoryMappingRepository;
            this._activityRepository = _activityRepository;
            this._contactRepository = _contactRepository;
            this._quoteRepository = _quoteRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateOpportunity(Opportunity newOpportunity)
        {
            _opportunityRepository.Add(newOpportunity);
            //newOpportunity.CustomerID = newOpportunity.Contact.CustomerID;
            var foundContact = _contactRepository.GetById(newOpportunity.ContactID.Value);
            if (foundContact != null)
            {
                newOpportunity.CustomerID = foundContact.CustomerID;
            }
            else
            {
                return 0;
            }
            newOpportunity.StageName = OpportunityStage.Consider;
            newOpportunity.ConsiderStart = DateTime.Now;
            newOpportunity.Priority = Priority.Low;
            _unitOfWork.Commit();
            return newOpportunity.ID;
        }

        public void EditInfo(Opportunity opportunity)
        {
            var foundOpp = _opportunityRepository.GetById(opportunity.ID);
            if (foundOpp == null)
            {
                throw new Exception(CustomError.OpportunityNotFound);
            }
            if (foundOpp.CreatedStaffID != opportunity.CreatedStaffID)
            {
                throw new Exception(CustomError.WrongAuthorizedStaff);
            }

            foundOpp.UpdatedStaffID = opportunity.CreatedStaffID;
            foundOpp.Title = opportunity.Title;
            foundOpp.Description = opportunity.Description;
            _unitOfWork.Commit();
        }

        public Opportunity Get(int id)
        {
            return _opportunityRepository.GetById(id);
        }

        public IEnumerable<Opportunity> GetAllOpportunities()
        {
            return _opportunityRepository.GetAll();
        }

        public IEnumerable<Opportunity> GetByCustomer(int customerID)
        {
            var foundCustomer = _customerRepository.GetById(customerID);
            if (foundCustomer != null)
            {
                var opportunities = foundCustomer.Opportunities;
                if (opportunities.Any())
                {
                    return opportunities;
                }
            }

            return null;
        }

        public Opportunity GetByID(int id)
        {
            return _opportunityRepository.GetById(id);
        }

        public Dictionary<string, string> GetStageDescription()
        {
            return OpportunityStage.GetList();
        }





        public bool MapOpportunityActivity(int insertedOpportunityID, int insertedActivityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(insertedOpportunityID);
            var foundActivity = _activityRepository.GetById(insertedActivityID);

            foundActivity.OpportunityID = foundOpportunity.ID;
            foundActivity.OfOpportunityStage = foundOpportunity.StageName;

            _unitOfWork.Commit();

            return true;
        }

        public void ProceedNextStage(Opportunity opportunity)
        {
            //validate business
            var foundOpp = _opportunityRepository.GetById(opportunity.ID);
            if (foundOpp == null)
            {
                throw new Exception(CustomError.OpportunityNotFound);
            }
            if (foundOpp.CreatedStaffID != opportunity.CreatedStaffID)
            {
                throw new Exception(CustomError.WrongAuthorizedStaff);
            }
            var lastQuote = _quoteRepository.GetLatestQuoteByOpportunity(foundOpp.ID);
            if (foundOpp.StageName == OpportunityStage.MakeQuote)
            {
                if(lastQuote == null)
                {
                    throw new Exception(CustomError.QuoteRequired); 
                }
                if(lastQuote.Status == QuoteStatus.NotValid)
                {
                    throw new Exception(CustomError.NewQuoteRequired);
                }
            }

            if(foundOpp.StageName == OpportunityStage.ValidateQuote)
            {
                if (lastQuote.Status == QuoteStatus.Validating)
                {
                    throw new Exception(CustomError.ValidateQuoteRequired);
                }
            }
            if(foundOpp.StageName == OpportunityStage.SendQuote)
            {
                if (!lastQuote.SentCustomerDate.HasValue)
                {
                    throw new Exception(CustomError.SendQuoteRequired);
                }
            }
            if(foundOpp.StageName == OpportunityStage.Won ||
                foundOpp.StageName==OpportunityStage.Lost)
            {
                throw new Exception(CustomError.OpportunityClosed);
            }
            //end validate business

            if (foundOpp.StageName == OpportunityStage.MakeQuote)
            {
                if(lastQuote.Status == QuoteStatus.Drafting)
                {
                    lastQuote.Status = QuoteStatus.Validating;
                }
            }
            //later add send email function
            if(foundOpp.StageName == OpportunityStage.ValidateQuote)
            {
                lastQuote.SentCustomerDate = DateTime.Now;
            }


            foundOpp.UpdatedDate = DateTime.Now;
            SetNextStage(foundOpp);
            _unitOfWork.Commit();

            //missing generate contract
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        public void SetMakeQuoteStage(int opportunityID)
        {
            var foundOpp = _opportunityRepository.GetById(opportunityID);
            if (foundOpp == null)
            {
                throw new Exception(CustomError.OpportunityNotFound);
            }
            foundOpp.StageName = OpportunityStage.MakeQuote;
            foundOpp.UpdatedDate = DateTime.Now;
            _unitOfWork.Commit();
        }

        public void Update(Opportunity opportunity)
        {
            var entity = _opportunityRepository.GetById(opportunity.ID);
            entity = opportunity;
            entity.UpdatedDate = DateTime.Now;
            _opportunityRepository.Update(entity);
        }

        private void SetNextStage(Opportunity opportunity)
        {
            var dateTimeNow = DateTime.Now;
            if (opportunity.StageName == OpportunityStage.Consider)
            {
                opportunity.StageName = OpportunityStage.MakeQuote;
                opportunity.MakeQuoteStart = dateTimeNow;
            }
            else if (opportunity.StageName == OpportunityStage.MakeQuote)
            {
                opportunity.StageName = OpportunityStage.ValidateQuote;
                opportunity.ValidateQuoteStart = dateTimeNow;

            }
            else if (opportunity.StageName == OpportunityStage.ValidateQuote)
            {
                opportunity.StageName = OpportunityStage.SendQuote;
                opportunity.SendQuoteStart = dateTimeNow;

            }
            else if (opportunity.StageName == OpportunityStage.SendQuote)
            {
                opportunity.StageName = OpportunityStage.Negotiation;
                opportunity.NegotiationStart = dateTimeNow;
            }
            else
            {
                opportunity.StageName = OpportunityStage.Won;
                opportunity.ClosedDate = dateTimeNow;
            }
        }
    }
}
