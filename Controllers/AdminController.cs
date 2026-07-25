using Microsoft.AspNetCore.Mvc;

namespace SHEIN_Clone_MVC.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
