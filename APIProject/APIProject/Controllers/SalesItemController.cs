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
        private readonly IStaffService _staffService;
        public SalesItemController(ISalesItemService _salesItemService,
            IStaffService _staffService)
        {
            this._salesItemService = _salesItemService;
            this._staffService = _staffService;
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
        [Route("PutSalesItem")]
        [HttpPost]
        public IHttpActionResult PutSalesItem(PutSalesItemViewModel request)
        {
            if (!ModelState.IsValid || request == null)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var foundItem = _salesItemService.Get(request.ID);
                var foundStaff = _staffService.Get(request.StaffID);
                _salesItemService.UpdateInfo(request.ToSalesItemModel());
                return Ok(new { ItemUpdated = true });
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
