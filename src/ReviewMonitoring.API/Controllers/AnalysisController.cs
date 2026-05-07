using Microsoft.AspNetCore.Mvc;

namespace ReviewMonitoring.API.Controllers
{
    public class AnalysisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
