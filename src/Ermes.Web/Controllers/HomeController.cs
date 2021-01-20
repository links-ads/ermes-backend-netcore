using Microsoft.AspNetCore.Mvc;

namespace Ermes.Web.Controllers
{
    public class HomeController : ErmesControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}