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
        IEnumerable<ContractItem> GetByContract(int contractID);
        ContractItem Add(ContractItem contractItem);
        void AddRange(Contract contract, List<ContractItem> contractItems);

    }
    public class ContractItemService:IContractItemService
    {
        private readonly IContractItemRepository _contractItemRepository;
        private readonly IAppConfigRepository _appConfigRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContractItemService(IContractItemRepository _contractItemRepository,
            IAppConfigRepository _appConfigRepository,
            IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
            this._contractItemRepository = _contractItemRepository;
            this._appConfigRepository = _appConfigRepository;
        }
        public IEnumerable<ContractItem> GetByContract(int contractID)
        {
            var entities = _contractItemRepository.GetAll()
                .Where(c => c.ContractID == contractID && c.IsDelete == false);
            return entities;
        }

        public ContractItem Add(ContractItem contractItem)
        {
            _contractItemRepository.Add(contractItem);
            return contractItem;
        }
        public void AddRange(Contract contract, List<ContractItem> contractItems)
        {
            int count = 1;
            var contractItemCode = contract.ContractCode
                + _appConfigRepository.GetContractItemCode();
            foreach(var item in contractItems)
            {
                item.ItemCode = contractItemCode + count.ToString("000");
                Add(item);
            }
        }



    }
}
