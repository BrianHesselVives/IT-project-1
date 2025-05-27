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
    [Authorize(Roles = "masseur")]
    [Route("[controller]")] // Basisroute voor de controller: /Masseur
    public class MasseurController : Controller
    {
        private IService<Masseur> _masseurService;
        private IService<Schema> _schemaService;
        private IService<Reservatie> _reservatieService;
        private IService<UitzonderingTijdslot> _uitzonderingTijdslotService;
        private IService<RegulierTijdslot> _regulierTijdslotService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public MasseurController(
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
        }
        [HttpGet]
        public IActionResult Index()
        {
            VerlofVM model = new VerlofVM();
            return View(model);
        }
        [HttpGet("SchemaWijzigen")]
        public IActionResult SchemaWijzigen()
        {
            return View();
        }
        [HttpGet("SchemaOverzicht")]
        async public Task<IActionResult> SchemaOverzicht(SchemaVM schema)
        {

            var masseurId = await _masseurService.GetAllAsync();
            masseurId = masseurId.Where(b => b.IdAspNetUsers == _userManager.GetUserId(User));
            var masseurSchemas = await _schemaService.GetAllAsync();
            masseurSchemas = masseurSchemas.Where(b => b.IdMasseur == masseurId.FirstOrDefault().Id);
            var schemaVMs = new List<SchemaVM>();
            foreach (var item in masseurSchemas)
            {
                var reguliereTijdsloten = await _regulierTijdslotService.GetAllAsync();
                reguliereTijdsloten = reguliereTijdsloten.Where(b=>b.IdSchema == item.Id);
                var reguliereTijdslotenVMs = new List<RegulierTijdslotVM>();
                foreach (var slot in reguliereTijdsloten)
                {
                    var reguliertijdslotVM = _mapper.Map<RegulierTijdslotVM>(slot);
                    reguliereTijdslotenVMs.Add(reguliertijdslotVM);
                }
                var schemaVM = _mapper.Map<SchemaVM>(item);
                schemaVM.ReguliereTijdsloten = reguliereTijdslotenVMs;
                schemaVMs.Add(schemaVM);
            }
            return View(schemaVMs);
        }

        [HttpPost("SchemaOpslaan")]
        public async Task<IActionResult> SchemaOpslaanAsync([FromBody] ScheduleSaveRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Geen data ontvangen. Het verzoeklichaam is leeg of ongeldig." });
            }

            //Ophalen van de Masseur ID
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "Niet geautoriseerd. Gebruiker niet ingelogd." });
            }

            var masseur = (await _masseurService.GetAllAsync()).FirstOrDefault(b => b.IdAspNetUsers == user.Id);

            if (masseur == null)
            {
                return StatusCode(404, new { success = false, message = "Masseur profiel niet gevonden." });
            }

            //Validatie van de verplichte velden in de request
            if (string.IsNullOrWhiteSpace(request.SchemaName))
            {
                return BadRequest(new { success = false, message = "De naam van het schema is verplicht." });
            }

            if (string.IsNullOrEmpty(request.DatesMode))
            {
                return BadRequest(new { success = false, message = "De datummodus (enkele dag of bereik) ontbreekt." });
            }

            //Datumvalidatie
            DateOnly parsedStartDate;
            if (!DateOnly.TryParseExact(request.StartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStartDate))
            {
                return BadRequest(new { success = false, message = "Ongeldig startdatumformaat. Verwacht JJJJ-MM-DD." });
            }

            DateOnly? parsedEndDate = null;
            if (request.DatesMode.ToLower() == "range")
            {
                if (string.IsNullOrEmpty(request.EndDate))
                {
                    return BadRequest(new { success = false, message = "Voor een datumbereik is een einddatum vereist." });
                }

                DateOnly tempEndDate;
                if (!DateOnly.TryParseExact(request.EndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempEndDate))
                {
                    return BadRequest(new { success = false, message = "Ongeldig einddatumformaat. Verwacht JJJJ-MM-DD." });
                }
                parsedEndDate = tempEndDate;

                if (parsedStartDate > parsedEndDate.Value)
                {
                    return BadRequest(new { success = false, message = "De startdatum kan niet na de einddatum liggen." });
                }
            }
            else if (request.DatesMode.ToLower() == "single")
            {
                parsedEndDate = parsedStartDate; //Voor "single" modus is de einddatum gelijk aan de startdatum
            }
            else
            {
                return BadRequest(new { success = false, message = "Ongeldige datummodus. Gebruik 'single' of 'range'." });
            }

            //Controleer of de startdatum niet in het verleden ligt voor nieuwe schema's
            if (parsedStartDate < DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { success = false, message = "De startdatum van een nieuw schema mag niet in het verleden liggen." });
            }

            //Tijdsloten validatie en verwerking
            if (request.TimeSlots == null || !request.TimeSlots.Any())
            {
                return BadRequest(new { success = false, message = "Er moeten tijdsloten worden opgegeven voor het schema." });
            }

            var reguliereTijdslots = new List<RegulierTijdslot>();
            var validDayNames = Enum.GetNames(typeof(DayOfWeek)).Select(d => d.ToLower()).ToList();

            foreach (var dayEntry in request.TimeSlots)
            {
                string dayName = dayEntry.Key.ToLower();
                if (!validDayNames.Contains(dayName))
                {
                    return BadRequest(new { success = false, message = $"Ongeldige dagnaam gevonden: '{dayEntry.Key}'." });
                }

                DayOfWeek dayOfWeek;
                // Parse de string dagnaam naar de DayOfWeek.
                // Zorg ervoor dat "sunday" (0) correct wordt behandeld als DayOfWeek.Sunday
                if (!Enum.TryParse(dayName, true, out dayOfWeek))
                {
                    return BadRequest(new { success = false, message = $"Kon dagnaam '{dayEntry.Key}' niet converteren." });
                }

                int dayInt = (int)dayOfWeek;

                if (dayEntry.Value == null || !dayEntry.Value.Any())
                {
                    return BadRequest(new { success = false, message = $"Geen tijdsloten opgegeven voor {dayEntry.Key}." });
                }

                foreach (var slot in dayEntry.Value)
                {
                    if (string.IsNullOrWhiteSpace(slot.Start) || string.IsNullOrWhiteSpace(slot.End))
                    {
                        return BadRequest(new { success = false, message = $"Start- of eindtijd ontbreekt voor een tijdslot op {dayEntry.Key}." });
                    }

                    TimeOnly parsedStart;
                    TimeOnly parsedEnd;

                    if (!TimeOnly.TryParseExact(slot.Start, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedStart))
                    {
                        return BadRequest(new { success = false, message = $"Ongeldig starttijdformaat ('{slot.Start}') voor een tijdslot op {dayEntry.Key}. Verwacht HH:mm." });
                    }
                    if (!TimeOnly.TryParseExact(slot.End, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedEnd))
                    {
                        return BadRequest(new { success = false, message = $"Ongeldig eindtijdformaat ('{slot.End}') voor een tijdslot op {dayEntry.Key}. Verwacht HH:mm." });
                    }

                    if (parsedStart >= parsedEnd)
                    {
                        return BadRequest(new { success = false, message = $"De starttijd ({slot.Start}) moet vóór de eindtijd ({slot.End}) liggen voor een tijdslot op {dayEntry.Key}." });
                    }

                    // Voeg het RegulierTijdslot toe aan de lijst
                    reguliereTijdslots.Add(new RegulierTijdslot
                    {
                        Dag = dayInt,
                        StartTijd = parsedStart,
                        EindTijd = parsedEnd,
                    });
                }
            }

            //Controle op overlappende tijdsloten per dag
            foreach (var dayGroup in reguliereTijdslots.GroupBy(s => s.Dag))
            {
                var sortedSlots = dayGroup.OrderBy(s => s.StartTijd).ToList();
                for (int i = 0; i < sortedSlots.Count - 1; i++)
                {
                    if (sortedSlots[i].EindTijd > sortedSlots[i + 1].StartTijd)
                    {
                        var dayName = ((DayOfWeek)dayGroup.Key).ToString();
                        return BadRequest(new { success = false, message = $"Overlappende tijdsloten op {dayName}: {sortedSlots[i].StartTijd}-{sortedSlots[i].EindTijd} overlapt met {sortedSlots[i + 1].StartTijd}-{sortedSlots[i + 1].EindTijd}." });
                    }
                }
            }


            //Nieuw Schema aanmaken en vullen
            var newSchema = new Schema()
            {
                Naam = request.SchemaName,
                IdMasseur = masseur.Id,
                StartDatum = parsedStartDate,
                EindDatum = parsedEndDate,
                Type = "standaard", 
                RegulierTijdslots = reguliereTijdslots // Wijs de tijdsloten toe
            };

            //Schema opslaan
            try
            {
                await _schemaService.AddAsync(newSchema);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Fout bij het opslaan van schema in de database: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.Error.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { success = false, message = $"Er is een onverwachte fout opgetreden bij het opslaan van het schema. Probeer het later opnieuw. Details: {ex.Message}" });
            }

            return Ok(new { success = true, message = "Schema succesvol opgeslagen!" });
        }


    }
}