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
        IEnumerable<OpportunityCategoryMapping> GetByOpportunity(int opportunityID);
    }

    public class OpportunityCategoryMappingRepository : RepositoryBase<OpportunityCategoryMapping>, IOpportunityCategoryMappingRepository
    {
        public OpportunityCategoryMappingRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public IEnumerable<OpportunityCategoryMapping> GetByOpportunity(int opportunityID)
        {
            return this.DbContext.OpportunityCategoryMappings.Where(c => c.OpportunityID == opportunityID && c.IsDeleted== false);
        }
    }
}
