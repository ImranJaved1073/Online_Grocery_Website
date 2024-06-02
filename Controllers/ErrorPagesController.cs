using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("ErrorPages/{statusCode}")]
    public class ErrorPagesController : Controller
    {
        public IActionResult Index(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    RedirectToAction("NotFound","Cart");
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Sorry, something went wrong on the server";
                    break;
                default:
                    ViewBag.ErrorMessage = "Sorry, something went wrong";
                    break;
            }
            return View(statusCode);
        }
    }
}
