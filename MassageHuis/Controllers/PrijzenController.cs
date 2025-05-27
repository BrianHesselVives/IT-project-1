using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models; // Zorg dat deze namespace klopt
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;


namespace MassageHuis.Controllers
{
    public class PrijzenController : Controller
    {
        private IService<Masseur> _masseurService;
        private IService<Schema> _schemaService;
        private IService<Reservatie> _reservatieService;
        private IService<KostPrijs> _kostPrijsService;
        private IService<TypeMassage> _typeMassageService;
        private IService<UitzonderingTijdslot> _uitzonderingTijdslotService;
        private IService<RegulierTijdslot> _regulierTijdslotService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public PrijzenController(
            IMapper mapper,
            UserManager<ApplicationUser> usermanager,
            IService<Masseur> masseurservice,
            IService<Schema> schemaservice,
            IService<KostPrijs> kostprijsservice,
            IService<TypeMassage> typemassageservice,
            IService<UitzonderingTijdslot> uitzonderingTijdslotservice,
            IService<Reservatie> reservatieservice,

            IService<RegulierTijdslot> regulierTijdslotservice,
            IEmailSend emailSender)
        {
            _masseurService = masseurservice;
            _schemaService = schemaservice;
            _kostPrijsService = kostprijsservice;
            _typeMassageService = typemassageservice;
            _regulierTijdslotService = regulierTijdslotservice;
            _uitzonderingTijdslotService = uitzonderingTijdslotservice;
            _reservatieService = reservatieservice;
            _userManager = usermanager;
            _mapper = mapper;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        async public Task<IActionResult> PrijzenOpslaan(KostPrijsVM newPrijzen)
        {
            return View();
        }
        async public Task<IActionResult> Index(KostPrijsVM prijzen)
        {
            var kostprijzen = await _kostPrijsService.GetAllAsync();
            prijzen.TypeMassages = await _typeMassageService.GetAllAsync();
            return View(prijzen);
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrijsInfo(KostPrijsVM model)
        {
            model.TypeMassages = (await _typeMassageService.GetAllAsync())?.OrderBy(t => t.Type).ToList();

            if (model.IdTypeMassage > 0)
            {
                var prijzen = await _kostPrijsService.GetAllAsync();
                model.VoorgaandePrijs = prijzen.Where(b => b.IdTypeMassage == model.IdTypeMassage).OrderByDescending(b => b.Startdatum).FirstOrDefault().Prijs;
            }
            else
            {
                model.VoorgaandePrijs = null; 
            }
            return View("~/Views/Prijzen/Index.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KostPrijsVM model)
        {
            try
            {
                var newKostprijs = _mapper.Map<KostPrijs>(model);
                var lijstKostprijzen = await _kostPrijsService.GetAllAsync();
                if (lijstKostprijzen.Where(b => b.IdTypeMassage == model.IdTypeMassage && b.Startdatum == model.StartDatum).FirstOrDefault() != null)
                {
                    throw new InvalidOperationException("Er bestaat al een prijs voor dit massagetype met deze startdatum.");
                }
              
                await _kostPrijsService.AddAsync(newKostprijs);

                TempData["SuccessMessage"] = "De nieuwe kostprijs is succesvol toegevoegd!";
                return RedirectToAction("Index", "Uitbater");
            }
            catch (Exception)
            {
                model.TypeMassages = await _typeMassageService.GetAllAsync();
                TempData["ErrorMessage"] = "De nieuwe kostprijs is mislukt om toe te voegen!";
                return View("~/Views/Prijzen/Index.cshtml", model);
            }
            
        }
    }
}
