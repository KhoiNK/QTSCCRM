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
    public interface IStaffService
    {
        Staff GetByOpportunity(int opportunityID);
    }
    public class StaffService:IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IUnitOfWork _unitOfWork;
        public StaffService(IUnitOfWork _unitOfWork, IStaffRepository _staffRepository,
            IOpportunityRepository _opportunityRepository)
        {
            this._unitOfWork = _unitOfWork;
            this._staffRepository = _staffRepository;
            this._opportunityRepository = _opportunityRepository;
        }

        public Staff GetByOpportunity(int opportunityID)
        {
            var foundOpportunity = _opportunityRepository.GetById(opportunityID);
            if(foundOpportunity != null)
            {
                return foundOpportunity.CreatedStaff;
            }
            return null;
        }
    }
}
