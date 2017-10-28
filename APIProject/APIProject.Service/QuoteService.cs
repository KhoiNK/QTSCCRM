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
    public interface IQuoteService
    {

        Quote Get(int id);
        Quote GetByOpportunity(int opportunityID);
        void Add(Quote quote);
        void SetValidatingStatus(Quote quote);
        void SetValid(Quote quote);
        void SetInvalid(Quote quote);
        void UpdateInfo(Quote quote);
        void Update(Quote quote);
        void Delete(Quote quote);
        void SaveChanges();
    }
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IOpportunityRepository _opportunityRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuoteService(IQuoteRepository _quoteRepository, IUnitOfWork _unitOfWork,
            IOpportunityRepository _opportunityRepository,
            ISalesItemRepository _salesItemRepository,
            IStaffRepository _staffRepository,
            IRoleRepository _roleRepository)
        {
            this._quoteRepository = _quoteRepository;
            this._roleRepository = _roleRepository;
            this._opportunityRepository = _opportunityRepository;
            this._salesItemRepository = _salesItemRepository;
            this._staffRepository = _staffRepository;
            this._unitOfWork = _unitOfWork;
        }

        

        public Quote Get(int id)
        {
            var entity = _quoteRepository.GetById(id);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception(CustomError.QuoteNotFound);
            }
        }

        public Quote GetByOpportunity(int opportunityID)
        {
            var entity = _quoteRepository.GetAll().Where(c => c.OpportunityID == opportunityID &&
              c.IsDelete == false).SingleOrDefault();
            if (entity != null)
            {
                return entity;
            }
            else
            {
                return null;
            }
        }

        public void Add(Quote quote)
        {
            quote.CreatedDate = DateTime.Now;
            _quoteRepository.Add(quote);
        }

        public void SetValidatingStatus(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            entity.Status = QuoteStatus.Validating;
            entity.UpdatedDate = DateTime.Now;
            _quoteRepository.Update(entity);
        }
        public void SetValid(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            VerifyCanSetValid(entity);
            var validateStaff = _staffRepository.GetById(quote.ValidatedStaffID.Value);
            VerifyCanSetValidStaff(validateStaff);
            entity.ValidatedStaffID = quote.ValidatedStaffID;
            entity.Notes = quote.Notes;
            entity.UpdatedDate = DateTime.Now;
            _quoteRepository.Update(entity);
        }
        public void SetInvalid(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            VerifyCanSetInvalid(entity);
            var validateStaff = _staffRepository.GetById(quote.ValidatedStaffID.Value);
            VerifyCanSetInvalidStaff(validateStaff);
            entity.ValidatedStaffID = quote.ValidatedStaffID;
            entity.Notes = quote.Notes;
            entity.UpdatedDate = DateTime.Now;
            _quoteRepository.Update(entity);
        }

        public void UpdateInfo(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            VerifyCanUpdateQuoteStatus(entity);
            var quoteStaff = _staffRepository.GetById(quote.CreatedStaffID);
            VerifyCanUpdateQuoteStaff(quoteStaff);
            entity.Tax = quote.Tax;
            entity.Discount = quote.Discount;
            entity.UpdatedDate = DateTime.Now;
            _quoteRepository.Update(entity);
        }
        public void Update(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            entity = quote;
            entity.UpdatedDate = DateTime.Now;
            _quoteRepository.Update(entity);
        }

        public void Delete(Quote quote)
        {
            var entity = _quoteRepository.GetById(quote.ID);
            entity = quote;
            entity.IsDelete = true;
            _quoteRepository.Update(entity);
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

        #region private verify
        private void VerifyCanSetValid(Quote quote)
        {
            if(quote.Status != QuoteStatus.Validating)
            {
                throw new Exception(CustomError.QuoteStatusRequired
                    + QuoteStatus.Validating);
            }
        }
        private void VerifyCanSetValidStaff(Staff staff)
        {
            var staffRoleName = _roleRepository.GetById(staff.RoleID).Name;
            if(staffRoleName != RoleName.Director)
            {
                throw new Exception(CustomError.StaffRoleRequired
                    + RoleName.Director);
            }
        }
        private void VerifyCanSetInvalid(Quote quote)
        {
            if (quote.Status != QuoteStatus.Validating)
            {
                throw new Exception(CustomError.QuoteStatusRequired
                    + QuoteStatus.Validating);
            }
        }
        private void VerifyCanSetInvalidStaff(Staff staff)
        {
            var staffRoleName = _roleRepository.GetById(staff.RoleID).Name;
            if (staffRoleName != RoleName.Director)
            {
                throw new Exception(CustomError.StaffRoleRequired
                    + RoleName.Director);
            }
        }
        private void VerifyCanUpdateQuoteStatus(Quote quote)
        {
            if (quote.Status != QuoteStatus.Drafting)
            {
                throw new Exception(CustomError.QuoteStatusRequired
                    + QuoteStatus.Drafting);
            }
        }
        private void VerifyCanUpdateQuoteStaff(Staff staff)
        {
            var staffRoleName = _roleRepository.GetById(staff.RoleID).Name;
            if (staffRoleName != RoleName.Sales)
            {
                throw new Exception(CustomError.StaffRoleRequired
                    + RoleName.Sales);
            }
        }
        #endregion
    }
}
