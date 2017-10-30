using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface IContractService
    {

    }
    public class ContractService:IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ContractService(IUnitOfWork _unitOfWork,
            IContractRepository _contractRepository)
        {
            this._unitOfWork = _unitOfWork;
            this._contractRepository = _contractRepository;
        }
    }
}
