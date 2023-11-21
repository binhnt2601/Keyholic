using BanPhimCanhCach.Models;
using Microsoft.AspNetCore.Mvc;

namespace BanPhimCanhCach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/Admin/[action]")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
