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

namespace MassageHuis.Controllers
{
    public class UitbaterController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private IService<UitzonderingTijdslot> _uitzonderingTijdslotService;
        private readonly IMapper _mapper;
        public UitbaterController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper, IService<UitzonderingTijdslot> uitzonderingTijdsloService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _uitzonderingTijdslotService = uitzonderingTijdsloService;
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
                            teRegistrerenUitzondering(startMo, new TimeOnly(9, 0, 0), new TimeOnly(12, 30, 0), "Voormiddag Verlof", teRegistrerenUitzonderingen);
                        }
                        else if (startMo < eindMo)
                        {
                            for (DateOnly dag = startMo; dag <= eindMo; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(9, 0, 0), new TimeOnly(12, 30, 0), "Voormiddag Verlof", teRegistrerenUitzonderingen);
                            }
                        }
                        break;
                    case VerlofVM.Afternoon:
                        DateOnly startAf = verlofPeriode.StartDatum;
                        DateOnly? eindAf = verlofPeriode.EindDatum;

                        if (!eindAf.HasValue || startAf == eindAf)
                        {
                            teRegistrerenUitzondering(startAf, new TimeOnly(13, 30, 0), new TimeOnly(17, 0, 0), "Namiddag Verlof", teRegistrerenUitzonderingen);
                        }
                        else if (startAf < eindAf)
                        {
                            for (DateOnly dag = startAf; dag <= eindAf; dag = dag.AddDays(1))
                            {
                                teRegistrerenUitzondering(dag, new TimeOnly(13, 30, 0), new TimeOnly(17, 0, 0), "Namiddag Verlof", teRegistrerenUitzonderingen);
                            }
                        }
                        break;
                    default:
                        ModelState.AddModelError("DagDeel", "Ongeldige periode geselecteerd.");
                        return View("VerlofAanvraag", verlofPeriode);
                }

                if (ModelState.IsValid && teRegistrerenUitzonderingen.Any())
                {
                    // Hier moet je de 'teRegistrerenUitzonderingen' opslaan in je database
                    // Voorbeeld (als je _context geïnjecteerd hebt):
                    // _context.UitzonderingTijdsloten.AddRange(teRegistrerenUitzonderingen);
                    // _context.SaveChanges();
                    try
                    {
                        foreach (UitzonderingTijdslot slot in teRegistrerenUitzonderingen)
                        {
                            await _uitzonderingTijdslotService.AddAsync(slot);
                        }
                        
                    }
                    catch
                    {
                        verlofPeriode.VerlofGeregistreerd = false;
                        ModelState.AddModelError("VerlofRegistratie","Kon verlof niet registreren");
                        return View("Index", verlofPeriode);
                    }


                    verlofPeriode.VerlofGeregistreerd = true;
                    return View("Index", verlofPeriode);
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
            Verlof.IdSchema = 1; // Hardcoded, overweeg dit dynamisch te maken
            lijst.Add(Verlof);
        }
    }
}
