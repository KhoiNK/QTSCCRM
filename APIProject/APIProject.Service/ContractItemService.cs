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
    public interface IContractItemService
    {
        ContractItem Add(ContractItem contractItem);
    }
    public class ContractItemService:IContractItemService
    {
        private readonly IContractItemRepository _contractItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContractItemService(IContractItemRepository _contractItemRepository,
            IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
            this._contractItemRepository = _contractItemRepository;
        }
        public ContractItem Add(ContractItem contractItem)
        {
            _contractItemRepository.Add(contractItem);
            _unitOfWork.Commit();
            return contractItem;
        }


    }
}
