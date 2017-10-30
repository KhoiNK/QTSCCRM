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
        Contract Add(Contract contract);
    }
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ContractService(IUnitOfWork _unitOfWork,
            IContractRepository _contractRepository)
        {
            this._unitOfWork = _unitOfWork;
            this._contractRepository = _contractRepository;
        }

        public Contract Add(Contract contract)
        {
            var entity = new Contract
            {
                CreatedStaffID = contract.CreatedStaffID,
                SalesCategoryID = contract.SalesCategoryID,
                Status = ContractStatus.Waiting,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ContractCode = Guid.NewGuid().ToString()
            };
            _contractRepository.Add(entity);
            _unitOfWork.Commit();
            return entity;
        }

    }
}
