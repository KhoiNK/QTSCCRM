using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace APIProject.Controllers
{
    [RoutePrefix("api/role")]
    public class RoleController : ApiController
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService _roleService)
        {
            this._roleService = _roleService;
        }

        [Route("GetRoles")]
        public IHttpActionResult GetRoles()
        {
            var roles = _roleService.GetAll();
            var response = roles.Select(c => new RoleViewModel
            {
                Name = c.Name,
                ID = c.ID
            });
            return Ok(response);
        }

    }
}
