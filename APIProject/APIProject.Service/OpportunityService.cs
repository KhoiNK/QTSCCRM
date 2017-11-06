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
        IEnumerable<Opportunity> GetByCustomer(int customerID);
        Dictionary<string, string> GetStageDescription();
        int CreateOpportunity(Opportunity newOpportunity);
        bool MapOpportunityActivity(int insertedOpportunityID, int insertedActivityID);
        Opportunity GetByID(int id);
        void EditInfo(Opportunity opportunity);
        Opportunity Get(int id);
        IEnumerable<Opportunity> GetAll();
        Opportunity GetByQuote(int quoteID);
        Opportunity Add(Opportunity opp);
        void UpdateInfo(Opportunity opportunity);
        Opportunity SetNextStage(Opportunity opp);
        void SetMakeQuoteStage(Opportunity opp);
        void SetWon(Opportunity opp);
        void SetLost(Opportunity opp);
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
            _opportunityRepository.Update(foundOpp);
            _unitOfWork.Commit();
        }

        public Opportunity Get(int id)
        {
            var entity = _opportunityRepository.GetById(id);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception(CustomError.OpportunityNotFound);
            }
        }

        public Opportunity GetByQuote(int quoteID)
        {
            var oppID = _quoteRepository.GetById(quoteID).OpportunityID;
            var quoteOpp = _opportunityRepository.GetById(oppID);
            return quoteOpp;
        }


        public IEnumerable<Opportunity> GetAll()
        {
            return _opportunityRepository.GetAll().Where(c => c.IsDelete == false);
        }

        public IEnumerable<Opportunity> GetByCustomer(int customerID)
        {
            var entities = _opportunityRepository.GetAll()
                .Where(c => c.CustomerID == customerID && c.IsDelete == false);
            return entities;
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



        public Opportunity Add(Opportunity opp)
        {
            var entity = new Opportunity
            {
                Title = opp.Title,
                Description = opp.Description,
                StageName = OpportunityStage.Consider,
                ConsiderStart = DateTime.Today,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ContactID = opp.ContactID,
                CreatedStaffID = opp.CreatedStaffID,
                UpdatedStaffID = opp.CreatedStaffID
            };
            var ContactCus = _contactRepository.GetById(opp.ContactID.Value);
            entity.CustomerID = ContactCus.ID;
            _opportunityRepository.Add(entity);
            _unitOfWork.Commit();
            return entity;
        }

        public void UpdateInfo(Opportunity opportunity)
        {
            var entity = _opportunityRepository.GetById(opportunity.ID);
            VerifyStageCanChangeInfo(entity);
            entity.Title = opportunity.Title;
            entity.Description = opportunity.Description;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedStaffID = opportunity.UpdatedStaffID;
            _opportunityRepository.Update(entity);
        }

        public Opportunity SetNextStage(Opportunity opp)
        {
            var entity = _opportunityRepository.GetById(opp.ID);
            
            VerifyCanSetNextStage(entity);
            #region move stage 
            if (entity.StageName == OpportunityStage.Consider)
            {
                entity.StageName = OpportunityStage.MakeQuote;
                entity.MakeQuoteStart = DateTime.Now;
            }
            else if (entity.StageName == OpportunityStage.MakeQuote)
            {
                entity.StageName = OpportunityStage.ValidateQuote;
                entity.ValidateQuoteStart = DateTime.Now;
            }
            else if (entity.StageName == OpportunityStage.ValidateQuote)
            {
                entity.StageName = OpportunityStage.SendQuote;
                entity.SendQuoteStart = DateTime.Now;
            }
            else
            {
                entity.StageName = OpportunityStage.Negotiation;
                entity.NegotiationStart = DateTime.Now;
            }
            #endregion
            entity.UpdatedStaffID = opp.UpdatedStaffID;
            entity.UpdatedDate = DateTime.Now;
            _opportunityRepository.Update(entity);
            return entity;
        }
        public void SetMakeQuoteStage(Opportunity opp)
        {
            var entity = _opportunityRepository.GetById(opp.ID);
            entity.StageName = OpportunityStage.MakeQuote;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedStaffID = opp.UpdatedStaffID;
            _opportunityRepository.Update(entity);
        }
        public void SetWon(Opportunity opp)
        {
            var entity = _opportunityRepository.GetById(opp.ID);
            VerifyCanSetWonStage(entity);
            entity.StageName = OpportunityStage.Won;
            entity.ClosedDate = DateTime.Today;
            entity.UpdatedStaffID = opp.UpdatedStaffID;
            entity.UpdatedDate = DateTime.Now;
            _opportunityRepository.Update(entity);
        }
        public void SetLost(Opportunity opp)
        {
            var entity = _opportunityRepository.GetById(opp.ID);
            VeryfiCanSetLostStage(entity);
            entity.StageName = OpportunityStage.Lost;
            entity.ClosedDate = DateTime.Today;
            entity.UpdatedStaffID = opp.UpdatedStaffID;
            entity.UpdatedDate = DateTime.Now;
            _opportunityRepository.Update(entity);
        }
        public void Update(Opportunity opportunity)
        {
            var entity = _opportunityRepository.GetById(opportunity.ID);
            entity = opportunity;
            entity.UpdatedDate = DateTime.Now;
            _opportunityRepository.Update(entity);
        }
        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }



        #region private verify
        private void VerifyStageCanChangeInfo(Opportunity opportunity)
        {
            var requiredStages = new List<string>
                {
                    OpportunityStage.Consider,
                    OpportunityStage.MakeQuote,
                    OpportunityStage.ValidateQuote,
                    OpportunityStage.SendQuote,
                };
            if (!requiredStages.Contains(opportunity.StageName))
            {
                throw new Exception(CustomError.ChangeInfoStageRequired +
                    String.Join(", ", requiredStages));
            }
        }
        private void VerifyCanSetNextStage(Opportunity opp)
        {
            List<string> RequiredStages = new List<string>
            {
                OpportunityStage.Consider,
                OpportunityStage.MakeQuote,
                OpportunityStage.ValidateQuote,
                OpportunityStage.SendQuote,
            };
            if (!RequiredStages.Contains(opp.StageName))
            {
                throw new Exception(CustomError.OppStageRequired +
                    String.Join(", ", RequiredStages));
            }
            var quoteEntity = opp.Quotes.Where(c => c.IsDelete == false).SingleOrDefault();

            if (opp.StageName == OpportunityStage.MakeQuote)
            {
                if (quoteEntity == null)
                {
                    throw new Exception(CustomError.CreateQuoteRequired);
                }
                else
                {
                    if(quoteEntity.Status == QuoteStatus.NotValid)
                    {
                        throw new Exception(CustomError.CreateQuoteRequired);
                    }
                }
            }
            if (opp.StageName == OpportunityStage.ValidateQuote)
            {
                if (quoteEntity.Status != QuoteStatus.Valid)
                {
                    throw new Exception(CustomError.QuoteStatusRequired +
                        QuoteStatus.Valid);
                }
            }
            if (opp.StageName == OpportunityStage.SendQuote)
            {
                if (!quoteEntity.SentCustomerDate.HasValue)
                {
                    throw new Exception(CustomError.SendQuoteRequired);
                }
            }
        }
        

        private void VerifyCanSetWonStage(Opportunity opp)
        {
            if (opp.StageName != OpportunityStage.Negotiation)
            {
                throw new Exception(CustomError.OppStageRequired
                    + OpportunityStage.Negotiation);
            }
        }
        private void VeryfiCanSetLostStage(Opportunity opp)
        {
            List<string> requiredStages = new List<string>
            {
                OpportunityStage.Consider,
                OpportunityStage.MakeQuote,
                OpportunityStage.ValidateQuote,
                OpportunityStage.SendQuote,
                OpportunityStage.Negotiation
            };
            if (!requiredStages.Contains(opp.StageName))
            {
                throw new Exception(CustomError.OppStageRequired
                    + String.Join(", ", requiredStages));
            }
        }
        #endregion
    }
}
