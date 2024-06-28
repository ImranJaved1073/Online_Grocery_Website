using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing.Printing;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            CategoryRepository _categoryRepository = new CategoryRepository();
            List<Category> nonparents = _categoryRepository.GetNonParentCategories();
            List<Product> products = new List<Product>();
            IRepository<Product> productRepository = new GenericRepository<Product>(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True");
            products = productRepository.Get().ToList();
            //getting top 10 products in last 7 days 
            products = products.OrderByDescending(p => p.CreatedAt > DateTime.Now.AddDays(-7)).Take(10).ToList();
            ViewBag.Products = products;
            return View(nonparents);
        }

        public IActionResult ShopItems(int? id,int pageNumber,int pageSize,string view)
        {
            CategoryRepository _categoryRepository = new CategoryRepository();
            List<Category> parents = _categoryRepository.GetCategoriesWithSubCategories();
            List<SubCategoryViewModel> subCategoryViewModels = new List<SubCategoryViewModel>();
            List<Product> products = new List<Product>();
            foreach (Category category in parents) 
            {
                SubCategoryViewModel subCategory = new SubCategoryViewModel();
                subCategory.Category = category;
                subCategory.GetSubCategories(category.Id);
                subCategoryViewModels.Add(subCategory);
            }

            if (id != null)
            {
                ProductRepository _productRepository = new ProductRepository();
                products = _productRepository.GetProductsByCategory((int)id);
                IRepository<Category> rep = new GenericRepository<Category>(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True");
                string name = rep.Get((int)id).CategoryName;
                if (name != null)
                    TempData["CategoryName"] = name;
            }
            else
            {
                IRepository<Product> productRepository = new GenericRepository<Product>(@"Data Source=(localdb)\ProjectModels;Initial Catalog=GroceryDb;Integrated Security=True;Trust Server Certificate=True");
                products = productRepository.Get().ToList();
            }
            ViewBag.Categories = subCategoryViewModels;

            if (pageSize <=0)
            {
                pageSize = 15;
            }

            int totalItems = products.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (pageNumber < 1)
                pageNumber = 1;
            var pages = new PaginatedList(pageNumber, totalPages, pageSize, totalItems);
            var data = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Pages = pages;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.View = view;

            return View(data);
        }


        public IActionResult ShopProduct(int id)
        {
            //SubCategoryViewModel subCategory = new SubCategoryViewModel();
            //subCategory.GetSubCategories(categoryId);
            //subCategory.GetProducts(categoryId);
            //return View(subCategory);
            ProductRepository pr = new ProductRepository();
            Product product = pr.Get(id);
            return View(product);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult ContactUS()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
