using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public interface IContractItemRepository : IRepository<ContractItem>
    {

    }
    public class ContractItemRepository : RepositoryBase<ContractItem>, IContractItemRepository
    {
        public ContractItemRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
