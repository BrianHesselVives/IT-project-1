using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models;
using MassageHuis.Services;
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Net.Sockets;

namespace MassageHuis.Controllers
{
    public class ReservatieController : Controller
    {
        private IService<Masseur> _masseurService;
        private IService<Schema> _schemaService;
        private IService<Reservatie> _reservatieService;
        private IService<UitzonderingTijdslot> _uitzonderingTijdslotService;
        private IService<RegulierTijdslot> _regulierTijdslotService;
        private readonly IMapper _mapper;
        private readonly IEmailSend _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;


        public ReservatieController(
            IMapper mapper,
            UserManager<ApplicationUser> usermanager,
            IService<Masseur> masseurservice,
            IService<Schema> schemaservice,
            IService<UitzonderingTijdslot> uitzonderingTijdslotservice,
            IService<Reservatie> reservatieservice,

            IService<RegulierTijdslot> regulierTijdslotservice,
            IEmailSend emailSender)
        {
            _masseurService = masseurservice;
            _schemaService = schemaservice;
            _regulierTijdslotService = regulierTijdslotservice;
            _uitzonderingTijdslotService = uitzonderingTijdslotservice;
            _reservatieService = reservatieservice;
            _userManager = usermanager;
            _mapper = mapper;
            _emailSender = emailSender;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OverzichtReservatie(ReservatieVM reservatieData)
        {
            return View("BevestigReservatie", reservatieData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BevestigReservatie(ReservatieVM reservatieData)
        {
            if (reservatieData?.GeselecteerdSlot != null && reservatieData?.MasseurId > 0)
            {
                var geselecteerdSlot = reservatieData?.GeselecteerdSlot;
                int? masseurId = reservatieData?.MasseurId;
                DateOnly geselecteerdeDatum = DateOnly.FromDateTime(geselecteerdSlot.Value);

                // 1. Haal relevante gegevens op
                var schemas = await _schemaService.GetAllAsync();
                var reservaties = await _reservatieService.GetAllAsync();
                var reguliereTijdsloten = await _regulierTijdslotService.GetAllAsync();
                var uitzonderingTijdsloten = await _uitzonderingTijdslotService.GetAllAsync();

                // 2. Filter actieve schema's op de datum van het geselecteerde slot
                var actiefSchema = schemas
                    .Where(s => s.IdMasseur == masseurId &&
                                s.StartDatum <= geselecteerdeDatum &&
                                s.EindDatum >= geselecteerdeDatum)
                    .OrderByDescending(s => s.StartDatum)
                    .FirstOrDefault();

                if (actiefSchema == null)
                {
                    ViewBag.ErrorMessage = "Het geselecteerde tijdslot is niet meer beschikbaar (geen actief schema).";
                    return View("~/Views/Shared/Error.cshtml");
                }

                // 3a. Controleer of het een geldig regulier tijdslot is
                var dagVanDeWeek = (int)geselecteerdSlot.Value.DayOfWeek;
                var startTijdVanSlot = geselecteerdSlot.Value.TimeOfDay;

                var geldigRegulierSlot = reguliereTijdsloten
                    .Any(r => r.IdSchema == actiefSchema.Id &&
                              r.Dag == dagVanDeWeek &&
                              r.StartTijd.ToTimeSpan() == startTijdVanSlot);

                if (!geldigRegulierSlot)
                {
                    ViewBag.ErrorMessage = "Het geselecteerde tijdslot is niet meer beschikbaar (geen geldig regulier tijdslot).";
                    return View("~/Views/Shared/Error.cshtml");
                }

                // 3b. Controleer op uitzonderingen
                var isUitzondering = uitzonderingTijdsloten
                    .Any(u => u.IdSchema == actiefSchema.Id &&
                              u.Datum == geselecteerdeDatum &&
                              u.Startijd.ToTimeSpan() == startTijdVanSlot); // Mogelijk moet je ook rekening houden met EindTijd als je dat hebt

                if (isUitzondering)
                {
                    ViewBag.ErrorMessage = "Het geselecteerde tijdslot is niet meer beschikbaar (valt binnen een uitzondering).";
                    return View("~/Views/Shared/Error.cshtml");
                }

                // 4. Controleer of het tijdslot al gereserveerd is
                var isGereserveerd = reservaties.Where(b => DateOnly.FromDateTime((DateTime)b.DatumReservatie) == geselecteerdeDatum && b.Status == "Gereserveerd" && b.IdRegulierTijdslot == reservatieData.IdTijdSlot);

                if (isGereserveerd.FirstOrDefault() == null)
                {

                    Reservatie reservatie = new Reservatie()
                    {
                        DatumCreatie = DateTime.Now,
                        DatumReservatie = geselecteerdSlot,// zorgt ervoor dat er een tijd ingevuld is bij de reservatie, 00:00:00
                        IdAspNetUsers = _userManager.GetUserId(User),
                        IdMasseur = (int)reservatieData.MasseurId,
                        IdTypeMassage = 3,//id massage dient nog meegegeven te worden.
                        IdRegulierTijdslot = reservatieData.IdTijdSlot,//tijdelijke waarde
                        IdPrijs = 4,//prijs moet nog opgehaald worden via dao een het type massage
                        Status = "Gereserveerd",
                        TeBetalenBedrag = 90, //dient berekend te worden aan de hand van de promocode.

                    };

                    await _reservatieService.AddAsync(reservatie);
                }
                else
                {
                    ViewBag.ErrorMessage = "Het geselecteerde tijdslot is helaas al gereserveerd.";
                    return View("~/Views/Shared/Error.cshtml");
                }
                    return View("../Home/Index",reservatieData); 
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }
    }
}
