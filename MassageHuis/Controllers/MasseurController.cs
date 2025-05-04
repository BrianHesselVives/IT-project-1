using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageHuis.Controllers
{
    public class MasseurController : Controller
    {
        [Authorize(Roles = "masseur")]

        public IActionResult Index()
        {
            VerlofVM model = new VerlofVM();
            return View(model);
        }
    }
}
