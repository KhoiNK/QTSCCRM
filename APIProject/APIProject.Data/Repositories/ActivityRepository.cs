using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public class ActivityRepository : RepositoryBase<Activity>, IActivityRepository
    {
        public ActivityRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public Activity GetLastOppActivty(int id)
        {
            var activities = this.DbContext.Activities.Where(c => c.OpportunityID == id).OrderByDescending(c=>c.ID);
            return activities.FirstOrDefault();
        }
    }

    public interface IActivityRepository : IRepository<Activity>
    {
        Activity GetLastOppActivty(int id);
    }
}
