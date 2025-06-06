using MassageHuis.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MassageHuis.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccessDeniedModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Page();
            }

            if (await _userManager.IsInRoleAsync(user, "admin"))
            {
                return RedirectToPage("/Admin/Dashboard");
            }
            else if (await _userManager.IsInRoleAsync(user, "uitbater"))
            {
                return RedirectToPage("/Uitbater/Overzicht");
            }
            else if (await _userManager.IsInRoleAsync(user, "masseur"))
            {
                return RedirectToPage("/Masseur/Overzicht");
            }


            return Page();
        }
    }
}