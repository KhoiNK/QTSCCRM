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
        Dictionary<string, int> GetUsingRates(SalesCategory category, List<int> usingYears);
        Contract Add(Contract contract);
        Contract Recontract(Contract foundContract, DateTime endDate);
        void Close(Contract contract);
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
        public Dictionary<string, int> GetUsingRates(SalesCategory category, List<int> usingYears)
        {
            var response = new Dictionary<string, int>();
            var categoryItemIDs = _salesItemRepository.GetAll().Where(c => c.SalesCategoryID == category.ID
            && c.IsDelete == false).Select(c => c.ID).ToList();
            var entities = GetAll().Where(c => categoryItemIDs.Contains(c.SalesItemID));
            usingYears.Sort();
            foreach (int usingYear in usingYears)
            {
                response.Add("Dưới " + usingYear + " năm",
                    entities.Where(c => (c.EndDate - c.StartDate).TotalDays <= (usingYear * daysInYear)).Count());
            }
            response.Add("Trên " + usingYears.Last() + " năm",
                entities.Where(c => (c.EndDate - c.StartDate).TotalDays > (usingYears.Last() * daysInYear)).Count());
            return response;
        }

        public Dictionary<string, int> GetAllUsingRates(List<int> usingYears)
        {
            var response = new Dictionary<string, int>();
            var entities = GetAll();
            usingYears.Sort();
            foreach (int usingYear in usingYears)
            {
                response.Add("Dưới " + usingYear + " năm",
                    entities.Where(c => (c.EndDate - c.StartDate).TotalDays <= (usingYear * daysInYear)).Count());
            }
            response.Add("Trên " + usingYears.Last() + " năm",
                entities.Where(c => (c.EndDate - c.StartDate).TotalDays > (usingYears.Last() * daysInYear)).Count());
            return response;
        }

        public Contract Add(Contract contract)
        {
            if (DateTime.Compare(DateTime.Now.Date, contract.StartDate.Date) == 0)
            {
                contract.Status = ContractStatus.Active;
            }
            contract.CreatedDate = DateTime.Now;
            _contractRepository.Add(contract);
            return contract;
        }
        public Contract Recontract(Contract foundContract, DateTime endDate)
        {
            var entity = _contractRepository.GetById(foundContract.ID);
            if (DateTime.Compare(entity.EndDate.Date, endDate.Date) >= 0)
            {
                throw new Exception("Ngày kết thúc không hợp lệ");
            }
            if (entity.Status != ContractStatus.NeedAction)
            {
                throw new Exception("Yêu cầu ở trạng thái:" + ContractStatus.NeedAction);
            }
            var recontractEntity = new Contract
            {
                SalesItemID = entity.SalesItemID,
                Name = entity.Name,
                Price = entity.Price,
                Unit = entity.Unit,
                CustomerID = entity.CustomerID,
                ContactID = entity.ContactID,
                CreatedStaffID = entity.CreatedStaffID,
                StartDate = entity.EndDate.AddDays(1),
                EndDate = endDate
            };
            Add(recontractEntity);
            entity.Status = ContractStatus.Recontracted;
            entity.UpdatedDate = DateTime.Now;
            _contractRepository.Update(entity);
            return recontractEntity;
        }
        public void Close(Contract contract)
        {
            var entity = _contractRepository.GetById(contract.ID);
            List<string> requiredStatus = new List<string>
            {
                ContractStatus.NeedAction,
                ContractStatus.Active
            };
            if (!requiredStatus.Contains(entity.Status))
            {
                throw new Exception("Yêu cầu trạng thái sử dụng dịch vụ:" +
                    String.Join(", ", requiredStatus));
            }
            if (entity.EndDate.Date != contract.EndDate.Date)
            {
                if (entity.Status != ContractStatus.Active)
                {
                    throw new Exception("Không thể thay đổi ngày kết thúc sử dụng dịch vụ");
                }
                if (DateTime.Compare(entity.EndDate.Date, contract.EndDate.Date) >= 0)
                {
                    throw new Exception("Ngày kết thúc không được quá ngày kết thúc sử dụng dịch vụ cũ");
                }
            }
            entity.EndDate = contract.EndDate;
            entity.UpdatedDate = DateTime.Now;
            entity.Status = ContractStatus.Closing;
            _contractRepository.Update(entity);
        }

        public void BackgroundUpdateStatus(int remindDays)
        {
            var entities = GetAll();
            foreach (var entity in entities)
            {
                ChangeContractStatus(entity, remindDays);
                entity.UpdatedDate = DateTime.Now;
                _contractRepository.Update(entity);
            }
        }

        private void ChangeContractStatus(Contract contract, int remindDays)
        {
            if (contract.Status == ContractStatus.Waiting)
            {
                if (DateTime.Compare(DateTime.Now.Date, contract.StartDate.Date) >= 0)
                {
                    contract.Status = ContractStatus.Active;
                }
            }
            if (contract.Status == ContractStatus.Active)
            {
                if ((contract.EndDate - DateTime.Now).TotalDays <= remindDays)
                {
                    contract.Status = ContractStatus.NeedAction;
                }
            }

            if(contract.Status == ContractStatus.NeedAction
                ||contract.Status==ContractStatus.Closing
                || contract.Status == ContractStatus.Recontracted)
            {
                if (DateTime.Compare(DateTime.Now.Date, contract.EndDate.Date) > 0)
                {
                    contract.Status = ContractStatus.Done;
                }
            }
        }

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

    }
}
