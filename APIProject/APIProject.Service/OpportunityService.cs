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
        List<string> GetStages();
        Opportunity CreateOpportunity(Opportunity newOpportunity);
    }
    public class OpportunityService : IOpportunityService
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OpportunityService(IOpportunityRepository _opportunityRepository,
            ICustomerRepository _customerRepository, IUnitOfWork _unitOfWork,
            IOpportunityCategoryMappingRepository _opportunityCategoryMappingRepository)
        {
            this._opportunityRepository = _opportunityRepository;
            this._customerRepository = _customerRepository;
            this._opportunityCategoryMappingRepository = _opportunityCategoryMappingRepository;
            this._unitOfWork = _unitOfWork;
        }

        public Opportunity CreateOpportunity(Opportunity newOpportunity)
        {
            _opportunityRepository.Add(newOpportunity);
            newOpportunity.StageName = OpportunityStage.Consider;
            newOpportunity.ConsiderStart = DateTime.Today.Date;
            newOpportunity.Priority = Priority.Low;
            _unitOfWork.Commit();
            //if (categoryIDs != null)
            //{
            //    categoryIDs.ForEach(c =>
            //    _opportunityCategoryMappingRepository.Add(new OpportunityCategoryMapping
            //    {
            //        SalesCategoryID = c,
            //        OpportunityID = newOpportunity.ID,
            //        IsDeleted = false,
            //    }));
            //    _unitOfWork.Commit();
            //}
            return newOpportunity;
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

        public List<string> GetStages()
        {
            return new List<string>
            {
                OpportunityStage.Open,
                OpportunityStage.Consider,
                OpportunityStage.MakeQuote,
                OpportunityStage.ValidateQuote,
                OpportunityStage.SendQuote,
                OpportunityStage.Negotiation,
                OpportunityStage.Won,
                OpportunityStage.Lost
            };
        }

    }
}
