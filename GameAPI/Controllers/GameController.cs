using Microsoft.AspNetCore.Mvc;

namespace GameAPI.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
