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
    public interface IOpportunityService
    {
        IEnumerable<Opportunity> GetAllOpportunities();
    }
    public class OpportunityService:IOpportunityService
    {
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OpportunityService(IOpportunityRepository _opportunityRepository, IUnitOfWork _unitOfWork)
        {
            this._opportunityRepository = _opportunityRepository;
            this._unitOfWork = _unitOfWork;
        }

        public IEnumerable<Opportunity> GetAllOpportunities()
        {
            return _opportunityRepository.GetAll();
        }
    }
}
