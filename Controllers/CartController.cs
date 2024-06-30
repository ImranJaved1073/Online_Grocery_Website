using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IProductRepository _productRepository;

        public CartController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IProductRepository productRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _productRepository = productRepository;
        }

        [Authorize]
        public IActionResult Cart()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            LoadCartFromCookies(user.Id);
            Cart? cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            if (cart == null || cart.Items.Count == 0)
            {
                cart = new Cart();
                TempData["Message"] = "Your cart is empty";
            }
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(Product p, int id)
        {
            Product product = _productRepository.Get(id);
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                if (product != null)
                {
                    Cart? cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id) ?? new Cart();

                    CartItem cartItem = new CartItem
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = p.Quantity,
                        ImagePath = product.ImagePath,
                        Weight = product.Weight,
                        Unit = product.GetUnitName(product.UnitID)
                    };
                    CartItem? item = cart.Items.Find(x => x.Id == product.Id);
                    if (item == null)
                    {
                        cart.Items.Add(cartItem);
                        cart.TotalPrice += cartItem.Price * cartItem.Quantity;
                        cart.TotalQuantity += cartItem.Quantity;
                    }
                    else
                    {
                        item.Quantity += p.Quantity;
                        cart.TotalPrice += item.Price * p.Quantity;
                        cart.TotalQuantity += p.Quantity;
                    }
                    CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320 , user.Id);
                }
                else return NotFound();
            }
            else return RedirectToAction("Login", "Account");
            return RedirectToAction("ShopItems","Home");
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveFromCart(int id)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Cart? cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart" , user.Id);
            CartItem? cartItem = cart?.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cart.Items.Remove(cartItem);
                CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320 , user.Id);
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateCart(int id, int quantity)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Cart? cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            CartItem? cartItem = cart?.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cartItem.Quantity = quantity;
                cart.TotalPrice += cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity += cartItem.Quantity;
                CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320, user.Id);
            }
            return RedirectToAction("Cart");
        }


        [Authorize]
        public IActionResult CheckOut()
        {
            List<DeliveryDay> deliveryDays = CreateDeliveryDays();
            ViewBag.Days = deliveryDays;
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            LoadCartFromCookies(user.Id);
            Cart? cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            //if cart is empty, redirect to cart page and show alert message your cart is empty
            if (cart == null || cart.Items.Count == 0)
            {
                //TempData["Message"] = "Your cart is empty";
                return RedirectToAction("Cart");
            }
            else
            {
                ViewBag.Cart = cart;
                return View();
            }
        }

        public List<DeliveryDay> CreateDeliveryDays()
        {
            // Create a list of DeliveryDay objects
            List<DeliveryDay> deliveryDays = new List<DeliveryDay>();

            // Loop through each day from the current day to the next 7 days
            for (int i = 0; i < 7; i++)
            {
                // Create a new DeliveryDay object
                DeliveryDay day = new DeliveryDay();

                // Set the name and date of the day
                day.Name = DateTime.Now.AddDays(i).DayOfWeek.ToString();
                day.Date = DateTime.Now.AddDays(i);

                // Create a list of TimeSlot objects for the day
                List<TimeSlot> timeSlots = new List<TimeSlot>();

                var intervals = new (string Name, TimeSpan Start, TimeSpan End)[]
                {
                    ("10:00 AM - 12:00 PM", new TimeSpan(10, 0, 0), new TimeSpan(12, 0, 0)),
                    ("12:00 PM - 02:00 PM", new TimeSpan(12, 0, 0), new TimeSpan(14, 0, 0)),
                    ("03:00 PM - 05:00 PM", new TimeSpan(15, 0, 0), new TimeSpan(17, 0, 0)),
                    ("06:00 PM - 09:00 PM", new TimeSpan(18, 0, 0), new TimeSpan(21, 0, 0))
                };

                // Loop through each hour of the day
                foreach (var interval in intervals)
                {
                    // Create a new TimeSlot object
                    TimeSlot timeSlot = new TimeSlot();

                    // Set the name and price of the time slot
                    timeSlot.Name = $"{interval.Name}";
                    timeSlot.Price = 0.0m; // Set the price to a default value

                    // Add the time slot to the list
                    timeSlots.Add(timeSlot);
                }

                // Add the day to the list
                day.TimeSlots = timeSlots;
                deliveryDays.Add(day);
            }
            if (HttpContext.Session.Keys.Contains("DeliveryDays"))
            {
                var val = HttpContext.Session.GetString("DeliveryDays");
            }
            else
                HttpContext.Session.SetString("DeliveryDays", JsonConvert.SerializeObject(deliveryDays));


            return deliveryDays;
        }

        public IActionResult NotFound()
        {
            return View();
        }


        private void LoadCartFromCookies(string userId)
        {
            var cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart" , userId);
            if (cart != null && cart.UserId == userId)
            {
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
        }

        private void SaveCartToCookies(string userId)
        {
            var cartData = HttpContext.Session.GetString("Cart");
            if (cartData != null)
            {
                var cart = JsonConvert.DeserializeObject<Cart>(cartData);
                cart.UserId = userId;
                CookieHelper.SetCookie(HttpContext, "Cart", cart, 4320 , userId);
            }
        }

    }
}
