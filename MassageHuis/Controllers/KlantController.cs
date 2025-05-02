using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models;
using MassageHuis.Services;
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using MassageHuis.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Sockets;

namespace MassageHuis.Controllers
{

    public class KlantController : Controller
    {
        private IService<Masseur> _masseurService;
        private IService<Schema> _schemaService;        
        private IService<RegulierTijdslot> _regulierTijdslotService;
        private readonly IMapper _mapper;
        private readonly IEmailSend _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;


        public KlantController(IMapper mapper, UserManager<ApplicationUser> usermanager, IService<Masseur> masseurservice, IService<Schema> schemaservice, IService<RegulierTijdslot> regulierTijdslotservice, IEmailSend emailSender)
        {
            _masseurService = masseurservice;
            _schemaService = schemaservice;
            _regulierTijdslotService = regulierTijdslotservice;
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
            masseurdata.Schemas = schemas;
            var tijdsloten = await _regulierTijdslotService.GetAllAsync();
            masseurdata.RegulierTijdsloten = tijdsloten;
            return View(masseurdata);
        }
        public static void BerekenBeschikbareSloten()
        {
            return;
        }
    }
}
