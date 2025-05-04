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
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MassageHuis.Controllers
{
    public class UitbaterController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private IService<UitzonderingTijdslot> _uitzonderingTijdslotService;
        private IService<Reservatie> _reservatieService;
        private IService<RegulierTijdslot> _regulierTijdslotService;
        private IService<Masseur> _masseurService;
        private IService<Schema> _schemaService;
        private readonly IMapper _mapper;

        public UitbaterController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper, IService<UitzonderingTijdslot> uitzonderingTijdsloService, IService<Reservatie> reservatieService, IService<RegulierTijdslot> regulierTijdslotService,
            IService<Masseur> masseurservice,
            IService<Schema> schemaservice)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _uitzonderingTijdslotService = uitzonderingTijdsloService;
            _reservatieService = reservatieService;
            _regulierTijdslotService = regulierTijdslotService;
            _masseurService = masseurservice;
            _schemaService = schemaservice;
        }

        [Authorize(Roles = "uitbater")]
        public IActionResult Index()
        {
            VerlofVM model = new VerlofVM();
            return View(model);
        }

        [Authorize(Roles = "masseur,uitbater")]
        public IActionResult VerlofAanvraag()
        {
            var model = new VerlofVM();
            model.StartDatum = DateOnly.FromDateTime(DateTime.Today);
            model.DagDeel = "full";
            return View(model);
        }

        [Authorize(Roles = "masseur,uitbater")]
        [HttpPost]
        public async Task<IActionResult> BevestigVerlof(VerlofVM verlofPeriode)
        {
            List<UitzonderingTijdslot> teRegistrerenUitzonderingen = new List<UitzonderingTijdslot>();

            if (ModelState.IsValid)
            {
                var masseurs = await _masseurService.GetAllAsync();
                var userId = _userManager.GetUserId(User);
                masseurs = masseurs.Where(p => p.IdAspNetUsers == userId);
                var actieveMasseur = masseurs.FirstOrDefault();
                var schemas = await _schemaService.GetAllAsync();
                Schema actiefSchema = null; // Declareer actiefSchema buiten de if-lus
                if (actieveMasseur != null)
                {
                    actiefSchema = schemas
                        .Where(s => s.IdMasseur == actieveMasseur.Id)
                        .OrderByDescending(s => s.StartDatum)
                        .FirstOrDefault();
                }

                switch (verlofPeriode.DagDeel)
                {
                    case VerlofVM.FullDay:
                        DateOnly start = verlofPeriode.StartDatum;
                        DateOnly? eind = verlofPeriode.EindDatum;
                        if (!eind.HasValue || start == eind)
                        {
                            teRegistrerenUitzondering(start, new TimeOnly(0, 0, 0), new TimeOnly(23, 59, 59), "Volledige dag Verlof", teRegistrerenUitzonderingen, actiefSchema?.Id ?? 0); // Gebruik 0 als standaardwaarde indien null
                        }
                        else if (start < eind)
                        {
                            for (DateOnly dag = start; dag <= eind; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(0, 0, 0), new TimeOnly(23, 59, 59), "Volledige dag Verlof", teRegistrerenUitzonderingen, actiefSchema?.Id ?? 0); // Gebruik 0 als standaardwaarde indien null
                            }
                        }
                        break;
                    case VerlofVM.Morning:
                        DateOnly startMo = verlofPeriode.StartDatum;
                        DateOnly? eindMo = verlofPeriode.EindDatum;
                        if (!eindMo.HasValue || startMo == eindMo)
                        {
                            teRegistrerenUitzondering(startMo, new TimeOnly(0, 0, 0), new TimeOnly(11, 59, 59), "Voormiddag Verlof", teRegistrerenUitzonderingen, actiefSchema?.Id ?? 0); // Gebruik 0 als standaardwaarde indien null
                        }
                        else if (startMo < eindMo)
                        {
                            for (DateOnly dag = startMo; dag <= eindMo; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(0, 0, 0), new TimeOnly(11, 59, 59), "Voormiddag Verlof", teRegistrerenUitzonderingen, actiefSchema?.Id ?? 0); // Gebruik 0 als standaardwaarde indien null
                            }
                        }
                        break;
                    case VerlofVM.Afternoon:
                        DateOnly startAf = verlofPeriode.StartDatum;
                        DateOnly? eindAf = verlofPeriode.EindDatum;
                        if (!eindAf.HasValue || startAf == eindAf)
                        {
                            teRegistrerenUitzondering(startAf, new TimeOnly(12, 00, 0), new TimeOnly(23, 59, 59), "Namiddag Verlof", teRegistrerenUitzonderingen, actiefSchema?.Id ?? 0); // Gebruik 0 als standaardwaarde indien null
                        }
                        else if (startAf < eindAf)
                        {
                            for (DateOnly dag = startAf; dag <= eindAf; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(12, 00, 0), new TimeOnly(23, 59, 59), "Namiddag Verlof", teRegistrerenUitzonderingen, actiefSchema?.Id ?? 0); // Gebruik 0 als standaardwaarde indien null
                            }
                        }
                        break;
                    default:
                        ModelState.AddModelError("DagDeel", "Ongeldige periode geselecteerd.");
                        return View("VerlofAanvraag", verlofPeriode);
                }

                if (ModelState.IsValid && teRegistrerenUitzonderingen.Any())
                {
                    var bestaandeVerlofDagen = await _uitzonderingTijdslotService.GetAllAsync();
                    // Filter bestaande verlofdagen op basis van het schema ID van de actieve masseur
                    bestaandeVerlofDagen = bestaandeVerlofDagen.Where(v => v.IdSchema == actiefSchema?.Id && v.TypeUitzondering.Contains("Verlof")).ToList();

                    bool overlappendVerlofGevonden = false;

                    // Controleer of de nieuwe verlof aanvraag overlapt met bestaande verlofdagen
                    foreach (var nieuwVerlof in teRegistrerenUitzonderingen)
                    {
                        foreach (var bestaandVerlof in bestaandeVerlofDagen)
                        {
                            if (nieuwVerlof.Datum == bestaandVerlof.Datum &&
                                nieuwVerlof.Startijd < bestaandVerlof.Eindtijd &&
                                nieuwVerlof.Eindtijd > bestaandVerlof.Startijd)
                            {
                                overlappendVerlofGevonden = true;
                                break;
                            }
                        }
                        if (overlappendVerlofGevonden)
                        {
                            break;
                        }
                    }

                    if (overlappendVerlofGevonden)
                    {
                        ModelState.AddModelError("VerlofRegistratie", "De aangevraagde verlofperiode overlapt met bestaande verlofdagen.");
                        return View("VerlofAanvraag", verlofPeriode);
                    }
                    else
                    {
                        // Haal alle bestaande reservaties op
                        var bestaandeReservaties = await _reservatieService.GetAllAsync();
                        bool overlappendeReservatieGevonden = false;

                        // Controleer of er reservaties binnen de aangevraagde verlofperiode vallen
                        foreach (var verlofSlot in teRegistrerenUitzonderingen)
                        {
                            foreach (var reservatie in bestaandeReservaties)
                            {
                                // Controleer of de reservatie op dezelfde dag valt
                                if (reservatie.DatumReservatie == verlofSlot.Datum)
                                {
                                    // Controleer op tijdsoverlapvar findId = new RegulierTijdslot();
                                    var findId = new RegulierTijdslot();
                                    findId.Id = reservatie.IdRegulierTijdslot;
                                    RegulierTijdslot tijd = await _regulierTijdslotService.FindByIdAsync(findId);
                                    if (tijd != null && tijd.EindTijd > verlofSlot.Startijd && tijd.StartTijd < verlofSlot.Eindtijd)
                                    {
                                        overlappendeReservatieGevonden = true;
                                        break;
                                    }
                                }
                            }
                            if (overlappendeReservatieGevonden)
                            {
                                break;
                            }
                        }

                        if (overlappendeReservatieGevonden)
                        {
                            ModelState.AddModelError("VerlofRegistratie", "Er zijn bestaande reservaties die binnen de aangevraagde verlofperiode vallen. Verlof kan niet worden geregistreerd.");
                            return View("VerlofAanvraag", verlofPeriode);
                        }
                        else
                        {
                            try
                            {
                                await _uitzonderingTijdslotService.AddRangeAsync(teRegistrerenUitzonderingen);
                                verlofPeriode.VerlofGeregistreerd = true;
                                if (User.IsInRole("uitbater"))
                                {
                                    return View("Index", verlofPeriode);
                                }
                                else
                                {
                                    return View("../Masseur/Index", verlofPeriode);
                                }
                            }
                            catch
                            {
                                verlofPeriode.VerlofGeregistreerd = false;
                                ModelState.AddModelError("VerlofRegistratie", "Kon verlof niet registreren.");
                                if (User.IsInRole("uitbater"))
                                {
                                    return View("Index", verlofPeriode);
                                }
                                else
                                {
                                    return View("../Masseur/Index", verlofPeriode);
                                }
                            }
                        }
                    }
                }

                {
                    var bestaandeVerlofDagen = await _uitzonderingTijdslotService.GetAllAsync();
                    bestaandeVerlofDagen = bestaandeVerlofDagen.Where(v => v.IdSchema == actiefSchema?.Id && v.TypeUitzondering.Contains("Verlof")).ToList(); // Gebruik 0 als standaardwaarde indien null

                    bool overlappendVerlofGevonden = false;

                    // Controleer of de nieuwe verlof aanvraag overlapt met bestaande verlofdagen
                    foreach (var nieuwVerlof in teRegistrerenUitzonderingen)
                    {
                        foreach (var bestaandVerlof in bestaandeVerlofDagen)
                        {
                            if (nieuwVerlof.Datum == bestaandVerlof.Datum &&
                                nieuwVerlof.Startijd < bestaandVerlof.Eindtijd &&
                                nieuwVerlof.Eindtijd > bestaandVerlof.Startijd )
                            {
                                overlappendVerlofGevonden = true;
                                break;
                            }
                        }
                        if (overlappendVerlofGevonden)
                        {
                            break;
                        }
                    }

                    if (overlappendVerlofGevonden)
                    {
                        ModelState.AddModelError("VerlofRegistratie", "De aangevraagde verlofperiode overlapt met bestaande verlofdagen.");
                        return View("VerlofAanvraag", verlofPeriode);
                    }
                    else
                    {
                        // Haal alle bestaande reservaties op
                        var bestaandeReservaties = await _reservatieService.GetAllAsync();
                        bool overlappendeReservatieGevonden = false;

                        // Controleer of er reservaties binnen de aangevraagde verlofperiode vallen
                        foreach (var verlofSlot in teRegistrerenUitzonderingen)
                        {
                            foreach (var reservatie in bestaandeReservaties)
                            {
                                // Controleer of de reservatie op dezelfde dag valt
                                if (reservatie.DatumReservatie == verlofSlot.Datum)
                                {
                                    // Controleer op tijdsoverlap
                                    var findId = new RegulierTijdslot();
                                    findId.Id = reservatie.IdRegulierTijdslot;
                                    RegulierTijdslot tijd = await _regulierTijdslotService.FindByIdAsync(findId); // Haal de tijdslot op basis van de ID
                                    if (tijd != null && tijd.EindTijd > verlofSlot.Startijd && tijd.StartTijd < verlofSlot.Eindtijd)
                                    {
                                        overlappendeReservatieGevonden = true;
                                        break;
                                    }
                                }
                            }
                            if (overlappendeReservatieGevonden)
                            {
                                break;
                            }
                        }

                        if (overlappendeReservatieGevonden)
                        {
                            ModelState.AddModelError("VerlofRegistratie", "Er zijn bestaande reservaties die binnen de aangevraagde verlofperiode vallen. Verlof kan niet worden geregistreerd.");
                            return View("VerlofAanvraag", verlofPeriode);
                        }
                        else
                        {
                            try
                            {
                                await _uitzonderingTijdslotService.AddRangeAsync(teRegistrerenUitzonderingen);
                                verlofPeriode.VerlofGeregistreerd = true;
                                if (User.IsInRole("uitbater"))
                                {
                                    return View("Index", verlofPeriode);
                                }
                                else
                                {
                                    return View("../Masseur/Index", verlofPeriode);
                                }
                            }
                            catch
                            {
                                verlofPeriode.VerlofGeregistreerd = false;
                                ModelState.AddModelError("VerlofRegistratie", "Kon verlof niet registreren.");
                                if (User.IsInRole("uitbater"))
                                {
                                    return View("Index", verlofPeriode);
                                }
                                else
                                {
                                    return View("../Masseur/Index", verlofPeriode);
                                }
                            }
                        }
                    }
                }
            }
            return View("VerlofAanvraag", verlofPeriode);
        }


        private void teRegistrerenUitzondering(DateOnly datum, TimeOnly startTijd, TimeOnly eindTijd, string type, List<UitzonderingTijdslot> lijst, int id)
        {
            UitzonderingTijdslot Verlof = new UitzonderingTijdslot();
            Verlof.Startijd = startTijd;
            Verlof.Eindtijd = eindTijd;
            Verlof.TypeUitzondering = type;
            Verlof.Datum = datum;
            if (User.IsInRole("uitbater"))
            {
                Verlof.IdSchema = 8; // schema uitbater en id masseur voor uitbater is 13
            }
            else
            {
                Verlof.IdSchema = id;
            }
            lijst.Add(Verlof);
        }

        [Authorize(Roles = "masseur,uitbater")]
        public async Task<IActionResult> VerlofOverzicht()
        {

            var verlofDagen = await _uitzonderingTijdslotService.GetAllAsync();

            var userId = _userManager.GetUserId(User);

            int schemaId = 0;
            if (User.IsInRole("uitbater"))
            {
                schemaId = 8;
            }
            else
            {
                var masseurs = await _masseurService.GetAllAsync();
                masseurs = masseurs.Where(p => p.IdAspNetUsers == userId);
                var actieveMasseur = masseurs.FirstOrDefault();
                var schemas = await _schemaService.GetAllAsync();
                Schema actiefSchema = null; // Declareer buiten de if-lus
                if (actieveMasseur != null)
                {
                    actiefSchema = schemas
                        .Where(s => s.IdMasseur == actieveMasseur.Id)
                        .OrderByDescending(s => s.StartDatum)
                        .FirstOrDefault();
                }
                schemaId = actiefSchema?.Id ?? 0; // Gebruik 0 als standaardwaarde indien null
            }
            var uitbaterVerlof = verlofDagen
                .Where(v => v.IdSchema == schemaId && v.TypeUitzondering.Contains("Verlof"))
                .OrderBy(v => v.Datum)
                .ThenBy(v => v.Startijd)
                .ToList();
            var verlofPeriodes = new List<VerlofPeriodeVM>();
            if (uitbaterVerlof.Any())
            {
                foreach (var verlof in uitbaterVerlof)
                {
                    verlofPeriodes.Add(new VerlofPeriodeVM
                    {
                        Id = verlof.Id,
                        StartDatum = verlof.Datum.ToDateTime(verlof.Startijd),
                        EindDatum = verlof.Datum.ToDateTime(verlof.Eindtijd),
                        TypeVerlof = verlof.TypeUitzondering
                    });
                }
            }
            var viewModel = new VerlofOverzichtVM { VerlofPeriodes = verlofPeriodes };
            return View(viewModel);
        }

        // Actie om een enkele verlofdag te verwijderen
        [HttpPost]
        [Authorize(Roles = "masseur,uitbater")]
        public async Task<IActionResult> VerlofVerwijderen(string datum)
        {
            if (string.IsNullOrEmpty(datum))
            {
                return BadRequest("Datum is vereist.");
            }

            DateOnly verlofDatum;
            if (!DateOnly.TryParse(datum, out verlofDatum))
            {
                return BadRequest("Ongeldig datumformaat.");
            }
            var verlofDagen = await _uitzonderingTijdslotService.GetAllAsync();
            var userId = _userManager.GetUserId(User);

            int schemaId = 0;
            if (User.IsInRole("uitbater"))
            {
                schemaId = 8;
            }
            else
            {
                var masseurs = await _masseurService.GetAllAsync();
                masseurs = masseurs.Where(p => p.IdAspNetUsers == userId);
                var actieveMasseur = masseurs.FirstOrDefault();
                var schemas = await _schemaService.GetAllAsync();
                Schema actiefSchema = null;
                if (actieveMasseur != null)
                {
                    actiefSchema = schemas
                    .Where(s => s.IdMasseur == actieveMasseur.Id)
                    .OrderByDescending(s => s.StartDatum)
                    .FirstOrDefault();
                }
                schemaId = actiefSchema?.Id ?? 0;
            }
            var verlof = verlofDagen.FirstOrDefault(v => v.IdSchema == schemaId && v.Datum == verlofDatum);
            if (verlof == null)
            {
                return NotFound();
            }

            try
            {
                await _uitzonderingTijdslotService.DeleteAsync(verlof);
                return RedirectToAction("VerlofOverzicht");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Er is een fout opgetreden bij het verwijderen van het verlof.";
                return RedirectToAction("VerlofOverzicht");
            }
        }

        // Actie om een reeks verlofdagen te verwijderen
        [HttpPost]
        [Authorize(Roles = "masseur,uitbater")]
        public async Task<IActionResult> VerlofVerwijderenRange(string startDatum, string eindDatum)
        {
            if (string.IsNullOrEmpty(startDatum))
            {
                return BadRequest("Startdatum is vereist.");
            }

            DateOnly verlofStartDatum;
            if (!DateOnly.TryParse(startDatum, out verlofStartDatum))
            {
                return BadRequest("Ongeldig startdatumformaat.");
            }

            DateOnly? verlofEindDatum = null;
            if (!string.IsNullOrEmpty(eindDatum))
            {
                if (!DateOnly.TryParse(eindDatum, out DateOnly parsedEindDatum))
                {
                    return BadRequest("Ongeldig einddatumformaat.");
                }
                verlofEindDatum = parsedEindDatum;
            }

            var verlofDagen = await _uitzonderingTijdslotService.GetAllAsync();
            var userId = _userManager.GetUserId(User);

            int schemaId = 0;
            if (User.IsInRole("uitbater"))
            {
                schemaId = 8;
            }
            else
            {
                var masseurs = await _masseurService.GetAllAsync();
                masseurs = masseurs.Where(p => p.IdAspNetUsers == userId);
                var actieveMasseur = masseurs.FirstOrDefault();
                var schemas = await _schemaService.GetAllAsync();
                Schema actiefSchema = null;
                if (actieveMasseur != null)
                {
                    actiefSchema = schemas
                    .Where(s => s.IdMasseur == actieveMasseur.Id)
                    .OrderByDescending(s => s.StartDatum)
                    .FirstOrDefault();
                }
                schemaId = actiefSchema?.Id ?? 0;
            }
            var teVerwijderenVerlofDagen = verlofDagen.Where(v => v.IdSchema == schemaId && v.Datum >= verlofStartDatum && (!verlofEindDatum.HasValue || v.Datum <= verlofEindDatum)).ToList();


            if (!teVerwijderenVerlofDagen.Any())
            {
                TempData["ErrorMessage"] = "Geen verlof gevonden om te verwijderen binnen het opgegeven datumbereik.";
                return RedirectToAction("VerlofOverzicht");
            }

            try
            {
                await _uitzonderingTijdslotService.DeleteRangeAsync(teVerwijderenVerlofDagen);
                return RedirectToAction("VerlofOverzicht");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Er is een fout opgetreden bij het verwijderen van de verlofperiode.";
                return RedirectToAction("VerlofOverzicht");
            }
        }
    }
}
