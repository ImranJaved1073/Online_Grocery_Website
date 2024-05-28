using Ecommerce.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using static NuGet.Packaging.PackagingConstants;

namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ProductController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult ProductList(string search,int pageNumber)
        {
            ProductRepository productsRepository = new ProductRepository();
            CategoryRepository categoryRepository = new();
            IRepository<Brand> brandRepository = new GenericRepository<Brand>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
            List<Product> products = new();
            Category category = new();
            Brand brand = new();
            if (!string.IsNullOrEmpty(search))
            {
                products = productsRepository.Search(search).ToList();
            }
            else
            {
                products = productsRepository.Get().ToList();
            }

            foreach (var product in products)
            {
                category = categoryRepository.Get(product.CategoryID);
                brand = brandRepository.Get(product.BrandID);
                product.CategoryName = category.CategoryName;
                product.BrandName = brand.BrandName;
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

            ProductRepository productRepository = new ProductRepository();
            Product? product = productRepository.Get(id);

            // Check if the product exists
            if (product == null)
            {
                // Product does not exist, show an alert
                TempData["ProductNotFound"] = "Product not found";
                return RedirectToAction("ProductList", "Product");
            }

            CategoryRepository categoryRepository = new CategoryRepository();
            IRepository<Brand> brandRepository = new GenericRepository<Brand>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
            
            product.CategoryName = categoryRepository.Get(product.CategoryID).CategoryName;
            product.BrandName = brandRepository.Get(product.BrandID).BrandName;

            //showing error message if product already exists
            if (TempData["ProductExists"] != null)
            {
                ViewBag.Alert = TempData["ProductExists"];
            }

            return View(product);

        }


        public IActionResult AddProduct()
        {
            CategoryRepository categoryRepository = new CategoryRepository();
            List<Category> categories = categoryRepository.GetNonParentCategories().ToList();
            IRepository<Brand> brandRepository = new GenericRepository<Brand>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
            List<Brand> brands = brandRepository.Get().ToList();

            AddProductViewModel addProduct = new AddProductViewModel();
            addProduct.Categories = new SelectList(categories, "Id", "CategoryName");
            addProduct.Brands = new SelectList(brands, "Id", "BrandName");

            //showing error message if product already exists
            if (TempData["ProductExists"] != null)
            {
                ViewBag.Alert = TempData["ProductExists"];
            }

            return View(addProduct);

        }

        public IActionResult Edit(int id)
        {

            ProductRepository productRepository = new ProductRepository();
            Product product = productRepository.Get(id);

            // Check if the product exists
            if (product == null)
            {
                // Product does not exist, show an alert
                TempData["ProductNotFound"] = "Product not found";
                return RedirectToAction("ProductList", "Product");
            }

            CategoryRepository categoryRepository = new CategoryRepository();
            List<Category> categories = categoryRepository.GetNonParentCategories().ToList();
            IRepository<Brand> brandRepository = new GenericRepository<Brand>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
            List<Brand> brands = brandRepository.Get().ToList();

            AddProductViewModel addProduct = new AddProductViewModel
            {
                Categories = new SelectList(categories, "Id", "CategoryName"),
                Brands = new SelectList(brands, "Id", "BrandName"),
                Product = product
            };

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
                // Update the product
                ProductRepository productRepository = new ProductRepository();

                // Update product image if a new one is uploaded
                if (p.Product.Picture != null)
                {
                    p.Product.ImagePath = GetPath(p.Product.Picture);
                }

                // Save changes to the repository
                productRepository.Update(p.Product);

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
            ProductRepository productRepository = new ProductRepository();
            Product? product = productRepository.Get(id);

            // Check if the product exists
            if (product == null)
            {
                // Product does not exist, show an alert
                TempData["ProductNotFound"] = "Product not found";
                return RedirectToAction("ProductList", "Product");
            }

            productRepository.Delete(product);
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
                    ProductRepository productRepository = new ProductRepository();
                    product = productRepository.GetProduct(model.Product.Name, model.Product.CategoryID, model.Product.BrandID);

                    if (product.Id == 0)
                    {
                        // Product does not exist, add new product
                        product = model.Product;
                        product.CreatedAt = DateTime.Now;
                        product.ImagePath = GetPath(model.Product.Picture);
                        productRepository.Add(product);

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

            

            //p.CreatedAt = DateTime.Now;
            //p.ImageUrl = GetPath(picture);
            //p.ImageName = picture.FileName;

            //ProductRepository productRepository = new ProductRepository();
            //productRepository.Add(p);

            //return RedirectToAction("ProductList", "Product");
        }
    }
}
