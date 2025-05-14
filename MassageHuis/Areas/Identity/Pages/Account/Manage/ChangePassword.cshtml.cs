// Gelicentieerd aan de .NET Foundation onder een of meer overeenkomsten.
// De .NET Foundation licenseert dit bestand aan u onder de MIT-licentie.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MassageHuis.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace MassageHuis.Areas.Identity.Pages.Account.Manage
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<ChangePasswordModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     Deze API ondersteunt de standaard UI-infrastructuur van ASP.NET Core Identity en is niet bedoeld om
        ///     rechtstreeks vanuit uw code te worden gebruikt. Deze API kan in toekomstige releases worden gewijzigd of verwijderd.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     Deze API ondersteunt de standaard UI-infrastructuur van ASP.NET Core Identity en is niet bedoeld om
        ///     rechtstreeks vanuit uw code te worden gebruikt. Deze API kan in toekomstige releases worden gewijzigd of verwijderd.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     Deze API ondersteunt de standaard UI-infrastructuur van ASP.NET Core Identity en is niet bedoeld om
        ///     rechtstreeks vanuit uw code te worden gebruikt. Deze API kan in toekomstige releases worden gewijzigd of verwijderd.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     Deze API ondersteunt de standaard UI-infrastructuur van ASP.NET Core Identity en is niet bedoeld om
            ///     rechtstreeks vanuit uw code te worden gebruikt. Deze API kan in toekomstige releases worden gewijzigd of verwijderd.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Huidig wachtwoord")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     Deze API ondersteunt de standaard UI-infrastructuur van ASP.NET Core Identity en is niet bedoeld om
            ///     rechtstreeks vanuit uw code te worden gebruikt. Deze API kan in toekomstige releases worden gewijzigd of verwijderd.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "De {0} moet minstens {2} en maximaal {1} tekens lang zijn.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nieuw wachtwoord")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     Deze API ondersteunt de standaard UI-infrastructuur van ASP.NET Core Identity en is niet bedoeld om
            ///     rechtstreeks vanuit uw code te worden gebruikt. Deze API kan in toekomstige releases worden gewijzigd of verwijderd.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Bevestig nieuw wachtwoord")]
            [Compare("NewPassword", ErrorMessage = "Het nieuwe wachtwoord en het bevestigingswachtwoord komen niet overeen.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kan gebruiker met ID '{_userManager.GetUserId(User)}' niet laden.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("Gebruiker heeft zijn wachtwoord succesvol gewijzigd.");
            StatusMessage = "Uw wachtwoord is gewijzigd.";

            return RedirectToPage();
        }
    }
}
