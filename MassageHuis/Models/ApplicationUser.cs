using Microsoft.AspNetCore.Identity;
using System;

namespace MassageHuis.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public DateTime? GeboorteDatum{ get; set; }
        public string Geslacht { get; set; }
    }
}