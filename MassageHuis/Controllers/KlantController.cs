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
        private readonly IMapper _mapper;
        private readonly IEmailSend _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;

        public KlantController(IMapper mapper, UserManager<ApplicationUser> usermanager, IService<Masseur> masseurservice, IEmailSend emailSender)
        {
            _masseurService = masseurservice;
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
            return View(masseurdata);
        }
    }
}
