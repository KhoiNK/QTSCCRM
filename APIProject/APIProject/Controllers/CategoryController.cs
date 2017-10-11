using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using APIProject.Service;
using APIProject.ViewModels;

namespace APIProject.Controllers
{
    public class CategoryController : ApiController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService _categoryService)
        {
            this._categoryService = _categoryService;
        }
        // GET: api/Category
        public IEnumerable<CategoryViewModel> Get()
        {
            return _categoryService.GetCategories().Where(c => !c.isDelete).Select(c => new CategoryViewModel() { Id = c.Id, Name = c.Name});
        }

        // POST: api/Category
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Category/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Category/5
        public void Delete(int id)
        {
        }
    }
}
