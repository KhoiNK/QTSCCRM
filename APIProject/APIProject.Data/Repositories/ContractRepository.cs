using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public interface IContractRepository : IRepository<Contract>
    {

    }
    public class ContractRepository : RepositoryBase<Contract>, IContractRepository
    {
        public ContractRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
