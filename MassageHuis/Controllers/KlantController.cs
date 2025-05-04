using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models;
using MassageHuis.Extensions;
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Data;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace MassageHuis.Controllers
{
    public class KlantController : Controller
    {
        private IService<Masseur> _masseurService;
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

        public async Task<IActionResult> Index()
        {
            MasseurVM masseursvm = new MasseurVM();
            masseursvm.Masseurs = await _masseurService.GetAllAsync();
            masseursvm.Gebruikers = await _userManager.GetUsersInRoleAsync("masseur");
            return View(masseursvm);
        }
        [HttpPost]
        public async Task<IActionResult> Kalender(MasseurVM masseurdata)
        {
            var schemas = await _schemaService.GetAllAsync();
            var schema = schemas.Where(b => b.IdMasseur == masseurdata.Id);

            var reservaties = await _reservatieService.GetAllAsync();
            var datumvandaag = DateOnly.FromDateTime(DateTime.Today);
            var toekomstiseReservaties = reservaties.Where(b => b.DatumReservatie >= datumvandaag);

            var actieveSchemas = schemas.Where(s => s.IdMasseur == masseurdata.Id && s.StartDatum <= datumvandaag && s.EindDatum >= datumvandaag)
            .OrderByDescending(s => s.StartDatum)
            .FirstOrDefault();

            var vandaagnummeriek = (int)DateTime.Today.DayOfWeek;
            var tijdsloten = await _regulierTijdslotService.GetAllAsync();


            if (actieveSchemas != null)
            {
                var tijdslotenFilterd = tijdsloten.Where(b => b.IdSchema == actieveSchemas.Id);
                var lsDataMaand = GetAllDaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

                lsDataMaand = lsDataMaand.Where(b => b.Date >= DateTime.Today.Date).ToList();
                var vrijeSlots = new List<DateTime>();

                // Haal alle uitzonderingstijdsloten op, zowel voor de masseur als voor de uitbater (IdSchema 8)
                var uitzonderingTijdsloten = await _uitzonderingTijdslotService.GetAllAsync();
                var uitzonderingTijdslotenFilterd = uitzonderingTijdsloten.Where(b => b.IdSchema == actieveSchemas.Id || b.IdSchema == 8).ToList();

                foreach (var dag in lsDataMaand)
                {
                    foreach (var slot in tijdslotenFilterd)
                    {
                        if ((int)dag.DayOfWeek == slot.Dag)
                        {
                            TimeSpan startTijd = slot.StartTijd.ToTimeSpan();
                            DateTime slotTijd = dag.Add(startTijd);

                            // Controleer of de slotTijd niet in een uitzonderingstijdslot valt
                            bool isVerlofDag = uitzonderingTijdslotenFilterd.Any(uitzondering =>
                                DateOnly.FromDateTime(slotTijd.Date) == uitzondering.Datum &&
                                slotTijd.TimeOfDay >= uitzondering.Startijd.ToTimeSpan() &&
                                slotTijd.TimeOfDay < uitzondering.Eindtijd.ToTimeSpan()
                            );

                            if (!isVerlofDag)
                            {
                                vrijeSlots.Add(slotTijd);
                            }
                        }
                    }
                }
                masseurdata.vrijeSlots = vrijeSlots;
            }
            else
            {
                Console.WriteLine($"Geen actief schema gevonden voor masseur-ID: {masseurdata.Id}");
            }
            ReservatieVM reservatieSessie = new ReservatieVM();
            reservatieSessie.MasseurId = masseurdata.Id;
            reservatieSessie.MasseurNaam = masseurdata.Naam;
            HttpContext.Session.SetObject("Reservatie", reservatieSessie);
            return View(masseurdata);
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
