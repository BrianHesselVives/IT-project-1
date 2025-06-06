using MassageHuis.Models;
using Microsoft.AspNetCore.Identity;
namespace MassageHuis.ViewModels
{
    public class RoleDetailsVM
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ApplicationUser> UsersInRole { get; set; }
        public IEnumerable<ApplicationUser> UsersNotInRole { get; set; }

    }
}