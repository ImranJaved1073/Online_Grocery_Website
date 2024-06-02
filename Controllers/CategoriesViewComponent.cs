using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class CategoriesViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            CategoryRepository _categoryRepository = new CategoryRepository();
            List<Category> parents = _categoryRepository.GetCategoriesWithSubCategories();
            List<SubCategoryViewModel> subCategoryViewModels = new List<SubCategoryViewModel>();
            foreach (Category category in parents)
            {
                SubCategoryViewModel subCategory = new SubCategoryViewModel();
                subCategory.Category = category;
                subCategory.GetSubCategories(category.Id);
                subCategoryViewModels.Add(subCategory);
            }
            return View(subCategoryViewModels);
        }


    }
}
