using MassageHuis.Entities;
using MassageHuis.Models;
using System.Collections.Generic;

namespace MassageHuis.ViewModels
{
    public class MasseurVM
    {
        public int Id { get; set; }

        public bool Actief { get; set; }

        public DateOnly? Einddienstverband { get; set; }

        public string? Beschrijving { get; set; }

        public string? IdAspNetUsers { get; set; }
        public string? Naam { get; set; }
        public IEnumerable<Masseur>? Masseurs { get; set; }
        public IEnumerable<ApplicationUser>? Gebruikers { get; set; }
        public IEnumerable<Schema>? Schemas { get; set; }
        public IEnumerable<RegulierTijdslot>? RegulierTijdsloten { get; set; }
        public IEnumerable<UitzonderingTijdslot>? UitzonderingTijdsloten { get; set; }
        public IEnumerable<Reservatie>? Reservaties { get; set; }
        public List<DateTime>? vrijeSlots { get; set; }
    }
}