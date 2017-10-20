using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public interface IOpportunityCategoryMappingRepository : IRepository<OpportunityCategoryMapping>
    {

    }

    public class OpportunityCategoryMappingRepository : RepositoryBase<OpportunityCategoryMapping>, IOpportunityCategoryMappingRepository
    {
        public OpportunityCategoryMappingRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
