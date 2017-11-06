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
        Contract Add(Contract contract);
        void SaveChanges();
    }
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IAppConfigRepository _appConfigRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ContractService(IUnitOfWork _unitOfWork,
            IAppConfigRepository _appConfigRepository,
            IContractRepository _contractRepository)
        {
            this._unitOfWork = _unitOfWork;
            this._appConfigRepository = _appConfigRepository;
            this._contractRepository = _contractRepository;
        }

        public Contract Get(int contractID)
        {
            var entity = _contractRepository.GetById(contractID);
            if(entity != null)
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
            var entities = _contractRepository.GetAll().Where(c=>c.IsDelete==false);
            return entities;
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

        public void SaveChanges()
        {
            _unitOfWork.Commit();
        }

    }
}
