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

        public CartController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public IActionResult Cart()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user == null) {
                return RedirectToAction("Login", "Account");
            }
            Cart? cart = null;
            if (!HttpContext.Session.Keys.Contains("Cart"))
            {
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(new Cart()));
                cart = JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("Cart")??string.Empty);
                return View(cart);
            }
            else
            {
                cart = JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("Cart")??string.Empty);
                return View(cart);

            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(Product p ,int id)
        {
            ProductRepository pR = new ProductRepository();
            Product product = pR.Get(id);
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                if (product != null)
                {
                    Cart? cart = new Cart();
                    if (HttpContext.Session.Keys.Contains("Cart"))
                    {
                        var value = HttpContext.Session.GetString("Cart");
                        cart = value == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(value);
                    }
                    else
                    {
                        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(new Cart()));
                    }
                    
                    //Cart cart = HttpContext.Session.GetString("Cart") == null ? new Cart() : JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("Cart"));
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
                    cart.Items.Add(cartItem);
                    cart.TotalPrice += product.Price * p.Quantity;
                    cart.TotalQuantity += p.Quantity;
                    cart.UserId = user.Id;
                    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
                }
                else return NotFound();
            }
            else return RedirectToAction("Login", "Account");
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveFromCart(int id)
        {
            Cart? cart = JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("Cart")??string.Empty);
            CartItem? cartItem = cart.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cart.Items.Remove(cartItem);
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        [Authorize]
        public IActionResult UpdateCart(int id, int quantity)
        {
            Cart? cart = JsonConvert.DeserializeObject<Cart>(HttpContext.Session.GetString("Cart")??string.Empty);
            CartItem? cartItem = cart.Items.Find(x => x.Id == id);
            if (cartItem != null)
            {
                cart.TotalPrice -= cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity -= cartItem.Quantity;
                cartItem.Quantity = quantity;
                cart.TotalPrice += cartItem.Price * cartItem.Quantity;
                cart.TotalQuantity += cartItem.Quantity;
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            }
            return RedirectToAction("Cart");
        }
    }
}
