using Microsoft.AspNetCore.Mvc;

namespace ComplainModule.Controllers
{
    public class Error : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}