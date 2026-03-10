using Microsoft.AspNetCore.Mvc;

namespace QuatTran.Web.Areas.Adminitrator.Controllers
{
    [Area("Administrator")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
