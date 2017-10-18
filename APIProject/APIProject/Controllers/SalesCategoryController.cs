using APIProject.Service;
using APIProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace APIProject.Controllers
{
    [RoutePrefix("api/salescategory")]
    public class SalesCategoryController : ApiController
    {
        private readonly ISalesCategoryService _salesCategoryService;

        public SalesCategoryController(ISalesCategoryService _salesCategoryService)
        {
            this._salesCategoryService = _salesCategoryService;
        }

        [Route("GetSalesCategories")]
        public IHttpActionResult GetSalesCategories()
        {
            return Ok(_salesCategoryService.GetAllCategories().Select(c => new SalesCategoryViewModel(c)));
        }

        [Route("GetOpportunitySalesCategories")]
        [ResponseType(typeof(SalesCategoryViewModel))]
        public IHttpActionResult GetOpportunitySalesCategories(int opportunityID = 0)
        {
            if(opportunityID == 0)
            {
                return BadRequest();
            }
            var foundCategories = _salesCategoryService.GetByOpportunity(opportunityID);
            if(foundCategories != null)
            {
                return Ok(foundCategories.Select(c => new SalesCategoryViewModel(c)));
            }
            else
            {
                return NotFound();
            }

        }
    }
}
