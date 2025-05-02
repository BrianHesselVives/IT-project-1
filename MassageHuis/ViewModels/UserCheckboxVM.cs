using System;

namespace MassageHuis.ViewModels
{
    public class UserCheckboxVM
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
    }
}