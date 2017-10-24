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
    }
    public class OpportunityService : IOpportunityService
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OpportunityService(IOpportunityRepository _opportunityRepository,
            ICustomerRepository _customerRepository, IUnitOfWork _unitOfWork,
            IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository,
            IActivityRepository _activityRepository,
            IContactRepository _contactRepository)
        {
            this._opportunityRepository = _opportunityRepository;
            this._customerRepository = _customerRepository;
            this._opportunityCategoryMappingRepository = _opportunityCategoryMappingRepository;
            this._activityRepository = _activityRepository;
            this._contactRepository = _contactRepository;
            this._unitOfWork = _unitOfWork;
        }

        public int CreateOpportunity(Opportunity newOpportunity)
        {
            _opportunityRepository.Add(newOpportunity);
            //newOpportunity.CustomerID = newOpportunity.Contact.CustomerID;
            var foundContact = _contactRepository.GetById(newOpportunity.ContactID.Value);
            if(foundContact != null)
            {
                newOpportunity.CustomerID = foundContact.CustomerID;
            }
            else
            {
                return 0;
            }
            newOpportunity.StageName = OpportunityStage.Consider;
            newOpportunity.ConsiderStart = DateTime.Today.Date;
            newOpportunity.Priority = Priority.Low;
            _unitOfWork.Commit();
            return newOpportunity.ID;
        }

        public void EditInfo(Opportunity opportunity)
        {
            var foundOpp = _opportunityRepository.GetById(opportunity.ID);
            if(foundOpp == null)
            {
                throw new Exception(CustomError.OpportunityNotFound);
            }
            if(foundOpp.CreateStaffID != opportunity.CreateStaffID)
            {
                throw new Exception(CustomError.WrongAuthorizedStaff);
            }

            foundOpp.ModifyStaffID = opportunity.CreateStaffID;
            foundOpp.Title = opportunity.Title;
            foundOpp.Description = opportunity.Description;
            _unitOfWork.Commit();
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
    }
}
