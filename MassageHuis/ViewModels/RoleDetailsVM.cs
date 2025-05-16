using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using MassageHuis.Models;
namespace MassageHuis.ViewModels
{
    public class RoleDetailsVM
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ApplicationUser> UsersInRole { get; set; }
        public IEnumerable<ApplicationUser> UsersNotInRole { get; set; }

    }
}