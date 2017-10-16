using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public class IssueCategoryMappingRepository : RepositoryBase<IssueCategoryMapping>, IIssueCategoryMappingRepository
    {
        public IssueCategoryMappingRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }

    public interface IIssueCategoryMappingRepository : IRepository<IssueCategoryMapping>
    {

    }
}
