using MassageHuis.Entities;
using MassageHuis.Models;
using MassageHuis.Services.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MassageHuis.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private IService<KostPrijs> _kostprijsService;

    public HomeController(ILogger<HomeController> logger, IService<KostPrijs> kostprijsservice)
    {
        _logger = logger;
        _kostprijsService = kostprijsservice;
    }
    [AllowAnonymous]
    async public Task<IActionResult> Index()
    {

        if (User.Identity.IsAuthenticated)
        {

            if (!User.IsInRole("klant"))
            {
                if (User.IsInRole("uitbater"))
                {
                    return RedirectToAction("Index", "Uitbater"); // Redirect naar de Index actie van de UitbaterController
                }
                else if (User.IsInRole("masseur"))
                {
                    return RedirectToAction("Index", "Masseur"); // Redirect naar de Index actie van de MasseurController
                }
                else if (User.IsInRole("administrator"))
                {
                    return RedirectToAction("Index", "Admin"); // Redirect naar de Index actie van de AdminController
                }
                else
                {
                    return Forbid();
                }
            }
        }

        var massages = await _kostprijsService.GetAllAsync();
        massages = massages.Where(b => b.IdTypeMassageNavigation.Actief == true);
        massages = massages.OrderByDescending(b => b.Startdatum);
        massages = massages.DistinctBy(b => b.IdTypeMassage);
        var typeMassages = new List<TypeMassageVM>();
        foreach (var item in massages)
        {
            var type = new TypeMassageVM()
            {
                Id = item.IdTypeMassageNavigation.Id,
                Prijs = item.Prijs,
                Type = item.IdTypeMassageNavigation.Type,
                Beschrijving = item.IdTypeMassageNavigation.Beschrijving
            };
            typeMassages.Add(type);
        }
        var modelHome = new ReservatieVM()
        {
            TypeMassages = typeMassages
        };

        return View(modelHome);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
