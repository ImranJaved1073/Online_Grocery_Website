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
            VariantRepository variantRepository = new VariantRepository();
            CategoryRepository categoryRepository = new();
            IRepository<Brand> brandRepository = new GenericRepository<Brand>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
            List<Product> products = new();
            List<ProductVariant> variants = new();
            List<AddProductViewModel> addProducts = new();
            Category category = new();
            Brand brand = new();
            if (!string.IsNullOrEmpty(search))
            {
                products = productsRepository.Search(search).ToList();
                foreach (var product in products)
                {
                    category = categoryRepository.Get(product.CategoryID);
                    brand = brandRepository.Get(product.BrandID);
                    product.CategoryName = category.CategoryName;
                    product.BrandName = brand.BrandName;
                    AddProductViewModel addProduct = new();
                    addProduct.Product = product;
                    variants = variantRepository.GetVariants(product.Id);
                    foreach (var variant in variants)
                    {
                        addProduct.Variant = variant;
                        addProducts.Add(addProduct);
                    }
                }
            }
            else
            {
                products = productsRepository.Get().ToList();
                foreach (var product in products)
                {
                    category = categoryRepository.Get(product.CategoryID);
                    brand = brandRepository.Get(product.BrandID);
                    product.CategoryName = category.CategoryName;
                    product.BrandName = brand.BrandName;
                    AddProductViewModel addProduct = new();
                    addProduct.Product = product;
                    variants = variantRepository.GetVariants(product.Id);
                    foreach (var variant in variants)
                    {
                        addProduct.Variant = variant;
                        addProducts.Add(addProduct);
                    }
                }
            }
            const int pageSize = 5;
            var rescCount = addProducts.Count();
            var totalPages = (int)Math.Ceiling((double)rescCount / pageSize);
            pageNumber = totalPages;
            var pager = new PaginatedList(pageNumber, totalPages, pageSize, rescCount);
            var data = addProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            ViewBag.Pager = pager;
            return View(data);
        }

        //public IActionResult Details(int id)
        //{
        //    //ProductRepository productRepository = new ProductRepository();
        //    //return View(productRepository.Get(id));

        //    ProductRepository productRepository = new ProductRepository();
        //    ProductVariant productVariant = new ProductVariant();
        //    productVariant.ProductID = id;
        //    VariantRepository variantRepository = new VariantRepository();
        //    List<ProductVariant> variants = variantRepository.GetVariants(id);
        //}


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
            return View(productRepository.Get(id));
            //CategoryRepository categoryRepository = new CategoryRepository();
            //List<Category> categories = categoryRepository.GetNonParentCategories().ToList();
            //IRepository<Brand> brandRepository = new GenericRepository<Brand>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
            //List<Brand> brands = brandRepository.Get().ToList();

            //ProductRepository productRepository = new ProductRepository();
            //ProductVariant productVariant = new ProductVariant();
            //productVariant.ProductID = id;
            //VariantRepository variantRepository = new VariantRepository();
            //List<ProductVariant> variants = variantRepository.GetVariants(id);
            //Product product = productRepository.Get(id);
            //AddProductViewModel addProduct = new AddProductViewModel();
            //addProduct.Product = product;
            //addProduct.Variant = productVariant;
            //addProduct.Categories = new SelectList(categories, "Id", "CategoryName");
            //addProduct.Brands = new SelectList(brands, "Id", "BrandName");
            //addProduct.Variants = variants;
            //return View(addProduct);
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

        public IActionResult Delete(int id)
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
                        p.Id = productRepository.GetProduct(model.Product.Name, model.Product.CategoryID, model.Product.BrandID).Id;
                    }
                }
                ProductVariant pV = new();
                VariantRepository pVRepository = new VariantRepository();
                if(p.Id != 0)
                    pV = pVRepository.GetProductVariant(model.Variant.Size, model.Variant.Color,model.Variant.ProductCode,p.Id);
                else 
                    pV = new ProductVariant();
                if (pV.Id == 0)
                {
                    pV = model.Variant;
                    pV.ProductID = p.Id;
                    pV.CreatedAt = DateTime.Now;
                    pV.ImagePath = GetPath(pV.Picture);
                    pVRepository.Add(pV);
                }
                //else display alert that product already exists
                else
                {
                    TempData["ProductExists"] = "Product already exists";
                    return RedirectToAction("AddProduct", "Product");
                }

                //ProductVariant pV = model.Variant;
                //pV.ProductID = p.Id;
                //pV.CreatedAt = DateTime.Now;
                //pV.ImagePath = GetPath(pV.Picture);
                //pVRepository.Add(pV);
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
