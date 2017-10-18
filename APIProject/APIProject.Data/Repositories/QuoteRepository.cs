using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public interface IQuoteRepository: IRepository<Quote>
    {
        Quote GetLatestQuoteByOpportunity(int opportunityID);
    }
    public class QuoteRepository : RepositoryBase<Quote>, IQuoteRepository
    {
        public QuoteRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public Quote GetLatestQuoteByOpportunity(int opportunityID)
        {
            return this.DbContext.Quotes.Where(c => c.OpportunityID == opportunityID).OrderByDescending(x=>x.ID).FirstOrDefault();
        }
    }
}
