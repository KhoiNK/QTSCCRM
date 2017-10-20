using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public interface ISalesItemRepository: IRepository<SalesItem>
    {

    }
    public class SalesItemRepository : RepositoryBase<SalesItem>, ISalesItemRepository
    {
        public SalesItemRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
