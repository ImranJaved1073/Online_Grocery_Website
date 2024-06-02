using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Ecommerce.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public OrderController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddOrder(CheckOut odrs)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
                if (cart != null)
                {
                    Orders order = new Orders
                    {
                        UserId = user.Id,
                        OrderNum = GenerateOrderNumber(),
                        OrderDate = DateTime.Now,
                        Status = "Pending",
                        TotalBill = cart.TotalPrice,
                        //TotalDiscount = cart.TotalDiscount,
                        CheckOut = new CheckOut
                        {
                            OrderDeliveryDate = DateTime.Today.AddDays(1),
                            PaymentMethod = "Cash on Delivery",
                        },
                        //PaymentStatus = "Pending",
                        ShipAddress = new ShipAddress
                        {
                            Address = "samanabd",
                            City = "Lahore",
                            Country = "Pakistan",
                            ZipCode = "7777",
                            State = "Punjab",
                        },
                        OrderDetails = cart.Items.Select(x => new OrderDetail
                        {
                            ProductId = x.Id,
                            Price = x.Price,
                            Quantity = x.Quantity,
                            Discount = 0,
                            TotalPrice = x.Price * x.Quantity,
                        }).ToList()
                    };

                    OrderRepository oR = new OrderRepository();
                    oR.Add(order);

                    IRepository<OrderDetail> oDR = new GenericRepository<OrderDetail>(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=newDb;Integrated Security=True;Trust Server Certificate=True");
                    foreach (var item in order.OrderDetails)
                    {
                        item.OrderId = oR.Get(order.OrderNum).Id;
                        oDR.Add(item);
                    }

                    CookieHelper.ClearCartCookies(HttpContext, user.Id);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [Authorize]
        public IActionResult Order(string statusFilter, DateTime? startDate, DateTime? endDate)
        {
            var user = _userManager.GetUserAsync(User).Result;
            OrderRepository oR = new OrderRepository();
            var orders = oR.Get();
            var status = orders.Select(o => o.Status).Distinct().ToList();
            ViewBag.StatusOptions = status;
            orders = orders.Where(x => x.UserId == user.Id).ToList();
            if (!string.IsNullOrEmpty(statusFilter))
            {
                orders = orders.Where(x => x.Status == statusFilter).ToList();
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                orders = orders.Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate).ToList();
            }

            orders = orders.ToList();


            return View(orders);

        }

        public static string GenerateOrderNumber()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string randomComponent = new Random().Next(1000, 9999).ToString();
            return $"{timestamp}{randomComponent}";
        }
    }
}
