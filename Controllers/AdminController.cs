using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;

namespace Ecommerce.Controllers
{
    [Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;

        public AdminController(IProductRepository productRepository, ICategoryRepository categoryRepository, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
        }


        public IActionResult Dashboard()
        {
            var orders = _orderRepository.Get();
            ViewBag.TotalSales = orders.Sum(x => x.TotalBill);
            ViewBag.TotalOrders = _orderRepository.Get().Count();
            ViewBag.TotalProducts = _productRepository.Get().Count();
            ViewBag.TotalCategories = _categoryRepository.Get().Count();
            return View();
        }
    }
}
