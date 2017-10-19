using APIProject.Data.Repositories;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface IRoleService
    {
        Role GetByID(int roleID);
    }
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository _roleRepository)
        {
            this._roleRepository = _roleRepository;
        }

        public Role GetByID(int roleID)
        {
            return _roleRepository.GetById(roleID);
        }
    }
}
