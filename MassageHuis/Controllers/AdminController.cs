using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MassageHuis.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using MassageHuis.Models;
using Microsoft.AspNetCore.Authorization;
using MassageHuis.Entities;
using MassageHuis.Services;
using AutoMapper;
using MassageHuis.Services.Interfaces;
using MassageHuis.Util.Mail.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Sockets;

public class AdminController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private IService<Masseur> _masseurService;
    private readonly IMapper _mapper;
    private readonly IEmailSend _emailSender;
    

    public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager,IMapper mapper, IService<Masseur> masseurservice, IEmailSend emailSender)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _masseurService = masseurservice;
        _mapper = mapper;
        _emailSender = emailSender;
    }
    [Authorize(Roles = "administrator")]
    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
        var allUsers = _userManager.Users.ToList();
        var usersNotInRole = allUsers.Except(usersInRole).ToList();

        var viewModel = new RoleDetailsVM
        {
            Role = role,
            UsersInRole = usersInRole,
            UsersNotInRole = usersNotInRole
        };

        return View(viewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> RemoveUserFromRole(AddUserToRoleVM model)
    {
        if (ModelState.IsValid)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Rol niet gevonden.");
                return View("Details", new RoleDetailsVM { Role = role }); // Herlaad de pagina met foutmelding
            }

            if (model.UsersToRemove != null && model.UsersToRemove.Any())
            {
                foreach (var userId in model.UsersToRemove)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                        if (role.Name == "masseur")
                        {
                            MasseurVM newMasseurToAdd = new MasseurVM();
                            newMasseurToAdd.IdAspNetUsers = user.Id;
                            var masseur = _mapper.Map<Masseur>(newMasseurToAdd);
                            await _masseurService.DeleteAsync(masseur);
                        }
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, $"Fout bij verwijderen van {user.UserName}: {error.Description}");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Gebruiker met ID {userId} niet gevonden.");
                    }
                }
            }

            if (ModelState.IsValid) // Controleer of er geen fouten zijn opgetreden tijdens het verwijderen
            {
                return RedirectToAction("Details", new { id = model.RoleId });
            }
        }

        // Als er fouten zijn, herlaad de pagina met de foutmeldingen
        var existingRole = await _roleManager.FindByIdAsync(model.RoleId);
        var usersInRole = await _userManager.GetUsersInRoleAsync(existingRole?.Name);
        var allUsers = _userManager.Users.ToList();
        var usersNotInRole = allUsers.Except(usersInRole).ToList();
        return View("Details", new RoleDetailsVM { Role = existingRole, UsersInRole = usersInRole, UsersNotInRole = usersNotInRole });
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "administrator")]
    public async Task<IActionResult> AddUserToRole(AddUserToRoleVM model)
    {
        if (ModelState.IsValid)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "Rol niet gevonden.");
                return View("Details", new RoleDetailsVM { Role = role }); // Herlaad de pagina met foutmelding
            }

            if (model.UsersToAdd != null && model.UsersToAdd.Any())
            {
                foreach (var userId in model.UsersToAdd)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, role.Name);
                        // hier komt de code voor het toevoegen aan de masseeur tabel
                        if(role.Name == "masseur")
                        {
                            MasseurVM newMasseurToAdd = new MasseurVM();
                            newMasseurToAdd.IdAspNetUsers = user.Id;
                            newMasseurToAdd.Beschrijving = model.Beschrijving;
                            newMasseurToAdd.Actief = true;
                            var masseur = _mapper.Map<Masseur>(newMasseurToAdd);
                            await _masseurService.AddAsync(masseur);


                        }
                        
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, $"Fout bij toevoegen van {user.UserName}: {error.Description}");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, $"Gebruiker met ID {userId} niet gevonden.");
                    }
                }
            }

            if (ModelState.IsValid) // Controleer of er geen fouten zijn opgetreden tijdens het toevoegen
            {
                return RedirectToAction("Details", new { id = model.RoleId });
            }
        }

        // Als er fouten zijn, herlaad de pagina met de foutmeldingen
        var existingRole = await _roleManager.FindByIdAsync(model.RoleId);
        var usersInRole = await _userManager.GetUsersInRoleAsync(existingRole?.Name);
        var allUsers = _userManager.Users.ToList();
        var usersNotInRole = allUsers.Except(usersInRole).ToList();
        return View("Details", new RoleDetailsVM { Role = existingRole, UsersInRole = usersInRole, UsersNotInRole = usersNotInRole });
    }
}