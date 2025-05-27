using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models; // Zorg dat deze namespace klopt
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var masseurId = await _masseurService.GetAllAsync();
                masseurId = masseurId.Where(b => b.IdAspNetUsers == user.Id);
                //validatie met server
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Geen data ontvangen. Het verzoeklichaam is leeg of ongeldig." });
                }

                if (string.IsNullOrEmpty(request.DatesMode))
                {
                    return BadRequest(new { success = false, message = "De datummodus (DatesMode) ontbreekt." });
                }

                if (string.IsNullOrEmpty(request.SchemaName))
                {
                    return BadRequest(new { success = false, message = "De naam (SchemaNaam) ontbreekt." });
                }

                if (request.DatesMode == "range" && string.IsNullOrEmpty(request.EndDate))
                {
                    return BadRequest(new { success = false, message = "Voor een datumbereik is een einddatum (EndDate) vereist." });
                }
                if (request.DatesMode == "range" && string.IsNullOrEmpty(request.EndDate))
                {
                    return BadRequest(new { success = false, message = "Voor een datumbereik is een einddatum (EndDate) vereist." });
                }
                DateOnly parsedStartDate;
                if (!DateOnly.TryParse(request.StartDate, out parsedStartDate))
                {
                    return BadRequest(new { success = false, message = "Ongeldig startdatumformaat. Verwacht JJJJ-MM-DD." });
                }

                DateOnly? parsedEndDate = null;
                if (request.DatesMode == "range" && !string.IsNullOrEmpty(request.EndDate))
                {
                    DateOnly tempEndDate;
                    if (!DateOnly.TryParse(request.EndDate, out tempEndDate))
                    {
                        return BadRequest(new { success = false, message = "Ongeldig einddatumformaat. Verwacht JJJJ-MM-DD." });
                    }
                    parsedEndDate = tempEndDate;

                    if (parsedStartDate > parsedEndDate.Value)
                    {
                        return BadRequest(new { success = false, message = "De startdatum kan niet na de einddatum liggen." });
                    }
                }

                //verwerk en sla data's op
                if (request.TimeSlots != null && request.TimeSlots.Any())
                {
                    if (request.DatesMode == "single")
                    {
                        parsedEndDate = parsedStartDate;
                    }
                    var newSchema = new Schema() {
                        Naam = request.SchemaName,
                        IdMasseur = masseurId.FirstOrDefault().Id,
                        StartDatum = parsedStartDate,
                        EindDatum = parsedEndDate,
                        Type  = "standaard"
                    };
                    await _schemaService.AddAsync(newSchema);



                    //foreach (var dayEntry in request.TimeSlots)
                    //{
                    //    string dayName = dayEntry.Key;
                    //    List<TimeSlot> slotsForDay = dayEntry.Value;

                    //    if (slotsForDay != null)
                    //    {
                    //        foreach (var slot in slotsForDay)
                    //        {

                    //            // Hier zal ik de database contacteren
                                
                    //        }
                    //    }
                    //}
                }
                else
                {
                    Console.WriteLine("Geen tijdsloten ingediend.");
                }
                return Ok(new { success = true, message = "Schema succesvol gewijzigd!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Er is een interne serverfout opgetreden: {ex.Message}" });
            }
        }
    }
}