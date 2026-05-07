using Microsoft.AspNetCore.Mvc;

namespace ReviewMonitoring.API.Controllers
{
    public class JobsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
