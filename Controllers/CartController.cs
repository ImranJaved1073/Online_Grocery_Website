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
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            LoadCartFromCookies(user.Id);
            Cart? cart = CookieHelper.GetCookie<Cart>(HttpContext, "Cart", user.Id);
            if (cart == null)
            {
                cart = new Cart();
            }
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(Product p, int id)
        {
            ProductRepository pR = new ProductRepository();
            Product product = pR.Get(id);
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
