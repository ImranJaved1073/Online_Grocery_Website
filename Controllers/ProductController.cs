using Ecommerce.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult ProductList(string search)
        {
            ProductRepository productsRepository = new ProductRepository();
            List<Product> products = new();
            if (!string.IsNullOrEmpty(search))
            {
                products = productsRepository.SearchProducts(search);
            }
            else
            {
                products = productsRepository.GetAllProducts();
            }
            return View(products);
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        public IActionResult Delete(int id)
        {
            ProductRepository productRepository = new ProductRepository();
            return View(productRepository.GetProductByID(id));
        }

        public IActionResult Details(int id)
        {
            ProductRepository productRepository = new ProductRepository();
            return View(productRepository.GetProductByID(id));
        }

        [HttpPost]
        public IActionResult Delete(Product p)
        {
            ProductRepository productRepository = new ProductRepository();
            productRepository.Delete(p);
            return RedirectToAction("ProductList", "Product");
        }

        public IActionResult Edit(int id)
        {
            ProductRepository productRepository = new ProductRepository();
            return View(productRepository.GetProductByID(id));
        }

        [HttpPost]
        public IActionResult Edit(Product p, IFormFile picture)
        {
            p.ImageUrl = GetPath(picture);
            p.ImageName = picture.FileName;
            ProductRepository productRepository = new ProductRepository();
            productRepository.Edit(p);
            return RedirectToAction("ProductList", "Product");
        }

        //[HttpPost]
        //public IActionResult Search(string search)
        //{
        //    ProductRepository productsRepository = new ProductRepository();
        //    List<Product> products = productsRepository.GetSearchProducts(search);
        //    return View("ProductList", products);

        //}

        public string GetPath(IFormFile picture)
        {
            string wwwrootPath = _env.WebRootPath;
            string path = Path.Combine(Path.Combine(wwwrootPath, "images"), "products");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (picture != null && picture.Length > 0)
            {
                path = Path.Combine(path, picture.FileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    picture.CopyTo(fileStream);
                }
            }
            return path;
        }

        [HttpPost]
        public IActionResult AddProduct(Product p, IFormFile picture)
        {
            try
            {
                p.CreatedAt = DateTime.Now;
                p.ImageUrl = GetPath(picture);
                p.ImageName = picture.FileName;

                ProductRepository productRepository = new ProductRepository();
                productRepository.Add(p);

                return RedirectToAction("ProductList", "Product");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
    }
}
