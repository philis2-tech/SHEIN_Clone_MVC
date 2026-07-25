using Microsoft.AspNetCore.Mvc;

namespace SHEIN_Clone_MVC.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult Orders()
        {
            return View();
        }
    }
}