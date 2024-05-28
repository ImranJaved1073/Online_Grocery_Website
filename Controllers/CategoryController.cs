using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public CategoryController(IWebHostEnvironment env)
        {
            _env = env;
        }
        public IActionResult List(string search, int pageNumber)
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            List<Category> categories = new();
            if (!string.IsNullOrEmpty(search))
            {
                categories = categoryRepository.Search(search).ToList();
            }
            else
            {
                categories = categoryRepository.GetParents().ToList();
            }
            const int pageSize = 5;
            //int excludeRecords = (pageSize * pageNumer) - pageSize;
            //var totalRecords = categories.Count;
            //var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            //categories = categories.Skip(excludeRecords).Take(pageSize).ToList();
            //ViewBag.TotalPages = totalPages;
            //ViewBag.PageNumber = pageNumer;
            //return View(categories);

            var rescCount = categories.Count();
            var totalPages = (int)Math.Ceiling((double)rescCount / pageSize);
            if (pageNumber < 1)
                pageNumber = 1;
            var pager = new PaginatedList(pageNumber, totalPages, pageSize, rescCount);
            var data = categories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);

        }

        public IActionResult Create()
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            List<Category> categories = categoryRepository.GetNames().ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category c)
        {
            if (c.CategoryImg != null)
                c.ImgPath = GetPath(c.CategoryImg);
            CategoryRepository categoryRepository = new CategoryRepository();
            categoryRepository.Add(c);
            return RedirectToAction("List", "Category");
        }

        private string GetPath(IFormFile picture)
        {
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(wwwrootPath, "images", "categories");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string UniqueFileName = Guid.NewGuid().ToString() + "_" + picture.FileName;
            if (picture != null && picture.Length > 0)
            {
                path = Path.Combine(path, UniqueFileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    picture.CopyTo(fileStream);
                }
            }
            return Path.Combine("images", "categories", UniqueFileName);
        }


        public IActionResult Delete(int id)
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            Category c = categoryRepository.Get(id);
            categoryRepository.Delete(c);
            return RedirectToAction("List", "Category");
        }

        public IActionResult Edit(int id)
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            List<Category> categories = categoryRepository.GetNames().ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            return View(categoryRepository.Get(id));
        }

        [HttpPost]

        public IActionResult Edit(Category c)
        {
            if (c.CategoryImg != null)
                c.ImgPath = GetPath(c.CategoryImg);
            CategoryRepository categoryRepository = new CategoryRepository();
            categoryRepository.Update(c);
            return RedirectToAction("List", "Category");
        }
    }
}
