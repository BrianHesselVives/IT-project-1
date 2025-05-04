using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models;
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
        private readonly IMapper _mapper;

        public UitbaterController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper, IService<UitzonderingTijdslot> uitzonderingTijdsloService, IService<Reservatie> reservatieService, IService<RegulierTijdslot> regulierTijdslotService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _uitzonderingTijdslotService = uitzonderingTijdsloService;
            _reservatieService = reservatieService;
            _regulierTijdslotService = regulierTijdslotService;
            _mapper = mapper;
        }

        [Authorize(Roles = "uitbater")]
        public IActionResult Index()
        {
            VerlofVM model = new VerlofVM();
            return View(model);
        }

        [Authorize(Roles = "uitbater")]
        public IActionResult VerlofAanvraag()
        {
            var model = new VerlofVM();
            model.StartDatum = DateOnly.FromDateTime(DateTime.Today);
            model.DagDeel = "full";
            return View(model);
        }

        [Authorize(Roles = "uitbater")]
        [HttpPost]
        public async Task<IActionResult> BevestigVerlof(VerlofVM verlofPeriode)
        {
            List<UitzonderingTijdslot> teRegistrerenUitzonderingen = new List<UitzonderingTijdslot>();

            if (ModelState.IsValid)
            {
                switch (verlofPeriode.DagDeel)
                {
                    case VerlofVM.FullDay:
                        DateOnly start = verlofPeriode.StartDatum;
                        DateOnly? eind = verlofPeriode.EindDatum;
                        if (!eind.HasValue || start == eind)
                        {
                            teRegistrerenUitzondering(start, new TimeOnly(0, 0, 0), new TimeOnly(23, 59, 59), "Volledige dag Verlof", teRegistrerenUitzonderingen);
                        }
                        else if (start < eind)
                        {
                            for (DateOnly dag = start; dag <= eind; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(0, 0, 0), new TimeOnly(23, 59, 59), "Volledige dag Verlof", teRegistrerenUitzonderingen);
                            }
                        }
                        break;
                    case VerlofVM.Morning:
                        DateOnly startMo = verlofPeriode.StartDatum;
                        DateOnly? eindMo = verlofPeriode.EindDatum;
                        if (!eindMo.HasValue || startMo == eindMo)
                        {
                            teRegistrerenUitzondering(startMo, new TimeOnly(0, 0, 0), new TimeOnly(11, 59, 59), "Voormiddag Verlof", teRegistrerenUitzonderingen);
                        }
                        else if (startMo < eindMo)
                        {
                            for (DateOnly dag = startMo; dag <= eindMo; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(0, 0, 0), new TimeOnly(11, 59, 59), "Voormiddag Verlof", teRegistrerenUitzonderingen);
                            }
                        }
                        break;
                    case VerlofVM.Afternoon:
                        DateOnly startAf = verlofPeriode.StartDatum;
                        DateOnly? eindAf = verlofPeriode.EindDatum;
                        if (!eindAf.HasValue || startAf == eindAf)
                        {
                            teRegistrerenUitzondering(startAf, new TimeOnly(12, 00, 0), new TimeOnly(23, 59, 59), "Namiddag Verlof", teRegistrerenUitzonderingen);
                        }
                        else if (startAf < eindAf)
                        {
                            for (DateOnly dag = startAf; dag <= eindAf; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(12, 00, 0), new TimeOnly(23, 59, 59), "Namiddag Verlof", teRegistrerenUitzonderingen);
                            }
                        }
                        break;
                    default:
                        ModelState.AddModelError("DagDeel", "Ongeldige periode geselecteerd.");
                        return View("VerlofAanvraag", verlofPeriode);
                }

                if (ModelState.IsValid && teRegistrerenUitzonderingen.Any())
                {
                    // Haal alle bestaande verlofdagen op voor deze masseur (uitbater met IdSchema 8)
                    var bestaandeVerlofDagen = await _uitzonderingTijdslotService.GetAllAsync();
                    bestaandeVerlofDagen = bestaandeVerlofDagen.Where(v => v.IdSchema == 8 && v.TypeUitzondering.Contains("Verlof")).ToList();

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
                                break; // Stop met zoeken naar overlappende verlof voor dit nieuwe verlofslot
                            }
                        }
                        if (overlappendVerlofGevonden)
                        {
                            break; // Stop met controleren van de overige nieuwe verlofslots
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
                                    RegulierTijdslot rsslt = new RegulierTijdslot();
                                    rsslt.Id = reservatie.Id;
                                    var tijd = await _regulierTijdslotService.FindByIdAsync(rsslt);

                                    if (tijd != null && tijd.EindTijd < verlofSlot.Eindtijd && tijd.EindTijd > verlofSlot.Startijd)
                                    {
                                        overlappendeReservatieGevonden = true;
                                        break; // Stop met zoeken naar overlappende reservaties voor dit verlofslot
                                    }
                                }
                            }
                            if (overlappendeReservatieGevonden)
                            {
                                break; // Stop met controleren van de overige verlofslots
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
                                return View("Index", verlofPeriode);
                            }
                            catch
                            {
                                verlofPeriode.VerlofGeregistreerd = false;
                                ModelState.AddModelError("VerlofRegistratie", "Kon verlof niet registreren.");
                                return View("Index", verlofPeriode);
                            }
                        }
                    }
                }
            }

            return View("VerlofAanvraag", verlofPeriode);
        }

        private void teRegistrerenUitzondering(DateOnly datum, TimeOnly startTijd, TimeOnly eindTijd, string type, List<UitzonderingTijdslot> lijst)
        {
            UitzonderingTijdslot Verlof = new UitzonderingTijdslot();
            Verlof.Startijd = startTijd;
            Verlof.Eindtijd = eindTijd;
            Verlof.TypeUitzondering = type;
            Verlof.Datum = datum;
            Verlof.IdSchema = 8; // schema uitbater en id masseur voor uitbater is 13
            lijst.Add(Verlof);
        }

        [Authorize(Roles = "uitbater")]
        public async Task<IActionResult> VerlofOverzicht()
        {
            var verlofDagen = await _uitzonderingTijdslotService.GetAllAsync();
            var uitbaterVerlof = verlofDagen
                .Where(v => v.IdSchema == 8 && v.TypeUitzondering.Contains("Verlof"))
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

        // Actie om een verlofdag te verwijderen
        [HttpPost]
        public async Task<IActionResult> VerlofVerwijderen(int id)
        {
            var verlof = await _uitzonderingTijdslotService.FindByIdAsync(new UitzonderingTijdslot { Id = id });
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
    }
}

