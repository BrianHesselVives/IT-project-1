using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageHuis.Controllers
{
    public class UitbaterController : Controller
    {
        [Authorize(Roles = "uitbater")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
