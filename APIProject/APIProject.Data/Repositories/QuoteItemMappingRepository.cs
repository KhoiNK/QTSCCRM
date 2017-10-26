using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public class QuoteItemMappingRepository : RepositoryBase<QuoteItemMapping>, IQuoteItemMappingRepository
    {
        public QuoteItemMappingRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public QuoteItemMapping GetBySalesItemID(int salesItemID)
        {
            return this.DbContext.QuoteItemMappings.Where(c => c.SalesItemID == salesItemID
            && c.IsDelete == false).SingleOrDefault();
        }
    }
    public interface IQuoteItemMappingRepository : IRepository<QuoteItemMapping>
    {
        QuoteItemMapping GetBySalesItemID(int salesItemID);
    }
}
