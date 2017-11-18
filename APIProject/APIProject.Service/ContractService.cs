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
    public interface IContractService
    {
        Contract Get(int contractID);
        IEnumerable<Contract> GetAll();
        IEnumerable<Contract> GetNeedAction();
        Dictionary<string, int> GetAllUsingRates(List<int> usingYears);
        Dictionary<string, int> GetUsingRates(Category category,List<int> usingYears);
        Contract Add(Contract contract);
        void BackgroundUpdateStatus(int remindDays);
        void SaveChanges();
    }
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IAppConfigRepository _appConfigRepository;
        private readonly ISalesItemRepository _salesItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        private readonly double daysInYear = 365;

        public ContractService(IUnitOfWork _unitOfWork,
            IAppConfigRepository _appConfigRepository,
            ISalesItemRepository _salesItemRepository,
            IContractRepository _contractRepository)
        {
            this._unitOfWork = _unitOfWork;
            this._appConfigRepository = _appConfigRepository;
            this._contractRepository = _contractRepository;
            this._salesItemRepository = _salesItemRepository;
        }

        public Contract Get(int contractID)
        {
            var entity = _contractRepository.GetById(contractID);
            if (entity != null)
            {
                return entity;
            }
            else
            {
                throw new Exception(CustomError.ContractNotFound);
            }
        }
        public IEnumerable<Contract> GetAll()
        {
            var entities = _contractRepository.GetAll().Where(c => c.IsDelete == false);
            return entities;
        }

        public IEnumerable<Contract> GetNeedAction()
        {
            var entities = GetAll().Where(c => c.Status == ContractStatus.NeedAction);
            return entities;
        }
        public Dictionary<string, int> GetUsingRates(Category category, List<int> usingYears)
        {
            var response = new Dictionary<string, int>();
            var categoryItemIDs = _salesItemRepository.GetAll().Where(c => c.SalesCategoryID == category.ID
            && c.IsDelete == false).Select(c=>c.ID).ToList();
            var entities = GetAll().Where(c => categoryItemIDs.Contains(c.SalesItemID));
            usingYears.Sort();
            foreach (int usingYear in usingYears)
            {
                response.Add("Dưới " + usingYear + " năm",
                    entities.Where(c => (c.EndDate - c.StartDate).TotalDays <= (usingYear * daysInYear)).Count());
            }
            response.Add("Trên " + usingYears + " năm",
                entities.Where(c => (c.EndDate - c.StartDate).TotalDays > (usingYears.Last() * daysInYear)).Count());
            return response;
        }

        public Dictionary<string, int> GetAllUsingRates(List<int> usingYears)
        {
            var response = new Dictionary<string, int>();
            var entities = GetAll();
            usingYears.Sort();
            foreach(int usingYear in usingYears)
            {
                response.Add("Dưới " + usingYear + " năm",
                    entities.Where(c => (c.EndDate - c.StartDate).TotalDays<=(usingYear*daysInYear)).Count());
            }
            response.Add("Trên " + usingYears + " năm",
                entities.Where(c => (c.EndDate - c.StartDate).TotalDays > (usingYears.Last() * daysInYear)).Count());
            return response;    
        }

        public Contract Add(Contract contract)
        {
            //var entity = new Contract
            //{
            //    CreatedStaffID = contract.CreatedStaffID,
            //    SalesCategoryID = contract.SalesCategoryID,
            //    Status = ContractStatus.Waiting,
            //    CreatedDate = DateTime.Now,
            //    UpdatedDate = DateTime.Now,
            //    ContractCode = Guid.NewGuid().ToString()
            //};
            //int contractCodeNumber = _contractRepository.GetAll().Count() + 1;
            //contract.ContractCode = _appConfigRepository.GetContractCode() + contractCodeNumber.ToString("00000");
            _contractRepository.Add(contract);
            return contract;
        }

        public void BackgroundUpdateStatus(int remindDays)
        {
            var entities = GetAll();
            foreach (var entity in entities)
            {
                if (DateTime.Compare(DateTime.Now, entity.StartDate) >= 0)
                {
                    entity.Status = ContractStatus.Active;
                }
                else
                {
                    entity.Status = ContractStatus.Waiting;
                }
                if (DateTime.Compare(DateTime.Now, entity.EndDate) > 0)
                {
                    entity.Status = ContractStatus.Done;
                }
                else if ((entity.EndDate - DateTime.Now).TotalDays <= remindDays)
                {
                    entity.Status = ContractStatus.NeedAction;
                }
                entity.UpdatedDate = DateTime.Now;
                _contractRepository.Update(entity);
            }
        }


        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

    }
}
