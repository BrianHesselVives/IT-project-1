using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Extensions;
using MassageHuis.Models;
using MassageHuis.Repositories;
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Net.Sockets;
using System.Text.Json;

namespace MassageHuis.Controllers
{
    public class KlantController : Controller
    {
        private IService<Masseur> _masseurService;
        private IService<TypeMassage> _typemassageService;
        private IService<KostPrijs> _kostprijsService;
        private IService<Schema> _schemaService;
        private IService<Reservatie> _reservatieService;
        private IService<UitzonderingTijdslot> _uitzonderingTijdslotService;
        private IService<RegulierTijdslot> _regulierTijdslotService;
        private readonly IMapper _mapper;
        private readonly IEmailSend _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public KlantController(
            IMapper mapper,
            UserManager<ApplicationUser> usermanager,
            IService<Masseur> masseurservice,
            IService<Schema> schemaservice,
            IService<UitzonderingTijdslot> uitzonderingTijdslotservice,
            IService<Reservatie> reservatieservice,
            IService<TypeMassage> typemassageservice,
            IService<KostPrijs> kostprijsservice,
            IService<RegulierTijdslot> regulierTijdslotservice,
            IEmailSend emailSender)
        {
            _masseurService = masseurservice;
            _schemaService = schemaservice;
            _regulierTijdslotService = regulierTijdslotservice;
            _uitzonderingTijdslotService = uitzonderingTijdslotservice;
            _reservatieService = reservatieservice;
            _userManager = usermanager;
            _kostprijsService = kostprijsservice;
            _typemassageService = typemassageservice;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        [Authorize(Roles = "klant")]
        [HttpGet]
        public async Task<IActionResult> Index(int IdTypeMassage)
        {
            MasseurVM masseursvm = new MasseurVM();
            var allMasseurs = await _masseurService.GetAllAsync();
            var allSchemas = await _schemaService.GetAllAsync();
            var today = DateOnly.FromDateTime(DateTime.Today);

            var masseurIdsWithActiveSchema = allSchemas
                .Where(s =>  s.EindDatum >= today)
                .Select(s => s.IdMasseur)
                .Distinct()
                .ToList();

            masseursvm.Masseurs = allMasseurs
                .Where(m => masseurIdsWithActiveSchema.Contains(m.Id))
                .ToList();

            masseursvm.Gebruikers = await _userManager.GetUsersInRoleAsync("masseur");
            masseursvm.IdTypeMassage = IdTypeMassage;

            return View(masseursvm);
        }

        [HttpGet]
        [Authorize(Roles = "klant")]
        public async Task<IActionResult> Kalender(int IdMasseur, string MasseurNaam, int IdTypeMassage, int? year, int? month)
        {
            var currentYear = year ?? DateTime.Now.Year;
            var currentMonth = month ?? DateTime.Now.Month;

            // Alles in één keer ophalen voor performance
            var allSchemas = await _schemaService.GetAllAsync();
            var allReservaties = await _reservatieService.GetAllAsync();
            var allReguliereTijdsloten = await _regulierTijdslotService.GetAllAsync();
            var allUitzonderingTijdsloten = await _uitzonderingTijdslotService.GetAllAsync();

            var datumvandaag = DateOnly.FromDateTime(DateTime.Today);
            var vrijeSlots = new List<VrijSlotVM>();

            var eersteDagVanDeMaand = new DateOnly(currentYear, currentMonth, 1);
            var laatsteDagVanDeMaand = new DateOnly(currentYear, currentMonth, DateTime.DaysInMonth(currentYear, currentMonth));

            // Haal relevante schema's op, sorteer op StartDatum aflopend voor 'meest recente' schema
            var relevanteSchemasVoorMasseur = allSchemas
                .Where(s => s.IdMasseur == IdMasseur &&
                            s.StartDatum <= laatsteDagVanDeMaand &&
                            s.EindDatum >= eersteDagVanDeMaand)
                .OrderByDescending(s => s.StartDatum) // Prioriteit: meest recent gestart schema
                .ToList();

            // Filter uitzonderingen: masseur of algemeen
            var uitzonderingTijdslotenFilterd = allUitzonderingTijdsloten
                .Where(b => (b.IdSchemaNavigation.IdMasseur == IdMasseur || b.IdSchema == 52) &&
                            b.Datum >= eersteDagVanDeMaand && b.Datum <= laatsteDagVanDeMaand)// 52 is hardcoded schema van uitbater om verlof te bekijken
                .ToList();

            // Filter gereserveerde slots voor deze masseur in deze maand
            var gereserveerdeSlotsVoorMasseurInMaand = allReservaties
                .Where(r => r.IdMasseur == IdMasseur &&
                            r.DatumReservatie.HasValue && // Check op nullable DateTime
                            DateOnly.FromDateTime(r.DatumReservatie.Value) >= eersteDagVanDeMaand &&
                            DateOnly.FromDateTime(r.DatumReservatie.Value) <= laatsteDagVanDeMaand &&
                            r.Status == "Gereserveerd")
                .ToList();

            // Duur van elke massage is altijd 60 minuten
            const int massageDuurInMinuten = 60;
            TimeSpan massageDuur = TimeSpan.FromMinutes(massageDuurInMinuten);


            // Loop door elke dag van de maand
            var lsDataMaand = GetAllDaysInMonth(currentYear, currentMonth);

            // Filter dagen in het verleden
            if (currentYear == DateTime.Now.Year && currentMonth == DateTime.Now.Month)
            {
                lsDataMaand = lsDataMaand.Where(b => b.Date >= DateTime.Today.Date).ToList();
            }

            foreach (var dag in lsDataMaand)
            {
                // Bepaal het actieve schema voor deze specifieke dag
                var geldendSchemaVoorDag = relevanteSchemasVoorMasseur
                    .FirstOrDefault(s => DateOnly.FromDateTime(dag.Date) >= s.StartDatum &&
                                         DateOnly.FromDateTime(dag.Date) <= s.EindDatum);

                if (geldendSchemaVoorDag != null)
                {
                    // Reguliere tijdsloten voor het geldende schema van deze dag
                    var reguliereTijdslotenVoorDezeDag = allReguliereTijdsloten
                        .Where(b => b.IdSchema == geldendSchemaVoorDag.Id && (int)dag.DayOfWeek == b.Dag)
                        .ToList();

                    foreach (var slot in reguliereTijdslotenVoorDezeDag)
                    {
                        TimeSpan startTijdRegulierSlot = slot.StartTijd.ToTimeSpan();
                        DateTime slotTijdStart = dag.Add(startTijdRegulierSlot);
                        DateTime slotTijdEind = slotTijdStart.Add(massageDuur); // Gebruik de vaste duur

                        // Check op uitzonderingen (verlof)
                        bool isUitzondering = uitzonderingTijdslotenFilterd.Any(uitzondering =>
                            DateOnly.FromDateTime(slotTijdStart.Date) == uitzondering.Datum &&
                            slotTijdStart.TimeOfDay < uitzondering.Eindtijd.ToTimeSpan() &&
                            slotTijdEind.TimeOfDay > uitzondering.Startijd.ToTimeSpan()
                        );

                        if (!isUitzondering)
                        {
                            // Check op bestaande reserveringen die overlappen (incl. 15 min offset)
                            var isGereserveerd = gereserveerdeSlotsVoorMasseurInMaand.Any(gereserveerdeSlot =>
                            {
                                DateTime gereserveerdeSlotStart = gereserveerdeSlot.DatumReservatie!.Value; // .Value is veilig door HasValue check eerder
                                DateTime gereserveerdeSlotEind = gereserveerdeSlotStart.Add(massageDuur); // Gebruik de vaste duur

                                // Werkelijke geblokkeerde tijd, inclusief 15 minuten offset
                                DateTime gereserveerdeSlotEindMetOffset = gereserveerdeSlotEind.Add(TimeSpan.FromMinutes(15));

                                // Overlap check: (StartA < EindB) EN (EindA > StartB)
                                return slotTijdStart < gereserveerdeSlotEindMetOffset && slotTijdEind > gereserveerdeSlotStart;
                            });

                            if (!isGereserveerd)
                            {
                                // Slot moet minstens 75 minuten in de toekomst liggen
                                if (slotTijdStart > DateTime.Now.AddMinutes(75))
                                {
                                    VrijSlotVM newSlot = new VrijSlotVM { Id = slot.Id, IdSchema = geldendSchemaVoorDag.Id, starttijd = slotTijdStart };
                                    vrijeSlots.Add(newSlot);
                                }
                            }
                        }
                    }
                }
            }

            var slotsPerDag = vrijeSlots
                .GroupBy(s => s.starttijd.Date)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Masseur en massagetype info voor de ViewModel
            var masseurToFind = new Masseur { Id = IdMasseur };
            var masseurEntity = await _masseurService.FindByIdAsync(masseurToFind);
            if (masseurEntity == null)
            {
                return RedirectToPage("../Shared/Error");
            }

            // Haal TypeMassage info op voor ViewModel (nog steeds nodig voor .Type)
            var typeMassageToFind = new TypeMassage { Id = IdTypeMassage };
            var typeMassageEntity = await _typemassageService.FindByIdAsync(typeMassageToFind);
            if (typeMassageEntity == null)
            {
                return RedirectToPage("../Shared/Error");
            }

            var kalenderVM = new KalenderVM
            {
                NaamMasseur = MasseurNaam,
                IdMasseur = IdMasseur,
                SlotsPerDag = slotsPerDag,
                IdTypeMassage = IdTypeMassage,
                TypeMassage = typeMassageEntity.Type, // Type blijft nodig voor de ViewModel
                CalendarYear = currentYear,
                CalendarMonth = currentMonth
            };

            return View(kalenderVM);
        }

        [HttpPost]
        [Authorize(Roles = "klant")]
        public async Task<IActionResult> Kalender(MasseurVM masseurdata)
        {
            // Redirect naar GET Kalender actie met geselecteerde parameters
            return RedirectToAction("Kalender", new { IdMasseur = masseurdata.Id, MasseurNaam = masseurdata.Naam, IdTypeMassage = masseurdata.IdTypeMassage, masseurdata.CalendarYear, masseurdata.CalendarMonth });
        }

        public static List<DateTime> GetAllDaysInMonth(int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            var dates = new List<DateTime>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                dates.Add(new DateTime(year, month, day));
            }

            return dates;
        }
    }
}