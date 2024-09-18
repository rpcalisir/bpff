using Microsoft.AspNetCore.Mvc;

namespace BalkanPanoramaFilmFestival.Controllers
{
    public class StaticPagesController : Controller
    {
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AdvisoryBoard()
        {
            return View();
        }
    }
}
