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
    [RoutePrefix("api/salesitem")]
    public class SalesItemController : ApiController
    {
        private readonly ISalesItemService _salesItemService;
        public SalesItemController(ISalesItemService _salesItemService)
        {
            this._salesItemService = _salesItemService;
        }

        [Route("GetSalesCategoryItems")]
        [ResponseType(typeof(SalesItemViewModel))]
        public IHttpActionResult GetSalesCategoryItems(int categoryID = 0)
        {
            if(categoryID == 0)
            {
                return BadRequest();
            }

            var foundItems = _salesItemService.GetByCategory(categoryID);
            if (foundItems != null)
            {
                return Ok(foundItems.Select(c => new SalesItemViewModel(c)));
            }

            return NotFound();
        }

        [Route("PostSalesItem")]
        public IHttpActionResult PostSalesItem(PostSalesItemViewModel request)
        {
            if(!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }

            int insertedID = _salesItemService.CreateNewItem(request.ToSalesItemModel());
            return Ok(insertedID);
        }
    }
}
