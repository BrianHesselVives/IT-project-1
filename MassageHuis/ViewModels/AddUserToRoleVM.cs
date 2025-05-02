using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MassageHuis.ViewModels
{
    public class AddUserToRoleVM
    {
        [Required]
        public string RoleId { get; set; }
        public string? Beschrijving { get; set; }
        public List<string> UsersToAdd { get; set; } = new List<string>();
        public List<string> UsersToRemove { get; set; } = new List<string>();
    }
}