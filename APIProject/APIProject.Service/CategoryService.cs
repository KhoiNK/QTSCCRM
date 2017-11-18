using APIProject.Data.Infrastructure;
using APIProject.Data.Repositories;
using APIProject.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIProject.Service
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetCategories(string name = null);
        IEnumerable<Category> GetAll();
        Category GetCategory(int id);
        Category GetCategory(string name);
        void CreateCategory(Category category);
        void SaveCategory();
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categorysRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository _categorysRepository, IUnitOfWork _unitOfWork)
        {
            this._categorysRepository = _categorysRepository;
            this._unitOfWork = _unitOfWork;
        }

        #region ICategoryService Members

        public IEnumerable<Category> GetCategories(string name = null)
        {
            if (string.IsNullOrEmpty(name))
                return _categorysRepository.GetAll();
            else
                return _categorysRepository.GetAll().Where(c => c.Name == name);
        }
        public IEnumerable<Category> GetAll()
        {
            var entities = _categorysRepository.GetAll().Where(c => c.IsDelete == false);
            return entities;
        }

        public Category GetCategory(int id)
        {
            var category = _categorysRepository.GetById(id);
            return category;
        }

        public Category GetCategory(string name)
        {
            var category = _categorysRepository.GetCategoryByName(name);
            return category;
        }

        public void CreateCategory(Category category)
        {
            _categorysRepository.Add(category);
        }

        public void SaveCategory()
        {
            _unitOfWork.Commit();
        }

        #endregion
    }
}
