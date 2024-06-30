using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using static NuGet.Packaging.PackagingConstants;

namespace Ecommerce.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Unit> _unitRepository;

        public ProductController(
            IWebHostEnvironment env,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IRepository<Brand> brandRepository,
            IRepository<Unit> unitRepository)
        {
            _env = env;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
            _unitRepository = unitRepository;
        }

        public IActionResult ProductList(string search,int pageNumber)
        {
            List<Product> products = new();
            if (!string.IsNullOrEmpty(search))
            {
                products = _productRepository.Search(search).ToList();
            }
            else
            {
                products = _productRepository.Get().ToList();
            }

            foreach (var product in products)
            {
                product.CategoryName = _categoryRepository.Get(product.CategoryID).CategoryName;
                product.BrandName = _brandRepository.Get(product.BrandID).BrandName;
            }

            const int pageSize = 5;
            var totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            var paginatedProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var pager = new PaginatedList(pageNumber, totalPages, pageSize, totalRecords);

            ViewBag.Pager = pager;
            return View(paginatedProducts);
        }

        public IActionResult Details(int id)
        {
            Product? product = _productRepository.Get(id);

            // Check if the product exists
            if (product == null)
            {
                // Product does not exist, show an alert
                TempData["ProductNotFound"] = "Product not found";
                return RedirectToAction("ProductList", "Product");
            }

            product.CategoryName = _categoryRepository.Get(product.CategoryID).CategoryName;
            product.BrandName = _brandRepository.Get(product.BrandID).BrandName;
            product.UnitName = _unitRepository.Get(product.UnitID).Name;

            //showing error message if product already exists
            if (TempData["ProductExists"] != null)
            {
                ViewBag.Alert = TempData["ProductExists"];
            }

            return View(product);

        }


        public IActionResult AddProduct(int id)
        {
            AddProductViewModel addProduct = new AddProductViewModel();
            if (id != 0)
            {
                Product product = _productRepository.Get(id);

                // Check if the product exists
                if (product == null)
                {
                    // Product does not exist, show an alert
                    TempData["ProductNotFound"] = "Product not found";
                    return RedirectToAction("ProductList", "Product");
                }
                addProduct.Product = product;
            }

            List<Category> categories = _categoryRepository.GetNonParentCategories().ToList();
            List<Brand> brands = _brandRepository.Get().ToList();
            List<Unit> units = _unitRepository.Get().ToList();

            addProduct.Categories = new SelectList(categories, "Id", "CategoryName");
            addProduct.Brands = new SelectList(brands, "Id", "BrandName");
            addProduct.Units = new SelectList(units, "Id", "Name");

            //showing error message if product already exists
            if (TempData["ProductExists"] != null)
            {
                ViewBag.Alert = TempData["ProductExists"];
            }

            return View(addProduct);
        }


        //[HttpPost]
        //public IActionResult Edit(Product p, IFormFile picture)
        //{
        //    //p.ImageUrl = GetPath(picture);
        //    //p.ImageName = picture.FileName;
        //    ProductRepository productRepository = new ProductRepository();
        //    productRepository.Update(p);
        //    return RedirectToAction("ProductList", "Product");
        //}

        [HttpPost]
        public IActionResult Edit(AddProductViewModel p)
        {
            if (!ModelState.IsValid)
            {
                if (p.Product.Picture != null)
                {
                    p.Product.ImagePath = GetPath(p.Product.Picture);
                }

                // Save changes to the repository
                _productRepository.Update(p.Product);

                return RedirectToAction("ProductList", "Product");
            }
            else
            {
                // If model state is not valid, return to the edit view with validation errors
                return View(p.Product);
            }
        }

        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Product? product = _productRepository.Get(id);

            // Check if the product exists
            if (product == null)
            {
                // Product does not exist, show an alert
                TempData["ProductNotFound"] = "Product not found";
                return RedirectToAction("ProductList", "Product");
            }

            _productRepository.Delete(id);
            return RedirectToAction("ProductList", "Product");
        }

        //[HttpPost]
        //public IActionResult Search(string search)
        //{
        //    ProductRepository productsRepository = new ProductRepository();
        //    List<Product> products = productsRepository.GetSearchProducts(search);
        //    return View("ProductList", products);

        //}

        private string GetPath(IFormFile picture)
        {
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(wwwrootPath, "images", "products");
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
            return Path.Combine("images", "products", UniqueFileName);
        }

        [HttpPost]
        public IActionResult AddProduct(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Product product = new();

                if (model.Product != null)
                {
                    product = _productRepository.GetProduct(model.Product.Name, model.Product.CategoryID, model.Product.BrandID);

                    if (product.Id == 0)
                    {
                        // Product does not exist, add new product
                        product = model.Product;
                        product.CreatedAt = string.IsNullOrEmpty(model.Product.CreatedAt.ToString()) ? DateTime.Now : model.Product.CreatedAt;
                        product.UpdatedAt = string.IsNullOrEmpty(model.Product.UpdatedAt.ToString()) ? DateTime.Now : model.Product.UpdatedAt;
                        product.ImagePath = GetPath(model.Product.Picture);
                        _productRepository.Add(product);

                        // Retrieve the product's ID after adding
                        //product.Id = productRepository.GetProduct(model.Product.Name, model.Product.CategoryID, model.Product.BrandID).Id;
                    }
                    else
                    {
                        // Product already exists, show an alert
                        TempData["ProductExists"] = "Product already exists";
                        return RedirectToAction("AddProduct", "Product");
                    }
                }
            }
            else
            {
                TempData["InvalidModel"] = "Invalid product details provided";
                return RedirectToAction("AddProduct", "Product");
            }

            return RedirectToAction("ProductList", "Product");
        }
    }
}
