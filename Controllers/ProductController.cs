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

        public IActionResult ProductList(string search)
        {
            ProductRepository productsRepository = new ProductRepository();
            List<Product> products = new();
            if (!string.IsNullOrEmpty(search))
            {
                products = productsRepository.Search(search);
            }
            else
            {
                products = productsRepository.Get();
            }
            return View(products);
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

            return View(addProduct);

        }

        public IActionResult Delete(int id)
        {
            ProductRepository productRepository = new ProductRepository();
            return View(productRepository.Get(id));
        }

        public IActionResult Details(int id)
        {
            ProductRepository productRepository = new ProductRepository();
            return View(productRepository.Get(id));
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
            return View(productRepository.Get(id));
        }

        [HttpPost]
        public IActionResult Edit(Product p, IFormFile picture)
        {
            //p.ImageUrl = GetPath(picture);
            //p.ImageName = picture.FileName;
            ProductRepository productRepository = new ProductRepository();
            productRepository.Update(p);
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
        public IActionResult AddProduct(AddProductViewModel model,IFormFile picture)
        {
            if(!ModelState.IsValid) 
            {
                Product p = new();
                //check if product exists
                if (model.Product != null)
                {
                    ProductRepository pRepository = new ProductRepository();
                    p = pRepository.GetProduct(model.Product.Name,model.Product.CategoryID, model.Product.BrandID);
                    if (p.Id == 0)
                    {
                        p = model.Product;
                        p.CreatedAt = DateTime.Now;
                        ProductRepository productRepository = new ProductRepository();
                        productRepository.Add(p);

                    }
                }
                ProductVariant pV = model.Variant;
                pV.ProductID = p.Id;
                pV.CreatedAt = DateTime.Now;
                pV.ImagePath = GetPath(pV.Picture);
                IRepository<ProductVariant> pVRepository = new GenericRepository<ProductVariant>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
                pVRepository.Add(pV);
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
