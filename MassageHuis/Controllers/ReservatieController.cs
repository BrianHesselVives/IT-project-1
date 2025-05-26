using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models;
using MassageHuis.Repositories;
using MassageHuis.Services;
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;
using System.Globalization;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;

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
        [Authorize(Roles = "klant")]
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

                //eindtijd massage
                DateTime eindtijdMassage = (DateTime)geselecteerdSlot;
                eindtijdMassage = eindtijdMassage.Date + (reguliereTijdsloten.Where(b => b.Id == reservatieData.IdTijdSlot).FirstOrDefault().EindTijd.ToTimeSpan());
                var massageDuur = eindtijdMassage - (DateTime)geselecteerdSlot;

                var user = await _userManager.GetUserAsync(User);
                var email = await _userManager.GetEmailAsync(user);

                StringBuilder EmailTextBuilder = new StringBuilder();

                EmailTextBuilder.AppendLine("<!DOCTYPE html>");
                EmailTextBuilder.AppendLine("<html>");
                EmailTextBuilder.AppendLine("<head>");
                EmailTextBuilder.AppendLine("<meta charset=\"UTF-8\">");
                EmailTextBuilder.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
                EmailTextBuilder.AppendLine("<title>Reserveringsbevestiging</title>");
                EmailTextBuilder.AppendLine("<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css\">"); // Gebruik CDN voor Bootstrap
                EmailTextBuilder.AppendLine("<style>");
                EmailTextBuilder.AppendLine("body { font-family: Arial, sans-serif; }");
                EmailTextBuilder.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ccc; border-radius: 5px; }");
                EmailTextBuilder.AppendLine("h1 { color: #0078d7; }");
                EmailTextBuilder.AppendLine("p { margin-bottom: 10px; }");
                EmailTextBuilder.AppendLine(".details-box { background-color: #f0f8ff; padding: 15px; border-radius: 5px; margin-bottom: 15px;}"); // Nieuwe class voor styling
                EmailTextBuilder.AppendLine("</style>");
                EmailTextBuilder.AppendLine("</head>");
                EmailTextBuilder.AppendLine("<body>");
                EmailTextBuilder.AppendLine("<div class=\"container\">");

                EmailTextBuilder.AppendLine($"<h1>Beste {user.Voornaam} {user.Naam},</h1>");
                EmailTextBuilder.AppendLine("<p>Hartelijk dank voor uw reservering bij ons massagehuis! We kijken ernaar uit om u te verwelkomen voor uw ontspannende massage.</p>");
                EmailTextBuilder.AppendLine("<p>Hieronder vindt u een overzicht van uw reserveringsgegevens:</p>");

                EmailTextBuilder.AppendLine("<div class=\"details-box\">");  // Start details box
                EmailTextBuilder.AppendLine($"<p><strong>Naam:</strong> {user.Voornaam} {user.Naam}</p>");
                EmailTextBuilder.AppendLine($"<p><strong>Datum:</strong> {geselecteerdSlot.Value.Date.ToString("dd-MM-yyyy")}</p>");
                EmailTextBuilder.AppendLine($"<p><strong>Tijd:</strong> {geselecteerdSlot.Value.TimeOfDay.ToString(@"hh\:mm")}</p>");
                EmailTextBuilder.AppendLine("<p><strong>Type massage:</strong> [Naam van het type massage, bijvoorbeeld: Ontspanningsmassage]</p>"); // Placeholder
                EmailTextBuilder.AppendLine($"<p><strong>Masseur:</strong> {reservatieData.MasseurNaam}</p>");
                EmailTextBuilder.AppendLine($"<p><strong>Duur van de massage:</strong> {massageDuur.TotalMinutes} minuten</p>");

                EmailTextBuilder.AppendLine("</div>"); //einde details box

                EmailTextBuilder.AppendLine("<p>Uw reservering is nu definitief bevestigd.</p>");
                EmailTextBuilder.AppendLine($"<p>Wij zien u graag op {geselecteerdSlot.Value.Date.ToString("dd-MM-yyyy")}!</p>");

                EmailTextBuilder.AppendLine("<p>Met ontspannende groet,</p>");
                EmailTextBuilder.AppendLine($"<p>{reservatieData.MasseurNaam} / Het team van Massagehuis</p>");

                EmailTextBuilder.AppendLine("</div>");
                EmailTextBuilder.AppendLine("</body>");
                EmailTextBuilder.AppendLine("</html>");

                
                //ICS file
                string calendarContent = GenerateICSInviteBody
                    (
                        "Massagehuis",
                        user.Email,
                        $"Reservatie {reservatieData.TypeMassage} op ",
                        $"Dit is de reservatie voor de een {reservatieData.TypeMassage} op {geselecteerdSlot.Value.Date.ToString("dd-MM-yyyy")} om {geselecteerdSlot.Value.TimeOfDay.ToString(@"hh\:mm")}",
                        "Ieper",
                        (DateTime)geselecteerdSlot,
                        eindtijdMassage,
                        isCancel: false
                    );
                byte[] calendarBytes = Encoding.UTF8.GetBytes(calendarContent);

                // Bijlage genereren
                Attachment calendarAttachment = new Attachment(new MemoryStream(calendarBytes), "Reservatie.ics", "text/calendar");
                _emailSender.SendReservationEmailAsync(email.ToString(), $"Massagehuis: Uw reservatie op {geselecteerdSlot.Value.Date.ToString("dd-MM-yyyy")}" , EmailTextBuilder.ToString(),calendarAttachment);
                return View("../Home/Index",reservatieData); 
            }
            else
            {
                return View("~/Views/Shared/Error.cshtml");
            }
        }

        private static string GenerateICSInviteBody(string organizer, string attendees, string subject, string description, string location, DateTime startTime, DateTime endTime, int? eventID = null, bool isCancel = false)
        {
            StringBuilder str = new StringBuilder();

            // Begin calendar
            str.AppendLine("BEGIN:VCALENDAR");
            str.AppendLine("PRODID:-//Microsoft Corporation//Outlook 12.0 MIMEDIR//EN");
            str.AppendLine("VERSION:2.0");
            str.AppendLine(string.Format("METHOD:{0}", (isCancel ? "CANCEL" : "REQUEST")));
            str.AppendLine("BEGIN:VEVENT");

            // Event details
            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startTime.ToUniversalTime()));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmss}", DateTime.UtcNow));
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endTime.ToUniversalTime()));
            if (isCancel)
            {
                str.AppendLine("STATUS:CANCELLED");
            }
            str.AppendLine(string.Format("LOCATION: {0}", location));
            str.AppendLine(string.Format("UID:{0}", (eventID.HasValue ? "Event" + eventID : Guid.NewGuid().ToString())));
            str.AppendLine(string.Format("DESCRIPTION:{0}", description.Replace("\n", "<br>")));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", description.Replace("\n", "<br>")));
            str.AppendLine(string.Format("SUMMARY:{0}", subject));

            // Organizer and attendees
            str.AppendLine(string.Format("ORGANIZER;CN=\"{0}\":MAILTO:{1}", organizer, "test@123.com"));
            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", attendees,attendees));

            // Alarm
            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:DISPLAY");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("END:VALARM");

            // End event and calendar
            str.AppendLine("END:VEVENT");
            str.AppendLine("END:VCALENDAR");

            return str.ToString();
        }
        [Authorize(Roles = "uitbater, administrator, klant, masseur")]
        public async Task<IActionResult> KlantReservatieOverzicht(IEnumerable<ReservatieVM> reservatieData, int weekOffset) {
            DateTime basisDatumVoorWeek = DateTime.Today.AddDays(weekOffset * 7);
            // Bepaal het begin en einde van de week op basis van de basisdatum
            DayOfWeek eersteDagVanDeWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            DateTime beginVanDeWeek = basisDatumVoorWeek.AddDays(-(int)basisDatumVoorWeek.DayOfWeek).AddDays((int)eersteDagVanDeWeek);
            DateTime eindeVanDeWeek = beginVanDeWeek.AddDays(7).AddSeconds(-1);
            var user = await _userManager.GetUserAsync(User);

            var reservatieVMs = new List<ReservatieVM>();

            if (await _userManager.IsInRoleAsync(user, "klant"))
            {
                var klantReservaties = await _reservatieService.GetAllAsync();
                var userReservaties = klantReservaties.Where(b => b.IdAspNetUsers == user.Id).OrderBy(b=> b.DatumReservatie);

                foreach (var Res in userReservaties)
                {
                    var reservatieVM = _mapper.Map<ReservatieVM>(Res); // Map de Reservatie naar ReservatieVM
                    reservatieVMs.Add(reservatieVM);
                }
            }
            if (await _userManager.IsInRoleAsync(user, "uitbater")) 
            {
                var klantReservaties = await _reservatieService.GetAllAsync();
                var userReservaties = klantReservaties.OrderBy(b => b.DatumReservatie).Where(b => b.DatumReservatie >= beginVanDeWeek && b.DatumReservatie<=eindeVanDeWeek);

                foreach (var Res in userReservaties)
                {
                    var reservatieVM = _mapper.Map<ReservatieVM>(Res); // Map de Reservatie naar ReservatieVM
                    reservatieVMs.Add(reservatieVM);
                }
            }
            if (await _userManager.IsInRoleAsync(user, "masseur"))
            {
                var klantReservaties = await _reservatieService.GetAllAsync();
                var masseurId = await _masseurService.GetAllAsync();
                masseurId = masseurId.Where(b => b.IdAspNetUsers == user.Id);
                klantReservaties = klantReservaties.Where(b => b.IdMasseur == masseurId.FirstOrDefault().Id);
                var userReservaties = klantReservaties.OrderBy(b => b.DatumReservatie).Where(b => b.DatumReservatie >= beginVanDeWeek && b.DatumReservatie <= eindeVanDeWeek);

                foreach (var Res in userReservaties)
                {
                    var reservatieVM = _mapper.Map<ReservatieVM>(Res); // Map de Reservatie naar ReservatieVM
                    reservatieVMs.Add(reservatieVM);
                }
            }

            ViewData["WeekOffset"] = weekOffset;
            ViewData["BeginVanDeWeek"] = beginVanDeWeek;
            ViewData["EindeVanDeWeek"] = eindeVanDeWeek;
            return View("OverzichtReservaties",reservatieVMs);
        }
        [Authorize(Roles = "uitbater, administrator, klant, masseur")]
        public async Task<IActionResult> AnnuleerReservatie(int id) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            Reservatie teAnnuleren = new Reservatie { Id=id};
            var reservatie = await _reservatieService.FindByIdAsync(teAnnuleren);
            if (reservatie == null)
            {
                return NotFound();
            }
            bool isOwner = reservatie.IdAspNetUsers == user.Id;
            bool isUitbater = await _userManager.IsInRoleAsync(user, "uitbater");
            bool isMasseur = await _userManager.IsInRoleAsync(user, "masseur");
            if (!isOwner && !isUitbater && !isMasseur) // Als NIET de eigenaar EN NIET uitbater EN NIET masseur
            {
                return Forbid();
            }

            if (reservatie.DatumReservatie <= DateTime.Today.AddDays(4))
            {
                ViewBag.ErrorMessage = "Reserveringen kunnen alleen tot 4 dagen voor de geplande datum worden geannuleerd.";
                return View("~/Views/Shared/Error.cshtml");
            }
            if (await _userManager.IsInRoleAsync(user, "uitbater") || await _userManager.IsInRoleAsync(user, "masseur") || user.Id == reservatie.IdAspNetUsers)
            {
                await _reservatieService.DeleteAsync(reservatie); // Verwijder de reservering
            }
            return RedirectToAction("KlantReservatieOverzicht");
        }

    }

}
