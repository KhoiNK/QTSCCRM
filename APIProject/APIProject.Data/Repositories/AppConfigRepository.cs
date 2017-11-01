using APIProject.Data.Infrastructure;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Data.Repositories
{
    public interface IAppConfigRepository: IRepository<AppConfig>
    {
        string GetHost();
        string GetContractCode();
        string GetContractItemCode();
    }
    public class AppConfigRepository : RepositoryBase<AppConfig>, IAppConfigRepository
    {
        public AppConfigRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }

        public string GetHost()
        {
            AppConfig hostName = this.DbContext.AppConfigs.Where(x => x.Name == "Host").SingleOrDefault();
            return hostName.Value;
        }
        public string GetContractCode()
        {
            AppConfig hostName = this.DbContext.AppConfigs.Where(x => x.Name == "ContractCode").SingleOrDefault();
            return hostName.Value;
        }
        public string GetContractItemCode()
        {
            AppConfig hostName = this.DbContext.AppConfigs.Where(x => x.Name == "ContractCode").SingleOrDefault();
            return hostName.Value;
        }
    }
}
