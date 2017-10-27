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
    public interface IStaffService
    {
        Staff GetByOpportunity(int opportunityID);
        Staff GetByActivity(int activityID);
        void CreateStaff(Staff staff);
        IEnumerable<Staff> GetAllStaffs();
        Staff Get(int id);
        void SaveChanges();
    }
    public class StaffService:IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IUnitOfWork _unitOfWork;
        public StaffService(IUnitOfWork _unitOfWork, IStaffRepository _staffRepository,
            IOpportunityRepository _opportunityRepository, IActivityRepository _activityRepository)
        {
            this._unitOfWork = _unitOfWork;
            this._staffRepository = _staffRepository;
            this._opportunityRepository = _opportunityRepository;
            this._activityRepository = _activityRepository;
        }

        public void CreateStaff(Staff staff)
        {
            _staffRepository.Add(staff);
            _unitOfWork.Commit();
        }

        public Staff Get(int id)
        {
            var entity = _staffRepository.GetById(id);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception(CustomError.StaffNotFound);
            }
        }

        public IEnumerable<Staff> GetAllStaffs()
        {
            return _staffRepository.GetAll();
        }

        public Staff GetByActivity(int activityID)
        {
            var foundActivity = _activityRepository.GetById(activityID);
            if (foundActivity != null)
            {
                return foundActivity.Staff;
            }
            return null;
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

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }
    }
}
